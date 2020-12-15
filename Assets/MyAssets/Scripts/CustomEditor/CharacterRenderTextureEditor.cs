#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEditor;
/// <summary>
/// Json파일로 저장될 캐릭터 데이터 
/// </summary>
public struct CharacterJsonDataFormat
{
	public string chName;
	public string chType;
	public string chLevel;
	public string detailScript;
	public string chFaceTextureName;
	public string chPrefabName;
}

/// <summary>
/// Chracter RenderTexture 생성기.
/// </summary>
public class CharacterRenderTextureEditor : EditorWindow
{
    private string filePath = null;

    private JSONObject chDataJsonObj;
    private TextAsset jsonFile;
    private List<Dictionary<string, string>> jsonDataSheet;
    private int maxChCard = 0;

    [MenuItem("CustomEditor/Character Front View To RenderTexture")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        CharacterRenderTextureEditor window = (CharacterRenderTextureEditor)EditorWindow.GetWindow(typeof(CharacterRenderTextureEditor));
        window.Show();
    }

    void OnGUI()
    {
        EditorGUILayout.BeginToggleGroup("Features", true);
        GUILayout.TextArea("(1) 캐릭터 프리팹들의 정면 모습을 렌더링텍스처 파일들로 만들어냅니다.\n " +
                           "(2) 작업이 완료되면, SelectChars 프리팹을 Scene에 배치시킨 후, TextureUtility 툴에서 렌더링텍스처 to 텍스처 작업을 시작합니다.");
        if (GUILayout.Button("Process_AllInOne( Make DataFile and RenderTextures )"))
        {
            CreateCharsDataFile();
            ClickOpenFile();
            CreateRenderTextureFiles();
        }

        EditorGUILayout.EndToggleGroup();
    }

	private void CreateCharsDataFile()
	{
		GameObject[] charPrefabs = Resources.LoadAll<GameObject>(ConstFilePath.PREFAB_CHARACTER_RESOURCE_PATH);
		if (charPrefabs == null) KojeomLogger.DebugLog("캐릭터 프리팹로딩에 실패했습니다.(데이터파일생성에 필요한)", LOG_TYPE.ERROR);
		else
		{
			int idx = 0;
			CharacterJsonDataFormat[] datas = new CharacterJsonDataFormat[charPrefabs.Length];
			foreach(var p in charPrefabs)
			{
				CharacterJsonDataFormat format = new CharacterJsonDataFormat();
				format.chName = p.name;
				format.chPrefabName = p.name;
				format.chFaceTextureName = p.name;
				format.detailScript = "Something here..";
				format.chLevel = "1";
				format.chType = idx.ToString();

				datas[idx] = format;
				idx++;
			}
			string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(datas, Newtonsoft.Json.Formatting.Indented);
			//Creates a new Json file, writes the specified string to the file,
			//and then closes the file. If the target file already exists, it is overwritten.
			File.WriteAllText(ConstFilePath.WINDOW_PATH_CHARACTER_DATAS_FILE, jsonData);
			KojeomLogger.DebugLog("CreateCharsDataFile Done.");
		}
	}

    private void ClickOpenFile()
    {
        //EditorUtility.OpenFilePanel("OpenFile Dialog", "C:\\", "");
        // load json file.
        jsonDataSheet = new List<Dictionary<string, string>>();
        jsonFile = Resources.Load(ConstFilePath.TXT_RESOURCE_CHARACTER_DATAS) as TextAsset;
        chDataJsonObj = new JSONObject(jsonFile.text);
        AccessData(chDataJsonObj);
        //
        KojeomLogger.DebugLog("LoadCharDatas Done.");
	}

