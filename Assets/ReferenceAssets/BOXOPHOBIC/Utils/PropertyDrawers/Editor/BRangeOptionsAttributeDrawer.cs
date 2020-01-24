// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;

namespace Boxophobic
{

    [CustomPropertyDrawer(typeof(BRangeOptionsAttribute))]
    public class BRangeOptionAttributeDrawer : PropertyDrawer
    {
        BRangeOptionsAttribute a;

        private float min;
        private float max;
        private string[] options;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            a = (BRangeOptionsAttribute)attribute;

            options = a.options;
            min = a.min;
            max = a.max;

            GUIStyle style = new GUIStyle();
            style.alignment = TextAnchor.MiddleCenter;
            if (EditorGUIUtility.isProSkin)
            {
                style.normal.textColor = Color.gray;
            }
            else
            {
                style.normal.textColor = Color.gray;
            }
            style.fontSize = 7;

            var sliderRect = GUILayoutUtility.GetRect(0, 0, 16, 0);
            property.floatValue = GUI.HorizontalSlider(new Rect(sliderRect.position.x + (sliderRect.width / 5 / 2) - 3, sliderRect.position.y, sliderRect.xMax - (sliderRect.width / 5 / 2) * 2 - 10, 16), property.floatValue, min, max);


            var optionsRect = GUILayoutUtility.GetRect(0, 0, 16, 0);
            for (int i = 0; i < options.Length; i++)
            {
                GUI.Label(new Rect(optionsRect.position.x + ((optionsRect.width / 5) * i), optionsRect.position.y, (optionsRect.width / 5), 16), options[i], style);
            }

        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {

            return -2;

        }
    }

}
