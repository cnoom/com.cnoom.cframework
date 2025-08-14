using CnoomFrameWork.Base.Events;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CnoomFrameWork.Services.AudioService
{
    /// <summary>
    /// Addressable音频服务使用示例
    /// </summary>
    public class AddressableAudioExample : MonoBehaviour
    {
        [Header("Addressable音频资源")]
        public AssetReference backgroundMusicRef;
        public AssetReference buttonClickSfxRef;
        public AssetReference explosionSfxRef;
        
        [Header("音频标签")]
        public string uiSfxLabel = "UI_SFX";
        public string battleSfxLabel = "Battle_SFX";
        
        [Header("音频路径列表")]
        public string[] preloadAudioPaths = {
            "Audio/BGM/MainTheme",
            "Audio/SFX/ButtonClick",
            "Audio/SFX/Explosion"
        };

        private void Start()
        {
            // 注册事件监听器
            EventManager.Register(this);
            
            // 订阅Addressable音频事件
            AddressableAudioEventManager.SubscribeAudioDownloadProgress(OnAudioDownloadProgress);
            AddressableAudioEventManager.SubscribeAudioDownloadCompleted(OnAudioDownloadCompleted);
            AddressableAudioEventManager.SubscribeAudioMemoryUsage(OnAudioMemoryUsage);
            
            // 预加载音频资源
            PreloadAudioResources();
        }

        private void OnDestroy()
        {
            // 注销事件监听器
            EventManager.Unregister(this);
            
            // 取消订阅
            AddressableAudioEventManager.UnsubscribeAudioDownloadProgress(OnAudioDownloadProgress);
            AddressableAudioEventManager.UnsubscribeAudioDownloadCompleted(OnAudioDownloadCompleted);
            AddressableAudioEventManager.UnsubscribeAudioMemoryUsage(OnAudioMemoryUsage);
        }

        private void Update()
        {
            // 示例：按键控制音频
            if (Input.GetKeyDown(KeyCode.M))
            {
                // 通过AssetReference播放背景音乐
                AddressableAudioEventManager.PlayMusicByReference(backgroundMusicRef, true, 1f);
            }
            
            if (Input.GetKeyDown(KeyCode.S))
            {
                // 停止背景音乐
                AudioEventManager.StopMusic(1f);
            }
            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // 通过AssetReference播放按钮点击音效
                AddressableAudioEventManager.PlaySfxByReference(buttonClickSfxRef, 0.8f);
            }
            
            if (Input.GetKeyDown(KeyCode.E))
            {
                // 通过AssetReference播放爆炸音效（高优先级）
                AddressableAudioEventManager.PlaySfxByReference(explosionSfxRef, 1f, 1f, 200);
            }
            
            if (Input.GetKeyDown(KeyCode.L))
            {
                // 通过标签预加载UI音效
                AddressableAudioEventManager.PreloadAudioByLabel(uiSfxLabel, 
                    progress => Debug.Log($"UI音效加载进度: {progress:P}"),
                    () => Debug.Log("UI音效加载完成"));
            }
            
            if (Input.GetKeyDown(KeyCode.B))
            {
                // 通过标签预加载战斗音效
                AddressableAudioEventManager.PreloadAudioByLabel(battleSfxLabel,
                    progress => Debug.Log($"战斗音效加载进度: {progress:P}"),
                    () => Debug.Log("战斗音效加载完成"));
            }
            
            if (Input.GetKeyDown(KeyCode.R))
            {
                // 释放UI音效资源
                AddressableAudioEventManager.ReleaseAudioByLabel(uiSfxLabel);
            }
            
            if (Input.GetKeyDown(KeyCode.C))
            {
                // 检查音频资源大小
                AddressableAudioEventManager.CheckAudioSize("Audio/BGM/MainTheme", size =>
                {
                    Debug.Log($"主题音乐大小: {size / 1024f / 1024f:F2} MB");
                });
            }
            
            if (Input.GetKeyDown(KeyCode.I))
            {
                // 获取音频内存使用情况
                var audioService = App.Instance.ServiceLocator.GetService<IAudioService>() as AudioService;
                audioService?.GetAudioMemoryUsage();
            }
            
            if (Input.GetKeyDown(KeyCode.U))
            {
                // 清理未使用的音频资源
                var audioService = App.Instance.ServiceLocator.GetService<IAudioService>() as AudioService;
                audioService?.CleanupUnusedAudio();
            }
        }

        private void PreloadAudioResources()
        {
            // 批量预加载音频列表
            AddressableAudioEventManager.PreloadAudioList(preloadAudioPaths,
                progress => Debug.Log($"音频列表加载进度: {progress:P}"),
                () => Debug.Log("音频列表加载完成"));
        }

        // 使用特性标记的事件处理方法
        [EventSubscriber(typeof(AudioLoadCompletedEvent))]
        private void OnAudioLoaded(AudioLoadCompletedEvent e)
        {
            if (e.Success)
            {
                Debug.Log($"音频加载成功: {e.AudioPath}");
            }
            else
            {
                Debug.LogError($"音频加载失败: {e.AudioPath}");
            }
        }

        [EventSubscriber(typeof(VolumeChangedEvent))]
        private void OnVolumeChanged(VolumeChangedEvent e)
        {
            Debug.Log($"{e.Type} 音量从 {e.OldVolume:F2} 变更为 {e.NewVolume:F2}");
        }

        // 通过便捷方法订阅的事件处理
        private void OnAudioDownloadProgress(AudioDownloadProgressEvent e)
        {
            Debug.Log($"音频下载进度 {e.AudioPath}: {e.Progress:P} ({e.DownloadedBytes}/{e.TotalBytes} bytes)");
        }

        private void OnAudioDownloadCompleted(AudioDownloadCompletedEvent e)
        {
            if (e.Success)
            {
                Debug.Log($"音频下载完成: {e.AudioPath}");
            }
            else
            {
                Debug.LogError($"音频下载失败: {e.AudioPath} - {e.ErrorMessage}");
            }
        }

        private void OnAudioMemoryUsage(AudioMemoryUsageEvent e)
        {
            Debug.Log($"音频内存使用情况:");
            Debug.Log($"  总内存使用: {e.TotalMemoryUsage / 1024f / 1024f:F2} MB");
            Debug.Log($"  已加载音频数量: {e.LoadedAudioCount}");
            Debug.Log($"  已加载音频列表: {string.Join(", ", e.LoadedAudioPaths)}");
        }

        // UI按钮事件示例
        public void OnPlayMusicButtonClick()
        {
            AddressableAudioEventManager.PlayMusicByReference(backgroundMusicRef, true, 1f);
        }

        public void OnStopMusicButtonClick()
        {
            AudioEventManager.StopMusic(1f);
        }

        public void OnPlaySfxButtonClick()
        {
            AddressableAudioEventManager.PlaySfxByReference(buttonClickSfxRef, 0.8f);
        }

        public void OnMasterVolumeChanged(float volume)
        {
            AudioEventManager.SetMasterVolume(volume);
        }

        public void OnMusicVolumeChanged(float volume)
        {
            AudioEventManager.SetMusicVolume(volume);
        }

        public void OnSfxVolumeChanged(float volume)
        {
            AudioEventManager.SetSfxVolume(volume);
        }

        public void OnMuteToggle(bool isMuted)
        {
            AudioEventManager.SetMute(isMuted);
        }
    }
}