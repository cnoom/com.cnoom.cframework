namespace CnoomFrameWork.Core
{
    public interface IConfig
    {
    }

    public static class IConfigExtension
    {
        public static void Register(this IConfig config)
        {
            ConfigManager.Instance.RegisterConfig(config);
        }
    }
}