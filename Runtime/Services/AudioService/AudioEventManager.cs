using CnoomFrameWork.Base.Events;
using UnityEngine;

namespace CnoomFrameWork.Services.AudioService
{
    /// <summary>
    /// 音频事件管理器，提供便捷的音频事件发布方法
    /// </summary>
    public static class AudioEventManager
    {
        /// <summary>
        /// 播放背景音乐
        /// </summary>
        public static void PlayMusic(AudioClip audioClip, bool loop = true, float fadeInDuration = 0f)
        {
            var musicEvent = EventManager.GetEventObject<PlayMusicEvent>();
            musicEvent.AudioClip = audioClip;
            musicEvent.Loop = loop;
            musicEvent.FadeInDuration = fadeInDuration;
            
            EventManager.Publish(musicEvent);
            EventManager.ReleaseEventObject(musicEvent);
        }

        /// <summary>
        /// 播放背景音乐（通过资源路径）
        /// </summary>
        public static void PlayMusic(string audioPath, bool loop = true, float fadeInDuration = 0f)
        {
            var musicEvent = EventManager.GetEventObject<PlayMusicEvent>();
            musicEvent.AudioPath = audioPath;
            musicEvent.Loop = loop;
            musicEvent.FadeInDuration = fadeInDuration;
            
            EventManager.Publish(musicEvent);
            EventManager.ReleaseEventObject(musicEvent);
        }

        /// <summary>
        /// 停止背景音乐
        /// </summary>
        public static void StopMusic(float fadeOutDuration = 0f)
        {
            var stopEvent = EventManager.GetEventObject<StopMusicEvent>();
            stopEvent.FadeOutDuration = fadeOutDuration;
            
            EventManager.Publish(stopEvent);
            EventManager.ReleaseEventObject(stopEvent);
        }

        /// <summary>
        /// 暂停背景音乐
        /// </summary>
        public static void PauseMusic()
        {
            var pauseEvent = EventManager.GetEventObject<PauseMusicEvent>();
            EventManager.Publish(pauseEvent);
            EventManager.ReleaseEventObject(pauseEvent);
        }

        /// <summary>
        /// 恢复背景音乐
        /// </summary>
        public static void ResumeMusic()
        {
            var resumeEvent = EventManager.GetEventObject<ResumeMusicEvent>();
            EventManager.Publish(resumeEvent);
            EventManager.ReleaseEventObject(resumeEvent);
        }

        /// <summary>
        /// 播放音效
        /// </summary>
        public static int PlaySfx(AudioClip audioClip, float volume = 1f, float pitch = 1f, int priority = 128)
        {
            var sfxEvent = EventManager.GetEventObject<PlaySfxEvent>();
            sfxEvent.AudioClip = audioClip;
            sfxEvent.Volume = volume;
            sfxEvent.Pitch = pitch;
            sfxEvent.Priority = priority;
            
            EventManager.Publish(sfxEvent);
            var sfxId = sfxEvent.SfxId;
            EventManager.ReleaseEventObject(sfxEvent);
            
            return sfxId;
        }

        /// <summary>
        /// 播放音效（通过资源路径）
        /// </summary>
        public static int PlaySfx(string audioPath, float volume = 1f, float pitch = 1f, int priority = 128)
        {
            var sfxEvent = EventManager.GetEventObject<PlaySfxEvent>();
            sfxEvent.AudioPath = audioPath;
            sfxEvent.Volume = volume;
            sfxEvent.Pitch = pitch;
            sfxEvent.Priority = priority;
            
            EventManager.Publish(sfxEvent);
            var sfxId = sfxEvent.SfxId;
            EventManager.ReleaseEventObject(sfxEvent);
            
            return sfxId;
        }

        /// <summary>
        /// 停止指定音效
        /// </summary>
        public static void StopSfx(int sfxId)
        {
            var stopEvent = EventManager.GetEventObject<StopSfxEvent>();
            stopEvent.SfxId = sfxId;
            
            EventManager.Publish(stopEvent);
            EventManager.ReleaseEventObject(stopEvent);
        }

        /// <summary>
        /// 停止所有音效
        /// </summary>
        public static void StopAllSfx()
        {
            var stopAllEvent = EventManager.GetEventObject<StopAllSfxEvent>();
            EventManager.Publish(stopAllEvent);
            EventManager.ReleaseEventObject(stopAllEvent);
        }

