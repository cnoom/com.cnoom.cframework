using System.Collections.Generic;

namespace CnoomFrameWork.Modules.UiModule
{
    public class UiParameter
    {
        private readonly Dictionary<string, object> _parameter = new();

        public void SetParameter(string key, object value)
        {
            _parameter[key] = value;
        }

        public T GetParameter<T>(string key)
        {
            if (_parameter.ContainsKey(key)) return (T)_parameter[key];

            return default;
        }

        public bool HasParameter(string key)
        {
            return _parameter.ContainsKey(key);
        }

        public bool TryGetParameter<T>(string key, out T value)
        {
            if (_parameter.ContainsKey(key))
            {
                value = (T)_parameter[key];
                return true;
            }

            value = default;
            return false;
        }
    }
}