    private void CreateRenderTextureFiles()
    {
        GameObject newSelectCharsGroup = Instantiate<GameObject>(Resources.Load<GameObject>(ConstFilePath.SELECT_CHARS_TEMPLATE_PREFAB));
        newSelectCharsGroup.name = "SelectCharacters";  

        Light sceneEnvLight = Instantiate<GameObject>(Resources.Load<GameObject>(ConstFilePath.EDITOR_SCENE_ENV_LIGHT_PREFAB)).GetComponent<Light>();
        sceneEnvLight.transform.parent = newSelectCharsGroup.transform;

        // create renderTexture files.
        for (int i = 0; i < maxChCard; i++)
        {
            string outTextureFileName = null;
            jsonDataSheet[i].TryGetValue("chFaceTextureName", out outTextureFileName);

            StringBuilder toPath = new StringBuilder();
            toPath = toPath.AppendFormat(ConstFilePath.CH_RT_BASE_FILE_DIR, outTextureFileName);

            string sourcePath = ConstFilePath.CH_RT_BASE_FILE_WITH_EXT;
            string destPath = toPath.ToString();

            FileUtil.DeleteFileOrDirectory(destPath); // 기존 파일 삭제
            FileUtil.CopyFileOrDirectory(sourcePath, destPath); // 새 파일 생성.

            // renderTexture에 사용될 캐릭터 prefab 생성.
            CreateSelectCharPrefab(i, newSelectCharsGroup.transform, outTextureFileName);
        }
        //
        StringBuilder selectCharsPrefabPath = new StringBuilder();
        selectCharsPrefabPath = selectCharsPrefabPath.AppendFormat(ConstFilePath.SAVE_PATH_FOR_SELECT_CHARS_PREFAB, "SelectCharacters");
        bool outSuccess = false;
        PrefabUtility.SaveAsPrefabAsset(newSelectCharsGroup, selectCharsPrefabPath.ToString(), out outSuccess);
        
        KojeomLogger.DebugLog("CreateRenderTextureFiles Done.");
	}

    private void CreateSelectCharPrefab(int idx, Transform group, string rtFileName)
    {
        int objIntervalPos = 10;

        string prefabName = null;
        jsonDataSheet[idx].TryGetValue("chPrefabName", out prefabName);
        string chName = null;
        jsonDataSheet[idx].TryGetValue("chName", out chName);

        // Instanciate character object;
        StringBuilder prefabPath = new StringBuilder();
        prefabPath.AppendFormat(ConstFilePath.PREFAB_CHARACTER_RESOURCE_PATH + "{0}", prefabName);
        GameObject character = Instantiate(Resources.Load(prefabPath.ToString()),
            new Vector3(idx * objIntervalPos, 0, idx * objIntervalPos), Quaternion.identity) as GameObject;
        character.name = chName;
        character.transform.parent = group;

        // set camera - position, rot, parenting, naming
        GameObject camObj = new GameObject();
        camObj.transform.position = character.transform.position;
        camObj.transform.Rotate(new Vector3(0, 180.0f, 0));
        camObj.transform.parent = character.transform;
        camObj.name = "RenderTextureCamera";
		Vector3 newPos = camObj.transform.position;
		newPos.y += 0.5f;
		newPos.z += 1.0f;
		camObj.transform.position = newPos;
		// set camera - addComp, cullMasks, SetRT
        Camera cam = camObj.AddComponent<Camera>();
		// 10번째 인덱스의 Layer를 컬링마스크 해야 한다.
		// ref : https://docs.unity3d.com/Manual/Layers.html
		cam.cullingMask = LayerMask.NameToLayer("PlayerCharacter");
		cam.farClipPlane = 1.0f;
        cam.clearFlags = CameraClearFlags.Nothing;

		StringBuilder targetRT_Path = new StringBuilder();
        targetRT_Path = targetRT_Path.AppendFormat(ConstFilePath.SELECT_CHARS_RT_RESOURCE_PATH, rtFileName);
		//FileStream fs = File.Open(targetRT_Path.ToString(), FileMode.Open);
		// RenderTexture는 Texture 상속, 그리고 Texture는 오브젝트 상속.
		RenderTexture rt = Resources.Load<RenderTexture>(targetRT_Path.ToString());
		cam.targetTexture = rt;
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
#endif