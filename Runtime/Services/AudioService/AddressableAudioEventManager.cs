using System;
using CnoomFrameWork.Base.Events;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CnoomFrameWork.Services.AudioService
{
    /// <summary>
    /// Addressable音频事件管理器，提供基于Addressable的音频事件发布方法
    /// </summary>
    public static class AddressableAudioEventManager
    {
        /// <summary>
        /// 通过AssetReference播放背景音乐
        /// </summary>
        public static void PlayMusicByReference(AssetReference audioReference, bool loop = true, float fadeInDuration = 0f)
        {
            var musicEvent = EventManager.GetEventObject<PlayMusicByReferenceEvent>();
            musicEvent.AudioReference = audioReference;
            musicEvent.Loop = loop;
            musicEvent.FadeInDuration = fadeInDuration;
            
            EventManager.Publish(musicEvent);
            EventManager.ReleaseEventObject(musicEvent);
        }

        /// <summary>
        /// 通过AssetReference播放音效
        /// </summary>
        public static int PlaySfxByReference(AssetReference audioReference, float volume = 1f, float pitch = 1f, int priority = 128)
        {
            var sfxEvent = EventManager.GetEventObject<PlaySfxByReferenceEvent>();
            sfxEvent.AudioReference = audioReference;
            sfxEvent.Volume = volume;
            sfxEvent.Pitch = pitch;
            sfxEvent.Priority = priority;
            
            EventManager.Publish(sfxEvent);
            var sfxId = sfxEvent.SfxId;
            EventManager.ReleaseEventObject(sfxEvent);
            
            return sfxId;
        }

        /// <summary>
        /// 通过标签批量预加载音频
        /// </summary>
        public static void PreloadAudioByLabel(string label, Action<float> onProgress = null, Action onCompleted = null)
        {
            var preloadEvent = EventManager.GetEventObject<PreloadAudioByLabelEvent>();
            preloadEvent.Label = label;
            preloadEvent.OnProgress = onProgress;
            preloadEvent.OnCompleted = onCompleted;
            
            EventManager.Publish(preloadEvent);
            EventManager.ReleaseEventObject(preloadEvent);
        }

        /// <summary>
        /// 通过标签释放音频资源
        /// </summary>
        public static void ReleaseAudioByLabel(string label)
        {
            var releaseEvent = EventManager.GetEventObject<ReleaseAudioByLabelEvent>();
            releaseEvent.Label = label;
            
            EventManager.Publish(releaseEvent);
            EventManager.ReleaseEventObject(releaseEvent);
        }

        /// <summary>
        /// 预加载音频列表
        /// </summary>
        public static void PreloadAudioList(string[] audioPaths, Action<float> onProgress = null, Action onCompleted = null)
        {
            var preloadEvent = EventManager.GetEventObject<PreloadAudioListEvent>();
            preloadEvent.AudioPaths = audioPaths;
            preloadEvent.OnProgress = onProgress;
            preloadEvent.OnCompleted = onCompleted;
            
            EventManager.Publish(preloadEvent);
            EventManager.ReleaseEventObject(preloadEvent);
        }

        /// <summary>
        /// 检查音频资源大小
        /// </summary>
        public static void CheckAudioSize(string audioPath, Action<long> onSizeChecked)
        {
            var checkEvent = EventManager.GetEventObject<CheckAudioSizeEvent>();
            checkEvent.AudioPath = audioPath;
            checkEvent.OnSizeChecked = onSizeChecked;
            
            EventManager.Publish(checkEvent);
            EventManager.ReleaseEventObject(checkEvent);
        }

        /// <summary>
        /// 订阅音频下载进度事件
        /// </summary>
        public static void SubscribeAudioDownloadProgress(Action<AudioDownloadProgressEvent> handler, int priority = 0, bool once = false)
        {
            EventManager.Subscribe(handler, priority, once);
        }

        /// <summary>
        /// 取消订阅音频下载进度事件
        /// </summary>
        public static void UnsubscribeAudioDownloadProgress(Action<AudioDownloadProgressEvent> handler)
        {
            EventManager.Unsubscribe(handler);
        }

        /// <summary>
        /// 订阅音频下载完成事件
        /// </summary>
        public static void SubscribeAudioDownloadCompleted(Action<AudioDownloadCompletedEvent> handler, int priority = 0, bool once = false)
        {
            EventManager.Subscribe(handler, priority, once);
        }

        /// <summary>
        /// 取消订阅音频下载完成事件
        /// </summary>
        public static void UnsubscribeAudioDownloadCompleted(Action<AudioDownloadCompletedEvent> handler)
        {
            EventManager.Unsubscribe(handler);
        }

        /// <summary>
        /// 订阅音频内存使用情况事件
        /// </summary>
        public static void SubscribeAudioMemoryUsage(Action<AudioMemoryUsageEvent> handler, int priority = 0, bool once = false)
        {
            EventManager.Subscribe(handler, priority, once);
        }

        /// <summary>
        /// 取消订阅音频内存使用情况事件
        /// </summary>
        public static void UnsubscribeAudioMemoryUsage(Action<AudioMemoryUsageEvent> handler)
        {
            EventManager.Unsubscribe(handler);
        }
    }
}