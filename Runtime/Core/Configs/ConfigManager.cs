using System;
using System.Collections.Generic;
using CnoomFrameWork.Singleton;

namespace CnoomFrameWork.Core
{
    /// <summary>
    ///     配置管理器，用于管理所有运行时配置文件
    /// </summary>
    public class ConfigManager : Singleton<ConfigManager>
    {
        public Dictionary<Type, IConfig> Configs { get; } = new Dictionary<Type, IConfig>();

        public T GetConfig<T>() where T : class, IConfig, new()
        {
            if(!Configs.ContainsKey(typeof(T)))
            {
                return new T();
            }
            return (T)Configs[typeof(T)];
        }

        public void RegisterConfig(IConfig config)
        {
            if(Configs.ContainsKey(config.GetType()))
            {
                Configs[config.GetType()] = config;
                return;
            }
            Configs.Add(config.GetType(), config);
        }
    }
}