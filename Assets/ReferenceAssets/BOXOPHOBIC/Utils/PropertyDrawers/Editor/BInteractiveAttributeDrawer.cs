// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;

namespace Boxophobic
{

    [CustomPropertyDrawer(typeof(BInteractiveAttribute))]
    public class BInteractiveAttributeDrawer : PropertyDrawer
    {
        BInteractiveAttribute a;

        private int Value;
        private string Keywork;
        public int Type;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            a = (BInteractiveAttribute)attribute;

            Value = a.Value;
            Keywork = a.Keyword;
            Type = a.Type;

            if (Type == 0)
            {
                //EditorGUI.PropertyField(position, property);

                if (property.intValue == Value)
                {
                    GUI.enabled = true;
                }
                else
                {
                    GUI.enabled = false;
                }
            }
            else if (Type == 1)
            {
                if (Keywork == "ON")
                {
                    GUI.enabled = true;
                }
                else if (Keywork == "OFF")
                {
                    GUI.enabled = false;
                }
            }

        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {

            //var height = -2;

            //if (Type == 0)
            //{
            //    height = 16;
            //}

            return -2;

        }
    }

}
