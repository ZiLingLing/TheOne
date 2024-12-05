using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguelike
{
    public class Blackboard : MonoBehaviour
    {
        private Dictionary<string, object> m_data = new Dictionary<string, object>();

        public T GetDate<T>(string key)
        {
            if(m_data.TryGetValue(key,out object value))
            {
                return (T)value;
            }
            return default;
        }

        public void AddData<T>(string key,T value)
        {
            m_data[key] = value;
        }

        public bool RemoveData(string key)
        {
            if (m_data.ContainsKey(key) == true)
            {
                m_data.Remove(key);
            }
            return m_data.ContainsKey(key);
        }
    }
}
