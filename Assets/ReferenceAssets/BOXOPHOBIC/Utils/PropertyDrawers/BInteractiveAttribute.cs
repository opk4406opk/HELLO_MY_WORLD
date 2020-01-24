// Cristian Pop - https://boxophobic.com/

using UnityEngine;

namespace Boxophobic
{

    public class BInteractiveAttribute : PropertyAttribute
    {
        public int Value;
        public string Keyword;
        public int Type;

        public BInteractiveAttribute(int v)
        {
            Type = 0;
            Value = v;
        }

        public BInteractiveAttribute(string k)
        {
            Type = 1;
            Keyword = k;
        }
    }

}

