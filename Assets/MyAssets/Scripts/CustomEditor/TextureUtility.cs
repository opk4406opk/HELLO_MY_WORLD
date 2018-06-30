#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

public class TextureUtility : EditorWindow
{
    //https://docs.unity3d.com/ScriptReference/RenderTexture-active.html
    //https://stackoverflow.com/questions/44264468/convert-rendertexture-to-texture2d
    [MenuItem("CustomEditor/TextureUtility/RenderTextureToTexture2D")]
    public static void RenderTextureToTexture2D()
    {
        KojeomLogger.DebugLog("캐릭터 렌더텍스처를 텍스처2D로 변환 시작합니다.", LOG_TYPE.EDITOR_TOOL);
        RenderTexture[] renderTextures = Resources.LoadAll<RenderTexture>(ConstFilePath.CH_RT_FILES_PATH);
        foreach(var rt in renderTextures)
        {
            RenderTexture.active = rt;

            Texture2D texture2D = new Texture2D(rt.width, rt.height, TextureFormat.RGBA32, false);
            texture2D.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            texture2D.Apply();
            string fileName = string.Format("texture2D_{0}.png", rt.name);
            string filePath = string.Format("{0}{1}", ConstFilePath.CHAR_TEXTURE2D_PATH, fileName);
            File.WriteAllBytes(filePath, texture2D.EncodeToPNG());
        }
        KojeomLogger.DebugLog("캐릭터 렌더텍스처를 텍스처2D로 변환 작업 완료했습니다.", LOG_TYPE.EDITOR_TOOL);
    }
}
#endif