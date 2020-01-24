// Cristian Pop - https://boxophobic.com/

using UnityEditor;
using UnityEngine;

namespace Boxophobic
{

    public static class BEditorUtils
    {
        private static object serializedObject;

        //public static BVersion GetVersionFile(MonoScript ms)
        //{

        //    BVersion versionFile = null;

        //    string scriptFileName = ms.name + ".cs";
        //    string scriptFilePath = AssetDatabase.GetAssetPath(ms);
        //    scriptFilePath = scriptFilePath.Replace(scriptFileName, "");


        //    if (AssetDatabase.LoadAssetAtPath(scriptFilePath + scriptFileName.Replace("Window.cs", "Version.asset"), typeof(BVersion)))
        //    {
        //        versionFile = AssetDatabase.LoadAssetAtPath(scriptFilePath + "ADSInstallVersion.asset", typeof(BVersion)) as BVersion;
        //    }
        //    else
        //    {
        //        versionFile = null;
        //    }

        //    return versionFile;

        //}

        public static BVersion GetVersionFile(string versionFile)
        {

            string bFolder = GetBoxophobicFolder();
            string versionFilePath = bFolder + "/Utils/Versions/" + versionFile;

            return (BVersion)AssetDatabase.LoadAssetAtPath(versionFilePath, typeof(BVersion));

        }

        public static string GetBoxophobicFolder()
        {

            string[] folder = AssetDatabase.FindAssets("BOXOPHOBIC");
            string boxFolder = null;

            for (int i = 0; i < folder.Length; i++)
            {
                if (AssetDatabase.GUIDToAssetPath(folder[i]).EndsWith("BOXOPHOBIC"))
                {
                    boxFolder = AssetDatabase.GUIDToAssetPath(folder[i]);
                }
            }

            return boxFolder;

        }

        public static void UnityToBoxophobicProperties(Material material)
        {

            if (material.GetFloat("_Internal_UnityToBoxophobic") == 0)
            {

                //Get Standard Shader properties and assign them to Boxophobic Shader properties
                if (material.HasProperty("_Mode"))
                {
                    material.SetFloat("_RenderType", material.GetFloat("_Mode"));
                }

                if (material.HasProperty("_MainTex"))
                {
                    material.SetTexture("_AlbedoTex", material.GetTexture("_MainTex"));

                    Vector4 UVZero = new Vector4(material.GetTextureScale("_MainTex").x, material.GetTextureScale("_MainTex").y, material.GetTextureOffset("_MainTex").x, material.GetTextureOffset("_MainTex").y);

                    material.SetVector("_UVZeo", UVZero);                    
                }

                if (material.HasProperty("_BumpMap"))
                {
                    material.SetTexture("_NormalTex", material.GetTexture("_BumpMap"));
                }

                if (material.HasProperty("_MetallicGlossMap"))
                {
                    material.SetTexture("_SurfaceTex", material.GetTexture("_MetallicGlossMap"));
                }

                if (material.HasProperty("_BumpScale"))
                {
                    material.SetFloat("_NormalScale", material.GetFloat("_BumpScale"));
                }

                if (material.HasProperty("_Glossiness"))
                {
                    material.SetFloat("_Smoothness", material.GetFloat("_Glossiness"));
                }

                material.SetFloat("_Internal_UnityToBoxophobic", 1);
            }

        }
    }
}



