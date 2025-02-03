using Cinemachine;
using SpaceGame.Utils;
using UnityEngine;
using Weapons;


public class ReticuleController : MonoBehaviour
{
    public Material crosshairMaterial;
    public RectTransform reticule;
    public RectTransform crosshair;
    public Canvas canvas;

    public float maxDistance;

    public float maxLockAngle = 15f;
    public float maxAssistAngle = 0f;
    public float scanRadius = 10f;

    public Transform target;
    public bool targetLocked = false;
    public bool aimAssisting = false;
    public Vector3 assistVector = Vector3.zero;

    private RectTransform canvasRect;
    [SerializeField] public Transform aimTransform;

    public delegate void OnAimAssistUpdated(bool assist, Targetable target);
    public OnAimAssistUpdated onAimAssist;

    private RaycastHit[] scanHits = new RaycastHit[20];

    private WeaponSystem[] weapons;

    private delegate void OnUpdate();
    private OnUpdate onFixedUpdate;
    private OnUpdate onLateUpdate;

    private void Awake()
    {
        onFixedUpdate = () => { };
        onLateUpdate = () => { };
        canvasRect = canvas.GetComponent<RectTransform>();
    }

    private void Start()
    {
        canvas.worldCamera = CameraUtils.UICamera;
    }

    private void OnEnable()
    {
        CinemachineCore.CameraUpdatedEvent.AddListener(UpdateCrosshairPosition);
        CinemachineCore.CameraUpdatedEvent.AddListener(UpdateReticulePosition);
    }

    private void OnDisable()
    {
        CinemachineCore.CameraUpdatedEvent.RemoveListener(UpdateCrosshairPosition);
        CinemachineCore.CameraUpdatedEvent.RemoveListener(UpdateReticulePosition);
    }

    private void FixedUpdate()
    {
        onFixedUpdate();
    }

    public void AssignWeapons(WeaponSystem mainWeapons, WeaponSystem secondaryWeapons)
    {
        weapons = new WeaponSystem[] { mainWeapons, secondaryWeapons };

        weapons[0].SetAimTransform(aimTransform);
        weapons[1].SetAimTransform(aimTransform);

        onFixedUpdate = () =>
        {
            Scan();
            UpdateTargetLock();
        };
    }

    private void Scan()
    {
        RaycastHit hit;
        Ray ray = new(transform.position, transform.forward);

        if (ScanAhead(out Targetable newTarget))
        {
            target = newTarget.lockPoint;
            targetLocked = true;
            aimAssisting = true;

            crosshairMaterial.SetFloat("_TargetLocked", 1f);
            aimTransform.position = target.position;

            reticule.gameObject.SetActive(true);
            onAimAssist?.Invoke(true, newTarget);
            return;
        }

        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            aimTransform.position = hit.point;

            //if (hit.transform.TryGetComponent<Targetable>(out Targetable newTarget) && newTarget.active)
            //{
            //    targetLocked = true;
            //    aimAssisting = true;
            //    target = newTarget.lockPoint;
            //    crosshairMaterial.SetFloat("_TargetLocked", 1f);
            //    reticule.gameObject.SetActive(true);
            //    return;
            //}
        }
        else
        {
            aimTransform.position = transform.position + transform.forward * maxDistance;
        }

        aimAssisting = false;
        onAimAssist?.Invoke(false, null);
        crosshairMaterial.SetFloat("_TargetLocked", 0f);
    }

    private bool ScanAhead(out Targetable target)
    {
        Ray ray = new(transform.position + transform.forward * (1 + scanRadius), transform.forward);
        int hits = Physics.SphereCastNonAlloc(ray, scanRadius, scanHits, maxDistance);
        float bestAngle = maxAssistAngle;
        float bestDist = maxDistance;
        target = null;

        for (int i = 0; i < hits; i++)
        {
            RaycastHit hit = scanHits[i];

            if (hit.distance < bestDist 
                && hit.transform.TryGetComponent<Targetable>(out Targetable newTarget)
                && newTarget.active)
            {
                Vector3 targetVector = newTarget.lockPoint.position - transform.position;
                float targetAngle = Vector3.Angle(transform.forward, targetVector);

                if (targetAngle < bestAngle)
                {
                    bestAngle = targetAngle;
                    bestDist = hit.distance;
                    target = newTarget;
                }
            }
        }

        return bestAngle != maxAssistAngle;
    }

    private void UpdateTargetLock()
    {
        if (!targetLocked) return;

        Vector3 targetVector = target.position - transform.position;
        float targetAngle = Vector3.Angle(transform.forward, targetVector);
        if (targetAngle > maxLockAngle || targetVector.magnitude > maxDistance)
        {
            targetLocked = false;
            reticule.gameObject.SetActive(false);
            //reticuleMaterial.SetFloat("_TargetLocked", 0f);
        }
    }

    private void UpdateReticulePosition()
    {
        if (!targetLocked) return;

        reticule.anchoredPosition = WorldToCanvasPosition(target.position);
    }

    private void UpdateReticulePosition(Cinemachine.CinemachineBrain cinemachineBrain)
    {
        UpdateReticulePosition();
    }

    private void UpdateCrosshairPosition()
    {
        crosshair.anchoredPosition = WorldToCanvasPosition(aimTransform.position);
    }

    private void UpdateCrosshairPosition(Cinemachine.CinemachineBrain cinemachineBrain)
    {
        UpdateCrosshairPosition();
    }

    private Vector2 WorldToCanvasPosition(Vector3 position)
    {
        Vector2 viewportPoint = Camera.main.WorldToViewportPoint(position);
        Vector2 screenPosition = new Vector2(
            ((viewportPoint.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)),
            ((viewportPoint.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f))
            );
        return screenPosition;
    }
}
