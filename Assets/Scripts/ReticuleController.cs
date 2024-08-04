using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReticuleController : MonoBehaviour
{
    //public Transform reticule;
    public Material reticuleMaterial;
    public RectTransform reticule;
    public Canvas canvas;

    public float maxDistance;

    public float maxLockAngle = 15f;

    public Transform target;
    public bool targetLocked = false;


    private void Start()
    {
        
    }

    private void Update()
    {
        //Debug.Log(Camera.main.WorldToScreenPoint(transform.position));
    }

    private void FixedUpdate()
    {
        Scan();
        UpdateTargetLock();
        UpdateReticulePosition();
    }

    private void LateUpdate()
    {

    }

    private void Scan()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, maxDistance))
        {
            //reticule.position = hit.point;
            //reticule.rotation = Quaternion.LookRotation(-hit.normal);

            if (hit.transform.TryGetComponent<Destructible>(out Destructible newTarget))
            {
                targetLocked = true;
                target = newTarget.lockPoint;
                reticuleMaterial.SetFloat("_TargetLocked", 1f);
            }
        }
        else
        {
            //reticule.position = transform.position + transform.forward * maxDistance;
            //reticule.rotation = transform.rotation;
        }
    }

    private void UpdateTargetLock()
    {
        if (!targetLocked) return;

        Vector3 targetVector = target.position - transform.position;
        float targetAngle = Vector3.Angle(transform.forward, targetVector);
        if (targetAngle > maxLockAngle || targetVector.magnitude > maxDistance)
        {
            targetLocked = false;
            reticuleMaterial.SetFloat("_TargetLocked", 0f);
        }
    }

    private void UpdateReticulePosition()
    {
        //if (targetLocked)
        //{
        //    reticule.position = target.position;
        //}
        //else
        //{
        //    reticule.position = transform.position + transform.forward * maxDistance;
        //}

        //Vector3 cameraDirection = (reticule.position - Camera.main.transform.position).normalized;
        //reticule.position = Camera.main.transform.position + cameraDirection * 50;

        Vector2 viewportPoint;

        if (targetLocked)
        {
            viewportPoint = Camera.main.WorldToViewportPoint(target.position);
        }
        else
        {
            viewportPoint = Camera.main.WorldToViewportPoint(transform.position + transform.forward * maxDistance);
        }

        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        Vector2 screenPosition = new Vector2(
            ((viewportPoint.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)),
            ((viewportPoint.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f))
            );


        //reticule.localPosition = viewportPoint - new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2);
        reticule.anchoredPosition = screenPosition;
    }
}