        /// <summary>
        /// 设置主音量
        /// </summary>
        public static void SetMasterVolume(float volume)
        {
            var volumeEvent = EventManager.GetEventObject<SetMasterVolumeEvent>();
            volumeEvent.Volume = volume;
            
            EventManager.Publish(volumeEvent);
            EventManager.ReleaseEventObject(volumeEvent);
        }

        /// <summary>
        /// 设置背景音乐音量
        /// </summary>
        public static void SetMusicVolume(float volume)
        {
            var volumeEvent = EventManager.GetEventObject<SetMusicVolumeEvent>();
            volumeEvent.Volume = volume;
            
            EventManager.Publish(volumeEvent);
            EventManager.ReleaseEventObject(volumeEvent);
        }

        /// <summary>
        /// 设置音效音量
        /// </summary>
        public static void SetSfxVolume(float volume)
        {
            var volumeEvent = EventManager.GetEventObject<SetSfxVolumeEvent>();
            volumeEvent.Volume = volume;
            
            EventManager.Publish(volumeEvent);
            EventManager.ReleaseEventObject(volumeEvent);
        }

        /// <summary>
        /// 设置静音状态
        /// </summary>
        public static void SetMute(bool isMuted)
        {
            var muteEvent = EventManager.GetEventObject<SetMuteEvent>();
            muteEvent.IsMuted = isMuted;
            
            EventManager.Publish(muteEvent);
            EventManager.ReleaseEventObject(muteEvent);
        }

        /// <summary>
        /// 预加载音频资源
        /// </summary>
        public static void PreloadAudio(string audioPath)
        {
            var preloadEvent = EventManager.GetEventObject<PreloadAudioEvent>();
            preloadEvent.AudioPath = audioPath;
            
            EventManager.Publish(preloadEvent);
            EventManager.ReleaseEventObject(preloadEvent);
        }

        /// <summary>
        /// 释放音频资源
        /// </summary>
        public static void ReleaseAudio(string audioPath)
        {
            var releaseEvent = EventManager.GetEventObject<ReleaseAudioEvent>();
            releaseEvent.AudioPath = audioPath;
            
            EventManager.Publish(releaseEvent);
            EventManager.ReleaseEventObject(releaseEvent);
        }

        /// <summary>
        /// 清理音频缓存
        /// </summary>
        public static void ClearAudioCache()
        {
            var clearEvent = EventManager.GetEventObject<ClearAudioCacheEvent>();
            EventManager.Publish(clearEvent);
            EventManager.ReleaseEventObject(clearEvent);
        }

        /// <summary>
        /// 订阅音频播放完成事件
        /// </summary>
        public static void SubscribeAudioPlayCompleted(System.Action<AudioPlayCompletedEvent> handler, int priority = 0, bool once = false)
        {
            EventManager.Subscribe(handler, priority, once);
        }

        /// <summary>
        /// 取消订阅音频播放完成事件
        /// </summary>
        public static void UnsubscribeAudioPlayCompleted(System.Action<AudioPlayCompletedEvent> handler)
        {
            EventManager.Unsubscribe(handler);
        }

        /// <summary>
        /// 订阅音频加载完成事件
        /// </summary>
        public static void SubscribeAudioLoadCompleted(System.Action<AudioLoadCompletedEvent> handler, int priority = 0, bool once = false)
        {
            EventManager.Subscribe(handler, priority, once);
        }

        /// <summary>
        /// 取消订阅音频加载完成事件
        /// </summary>
        public static void UnsubscribeAudioLoadCompleted(System.Action<AudioLoadCompletedEvent> handler)
        {
            EventManager.Unsubscribe(handler);
        }

        /// <summary>
        /// 订阅音量变化事件
        /// </summary>
        public static void SubscribeVolumeChanged(System.Action<VolumeChangedEvent> handler, int priority = 0, bool once = false)
        {
            EventManager.Subscribe(handler, priority, once);
        }

        /// <summary>
        /// 取消订阅音量变化事件
        /// </summary>
        public static void UnsubscribeVolumeChanged(System.Action<VolumeChangedEvent> handler)
        {
            EventManager.Unsubscribe(handler);
        }
    }
}