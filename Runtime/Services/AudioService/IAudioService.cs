using System;
using UnityEngine;

namespace CnoomFrameWork.Services.AudioService
{
    /// <summary>
    /// 音频服务接口，提供音乐和音效播放功能
    /// </summary>
    public interface IAudioService : CnoomFrameWork.Core.IService
    {
        /// <summary>
        /// 主音量 (0-1)
        /// </summary>
        float MasterVolume { get; set; }
        
        /// <summary>
        /// 背景音乐音量 (0-1)
        /// </summary>
        float MusicVolume { get; set; }
        
        /// <summary>
        /// 音效音量 (0-1)
        /// </summary>
        float SfxVolume { get; set; }
        
        /// <summary>
        /// 是否静音
        /// </summary>
        bool IsMuted { get; set; }

        /// <summary>
        /// 播放背景音乐
        /// </summary>
        /// <param name="audioClip">音频剪辑</param>
        /// <param name="loop">是否循环</param>
        /// <param name="fadeInDuration">淡入时长</param>
        void PlayMusic(AudioClip audioClip, bool loop = true, float fadeInDuration = 0f);
        
        /// <summary>
        /// 播放背景音乐（通过资源路径）
        /// </summary>
        /// <param name="audioPath">音频资源路径</param>
        /// <param name="loop">是否循环</param>
        /// <param name="fadeInDuration">淡入时长</param>
        void PlayMusic(string audioPath, bool loop = true, float fadeInDuration = 0f);
        
        /// <summary>
        /// 停止背景音乐
        /// </summary>
        /// <param name="fadeOutDuration">淡出时长</param>
        void StopMusic(float fadeOutDuration = 0f);
        
        /// <summary>
        /// 暂停背景音乐
        /// </summary>
        void PauseMusic();
        
        /// <summary>
        /// 恢复背景音乐
        /// </summary>
        void ResumeMusic();
        
        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="audioClip">音频剪辑</param>
        /// <param name="volume">音量 (0-1)</param>
        /// <param name="pitch">音调 (0.1-3)</param>
        /// <param name="priority">优先级 (0-256)</param>
        /// <returns>音效播放ID</returns>
        int PlaySfx(AudioClip audioClip, float volume = 1f, float pitch = 1f, int priority = 128);
        
        /// <summary>
        /// 播放音效（通过资源路径）
        /// </summary>
        /// <param name="audioPath">音频资源路径</param>
        /// <param name="volume">音量 (0-1)</param>
        /// <param name="pitch">音调 (0.1-3)</param>
        /// <param name="priority">优先级 (0-256)</param>
        /// <returns>音效播放ID</returns>
        int PlaySfx(string audioPath, float volume = 1f, float pitch = 1f, int priority = 128);
        
        /// <summary>
        /// 停止指定音效
        /// </summary>
        /// <param name="sfxId">音效播放ID</param>
        void StopSfx(int sfxId);
        
        /// <summary>
        /// 停止所有音效
        /// </summary>
        void StopAllSfx();
        
        /// <summary>
        /// 预加载音频资源
        /// </summary>
        /// <param name="audioPath">音频资源路径</param>
        /// <param name="callback">加载完成回调</param>
        void PreloadAudio(string audioPath, Action<AudioClip> callback = null);
        
        /// <summary>
        /// 释放音频资源
        /// </summary>
        /// <param name="audioPath">音频资源路径</param>
        void ReleaseAudio(string audioPath);
        
        /// <summary>
        /// 清理所有缓存的音频资源
        /// </summary>
        void ClearAudioCache();
    }
}