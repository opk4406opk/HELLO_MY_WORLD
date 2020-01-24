// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;

namespace Boxophobic
{
    public static class BEditorGUI 
    {
        public static void DrawBanner(Color bannerColor, string bannerText, string helpURL)
        {
            GUILayout.Space(20);
            var bannerFullRect = GUILayoutUtility.GetRect(0, 0, 40, 0);
            var bannerBeginRect = new Rect(bannerFullRect.position.x, bannerFullRect.position.y, 20, 40);
            var bannerMiddleRect = new Rect(bannerFullRect.position.x + 20, bannerFullRect.position.y, bannerFullRect.xMax - 54, 40);
            var bannerEndRect = new Rect(bannerFullRect.xMax - 20, bannerFullRect.position.y, 20, 40);
            var iconRect = new Rect(bannerFullRect.xMax - 36, bannerFullRect.position.y + 5, 30, 30);

            Color guiColor;

            if (EditorGUIUtility.isProSkin)
            {
                bannerColor = new Color(bannerColor.r, bannerColor.g, bannerColor.b, 1f);
            }
            else
            {
                bannerColor = BConst.ColorLightGray;
            }

            if (bannerColor.r + bannerColor.g + bannerColor.b <= 1.5f)
            {
                guiColor = BConst.ColorLightGray;
            }
            else
            {
                guiColor = BConst.ColorDarkGray;
            }

            GUI.color = bannerColor;

            GUI.DrawTexture(bannerBeginRect, BConst.BannerImageBegin, ScaleMode.StretchToFill, true);
            GUI.DrawTexture(bannerMiddleRect, BConst.BannerImageMiddle, ScaleMode.StretchToFill, true);
            GUI.DrawTexture(bannerEndRect, BConst.BannerImageEnd, ScaleMode.StretchToFill, true);

            GUI.Label(bannerFullRect, "<size=14><color=#" + ColorUtility.ToHtmlStringRGB(guiColor) + "><b>" + bannerText + "</b></color></size>", BConst.BannerTitleStyle);

            GUI.color = guiColor;

            if (GUI.Button(iconRect, BConst.IconHelp, new GUIStyle { alignment = TextAnchor.MiddleCenter }))
            {
                Application.OpenURL(helpURL);
            }

            GUI.color = Color.white;
            GUILayout.Space(10);
        }

        public static void DrawWindowBanner(Color bannerColor, string bannerText, string helpURL)
        {
            GUILayout.Space(20);
            var bannerFullRect = GUILayoutUtility.GetRect(0, 0, 40, 0);
            var bannerBeginRect = new Rect(bannerFullRect.position.x + 20, bannerFullRect.position.y, 20, 40);
            var bannerMiddleRect = new Rect(bannerFullRect.position.x + 40, bannerFullRect.position.y, bannerFullRect.xMax - 75, 40);
            var bannerEndRect = new Rect(bannerFullRect.xMax - 36, bannerFullRect.position.y, 20, 40);
            var iconRect = new Rect(bannerFullRect.xMax - 53, bannerFullRect.position.y + 5, 30, 30);

            Color guiColor;

            if (EditorGUIUtility.isProSkin)
            {
                bannerColor = new Color(bannerColor.r, bannerColor.g, bannerColor.b, 1f);
            }
            else
            {
                bannerColor = BConst.ColorLightGray;
            }

            if (bannerColor.r + bannerColor.g + bannerColor.b <= 1.5)
            {
                guiColor = BConst.ColorLightGray;
            }
            else
            {
                guiColor = BConst.ColorDarkGray;
            }

            GUI.color = bannerColor;

            GUI.DrawTexture(bannerBeginRect, BConst.BannerImageBegin, ScaleMode.StretchToFill, true);
            GUI.DrawTexture(bannerMiddleRect, BConst.BannerImageMiddle, ScaleMode.StretchToFill, true);
            GUI.DrawTexture(bannerEndRect, BConst.BannerImageEnd, ScaleMode.StretchToFill, true);

            GUI.color = guiColor;

            GUI.Label(bannerFullRect, "<size=14><color=#" + ColorUtility.ToHtmlStringRGB(guiColor) + "><b>" + bannerText + "</b></color></size>", BConst.BannerTitleStyle);

            if (GUI.Button(iconRect, BConst.IconHelp, new GUIStyle { alignment = TextAnchor.MiddleCenter }))
            {
                Application.OpenURL(helpURL);
            }

            GUI.color = Color.white;
            GUILayout.Space(20);            
        }

