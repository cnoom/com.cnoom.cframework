using CnoomFrameWork.Log;

namespace FrameWork.Editor
{
    public class EditorLog : BaseLog
    {
        // 静态私有字段，用于保存单例实例
        private static EditorLog _instance;

        // 公共静态属性，用于获取单例实例
        public static EditorLog Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new EditorLog();
                }
                return _instance;
            }
        }

        // 私有构造函数，防止外部实例化
        private EditorLog() { }
    }
}