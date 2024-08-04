using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public Transform target;
    protected Rigidbody targetRb;
    protected Rigidbody rb;

    protected Vector3 targetAngle;
    protected Vector3 errorDirection;
    protected Vector3 pitchError;
    protected Vector3 yawError;
    protected Vector3 rollError;

    public Vector3 scanPointOffset = Vector3.forward * 2;
    public float minScanRange = 30f;

    public float minScanAngle = 30;
    public float maxScanAngle = 90;
    public float maxScanAngleVelocity = 40f;

    public int scanPrecision = 2;

    protected ObstacleCheck obstacleReport;

    private void Awake()
    {
        obstacleReport = new();
    }

    // Start is called before the first frame update
    public virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    public virtual void Update() { }

    public void SetTarget(Transform target)
    {
        this.target = target;
        targetRb = target.GetComponent<Rigidbody>();
    }

    public void UpdateAimValues()
    {
        UpdateAimValues(target.position);
        targetAngle = target.position - transform.position;
    }

    public void UpdateAimValues(Vector3 aimPoint)
    {
        targetAngle = aimPoint - transform.position;
        Vector3 error = Quaternion.Inverse(transform.rotation) * targetAngle;

        errorDirection = error.normalized;
        pitchError = new Vector3(0f, error.y, error.z).normalized;
        yawError = new Vector3(error.x, 0f, error.z).normalized;
        rollError = new Vector3(error.x, error.y, 0f).normalized;
    }

    public class ScanHit
    {
        public bool isHit = false;
        public Transform transform;
        public float distance = Mathf.Infinity;

        public void CheckAgainst(RaycastHit hit)
        {
            if (!isHit || hit.distance < distance)
            {
                transform = hit.transform;
                distance = hit.distance;
                isHit = true;
            }
        }

        public bool CompareObjects(ScanHit other)
        {
            Debug.Log($"This: {isHit} {transform}; Other: {isHit} {other.transform}");

            if (!(isHit && other.isHit)) return false;
            return transform.root == other.transform.root;
        }
    }

    protected class ObstacleCheck
    {
        public bool pathObstruction = false;
        public ScanHit up, down, left, right = new();

        public float scanRange;
        public float scanAngle;

        private float pitchCorrection, rollCorrection, yawCorrection = 0f;

        public bool DangerAhead()
        {
            return (CompareTransforms(up, down) && CompareTransforms(down, left) && CompareTransforms(left, right));
        }

        private bool CompareTransforms(ScanHit a, ScanHit b)
        {
            if (a == null || b == null) return false;
            return a.isHit && b.isHit && a.transform.root == b.transform.root;
        }

        private float CalculatePitchCorrection()
        {
            if (!up.isHit && !down.isHit) return 0f;
            if (down.distance < up.distance) return -1f;
            return 1f;
        }

        private float CalculateYawCorrection()
        {
            if (left.distance > scanRange && right.distance > scanRange) return 0f;
            if (right.distance < left.distance) return 1f;
            return -1f;
        }

        private float CalculateRollCorrection()
        {
            if (pitchCorrection != 0f) return 0f;
            else if (yawCorrection != 0f) return yawCorrection;
            return 0f;
        }

        public void UpdateCorrectionValues()
        {
            pitchCorrection = CalculatePitchCorrection();
            yawCorrection = CalculateYawCorrection();
            rollCorrection = CalculateRollCorrection();
        }

        public Vector3 GetCorrectionValues()
        {
            return new(pitchCorrection, rollCorrection, yawCorrection);
        }
    }

    protected virtual void ScanForObstacles(float scanRangeModifier)
    {
        Vector3 scanOrigin = transform.position + scanPointOffset;

        float currentSpeed = rb.velocity.magnitude;
        obstacleReport.scanRange = (minScanRange * scanRangeModifier) + currentSpeed;
        obstacleReport.scanAngle = Mathf.Lerp(maxScanAngle, minScanAngle, currentSpeed / maxScanAngleVelocity);

        obstacleReport.up = ScanSingleDirection(scanOrigin, transform.right);
        obstacleReport.down = ScanSingleDirection(scanOrigin, -transform.right);
        obstacleReport.left = ScanSingleDirection(scanOrigin, -transform.up);
        obstacleReport.right = ScanSingleDirection(scanOrigin, transform.up);

        obstacleReport.UpdateCorrectionValues();
    }

    protected virtual void ScanForObstacles()
    {
        ScanForObstacles(1);
    }

    private ScanHit ScanSingleDirection(Vector3 scanOrigin, Vector3 axis)
    {
        float angle = obstacleReport.scanAngle / scanPrecision;

        Quaternion rotation;
        RaycastHit hit;
        ScanHit scanHit = new ScanHit();

        for (int i = 1; i <= scanPrecision; i++)
        {
            rotation = Quaternion.Euler(axis * (angle * i));

            if (Physics.Raycast(scanOrigin, rotation * transform.forward, out hit, obstacleReport.scanRange))
            {
                Debug.DrawLine(scanOrigin, hit.point, Color.red, .1f);
                scanHit.CheckAgainst(hit);
                
                //dist = Mathf.Min(hit.distance, dist);
                
                //scanHit.distance = dist;
                //scanHit.transform = hit.transform;
                //scanHit.isHit = true;
            }
            else
            {
                Debug.DrawRay(scanOrigin, rotation * transform.forward * obstacleReport.scanRange, Color.green, .1f);
            }
        }

        return scanHit;
    }
}