        public static void DrawMaterialBanner(Color bannerColor, string bannerText, string bannerSubText, Shader shader)
        {
            GUILayout.Space(10);
            var bannerFullRect = GUILayoutUtility.GetRect(0, 0, 40, 0);
            var bannerBeginRect = new Rect(bannerFullRect.position.x, bannerFullRect.position.y, 20, 40);
            var bannerMiddleRect = new Rect(bannerFullRect.position.x + 20, bannerFullRect.position.y, bannerFullRect.xMax - 54, 40);
            var bannerEndRect = new Rect(bannerFullRect.xMax - 20, bannerFullRect.position.y, 20, 40);
            var iconRect = new Rect(bannerFullRect.xMax - 36, bannerFullRect.position.y + 5, 30, 30);

            Color guiColor;

            if (EditorGUIUtility.isProSkin)
            {
                bannerColor = new Color(bannerColor.r, bannerColor.g, bannerColor.b, 1f);
            }
            else
            {
                bannerColor = BConst.ColorLightGray;
            }

            if (bannerColor.r + bannerColor.g + bannerColor.b <= 1.5)
            {
                guiColor = BConst.ColorLightGray;
            }
            else
            {
                guiColor = BConst.ColorDarkGray;
            }

            GUI.color = bannerColor;

            GUI.DrawTexture(bannerBeginRect, BConst.BannerImageBegin, ScaleMode.StretchToFill, true);
            GUI.DrawTexture(bannerMiddleRect, BConst.BannerImageMiddle, ScaleMode.StretchToFill, true);
            GUI.DrawTexture(bannerEndRect, BConst.BannerImageEnd, ScaleMode.StretchToFill, true);

            GUI.color = guiColor;

            GUI.Label(bannerFullRect, "<size=14><color=#" + ColorUtility.ToHtmlStringRGB(guiColor) + "><b>" + bannerText + "</b> " + bannerSubText + "</color></size>", BConst.BannerTitleStyle);

#if AMPLIFY_SHADER_EDITOR
            if (GUI.Button(iconRect, BConst.IconEdit, new GUIStyle { alignment = TextAnchor.MiddleCenter }))
            {                
                AmplifyShaderEditor.AmplifyShaderEditorWindow.ConvertShaderToASE(shader);
            }
#else
            if (GUI.Button(iconRect, BConst.IconEdit, new GUIStyle { alignment = TextAnchor.MiddleCenter }))
            {                
                AssetDatabase.OpenAsset(shader, 1);
            }
#endif

            GUI.color = Color.white;
            GUILayout.Space(10); 
        }

        public static void DrawCategory(Rect position, string bannerText)
        {
            var categoryFullRect = new Rect(position.position.x, position.position.y + 10, position.width, position.height);            
            var categoryBeginRect = new Rect(categoryFullRect.position.x, categoryFullRect.position.y, 10, 20);
            var categoryMiddleRect = new Rect(categoryFullRect.position.x + 10, categoryFullRect.position.y, categoryFullRect.xMax - 32, 20);
            var categoryEndRect = new Rect(categoryFullRect.xMax - 10, categoryFullRect.position.y, 10, 20);
            var titleRect = new Rect(categoryFullRect.position.x, categoryFullRect.position.y, categoryFullRect.width, 18);

            GUI.color = BConst.ColorStandardDim;

            //GUI.DrawTexture(categoryBeginRect, BConst.CategoryImageBegin, ScaleMode.StretchToFill, true);
            //GUI.DrawTexture(categoryMiddleRect, BConst.CategoryImageMiddle, ScaleMode.StretchToFill, true);
            //GUI.DrawTexture(categoryEndRect, BConst.CategoryImageEnd, ScaleMode.StretchToFill, true);

            //Workaround for flickering images in CustomInspector with Attribute
            GUIStyle styleB = new GUIStyle();
            styleB.normal.background = BConst.CategoryImageBegin;
            EditorGUI.LabelField(categoryBeginRect, GUIContent.none, styleB);

            GUIStyle styleM = new GUIStyle();
            styleM.normal.background = BConst.CategoryImageMiddle;
            EditorGUI.LabelField(categoryMiddleRect, GUIContent.none, styleM);

            GUIStyle styleE = new GUIStyle();
            styleE.normal.background = BConst.CategoryImageEnd;
            EditorGUI.LabelField(categoryEndRect, GUIContent.none, styleE);

            GUI.color = Color.white;
            GUI.Label(titleRect, bannerText, BConst.TitleStyle);
        }

        public static void DrawWindowCategory(string bannerText)
        {

            var position = GUILayoutUtility.GetRect(0, 0, 40, 0);
            var categoryFullRect = new Rect(position.position.x, position.position.y + 10, position.width, position.height);
            var categoryBeginRect = new Rect(categoryFullRect.position.x, categoryFullRect.position.y, 10, 20);
            var categoryMiddleRect = new Rect(categoryFullRect.position.x + 10, categoryFullRect.position.y, categoryFullRect.xMax - 33, 20);
            var categoryEndRect = new Rect(categoryFullRect.xMax - 6, categoryFullRect.position.y, 10, 20);
            var titleRect = new Rect(categoryFullRect.position.x, categoryFullRect.position.y, categoryFullRect.width, 18);

            GUI.color = BConst.ColorStandardDim;

            //GUI.DrawTexture(categoryBeginRect, BConst.CategoryImageBegin, ScaleMode.StretchToFill, true);
            //GUI.DrawTexture(categoryMiddleRect, BConst.CategoryImageMiddle, ScaleMode.StretchToFill, true);
            //GUI.DrawTexture(categoryEndRect, BConst.CategoryImageEnd, ScaleMode.StretchToFill, true);

            //Workaround for flickering images in CustomInspector with Attribute
            GUIStyle styleB = new GUIStyle();
            styleB.normal.background = BConst.CategoryImageBegin;
            EditorGUI.LabelField(categoryBeginRect, GUIContent.none, styleB);

            GUIStyle styleM = new GUIStyle();
            styleM.normal.background = BConst.CategoryImageMiddle;
            EditorGUI.LabelField(categoryMiddleRect, GUIContent.none, styleM);

            GUIStyle styleE = new GUIStyle();
            styleE.normal.background = BConst.CategoryImageEnd;
            EditorGUI.LabelField(categoryEndRect, GUIContent.none, styleE);

            GUI.color = Color.white;
            GUI.Label(titleRect, bannerText, BConst.TitleStyle);

        }
        public static void DrawLogo()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("");

            if (GUILayout.Button(BConst.LogoImage, GUI.skin.label, GUILayout.Width(40)))
            {
                Application.OpenURL("https://boxophobic.com/");
            }
            GUILayout.Label("");
            GUILayout.EndHorizontal();
            GUILayout.Space(20);
        }
    }
}

