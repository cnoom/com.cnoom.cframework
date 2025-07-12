using System.Collections.Generic;

namespace CnoomFrameWork.Modules.UiModule
{
    public class UiParameter
    {
        private Dictionary<string, object> _parameter = new Dictionary<string, object>();

        public void SetParameter(string key, object value)
        {
            _parameter[key] = value;
        }

        public T GetParameter<T>(string key)
        {
            if (_parameter.ContainsKey(key))
            {
                return (T)_parameter[key];
            }
            return default;
        }
    }
}