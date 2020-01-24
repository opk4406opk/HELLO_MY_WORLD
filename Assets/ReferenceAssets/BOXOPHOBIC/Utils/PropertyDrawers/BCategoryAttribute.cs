// Cristian Pop - https://boxophobic.com/

using UnityEngine;

namespace Boxophobic
{

    public class BCategoryAttribute : PropertyAttribute
    {
        public string Category;

        public BCategoryAttribute(string c)
        {
            Category = c;
        }
    }

}

