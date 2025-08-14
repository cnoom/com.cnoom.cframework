using UnityEngine;

namespace CnoomFrameWork.Services.AudioService
{
    /// <summary>
    /// 音频事件基类
    /// </summary>
    public abstract class AudioEventBase
    {
        public float Volume { get; set; } = 1f;
        public int Priority { get; set; } = 128;
    }

    /// <summary>
    /// 播放背景音乐事件
    /// </summary>
    public class PlayMusicEvent : AudioEventBase
    {
        public AudioClip AudioClip { get; set; }
        public string AudioPath { get; set; }
        public bool Loop { get; set; } = true;
        public float FadeInDuration { get; set; } = 0f;
    }

    /// <summary>
    /// 停止背景音乐事件
    /// </summary>
    public class StopMusicEvent
    {
        public float FadeOutDuration { get; set; } = 0f;
    }

    /// <summary>
    /// 暂停背景音乐事件
    /// </summary>
    public class PauseMusicEvent
    {
    }

    /// <summary>
    /// 恢复背景音乐事件
    /// </summary>
    public class ResumeMusicEvent
    {
    }

    /// <summary>
    /// 播放音效事件
    /// </summary>
    public class PlaySfxEvent : AudioEventBase
    {
        public AudioClip AudioClip { get; set; }
        public string AudioPath { get; set; }
        public float Pitch { get; set; } = 1f;
        public int SfxId { get; set; } = -1; // 用于返回音效ID
    }

    /// <summary>
    /// 停止指定音效事件
    /// </summary>
    public class StopSfxEvent
    {
        public int SfxId { get; set; }
    }

    /// <summary>
    /// 停止所有音效事件
    /// </summary>
    public class StopAllSfxEvent
    {
    }

    /// <summary>
    /// 设置主音量事件
    /// </summary>
    public class SetMasterVolumeEvent
    {
        public float Volume { get; set; }
    }

    /// <summary>
    /// 设置背景音乐音量事件
    /// </summary>
    public class SetMusicVolumeEvent
    {
        public float Volume { get; set; }
    }

    /// <summary>
    /// 设置音效音量事件
    /// </summary>
    public class SetSfxVolumeEvent
    {
        public float Volume { get; set; }
    }

    /// <summary>
    /// 设置静音状态事件
    /// </summary>
    public class SetMuteEvent
    {
        public bool IsMuted { get; set; }
    }

    /// <summary>
    /// 预加载音频事件
    /// </summary>
    public class PreloadAudioEvent
    {
        public string AudioPath { get; set; }
    }

    /// <summary>
    /// 释放音频事件
    /// </summary>
    public class ReleaseAudioEvent
    {
        public string AudioPath { get; set; }
    }

    /// <summary>
    /// 清理音频缓存事件
    /// </summary>
    public class ClearAudioCacheEvent
    {
    }

    /// <summary>
    /// 音频播放完成事件
    /// </summary>
    public class AudioPlayCompletedEvent
    {
        public int SfxId { get; set; }
        public AudioClip AudioClip { get; set; }
        public bool IsMusic { get; set; }
    }

    /// <summary>
    /// 音频加载完成事件
    /// </summary>
    public class AudioLoadCompletedEvent
    {
        public string AudioPath { get; set; }
        public AudioClip AudioClip { get; set; }
        public bool Success { get; set; }
    }

    /// <summary>
    /// 音量变化事件
    /// </summary>
    public class VolumeChangedEvent
    {
        public enum VolumeType
        {
            Master,
            Music,
            Sfx
        }

        public VolumeType Type { get; set; }
        public float OldVolume { get; set; }
        public float NewVolume { get; set; }
    }
}