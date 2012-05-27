using System;
using System.Collections.Generic;

namespace GameWarden
{
    public class Meta : IEnumerable<KeyValuePair<string, string>>
    {
        private readonly Dictionary<string, string> Info;

        public Meta()
        {
            Info = new Dictionary<string, string>();
        }

        public String this[String key]
        {
            get 
            {
                return Info.ContainsKey(key) ? Info[key] : "";
            }
            set
            {
                if (Info.ContainsKey(key))
                {
                    Info[key] = value;
                }
                else
                {
                    Info.Add(key, value);
                }
            }
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return Info.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Info.GetEnumerator();
        }
    }
}