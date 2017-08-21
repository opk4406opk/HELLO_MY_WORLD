using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Chracter RenderTexture 생성기.
/// </summary>
public class Ch_RTMaker : EditorWindow
{
    private string filePath = null;

    private JSONObject chDataJsonObj;
    private TextAsset jsonFile;
    private List<Dictionary<string, string>> jsonDataSheet;
    private int maxChCard = 0;

    [MenuItem("CustomEditor/Ch_RTMaker")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        Ch_RTMaker window = (Ch_RTMaker)EditorWindow.GetWindow(typeof(Ch_RTMaker));
        window.Show();
    }

    void OnGUI()
    {
        EditorGUILayout.BeginToggleGroup("Func", true);
        if (GUILayout.Button("Open CharacteInfo file")) ClickOpenFile();
        if (GUILayout.Button("Start Process")) CreateRT_Files();
        EditorGUILayout.EndToggleGroup();
    }

    private void ClickOpenFile()
    {
        filePath =  EditorUtility.OpenFilePanel("OpenFile Dialog", "C:\\", "");
        if(filePath != null)
        {
            LoadChDatas();
        }
    }

    private void CreateRT_Files()
    {
        GameObject rtChGroup = new GameObject();
        rtChGroup.name = "RT_Ch_Group";

        // create renderTexture files.
        for(int i = 0; i < maxChCard; i++)
        {
            string textureFileName = null;
            jsonDataSheet[i].TryGetValue("chFaceTextureName", out textureFileName);

            StringBuilder toPath = new StringBuilder();
            toPath.AppendFormat(ConstFilePath.CH_RT_BASE_FILE_DIR, textureFileName);

            string sourcePath = ConstFilePath.CH_RT_BASE_FILE_WITH_EXT;
            string destPath = toPath.ToString();
            if (File.Exists(destPath) == false) FileUtil.CopyFileOrDirectory(sourcePath, destPath);
            else Debug.Log(string.Format("{0} 파일은 {1} 위치에 이미 존재합니다.", textureFileName, destPath));
            // renderTexture에 사용될 캐릭터 prefab 생성.
            CreateChPrefabs_ForRT(i, rtChGroup.transform, textureFileName);
        }

        // create rtChGroup prefab.
        StringBuilder rtGroupPrefabPath = new StringBuilder();
        rtGroupPrefabPath.AppendFormat(ConstFilePath.SAVE_PATH_FOR_RT_PREFAB, rtChGroup.name);
        PrefabUtility.CreatePrefab(rtGroupPrefabPath.ToString(), rtChGroup);
        // and then destroy object in Scene.
        DestroyImmediate(rtChGroup);
    }

    private void CreateChPrefabs_ForRT(int idx, Transform group, string rtFileName)
    {
        int objOffset = 10;

        string prefabName = null;
        jsonDataSheet[idx].TryGetValue("chPrefabName", out prefabName);
        string chName = null;
        jsonDataSheet[idx].TryGetValue("chName", out chName);

        // Instanciate character object;
        StringBuilder prefabPath = new StringBuilder();
        prefabPath.AppendFormat(ConstFilePath.PREFAB_CHARACTER, prefabName);
        GameObject character = Instantiate(Resources.Load(prefabPath.ToString()),
            new Vector3(idx * objOffset, 0, idx * objOffset), Quaternion.identity) as GameObject;
        character.name = chName;
        character.transform.parent = group;

        // set camera - position, rot, parenting, naming
        GameObject cam = new GameObject();
        cam.transform.position = character.transform.position;
        cam.transform.Rotate(new Vector3(0, 180.0f, 0));
        cam.transform.parent = character.transform;
        cam.name = "RT_Camera";
        // set camera - addComp, cullMaks, SetRT
        cam.AddComponent<Camera>();
        cam.GetComponent<Camera>().cullingMask = LayerMask.NameToLayer("PlayerCharacter");

        StringBuilder targetRT_Path = new StringBuilder();
        targetRT_Path.AppendFormat(ConstFilePath.CH_RT_RESOURCE_PATH, rtFileName);
        //FileStream fs = File.Open(targetRT_Path.ToString(), FileMode.Open);
        // RenderTexture는 Object를 상속한 형태가 아님.
        // 그냥 byte 덩어리의 파일.
        // 따라서, RenderTexture 파일을 런타임에 읽어들여 카메라에 붙이고싶어도..음..
        // De/Serialize 를 이용하면 가능은한데 쓸데없이 부피가 커진다.
        // 어쨋든 Resources.Load 메소드로는 Object 형태가 아닌 RenderTexture는 불러올 수 없다. 
        cam.GetComponent<Camera>().targetTexture = Resources.Load<RenderTexture>(targetRT_Path.ToString());
    }

    private void LoadChDatas()
    {
        jsonDataSheet = new List<Dictionary<string, string>>();
        jsonFile = Resources.Load(ConstFilePath.TXT_CHARACTER_DATAS) as TextAsset;
        chDataJsonObj = new JSONObject(jsonFile.text);
        AccessData(chDataJsonObj);
    }

    private void AccessData(JSONObject jsonObj)
    {
        switch (jsonObj.type)
        {
            case JSONObject.Type.OBJECT:
                break;
            case JSONObject.Type.ARRAY:
                maxChCard = jsonObj.Count;
                for (int idx = 0; idx < jsonObj.Count; ++idx)
                {
                    jsonDataSheet.Add(jsonObj.list[idx].ToDictionary());
                }
                break;
            default:
                Debug.Log("Json Level Data Sheet Access ERROR");
                break;
        }

    }
}
