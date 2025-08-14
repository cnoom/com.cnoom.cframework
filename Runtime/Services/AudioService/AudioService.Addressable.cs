using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CnoomFrameWork.Base.Events;
using CnoomFrameWork.Core;
using CnoomFrameWork.Modules.AddressableModule;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CnoomFrameWork.Services.AudioService
{
    /// <summary>
    /// AudioService的Addressable扩展部分
    /// </summary>
    public partial class AudioService
    {
        private AssetsService _assetsService;
        private readonly Dictionary<string, long> _audioSizeCache = new();

        private void InitializeAddressableSupport()
        {
            _assetsService = App.Instance.ServiceLocator.GetService<AssetsService>();
        }

        #region Addressable事件处理方法

        [EventSubscriber(typeof(PlayMusicByReferenceEvent))]
        private void OnPlayMusicByReference(PlayMusicByReferenceEvent e)
        {
            if (e.AudioReference != null && e.AudioReference.RuntimeKeyIsValid())
            {
                _assetsService.LoadByReference<AudioClip>(e.AudioReference, audioClip =>
                {
                    if (audioClip != null)
                    {
                        PlayMusic(audioClip, e.Loop, e.FadeInDuration);
                    }
                });
            }
        }

        [EventSubscriber(typeof(PlaySfxByReferenceEvent))]
        private void OnPlaySfxByReference(PlaySfxByReferenceEvent e)
        {
            if (e.AudioReference != null && e.AudioReference.RuntimeKeyIsValid())
            {
                _assetsService.LoadByReference<AudioClip>(e.AudioReference, audioClip =>
                {
                    if (audioClip != null)
                    {
                        e.SfxId = PlaySfx(audioClip, e.Volume, e.Pitch, e.Priority);
                    }
                });
            }
        }

        [EventSubscriber(typeof(PreloadAudioByLabelEvent))]
        private void OnPreloadAudioByLabel(PreloadAudioByLabelEvent e)
        {
            _audioHolder.StartCoroutine(PreloadAudioByLabelCoroutine(e.Label, e.OnProgress, e.OnCompleted));
        }

        [EventSubscriber(typeof(ReleaseAudioByLabelEvent))]
        private void OnReleaseAudioByLabel(ReleaseAudioByLabelEvent e)
        {
            _assetsService.ReleaseOfLabel(e.Label);
            
            // 从缓存中移除相关音频
            var keysToRemove = _audioCache.Keys.Where(key => key.Contains(e.Label)).ToList();
            foreach (var key in keysToRemove)
            {
                _audioCache.Remove(key);
            }
        }

        [EventSubscriber(typeof(PreloadAudioListEvent))]
        private void OnPreloadAudioList(PreloadAudioListEvent e)
        {
            _audioHolder.StartCoroutine(PreloadAudioListCoroutine(e.AudioPaths, e.OnProgress, e.OnCompleted));
        }

        [EventSubscriber(typeof(CheckAudioSizeEvent))]
        private void OnCheckAudioSize(CheckAudioSizeEvent e)
        {
            _audioHolder.StartCoroutine(CheckAudioSizeCoroutine(e.AudioPath, e.OnSizeChecked));
        }

        #endregion

        #region Addressable协程方法

        private IEnumerator PreloadAudioByLabelCoroutine(string label, Action<float> onProgress, Action onCompleted)
        {
            var loadedCount = 0;
            var totalCount = 0;
            
            // 先获取标签下的所有音频资源数量
            var locationHandle = Addressables.LoadResourceLocationsAsync(label, typeof(AudioClip));
            yield return locationHandle;
            
            if (locationHandle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                totalCount = locationHandle.Result.Count;
                
                _assetsService.LoadAssetsByLabel<AudioClip>(label, audioClip =>
                {
                    if (audioClip != null && _config.EnableAudioCache)
                    {
                        // 检查缓存大小限制
                        if (_audioCache.Count >= _config.MaxCacheSize)
                        {
                            var firstKey = _audioCache.Keys.First();
                            ReleaseAudio(firstKey);
                        }
                        
                        _audioCache[audioClip.name] = audioClip;
                    }
                    
                    loadedCount++;
                    onProgress?.Invoke((float)loadedCount / totalCount);
                    
                    // 发布音频加载完成事件
                    EventManager.Publish(new AudioLoadCompletedEvent
                    {
                        AudioPath = audioClip.name,
                        AudioClip = audioClip,
                        Success = true
                    });
                    
                    if (loadedCount >= totalCount)
                    {
                        onCompleted?.Invoke();
                    }
                });
            }
            else
            {
                onCompleted?.Invoke();
            }
        }

        private IEnumerator PreloadAudioListCoroutine(string[] audioPaths, Action<float> onProgress, Action onCompleted)
        {
            var loadedCount = 0;
            var totalCount = audioPaths.Length;
            
            foreach (var audioPath in audioPaths)
            {
                _assetsService.LoadAssetAsync<AudioClip>(audioPath, audioClip =>
                {
                    if (audioClip != null && _config.EnableAudioCache)
                    {
                        // 检查缓存大小限制
                        if (_audioCache.Count >= _config.MaxCacheSize)
                        {
                            var firstKey = _audioCache.Keys.First();
                            ReleaseAudio(firstKey);
                        }
                        
                        _audioCache[audioPath] = audioClip;
                    }
                    
                    loadedCount++;
                    onProgress?.Invoke((float)loadedCount / totalCount);
                    
                    // 发布音频加载完成事件
                    EventManager.Publish(new AudioLoadCompletedEvent
                    {
                        AudioPath = audioPath,
                        AudioClip = audioClip,
                        Success = audioClip != null
                    });
                    
                    if (loadedCount >= totalCount)
                    {
                        onCompleted?.Invoke();
                    }
                });
                
                yield return null; // 分帧加载，避免卡顿
            }
        }

        private IEnumerator CheckAudioSizeCoroutine(string audioPath, Action<long> onSizeChecked)
        {
            if (_audioSizeCache.TryGetValue(audioPath, out var cachedSize))
            {
                onSizeChecked?.Invoke(cachedSize);
                yield break;
            }
            
            var sizeHandle = Addressables.GetDownloadSizeAsync(audioPath);
            yield return sizeHandle;
            
            if (sizeHandle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                var size = sizeHandle.Result;
                _audioSizeCache[audioPath] = size;
                onSizeChecked?.Invoke(size);
            }
            else
            {
                onSizeChecked?.Invoke(0);
            }
        }

        #endregion

        #region 内存和性能监控

        /// <summary>
        /// 获取音频内存使用情况
        /// </summary>
        public void GetAudioMemoryUsage()
        {
            var totalMemory = 0L;
            var loadedPaths = new List<string>();
            
            foreach (var kvp in _audioCache)
            {
                if (kvp.Value != null)
                {
                    // 估算音频内存使用（采样率 * 通道数 * 时长 * 2字节）
                    var clip = kvp.Value;
                    var memoryUsage = clip.samples * clip.channels * 2;
                    totalMemory += memoryUsage;
                    loadedPaths.Add(kvp.Key);
                }
            }
            
            // 发布内存使用情况事件
            EventManager.Publish(new AudioMemoryUsageEvent
            {
                TotalMemoryUsage = totalMemory,
                LoadedAudioCount = loadedPaths.Count,
                LoadedAudioPaths = loadedPaths.ToArray()
            });
        }

        /// <summary>
        /// 清理未使用的音频资源
        /// </summary>
        public void CleanupUnusedAudio()
        {
            var keysToRemove = new List<string>();
            
            foreach (var kvp in _audioCache)
            {
                var audioClip = kvp.Value;
                if (audioClip == null)
                {
                    keysToRemove.Add(kvp.Key);
                    continue;
                }
                
                // 检查音频是否正在使用
                var isInUse = false;
                
                // 检查是否是当前播放的背景音乐
                if (_musicSource.clip == audioClip)
                {
                    isInUse = true;
                }
                
                // 检查是否有音效正在使用
                foreach (var sfxSource in _sfxSources)
                {
                    if (sfxSource.clip == audioClip && sfxSource.isPlaying)
                    {
                        isInUse = true;
                        break;
                    }
                }
                
                if (!isInUse)
                {
                    keysToRemove.Add(kvp.Key);
                }
            }
            
            // 释放未使用的音频资源
            foreach (var key in keysToRemove)
            {
                ReleaseAudio(key);
            }
        }

        #endregion
    }
}