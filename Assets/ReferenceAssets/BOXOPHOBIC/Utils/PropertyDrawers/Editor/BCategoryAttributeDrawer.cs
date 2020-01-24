// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;

namespace Boxophobic
{

    [CustomPropertyDrawer(typeof(BCategoryAttribute))]
    public class BCategoryAttributeDrawer : PropertyDrawer
    {
        BCategoryAttribute a;
        private string category;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            a = (BCategoryAttribute)attribute;
            category = a.Category;

            GUI.enabled = true;
            BEditorGUI.DrawCategory(position, category);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {

            return 40;

        }
    }

}
