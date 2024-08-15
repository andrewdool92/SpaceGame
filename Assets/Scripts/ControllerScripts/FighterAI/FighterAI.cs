using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(ShipController))]
[RequireComponent(typeof(BlasterController))]
public class FighterAI : AIController
{
    private ShipController ship;
    private BlasterController weapons;

    public IFighterState currentState;

    public IFighterState patrolling;
    public IFighterState strafing = new FighterStrafingState();
    public IFighterState passing = new FighterPassingState();
    public IFighterState adjusting = new FighterAdjustingState();
    public IFighterState closing = new FighterClosingState();

    public float yawSteeringAngle = 15f;
    public float firingRange = 200f;

    public Vector2Int burstRoundsRange = new(2, 5);
    public Vector2 burstCooldownRange = new(1, 3);
    private bool weaponsReady = true;

    public float passDistance = 50f;
    public float retreatDistance = 150f;
    public float lockOnAngle = 30f;

    public float boostScanModifier = 2f;
    private float scanRangeMod = 1f;
    public float reactionTime = 0.5f;

    public bool alive = true;

    private Vector3 aimCorrection = Vector3.zero;

    public override void Start()
    {
        base.Start();
        ship = GetComponent<ShipController>();
        weapons = GetComponent<BlasterController>();
        SetTarget(target);

        ship.hull.onDestruction += OnDeath;

        currentState = adjusting;
        currentState.EnterState(this);

        StartCoroutine(ObstacleScanner());
    }

    private void OnDestroy()
    {
        ship.hull.onDestruction -= OnDeath;
    }

    public override void Update()
    {
        if (alive)
        {
            currentState.UpdateState(this);

            if (TargetLocked() && GetTargetDistance() < firingRange)
            {
                FireWeapons();
            }
        }
    }

    public IEnumerator ObstacleScanner()
    {
        while (alive)
        {
            ScanForObstacles();
            yield return new WaitForSeconds(reactionTime);
        }
    }

    public void SwitchState(IFighterState state)
    {
        //Debug.Log($"Changing to {state}");
        currentState = state;
        state.EnterState(this);
    }

    protected void CheckPath()
    {
        Vector3 directPath = target.position - transform.position;
        obstacleReport.pathObstruction = Physics.Raycast(transform.position, directPath, directPath.magnitude * 0.9f);
    }

    protected override void ScanForObstacles()
    {
        CheckPath();

        base.ScanForObstacles(scanRangeMod);
        aimCorrection = obstacleReport.GetCorrectionValues();

        ship.verticalValue = aimCorrection.x;
        ship.strafeValue = -aimCorrection.z;
    }

    public void SteerPitch(float pitchAngle)
    {
        if (aimCorrection.x != 0) ship.pitchValue = aimCorrection.x;
        else ship.pitchValue = -Mathf.Clamp(pitchAngle, -1, 1);
    }

    public void SteerRoll(float pitchAngle, float errorAngle)
    {
        if (aimCorrection.z != 0)
        {
            ship.rollValue = aimCorrection.z;
        }

        else if (Mathf.Abs(pitchAngle) > 110f || errorAngle < yawSteeringAngle)
        {
            ship.rollValue = 0f;
        }

        else
        {
            ship.rollValue = -Mathf.Clamp(Vector3.SignedAngle(Vector3.up, rollError, Vector3.forward), -1, 1);
        }
    }

    public void SteerYaw(float pitchAngle, float errorAngle)
    {
        if (aimCorrection.y != 0)
        {
            ship.yawValue = -aimCorrection.y;
        }

        else if (Mathf.Abs(pitchAngle) > 110f || errorAngle >= yawSteeringAngle)
        {
            ship.yawValue = 0f;
        }

        else ship.yawValue = Mathf.Clamp(Vector3.SignedAngle(Vector3.forward, yawError, Vector3.up), -1, 1);
    }

    public void Steer()
    {
        Steer(weapons.CalculateLeadPoint(transform.position, target.position, targetRb.velocity));
    }

    public void Steer(Vector3 aimPoint)
    {
        UpdateAimValues(aimPoint);
        //ScanForObstacles();

        Debug.DrawLine(transform.position, aimPoint, Color.magenta, Time.deltaTime);

        float pitch = Vector3.SignedAngle(Vector3.forward, pitchError, Vector3.right);
        float errorAngle = Vector3.Angle(errorDirection, Vector3.forward);

        SteerPitch(pitch);
        SteerYaw(pitch, errorAngle);
        SteerRoll(pitch, errorAngle);

        ship.UpdateThrusters();
    }

    public void Steer(float pitch, float yaw, float roll)
    {
        ship.pitchValue = pitch;
        ship.yawValue = yaw;
        ship.rollValue = roll;
        ship.UpdateThrusters();
    }

    public bool EvasiveActionRequired()
    {
        return obstacleReport.DangerAhead();
    }

    public bool TargetIsAhead()
    {
        return Vector3.Angle(transform.forward, target.position - transform.position) < 60f;
    }

    public bool TargetLocked()
    {
        return Vector3.Angle(transform.forward, target.position - transform.position) < lockOnAngle;
    }

    public float GetTargetDistance()
    {
        float distance = (transform.position - target.position).magnitude;

        return distance;
    }

    public void SetForwardThrusters()
    {
        ship.boosting = true;
        ship.reversing = false;

        ship.UpdateThrusters();
    }

    public void SetSuperBoost(bool value)
    {
        ship.superBoosting = value;
        scanRangeMod = value ? boostScanModifier : 1f;

        ship.UpdateThrusters();
    }

    public void SetReverse()
    {
        ship.boosting = false;
        ship.superBoosting = false;
        ship.reversing = true;

        ship.UpdateThrusters();
    }

    public void CutThrusters()
    {
        ship.boosting = false;
        ship.superBoosting = false;
        ship.reversing = false;

        ship.UpdateThrusters();
    }

    public void FireWeapons()
    {
        if (weaponsReady)
        {
            StartCoroutine(BurstCooldown());
            weapons.FireBurst(Random.Range(burstRoundsRange.x, burstRoundsRange.y));
        }
    }

    public IEnumerator BurstCooldown()
    {
        weaponsReady = false;
        yield return new WaitForSeconds(Random.Range(burstCooldownRange.x, burstCooldownRange.y));
        weaponsReady = true;
    }

    public void OnDeath()
    {
        alive = false;
    }
}
