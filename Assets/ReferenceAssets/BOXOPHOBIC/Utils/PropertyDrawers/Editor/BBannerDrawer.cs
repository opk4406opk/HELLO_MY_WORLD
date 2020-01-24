// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;
using System;

namespace Boxophobic
{
    public class BBannerDrawer : MaterialPropertyDrawer
    {
        protected string bannerText;
        protected string bannerSubText;
        protected Color bannerColor;

        protected float rValue = -1;
        protected float gValue = -1;
        protected float bValue = -1;
        protected string title;
        protected string subtitle;

        public BBannerDrawer()
        {
            title = null;
            subtitle = null;
        }

        public BBannerDrawer(string t)
        {
            title = t;
            subtitle = null;
        }

        public BBannerDrawer(string t, string s)
        {
            title = t;
            subtitle = s;
        }

        public BBannerDrawer(float r, float g, float b, string t)
        {
            rValue = r;
            gValue = g;
            bValue = b;
            title = t;
        }

        public BBannerDrawer(float r, float g, float b, string t, string s)
        {
            rValue = r;
            gValue = g;
            bValue = b;
            title = t;
            subtitle = s;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, String label, MaterialEditor materialEditor)
        {
            //EditorGUI.DrawRect(position, new Color(0, 1, 0, 0.05f));

            Material material = materialEditor.target as Material;

            if (title == null && subtitle == null)
            {
                title = prop.displayName;
                subtitle = null;
            }

            bannerText = title;
            bannerSubText = subtitle;

            if (rValue < 0)
            {
                bannerColor = BConst.ColorDarkGray;
            }
            else
            {
                bannerColor = new Color(rValue, gValue, bValue);
            }

            BEditorGUI.DrawMaterialBanner(bannerColor, bannerText, bannerSubText, material.shader);
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            return 0;
        }
    }
}