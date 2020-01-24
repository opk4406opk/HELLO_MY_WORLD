// Cristian Pop - https://boxophobic.com/

using UnityEngine;

namespace Boxophobic
{

    public class BRangeSimpleAttribute : PropertyAttribute
    {

        public float min;
        public float max;

        public BRangeSimpleAttribute(float m_min, float m_max)
        {
            min = m_min;
            max = m_max;
        }

    }

}

