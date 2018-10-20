using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct WorldConfig
{
    public float chunkLoadIntervalSeconds;
}

public class WorldConfigFile : MonoBehaviour {
    private JSONObject jsonObject;
    private TextAsset jsonFile;

    private WorldConfig config;
    public static WorldConfigFile instance = null;
    // Use this for initialization
    public void Init ()
    {
        instance = this;
        jsonFile = Resources.Load(ConstFilePath.TXT_WORLD_CONFIG_DATA) as TextAsset;
        jsonObject = new JSONObject(jsonFile.text);
        AccessData(jsonObject);
    }
    public WorldConfig GetConfig()
    {
        return config;
    }
    private void AccessData(JSONObject jsonObj)
    {
        switch (jsonObj.type)
        {
            case JSONObject.Type.OBJECT:
                //to do
                break;
            case JSONObject.Type.ARRAY:
                var data = jsonObject.ToDictionary();
                string extractedValue;
                data.TryGetValue("chunkLoadIntervalSeconds", out extractedValue);
                config.chunkLoadIntervalSeconds = float.Parse(extractedValue);
                break;
            default:
                Debug.Log("Json Level Data Sheet Access ERROR");
                break;
        }

    }
}
