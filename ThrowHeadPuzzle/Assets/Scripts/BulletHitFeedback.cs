using System;
using UnityEngine;

/// <summary>
/// 子弹命中反馈脚本（零依赖 + 零报错 + 中文命名）
/// 文件名：BulletHitFeedback.cs | 无需EventManager/CameraShakeManager
/// </summary>
public class 子弹命中反馈 : MonoBehaviour
{
    // ========== 魔法数字提取为常量 ==========
    private const float 默认粒子停止延时 = 1.5f;
    private const float 默认音效音量 = 0.8f;
    private const float 默认粒子缩放 = 1f;

    // ========== 核心配置（TopDown风格，无冗余） ==========
    [Header("粒子配置")]
    public ParticleSystem 命中粒子;
    public float 粒子播放时长 = 默认粒子停止延时;
    public Vector3 粒子缩放 = new Vector3(默认粒子缩放, 默认粒子缩放, 默认粒子缩放);
    public bool 播放后销毁粒子 = false;

    [Header("音效配置")]
    public AudioClip[] 命中音效列表;

    [Range(0f, 1f)]
    public float 音效音量 = 默认音效音量;

    [Range(0.8f, 1.2f)]
    public float 音效音高范围 = 1f;

    // ========== 初始化（仅处理粒子缩放） ==========
    private void Awake()
    {
        bool 粒子已配置 = 命中粒子 != null;
        if (粒子已配置)
        {
            命中粒子.transform.localScale = 粒子缩放;
        }
    }

    // ========== 外部调用（子弹直接调用这个方法，绕开EventManager） ==========
    /// <summary>
    /// 播放命中反馈（子弹碰撞时直接调用：GetComponent<子弹命中反馈>().播放命中反馈()）
    /// </summary>
    public void 播放命中反馈()
    {
        // 1. 播放粒子
        播放粒子效果();

        // 2. 播放随机音效
        播放随机音效();
    }

    // ========== 核心逻辑（无任何外部依赖） ==========
    private void 播放粒子效果()
    {
        bool 粒子已配置 = 命中粒子 != null;
        if (粒子已配置)
        {
            命中粒子.Clear();
            命中粒子.Play();
            Invoke(nameof(停止粒子), 粒子播放时长);

            if (播放后销毁粒子)
            {
                Destroy(命中粒子.gameObject, 粒子播放时长 + 0.1f);
            }
        }
    }

    private void 停止粒子()
    {
        bool 粒子已配置 = 命中粒子 != null;
        if (粒子已配置 && !播放后销毁粒子)
        {
            命中粒子.Stop();
        }
    }

    private void 播放随机音效()
    {
        bool 音效列表有内容 = 命中音效列表 != null && 命中音效列表.Length > 0;
        if (音效列表有内容)
        {
            int 随机索引 = UnityEngine.Random.Range(0, 命中音效列表.Length);
            AudioClip 选中音效 = 命中音效列表[随机索引];

            // 临时音效物体（无依赖）
            GameObject 临时音效物体 = new GameObject("临时命中音效");
            AudioSource 音频源 = 临时音效物体.AddComponent<AudioSource>();
            音频源.clip = 选中音效;
            音频源.volume = 音效音量;
            音频源.pitch = UnityEngine.Random.Range(0.8f, 音效音高范围);
            音频源.Play();

            Destroy(临时音效物体, 选中音效.length + 0.1f);
        }
    }
}
