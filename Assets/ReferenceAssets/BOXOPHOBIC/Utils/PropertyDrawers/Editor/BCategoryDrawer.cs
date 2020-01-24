// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;
using System;

namespace Boxophobic
{

    public class BCategoryDrawer : MaterialPropertyDrawer
    {

        protected string Category;

        public BCategoryDrawer(string c)
        {
            Category = c;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, String label, MaterialEditor materiaEditor)
        {

            //EditorGUI.DrawRect(position, new Color(0, 1, 0, 0.1f));

            GUI.enabled = true;
            BEditorGUI.DrawCategory(position, Category);

        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {

            return 40;

        }
    }

}
