using System.Collections;
using UnityEngine;
using Weapons;
using SpaceGame.Utils;

[RequireComponent(typeof(WeaponSystem))]
public class TurretAI : AIController
{
    public float rotationSpeed;
    public float pitchSpeed;

    [Header("Joint constraints")]
    public Transform turretYawJoint;
    public Transform turretPitchJoint;
    public Vector2 pitchRangeDegrees;

    private Vector3 targetDir = Vector3.zero;

    private IEnumerator firingSequence;


    public override void Init()
    {
        weapons = GetComponent<WeaponSystem>();
        SetTarget(target);

        rotationJoint = turretYawJoint;

        firingSequence = FireSequence();
        StartCoroutine(firingSequence);
    }

    public override void Update()
    {
        RotateCannons();
    }

    public override void SetTarget(Targetable target)
    {
        base.SetTarget(target);
        weapons.SetAimTransform(target.lockPoint);
        weapons.SetTarget(target);
    }

    protected override Vector3 GetAimDirection()
    {
        return turretPitchJoint.forward;
    }

    protected override Vector3 GetScanPoint()
    {
        return turretPitchJoint.position + turretPitchJoint.forward * scanPointOffset;
    }

    protected override bool UpdateTargetLock()
    {
        base.UpdateTargetLock();
        weapons.OnAimAssist(targetLocked, target);
        return targetLocked;
    }

    public void RotateCannons()
    {
        WeaponData wpn = weapons.GetCurrentWeaponInfo();
        targetDir = WeaponUtilities.FirstOrderIntercept(turretYawJoint.position, Vector3.zero, wpn.projectileSpeed, target.GetPosition(), target.GetVelocity());
        UpdateAimValues(targetDir);

        float maxTurn = rotationSpeed * Time.deltaTime;
        float maxPitch = pitchSpeed * Time.deltaTime;

        float YawTurnDegrees = Vector3.SignedAngle(Vector3.forward, yawError, Vector3.up);

        float pitchTurnDegrees = Vector3.SignedAngle(turretPitchJoint.localRotation * Vector3.forward, pitchError, Vector3.right);

        turretYawJoint.rotation *= Quaternion.AngleAxis(Mathf.Clamp(YawTurnDegrees, -maxTurn, maxTurn), Vector3.up);

        //Debug.DrawRay(turretYawJoint.position - turretYawJoint.right * 25, turretYawJoint.right * 50, Color.green, 0.1f);
        //Debug.DrawRay(turretPitchJoint.position, turretPitchJoint.forward * 500, Color.red, .1f);

        Quaternion newPitch = turretPitchJoint.rotation * Quaternion.AngleAxis(Mathf.Clamp(pitchTurnDegrees, -maxPitch, maxPitch), Vector3.right);
        TryRotatePitch(newPitch);
    }

    private void TryRotatePitch(Quaternion newRotation)
    {
        float diff = Vector3.SignedAngle(turretYawJoint.forward, newRotation * Vector3.forward, turretYawJoint.right);

        if (diff > pitchRangeDegrees.y || diff < pitchRangeDegrees.x)
        {
            return;
        }
        turretPitchJoint.rotation = newRotation;
    }

    protected override void OnHealthDestroyed()
    {
        weapons.OnFiringButtonReleased();
        StopCoroutine(firingSequence);
    }
}
