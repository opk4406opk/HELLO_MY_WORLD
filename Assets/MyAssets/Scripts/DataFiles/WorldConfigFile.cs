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
    public WorldEnviromentsConfig enviromentsConfig;
}

/// <summary>
/// 나무, 산, 바다,강 등 여러 자연적인 형태에 대한 Config 정보.
/// </summary>
public struct WorldEnviromentsConfig
{
    //tree
    public int minTreeBodyLength;
    public int maxTreeBodyLength;
    public int minTreeBranchDepth;
    public int maxTreeBranchDepth;
    //
}

public class WorldConfigFile {
    private JSONObject jsonObject;
    private TextAsset jsonFile;

    private WorldConfig Config;
    public static WorldConfigFile Instance = null;
    // Use this for initialization
    public void Init ()
    {
        Instance = this;
        jsonFile = Resources.Load(ConstFilePath.TXT_WORLD_CONFIG_DATA) as TextAsset;
        jsonObject = new JSONObject(jsonFile.text);
        AccessData(jsonObject);
    }
    public WorldConfig GetConfig()
    {
        return Config;
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
                Config.chunkLoadIntervalSeconds = float.Parse(extractedValue);
                //
                data.TryGetValue("sub_world_x_size", out extractedValue);
                Config.sub_world_x_size = int.Parse(extractedValue);
                //
                data.TryGetValue("sub_world_y_size", out extractedValue);
                Config.sub_world_y_size = int.Parse(extractedValue);
                //
                data.TryGetValue("sub_world_z_size", out extractedValue);
                Config.sub_world_z_size = int.Parse(extractedValue);
                //
                data.TryGetValue("one_tile_unit", out extractedValue);
                Config.one_tile_unit = float.Parse(extractedValue);
                //
                data.TryGetValue("chunk_size", out extractedValue);
                Config.chunk_size = int.Parse(extractedValue);
                break;
            case JSONObject.Type.ARRAY:
                break;
            default:
                Debug.Log("Json Level Data Sheet Access ERROR");
                break;
        }

    }
}
