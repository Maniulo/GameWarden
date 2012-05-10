using System;
using System.Collections.Generic;
using System.Text;

namespace GameWarden
{
    public class Meta : IEnumerable<KeyValuePair<String,String>>
    {
        private readonly Dictionary<String, String> Info;

        public Meta()
        {
            Info = new Dictionary<String, String>();
        }

        public String this[String key]
        {
            get 
            {
                return Info.ContainsKey(key) ? Info[key] : null;
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

    public class Player
    {
        public int Order;

        public Player(int order)
        {
            Order = order;
        }
    }
}
