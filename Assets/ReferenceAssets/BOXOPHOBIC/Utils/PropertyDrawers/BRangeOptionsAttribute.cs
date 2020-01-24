// Cristian Pop - https://boxophobic.com/

using UnityEngine;

namespace Boxophobic
{

    public class BRangeOptionsAttribute : PropertyAttribute
    {

        public float min;
        public float max;
        public string[] options;

        public BRangeOptionsAttribute(float m_min, float m_max, string[] m_options)
        {
            options = m_options;
            min = m_min;
            max = m_max;
        }

    }

}

