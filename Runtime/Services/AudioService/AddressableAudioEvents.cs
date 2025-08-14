using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CnoomFrameWork.Services.AudioService
{
    /// <summary>
    /// 通过AssetReference播放音乐事件
    /// </summary>
    public class PlayMusicByReferenceEvent : AudioEventBase
    {
        public AssetReference AudioReference { get; set; }
        public bool Loop { get; set; } = true;
        public float FadeInDuration { get; set; } = 0f;
    }

    /// <summary>
    /// 通过AssetReference播放音效事件
    /// </summary>
    public class PlaySfxByReferenceEvent : AudioEventBase
    {
        public AssetReference AudioReference { get; set; }
        public float Pitch { get; set; } = 1f;
        public int SfxId { get; set; } = -1; // 用于返回音效ID
    }

    /// <summary>
    /// 通过标签批量预加载音频事件
    /// </summary>
    public class PreloadAudioByLabelEvent
    {
        public string Label { get; set; }
        public Action<float> OnProgress { get; set; }
        public Action OnCompleted { get; set; }
    }

    /// <summary>
    /// 通过标签释放音频事件
    /// </summary>
    public class ReleaseAudioByLabelEvent
    {
        public string Label { get; set; }
    }

    /// <summary>
    /// 预加载音频列表事件
    /// </summary>
    public class PreloadAudioListEvent
    {
        public string[] AudioPaths { get; set; }
        public Action<float> OnProgress { get; set; }
        public Action OnCompleted { get; set; }
    }

    /// <summary>
    /// 音频资源下载进度事件
    /// </summary>
    public class AudioDownloadProgressEvent
    {
        public string AudioPath { get; set; }
        public float Progress { get; set; }
        public long DownloadedBytes { get; set; }
        public long TotalBytes { get; set; }
    }

    /// <summary>
    /// 音频资源下载完成事件
    /// </summary>
    public class AudioDownloadCompletedEvent
    {
        public string AudioPath { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }

    /// <summary>
    /// 检查音频资源大小事件
    /// </summary>
    public class CheckAudioSizeEvent
    {
        public string AudioPath { get; set; }
        public Action<long> OnSizeChecked { get; set; }
    }

    /// <summary>
    /// 音频内存使用情况事件
    /// </summary>
    public class AudioMemoryUsageEvent
    {
        public long TotalMemoryUsage { get; set; }
        public int LoadedAudioCount { get; set; }
        public string[] LoadedAudioPaths { get; set; }
    }
}