using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CameraZoomController : MonoBehaviour
{
    private CinemachineVirtualCamera _vcam;
    private CinemachineFramingTransposer _transposer;

    [Section("自动模式设置")]
    // 记录你原始在 Inspector 里设置的构图模式
    public CinemachineFramingTransposer.FramingMode autoFramingMode = CinemachineFramingTransposer
        .FramingMode
        .HorizontalAndVertical;

    [Section("手动模式设置")]
    public float zoomSensitivity = 2f;
    public float minOrthoSize = 3f;
    public float maxOrthoSize = 20f;
    public KeyCode resetKey = KeyCode.Space; // 按空格恢复自动

    private bool _isManualMode = false;

    void Start()
    {
        _vcam = GetComponent<CinemachineVirtualCamera>();
        // 获取 FramingTransposer 组件
        _transposer = _vcam.GetCinemachineComponent<CinemachineFramingTransposer>();

        if (_transposer == null)
        {
            Debug.LogError("请确保虚拟相机的 Body 设置为 Framing Transposer!");
        }
    }

    void Update()
    {
        if (_transposer == null)
            return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        // --- 1. 检测滚轮输入，进入手动模式 ---
        if (Mathf.Abs(scroll) > 0.01f)
        {
            EnterManualMode();
            ApplyManualZoom(scroll);
        }

        // --- 2. 检测恢复按键，重回自动模式 ---
        if (Input.GetKeyDown(resetKey))
        {
            ExitManualMode();
        }
    }

    private void EnterManualMode()
    {
        if (_isManualMode)
            return;

        _isManualMode = true;
        // 关键：关闭 Cinemachine 的自动构图缩放
        _transposer.m_GroupFramingMode = CinemachineFramingTransposer.FramingMode.None;
        Debug.Log("进入手动缩放模式");
    }

    private void ExitManualMode()
    {
        if (!_isManualMode)
            return;

        _isManualMode = false;
        // 关键：恢复 Cinemachine 的自动构图缩放
        _transposer.m_GroupFramingMode = autoFramingMode;
        Debug.Log("恢复自动追踪模式");
    }

    private void ApplyManualZoom(float scroll)
    {
        // 手动调整 OrthographicSize
        float currentSize = _vcam.m_Lens.OrthographicSize;
        currentSize -= scroll * zoomSensitivity;
        currentSize = Mathf.Clamp(currentSize, minOrthoSize, maxOrthoSize);

        _vcam.m_Lens.OrthographicSize = currentSize;
    }
}
