using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CnoomFrameWork.Base.Config;
using CnoomFrameWork.Base.Events;
using CnoomFrameWork.Core;
using CnoomFrameWork.Services.StorageService;
using CnoomFrameWork.Singleton;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CnoomFrameWork.Services.AudioService
{
    /// <summary>
    /// 音频服务实现类
    /// </summary>
    public partial class AudioService : AService, IAudioService
    {
        private const string VolumeSettingsSection = "AudioSettings";
        private const string MasterVolumeKey = "MasterVolume";
        private const string MusicVolumeKey = "MusicVolume";
        private const string SfxVolumeKey = "SfxVolume";
        private const string IsMutedKey = "IsMuted";

        private AudioConfig _config;
        private IStorageService _storageService;
        private AudioServiceHolder _audioHolder;
        
        // 音频源管理
        private AudioSource _musicSource;
        private readonly List<AudioSource> _sfxSources = new();
        private readonly Queue<AudioSource> _availableSfxSources = new();
        
        // 音频资源缓存
        private readonly Dictionary<string, AudioClip> _audioCache = new();
        private readonly Dictionary<int, AudioSource> _activeSfxSources = new();
        
        // 音效ID生成器
        private int _nextSfxId = 1;
        
        // 音量控制
        private float _masterVolume = 1f;
        private float _musicVolume = 0.8f;
        private float _sfxVolume = 1f;
        private bool _isMuted;
        
        // 淡入淡出协程
        private Coroutine _musicFadeCoroutine;

        public float MasterVolume
        {
            get => _masterVolume;
            set
            {
                _masterVolume = Mathf.Clamp01(value);
                UpdateAllVolumes();
                SaveVolumeSettings();
            }
        }

        public float MusicVolume
        {
            get => _musicVolume;
            set
            {
                _musicVolume = Mathf.Clamp01(value);
                UpdateMusicVolume();
                SaveVolumeSettings();
            }
        }

        public float SfxVolume
        {
            get => _sfxVolume;
            set
            {
                _sfxVolume = Mathf.Clamp01(value);
                UpdateSfxVolumes();
                SaveVolumeSettings();
            }
        }

        public bool IsMuted
        {
            get => _isMuted;
            set
            {
                _isMuted = value;
                UpdateAllVolumes();
                SaveVolumeSettings();
            }
        }

        public override void Initialize()
        {
            _config = ConfigManager.Instance.GetConfig<AudioConfig>();
            _storageService = App.Instance.ServiceLocator.GetService<IStorageService>();
            
            InitializeAudioHolder();
            InitializeAudioSources();
            InitializeAddressableSupport();
            LoadVolumeSettings();
            
            if (_config.PauseOnApplicationPause)
            {
                _audioHolder.SetApplicationPauseCallback(OnApplicationPause);
            }
        }

        public override void Dispose()
        {
            if (_musicFadeCoroutine != null)
            {
                _audioHolder.StopCoroutine(_musicFadeCoroutine);
            }
            
            StopAllSfx();
            StopMusic();
            ClearAudioCache();
            
            if (_audioHolder != null)
            {
                _audioHolder.SetApplicationPauseCallback(null);
                Object.Destroy(_audioHolder.gameObject);
            }
        }

        #region 事件处理方法

        [EventSubscriber(typeof(PlayMusicEvent))]
        private void OnPlayMusic(PlayMusicEvent e)
        {
            if (e.AudioClip != null)
            {
                PlayMusic(e.AudioClip, e.Loop, e.FadeInDuration);
            }
            else if (!string.IsNullOrEmpty(e.AudioPath))
            {
                PlayMusic(e.AudioPath, e.Loop, e.FadeInDuration);
            }
        }

        [EventSubscriber(typeof(StopMusicEvent))]
        private void OnStopMusic(StopMusicEvent e)
        {
            StopMusic(e.FadeOutDuration);
        }

        [EventSubscriber(typeof(PauseMusicEvent))]
        private void OnPauseMusic(PauseMusicEvent e)
        {
            PauseMusic();
        }

        [EventSubscriber(typeof(ResumeMusicEvent))]
        private void OnResumeMusic(ResumeMusicEvent e)
        {
            ResumeMusic();
        }

        [EventSubscriber(typeof(PlaySfxEvent))]
        private void OnPlaySfx(PlaySfxEvent e)
        {
            int sfxId = -1;
            if (e.AudioClip != null)
            {
                sfxId = PlaySfx(e.AudioClip, e.Volume, e.Pitch, e.Priority);
            }
            else if (!string.IsNullOrEmpty(e.AudioPath))
            {
                sfxId = PlaySfx(e.AudioPath, e.Volume, e.Pitch, e.Priority);
            }
            e.SfxId = sfxId; // 返回音效ID
        }

        [EventSubscriber(typeof(StopSfxEvent))]
        private void OnStopSfx(StopSfxEvent e)
        {
            StopSfx(e.SfxId);
        }

        [EventSubscriber(typeof(StopAllSfxEvent))]
        private void OnStopAllSfx(StopAllSfxEvent e)
        {
            StopAllSfx();
        }

        [EventSubscriber(typeof(SetMasterVolumeEvent))]
        private void OnSetMasterVolume(SetMasterVolumeEvent e)
        {
            var oldVolume = MasterVolume;
            MasterVolume = e.Volume;
            
            // 发布音量变化事件
            EventManager.Publish(new VolumeChangedEvent
            {
                Type = VolumeChangedEvent.VolumeType.Master,
                OldVolume = oldVolume,
                NewVolume = MasterVolume
            });
        }

        [EventSubscriber(typeof(SetMusicVolumeEvent))]
        private void OnSetMusicVolume(SetMusicVolumeEvent e)
        {
            var oldVolume = MusicVolume;
            MusicVolume = e.Volume;
            
            // 发布音量变化事件
            EventManager.Publish(new VolumeChangedEvent
            {
                Type = VolumeChangedEvent.VolumeType.Music,
                OldVolume = oldVolume,
                NewVolume = MusicVolume
            });
        }

        [EventSubscriber(typeof(SetSfxVolumeEvent))]
        private void OnSetSfxVolume(SetSfxVolumeEvent e)
        {
            var oldVolume = SfxVolume;
            SfxVolume = e.Volume;
            
            // 发布音量变化事件
            EventManager.Publish(new VolumeChangedEvent
            {
                Type = VolumeChangedEvent.VolumeType.Sfx,
                OldVolume = oldVolume,
                NewVolume = SfxVolume
            });
        }

        [EventSubscriber(typeof(SetMuteEvent))]
        private void OnSetMute(SetMuteEvent e)
        {
            IsMuted = e.IsMuted;
        }

        [EventSubscriber(typeof(PreloadAudioEvent))]
        private void OnPreloadAudio(PreloadAudioEvent e)
        {
            PreloadAudio(e.AudioPath, clip =>
            {
                // 发布音频加载完成事件
                EventManager.Publish(new AudioLoadCompletedEvent
                {
                    AudioPath = e.AudioPath,
                    AudioClip = clip,
                    Success = clip != null
                });
            });
        }

        [EventSubscriber(typeof(ReleaseAudioEvent))]
        private void OnReleaseAudio(ReleaseAudioEvent e)
        {
            ReleaseAudio(e.AudioPath);
        }

        [EventSubscriber(typeof(ClearAudioCacheEvent))]
        private void OnClearAudioCache(ClearAudioCacheEvent e)
        {
            ClearAudioCache();
        }

        #endregion

        public void PlayMusic(AudioClip audioClip, bool loop = true, float fadeInDuration = 0f)
        {
            if (audioClip == null) return;
            
            if (_musicFadeCoroutine != null)
            {
                _audioHolder.StopCoroutine(_musicFadeCoroutine);
            }
            
            _musicSource.clip = audioClip;
            _musicSource.loop = loop;
            
            if (fadeInDuration > 0f)
            {
                _musicFadeCoroutine = _audioHolder.StartCoroutine(FadeInMusic(fadeInDuration));
            }
            else
            {
                _musicSource.volume = GetEffectiveMusicVolume();
                _musicSource.Play();
            }
        }

        public void PlayMusic(string audioPath, bool loop = true, float fadeInDuration = 0f)
        {
            LoadAudioClip(audioPath, clip =>
            {
                if (clip != null)
                {
                    PlayMusic(clip, loop, fadeInDuration);
                }
            });
        }

        public void StopMusic(float fadeOutDuration = 0f)
        {
            if (!_musicSource.isPlaying) return;
            
            if (_musicFadeCoroutine != null)
            {
                _audioHolder.StopCoroutine(_musicFadeCoroutine);
            }
            
            if (fadeOutDuration > 0f)
            {
                _musicFadeCoroutine = _audioHolder.StartCoroutine(FadeOutMusic(fadeOutDuration));
            }
            else
            {
                _musicSource.Stop();
            }
        }

        public void PauseMusic()
        {
            if (_musicSource.isPlaying)
            {
                _musicSource.Pause();
            }
        }

        public void ResumeMusic()
        {
            if (!_musicSource.isPlaying && _musicSource.clip != null)
            {
                _musicSource.UnPause();
            }
        }

        public int PlaySfx(AudioClip audioClip, float volume = 1f, float pitch = 1f, int priority = 128)
        {
            if (audioClip == null) return -1;
            
            var sfxSource = GetAvailableSfxSource();
            if (sfxSource == null) return -1;
            
            var sfxId = _nextSfxId++;
            
            sfxSource.clip = audioClip;
            sfxSource.volume = GetEffectiveSfxVolume() * Mathf.Clamp01(volume);
            sfxSource.pitch = Mathf.Clamp(pitch, 0.1f, 3f);
            sfxSource.priority = Mathf.Clamp(priority, 0, 256);
            sfxSource.Play();
            
            _activeSfxSources[sfxId] = sfxSource;
            _audioHolder.StartCoroutine(ReleaseSfxSourceWhenFinished(sfxId, sfxSource));
            
            return sfxId;
        }

        public int PlaySfx(string audioPath, float volume = 1f, float pitch = 1f, int priority = 128)
        {
            LoadAudioClip(audioPath, clip =>
            {
                if (clip != null)
                {
                    PlaySfx(clip, volume, pitch, priority);
                }
            });
            return -1; // 异步加载，无法立即返回ID
        }

        public void StopSfx(int sfxId)
        {
            if (_activeSfxSources.TryGetValue(sfxId, out var sfxSource))
            {
                sfxSource.Stop();
                ReleaseSfxSource(sfxId, sfxSource);
            }
        }

        public void StopAllSfx()
        {
            var activeIds = _activeSfxSources.Keys.ToArray();
            foreach (var sfxId in activeIds)
            {
                StopSfx(sfxId);
            }
        }

        public void PreloadAudio(string audioPath, Action<AudioClip> callback = null)
        {
            LoadAudioClip(audioPath, callback);
        }

        public void ReleaseAudio(string audioPath)
        {
            if (_audioCache.TryGetValue(audioPath, out var clip))
            {
                _audioCache.Remove(audioPath);
                
                // 使用Addressable系统释放音频资源
                var assetsService = App.Instance.ServiceLocator.GetService<CnoomFrameWork.Modules.AddressableModule.AssetsService>();
                assetsService.ReleaseAsset<AudioClip>(audioPath);
            }
        }

        public void ClearAudioCache()
        {
            var assetsService = App.Instance.ServiceLocator.GetService<CnoomFrameWork.Modules.AddressableModule.AssetsService>();
            
            foreach (var audioPath in _audioCache.Keys)
            {
                assetsService.ReleaseAsset<AudioClip>(audioPath);
            }
            _audioCache.Clear();
        }

        private void InitializeAudioHolder()
        {
            _audioHolder = AudioServiceHolder.Instance;
        }

        private void InitializeAudioSources()
        {
            // 创建背景音乐音频源
            _musicSource = _audioHolder.gameObject.AddComponent<AudioSource>();
            _musicSource.playOnAwake = false;
            _musicSource.loop = true;
            
            // 创建音效音频源池
            for (int i = 0; i < _config.MaxSfxSources; i++)
            {
                var sfxSource = _audioHolder.gameObject.AddComponent<AudioSource>();
                sfxSource.playOnAwake = false;
                sfxSource.loop = false;
                
                _sfxSources.Add(sfxSource);
                _availableSfxSources.Enqueue(sfxSource);
            }
        }

        private void LoadVolumeSettings()
        {
            if (!_config.SaveVolumeSettings || _storageService == null) return;
            
            _masterVolume = _storageService.Get(MasterVolumeKey, _config.DefaultMasterVolume, VolumeSettingsSection);
            _musicVolume = _storageService.Get(MusicVolumeKey, _config.DefaultMusicVolume, VolumeSettingsSection);
            _sfxVolume = _storageService.Get(SfxVolumeKey, _config.DefaultSfxVolume, VolumeSettingsSection);
            _isMuted = _storageService.Get(IsMutedKey, false, VolumeSettingsSection);
            
            UpdateAllVolumes();
        }

        private void SaveVolumeSettings()
        {
            if (!_config.SaveVolumeSettings || _storageService == null) return;
            
            _storageService.Save(MasterVolumeKey, _masterVolume, VolumeSettingsSection);
            _storageService.Save(MusicVolumeKey, _musicVolume, VolumeSettingsSection);
            _storageService.Save(SfxVolumeKey, _sfxVolume, VolumeSettingsSection);
            _storageService.Save(IsMutedKey, _isMuted, VolumeSettingsSection);
        }

        private void UpdateAllVolumes()
        {
            UpdateMusicVolume();
            UpdateSfxVolumes();
        }

        private void UpdateMusicVolume()
        {
            if (_musicSource != null)
            {
                _musicSource.volume = GetEffectiveMusicVolume();
            }
        }

        private void UpdateSfxVolumes()
        {
            var effectiveVolume = GetEffectiveSfxVolume();
            foreach (var sfxSource in _sfxSources)
            {
                if (sfxSource.isPlaying)
                {
                    // 保持相对音量比例
                    var relativeVolume = sfxSource.volume / GetEffectiveSfxVolume();
                    sfxSource.volume = effectiveVolume * relativeVolume;
                }
            }
        }

        private float GetEffectiveMusicVolume()
        {
            return _isMuted ? 0f : _masterVolume * _musicVolume;
        }

        private float GetEffectiveSfxVolume()
        {
            return _isMuted ? 0f : _masterVolume * _sfxVolume;
        }

        private AudioSource GetAvailableSfxSource()
        {
            if (_availableSfxSources.Count > 0)
            {
                return _availableSfxSources.Dequeue();
            }
            
            // 如果没有可用的音频源，尝试停止优先级最低的音效
            var lowestPrioritySource = _activeSfxSources.Values
                .Where(source => source.isPlaying)
                .OrderBy(source => source.priority)
                .FirstOrDefault();
                
            if (lowestPrioritySource != null)
            {
                var sfxId = _activeSfxSources.FirstOrDefault(pair => pair.Value == lowestPrioritySource).Key;
                StopSfx(sfxId);
                return lowestPrioritySource;
            }
            
            return null;
        }

        private void ReleaseSfxSource(int sfxId, AudioSource sfxSource)
        {
            _activeSfxSources.Remove(sfxId);
            _availableSfxSources.Enqueue(sfxSource);
        }

        private IEnumerator ReleaseSfxSourceWhenFinished(int sfxId, AudioSource sfxSource)
        {
            var audioClip = sfxSource.clip;
            yield return new WaitWhile(() => sfxSource.isPlaying);
            
            // 发布音效播放完成事件
            EventManager.Publish(new AudioPlayCompletedEvent
            {
                SfxId = sfxId,
                AudioClip = audioClip,
                IsMusic = false
            });
            
            ReleaseSfxSource(sfxId, sfxSource);
        }

        private void LoadAudioClip(string audioPath, Action<AudioClip> callback)
        {
            if (_config.EnableAudioCache && _audioCache.TryGetValue(audioPath, out var cachedClip))
            {
                callback?.Invoke(cachedClip);
                return;
            }
            
            // 使用Addressable系统加载音频
            var assetsService = App.Instance.ServiceLocator.GetService<CnoomFrameWork.Modules.AddressableModule.AssetsService>();
            assetsService.LoadAssetAsync<AudioClip>(audioPath, audioClip =>
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
                
                callback?.Invoke(audioClip);
            });
        }

        private IEnumerator FadeInMusic(float duration)
        {
            _musicSource.volume = 0f;
            _musicSource.Play();
            
            var targetVolume = GetEffectiveMusicVolume();
            var elapsedTime = 0f;
            
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                _musicSource.volume = Mathf.Lerp(0f, targetVolume, elapsedTime / duration);
                yield return null;
            }
            
            _musicSource.volume = targetVolume;
            _musicFadeCoroutine = null;
        }

        private IEnumerator FadeOutMusic(float duration)
        {
            var startVolume = _musicSource.volume;
            var elapsedTime = 0f;
            
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                _musicSource.volume = Mathf.Lerp(startVolume, 0f, elapsedTime / duration);
                yield return null;
            }
            
            _musicSource.volume = 0f;
            _musicSource.Stop();
            _musicFadeCoroutine = null;
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                PauseMusic();
            }
            else
            {
                ResumeMusic();
            }
        }

        private class AudioServiceHolder : PersistentMonoSingleton<AudioServiceHolder>
        {
            private Action<bool> _applicationPauseCallback;

            private void OnApplicationPause(bool pauseStatus)
            {
                _applicationPauseCallback?.Invoke(pauseStatus);
            }

            public void SetApplicationPauseCallback(Action<bool> callback)
            {
                _applicationPauseCallback = callback;
            }
        }
    }
}