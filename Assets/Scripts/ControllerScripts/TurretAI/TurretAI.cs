using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BlasterController))]
public class TurretAI : AIController
{
    public BlasterController weapons;

    public int burstFireCount;
    public float burstDelay;
    public float rotationSpeed;
    public float pitchSpeed;

    public Transform turretYawJoint;
    public Transform turretPitchJoint;


    public override void Start()
    {
        base.Start();

        weapons = GetComponent<BlasterController>();
        SetTarget(target);

        rotationJoint = turretYawJoint;

        StartCoroutine(TestFire());
    }

    public override void Update()
    {
        RotateCannons();
    }

    private IEnumerator TestFire()
    {
        yield return new WaitForSeconds(2);

        while (true)
        {
            weapons.FireBurst(burstFireCount);
            yield return new WaitForSeconds(weapons.firingSpeed * burstFireCount + burstDelay);
        }
    }

    public void RotateCannons()
    {
        Vector3 targetDir = weapons.CalculateLeadPoint(turretYawJoint.position, target.position, targetRb.velocity);
        UpdateAimValues(targetDir);

        float maxTurn = rotationSpeed * Time.deltaTime;
        float maxPitch = pitchSpeed * Time.deltaTime;

        float YawTurnDegrees = Vector3.SignedAngle(Vector3.forward, yawError, Vector3.up);
        if (Mathf.Abs(YawTurnDegrees) > 90) YawTurnDegrees = (180 * -Mathf.Sign(YawTurnDegrees)) + YawTurnDegrees;

        float pitchTurnDegrees = Vector3.SignedAngle(turretPitchJoint.localRotation * Vector3.forward, pitchError, Vector3.right);

        turretYawJoint.rotation *= Quaternion.AngleAxis(Mathf.Clamp(YawTurnDegrees, -maxTurn, maxTurn), Vector3.up);

        Debug.DrawRay(turretYawJoint.position - turretYawJoint.right * 25, turretYawJoint.right * 50, Color.green, 0.1f);
        Debug.DrawRay(turretPitchJoint.position, turretPitchJoint.forward * 500, Color.red, .1f);

        Quaternion newPitch = turretPitchJoint.rotation * Quaternion.AngleAxis(Mathf.Clamp(pitchTurnDegrees, -maxPitch, maxPitch), Vector3.right);
        turretPitchJoint.rotation = ClampPitch(newPitch);
    }

    private Quaternion ClampPitch(Quaternion newRotation)
    {
        float diff = Vector3.SignedAngle(turretYawJoint.forward, newRotation * Vector3.forward, turretYawJoint.right);

        if (diff > 0)
        {
            if (diff < 90)
            {
                return turretYawJoint.rotation;
            }
            else
            {
                return turretYawJoint.rotation * Quaternion.AngleAxis(180, Vector3.right);
            }
        }

        return newRotation;
    }
}
