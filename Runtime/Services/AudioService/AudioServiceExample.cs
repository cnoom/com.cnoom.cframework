using CnoomFrameWork.Base.Events;
using UnityEngine;

namespace CnoomFrameWork.Services.AudioService
{
    /// <summary>
    /// 音频服务使用示例
    /// </summary>
    public class AudioServiceExample : MonoBehaviour
    {
        [Header("音频资源")]
        public AudioClip backgroundMusic;
        public AudioClip buttonClickSfx;
        public AudioClip explosionSfx;

        private void Start()
        {
            // 注册事件监听器
            EventManager.Register(this);
            
            // 或者使用便捷方法订阅事件
            AudioEventManager.SubscribeAudioPlayCompleted(OnAudioPlayCompleted);
            AudioEventManager.SubscribeVolumeChanged(OnVolumeChanged);
        }

        private void OnDestroy()
        {
            // 注销事件监听器
            EventManager.Unregister(this);
            
            // 或者取消订阅
            AudioEventManager.UnsubscribeAudioPlayCompleted(OnAudioPlayCompleted);
            AudioEventManager.UnsubscribeVolumeChanged(OnVolumeChanged);
        }

        private void Update()
        {
            // 示例：按键控制音频
            if (Input.GetKeyDown(KeyCode.M))
            {
                // 播放背景音乐
                AudioEventManager.PlayMusic(backgroundMusic, true, 1f);
            }
            
            if (Input.GetKeyDown(KeyCode.S))
            {
                // 停止背景音乐
                AudioEventManager.StopMusic(1f);
            }
            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // 播放按钮点击音效
                AudioEventManager.PlaySfx(buttonClickSfx, 0.8f);
            }
            
            if (Input.GetKeyDown(KeyCode.E))
            {
                // 播放爆炸音效（高优先级）
                AudioEventManager.PlaySfx(explosionSfx, 1f, 1f, 200);
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                // 设置主音量为50%
                AudioEventManager.SetMasterVolume(0.5f);
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                // 设置音乐音量为30%
                AudioEventManager.SetMusicVolume(0.3f);
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                // 切换静音状态
                AudioEventManager.SetMute(!GetCurrentMuteState());
            }
        }

        // 使用特性标记的事件处理方法
        [EventSubscriber(typeof(AudioPlayCompletedEvent))]
        private void OnAudioCompleted(AudioPlayCompletedEvent e)
        {
            if (e.IsMusic)
            {
                Debug.Log($"背景音乐播放完成: {e.AudioClip.name}");
            }
            else
            {
                Debug.Log($"音效播放完成: {e.AudioClip.name}, ID: {e.SfxId}");
            }
        }

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

        // 通过便捷方法订阅的事件处理
        private void OnAudioPlayCompleted(AudioPlayCompletedEvent e)
        {
            // 可以在这里处理音频播放完成的逻辑
            // 例如：播放下一首音乐、触发特效等
        }

        private void OnVolumeChanged(VolumeChangedEvent e)
        {
            Debug.Log($"{e.Type} 音量从 {e.OldVolume:F2} 变更为 {e.NewVolume:F2}");
            
            // 可以在这里更新UI显示
            UpdateVolumeUI(e.Type, e.NewVolume);
        }

        private void UpdateVolumeUI(VolumeChangedEvent.VolumeType type, float volume)
        {
            // 更新音量UI的示例代码
            switch (type)
            {
                case VolumeChangedEvent.VolumeType.Master:
                    // 更新主音量滑条
                    break;
                case VolumeChangedEvent.VolumeType.Music:
                    // 更新音乐音量滑条
                    break;
                case VolumeChangedEvent.VolumeType.Sfx:
                    // 更新音效音量滑条
                    break;
            }
        }

        private bool GetCurrentMuteState()
        {
            // 这里应该从音频服务获取当前静音状态
            // 由于我们使用事件系统，可能需要通过其他方式获取状态
            return false; // 示例返回值
        }

        // 预加载音频资源的示例
        public void PreloadAudioResources()
        {
            AudioEventManager.PreloadAudio("Audio/BGM/MainTheme");
            AudioEventManager.PreloadAudio("Audio/SFX/ButtonClick");
            AudioEventManager.PreloadAudio("Audio/SFX/Explosion");
        }

        // 清理音频缓存的示例
        public void CleanupAudioCache()
        {
            AudioEventManager.ClearAudioCache();
        }
    }
}