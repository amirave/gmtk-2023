using System.Collections.Generic;

namespace GameManager
{
    public class DynamicTextStore
    {
        private Dictionary<string, string> values = new Dictionary<string, string>();

        public void SetValue(string name, string value)
        {
            values[name] = value;
        }

        public string GetValue(string name)
        {
            if (values.ContainsKey(name))
            {
                return values[name];
            }
            else
            {
                throw new KeyNotFoundException($"Variable '{name}' not found in store.");
            }
        }

        public bool HasValue(string name)
        {
            return values.ContainsKey(name);
        }
    }
}