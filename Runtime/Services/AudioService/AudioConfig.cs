using CnoomFrameWork.Base.Config;

namespace CnoomFrameWork.Services.AudioService
{
    /// <summary>
    /// 音频服务配置
    /// </summary>
    public class AudioConfig : IConfig
    {
        /// <summary>
        /// 默认主音量
        /// </summary>
        public float DefaultMasterVolume { get; set; } = 1f;
        
        /// <summary>
        /// 默认背景音乐音量
        /// </summary>
        public float DefaultMusicVolume { get; set; } = 0.8f;
        
        /// <summary>
        /// 默认音效音量
        /// </summary>
        public float DefaultSfxVolume { get; set; } = 1f;
        
        /// <summary>
        /// 最大同时播放的音效数量
        /// </summary>
        public int MaxSfxSources { get; set; } = 10;
        
        /// <summary>
        /// 音频淡入淡出的默认时长
        /// </summary>
        public float DefaultFadeDuration { get; set; } = 1f;
        
        /// <summary>
        /// 是否启用音频资源缓存
        /// </summary>
        public bool EnableAudioCache { get; set; } = true;
        
        /// <summary>
        /// 音频缓存最大数量
        /// </summary>
        public int MaxCacheSize { get; set; } = 50;
        
        /// <summary>
        /// 是否在应用暂停时暂停音频
        /// </summary>
        public bool PauseOnApplicationPause { get; set; } = true;
        
        /// <summary>
        /// 是否保存音量设置到本地
        /// </summary>
        public bool SaveVolumeSettings { get; set; } = true;
    }
}