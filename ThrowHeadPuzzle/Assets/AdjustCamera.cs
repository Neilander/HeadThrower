using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class AdjustCamera : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public Transform 人物身体;
    public float adjustmentSpeed = 0.1f;
    public float maxOrthoSize = 10f;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (!IsObjectInCameraView(人物身体))
        {
            IncreaseOrthoSize();
        }
    }

    bool IsObjectInCameraView(Transform target)
    {
        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(target.position);
        return viewportPoint.x >= 0 && viewportPoint.x <= 1 && viewportPoint.y >= 0 && viewportPoint.y <= 1;
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
