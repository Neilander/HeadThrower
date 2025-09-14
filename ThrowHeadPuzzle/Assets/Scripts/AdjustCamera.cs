using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class AdjustCamera : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public Transform 人物_身体;

    //public Transform 人物_头;

    [SerializeField]
    float adjustmentSpeed = 0.1f;

    [SerializeField]
    public float maxOrthoSize = 20f;

    [SerializeField]
    public float minOrthoSize = 3f;

    [SerializeField]
    float changeVelocity;

    [SerializeField]
    float smoothTime;
    public float edgeThreshold = 0.3f; // 边缘阈值 (0.8 = 80%屏幕宽度)
    public float minedgeThreshold = 0.2f; // 边缘阈值 (0.8 = 80%屏幕宽度)
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // 总逻辑
        // 头 超出<施密特曲线的>画框线=>>更新相机尺寸
        //计算与屏幕边缘的距离
        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(人物_身体.position);

        float distanceToEdge = Mathf.Max(
            Mathf.Abs(viewportPoint.x - 0.5f) * 1f, // 水平方向距离
            Mathf.Abs(viewportPoint.y - 0.5f) * 1f // 垂直方向距离
        );
        //        Debug.Log(distanceToEdge);
        if (distanceToEdge > edgeThreshold)
        {
            IncreaseOrthoSize();
        }
        else if (distanceToEdge < minedgeThreshold)
        {
            DecreaseOrthoSize();
        }
    }

    private void DecreaseOrthoSize()
    {
        float currentOrthoSize = virtualCamera.m_Lens.OrthographicSize;
        if (currentOrthoSize > minOrthoSize)
        {
            // virtualCamera.m_Lens.OrthographicSize -= adjustmentSpeed * Time.deltaTime;

            float targetOrthographicSize = currentOrthoSize - adjustmentSpeed * Time.deltaTime;
            virtualCamera.m_Lens.OrthographicSize = Mathf.SmoothDamp(
                currentOrthoSize,
                targetOrthographicSize,
                ref changeVelocity,
                smoothTime
            );
        }
    }

    // bool IsObjectOutCameraView(Transform target)
    // {
    //     Vector3 viewportPoint = mainCamera.WorldToViewportPoint(target.position);
    //     Debug.Log(viewportPoint);
    //     return viewportPoint.x > 0.85
    //         || viewportPoint.x < 0.15
    //         || viewportPoint.y > 0.85
    //         || viewportPoint.y < 0.15;
    // }

    // bool IsObjectMuchInCameraView(Transform target)
    // {
    //     Vector3 viewportPoint = mainCamera.WorldToViewportPoint(target.position);
    //     // return (viewportPoint.x > 0.2 && viewportPoint.x < 0.4)
    //     //     || (viewportPoint.x > 0.6 && viewportPoint.x < 0.8)
    //     //     || (viewportPoint.y > 0.2 && viewportPoint.y < 0.3)
    //     //     || (viewportPoint.y > 0.7 && viewportPoint.y < 0.8);
    //     return (viewportPoint.x > 0.2)
    //         || (viewportPoint.x < 0.8)
    //         || (viewportPoint.y > 0.2)
    //         || (viewportPoint.y < 0.8);
    // }

    void IncreaseOrthoSize()
    {
        float currentOrthoSize = virtualCamera.m_Lens.OrthographicSize;
        if (currentOrthoSize < maxOrthoSize)
        {
            //virtualCamera.m_Lens.OrthographicSize += adjustmentSpeed * Time.deltaTime;
            float targetOrthographicSize = currentOrthoSize + adjustmentSpeed * Time.deltaTime;
            virtualCamera.m_Lens.OrthographicSize = Mathf.SmoothDamp(
                virtualCamera.m_Lens.OrthographicSize,
                targetOrthographicSize,
                ref changeVelocity,
                smoothTime
            );
        }
    }
}
