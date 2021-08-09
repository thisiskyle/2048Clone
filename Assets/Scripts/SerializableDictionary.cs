using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KE.Serialization
{
    [System.Serializable]
    public class SerializableDictionary<TK, TV>
    {
        [SerializeField]
        public List<TK> keys;

        [SerializeField]
        public List<TV> values;


        public TV Get(TK key)
        {
            int i = keys.IndexOf(key);
            return values[i];
        }

        public void Set(TK key, TV val)
        {
            int i = keys.IndexOf(key);

            if(i < 0)
            {
                keys.Add(key);
                values.Add(val);
            }
            else
            {
                values[i] = val;
            }
        }

        public TV this[TK key]
        {
            get => Get(key);
            set => Set(key, value);
        }
    }
}
