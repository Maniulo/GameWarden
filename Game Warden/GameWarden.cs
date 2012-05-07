using System;
using System.Collections.Generic;
using System.Text;

namespace GameWarden
{
    public class Meta
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
