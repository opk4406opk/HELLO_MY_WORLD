using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct WorldConfig
{
    public float one_tile_unit;
    public int sub_world_x_size;
    public int sub_world_y_size;
    public int sub_world_z_size;
    public int chunk_size;
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
                var data = jsonObject.ToDictionary();
                string extractedValue;
                data.TryGetValue("chunkLoadIntervalSeconds", out extractedValue);
                config.chunkLoadIntervalSeconds = float.Parse(extractedValue);
                //
                data.TryGetValue("sub_world_x_size", out extractedValue);
                config.sub_world_x_size = int.Parse(extractedValue);
                //
                data.TryGetValue("sub_world_y_size", out extractedValue);
                config.sub_world_y_size = int.Parse(extractedValue);
                //
                data.TryGetValue("sub_world_z_size", out extractedValue);
                config.sub_world_z_size = int.Parse(extractedValue);
                //
                data.TryGetValue("one_tile_unit", out extractedValue);
                config.one_tile_unit = float.Parse(extractedValue);
                //
                data.TryGetValue("chunk_size", out extractedValue);
                config.chunk_size = int.Parse(extractedValue);
                break;
            case JSONObject.Type.ARRAY:
                break;
            default:
                Debug.Log("Json Level Data Sheet Access ERROR");
                break;
        }

    }
}
