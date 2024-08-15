using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReticuleController : MonoBehaviour
{
    //public Transform reticule;
    public Material crosshairMaterial;
    public RectTransform reticule;
    public RectTransform crosshair;
    public Canvas canvas;

    public float maxDistance;

    public float maxLockAngle = 15f;

    public Transform target;
    public bool targetLocked = false;

    private RectTransform canvasRect;
    private Vector3 crosshairPosition = Vector3.zero;


    private void Start()
    {
        canvasRect = canvas.GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        //CinemachineCore.CameraUpdatedEvent.AddListener(UpdateCrosshairPosition);
    }

    private void Update()
    {

    }

    private void FixedUpdate()
    {
        Scan();
        UpdateTargetLock();
        UpdateReticulePosition();
        UpdateCrosshairPosition();
    }

    private void LateUpdate()
    {

    }

    private void Scan()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, maxDistance))
        {
            //crosshair.position = hit.point;
            //crosshair.rotation = Quaternion.LookRotation(-hit.normal);
            crosshairPosition = hit.point;

            if (hit.transform.TryGetComponent<Destructible>(out Destructible newTarget))
            {
                targetLocked = true;
                target = newTarget.lockPoint;
                crosshairMaterial.SetFloat("_TargetLocked", 1f);
                reticule.gameObject.SetActive(true);
                return;
            }
        }
        else
        {
            crosshairPosition = transform.position + transform.forward * maxDistance;
            //crosshair.position = transform.position + transform.forward * maxDistance;
            //crosshair.rotation = transform.rotation;
        }

        crosshairMaterial.SetFloat("_TargetLocked", 0f);
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

    private delegate void UpdateElement();

    private void UpdateReticulePosition()
    {
        if (!targetLocked) return;

        reticule.anchoredPosition = WorldToCanvasPosition(target.position);
    }

    private void UpdateCrosshairPosition()
    {
        crosshair.anchoredPosition = WorldToCanvasPosition(crosshairPosition);
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
