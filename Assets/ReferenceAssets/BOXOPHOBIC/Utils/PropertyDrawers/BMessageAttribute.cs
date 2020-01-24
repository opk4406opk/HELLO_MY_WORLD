// Cristian Pop - https://boxophobic.com/

using UnityEngine;

namespace Boxophobic
{

    public class BMessageAttribute : PropertyAttribute
    {

        public string Type;
        public string Message;
        public float Top;
        public float Down;

        public BMessageAttribute(string t, string m, float top, float down)
        {
            Type = t;
            Message = m;
            Top = top;
            Down = down;
        }

    }

}

