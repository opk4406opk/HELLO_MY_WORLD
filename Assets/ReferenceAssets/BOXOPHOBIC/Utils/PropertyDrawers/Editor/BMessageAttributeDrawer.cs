// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;

namespace Boxophobic
{

    [CustomPropertyDrawer(typeof(BMessageAttribute))]
    public class BMessageAttributeDrawer : PropertyDrawer
    {
        BMessageAttribute a;

        public string Type;
        public string Message;
        public bool Show;
        public float Top;
        public float Down;

        MessageType mType;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            Show = property.boolValue;



            if (Show)
            {
                a = (BMessageAttribute)attribute;

                Type = a.Type;
                Message = a.Message;
                Top = a.Top;
                Down = a.Down;

                if (Type == "None")
                {
                    mType = MessageType.None;
                }
                else if (Type == "Info")
                {
                    mType = MessageType.Info;
                }
                else if (Type == "Warning")
                {
                    mType = MessageType.Warning;
                }
                else if (Type == "Error")
                {
                    mType = MessageType.Error;
                }

                GUILayout.Space(Top);
                EditorGUI.HelpBox(new Rect(position.x, position.y + Top, position.width, position.height), Message, mType);
                GUILayout.Space(Down);

            }

        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {

            float height;

            if (Show)
                height = 40;
            else
                height = -2;

            return height;

        }
    }

}
