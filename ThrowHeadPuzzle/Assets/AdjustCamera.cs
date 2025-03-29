using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class AdjustCamera : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public Transform 人物身体;
    public float adjustmentSpeed = 0.1f;
    public float maxOrthoSize = 20f;
    public float minOrthoSize = 3f;


    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (!IsObjectOutCameraView(人物身体))
        {
            IncreaseOrthoSize();
        }
        else if (IsObjectMuchInCameraView(人物身体))
        {
            DecreaseOrthoSize();
        }
    }

    private void DecreaseOrthoSize()
    {
        float currentOrthoSize = virtualCamera.m_Lens.OrthographicSize;
        if (currentOrthoSize > minOrthoSize)
        {
            virtualCamera.m_Lens.OrthographicSize -= adjustmentSpeed * Time.deltaTime;
        }
    }

    bool IsObjectOutCameraView(Transform target)
    {
        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(target.position);
        Debug.Log(viewportPoint);
        return viewportPoint.x > 0 && viewportPoint.x < 1 && viewportPoint.y > 0 && viewportPoint.y < 1;
    }

    bool IsObjectMuchInCameraView(Transform target)
    {
        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(target.position);
        return (viewportPoint.x > 0.2 && viewportPoint.x < 0.4) || (viewportPoint.x > 0.6 && viewportPoint.x < 0.8) || (viewportPoint.y > 0.2 && viewportPoint.y < 0.4) || (viewportPoint.y > 0.6 && viewportPoint.y < 0.8);
    }

    void IncreaseOrthoSize()
    {
        float currentOrthoSize = virtualCamera.m_Lens.OrthographicSize;
        if (currentOrthoSize < maxOrthoSize)
        {
            virtualCamera.m_Lens.OrthographicSize += adjustmentSpeed * Time.deltaTime;
        }
    }
}
