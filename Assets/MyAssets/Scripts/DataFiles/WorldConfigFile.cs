using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct WorldConfig
{
    public int SubWorld_Count_X_Axis_Per_WorldArea;
    public int SubWorld_Count_Y_Axis_Per_WorldArea;
    public int SubWorld_Count_Z_Axis_Per_WorldArea;
    public float OneTileUnit;
    public int SubWorldSizeX;
    public int SubWorldSizeY;
    public int SubWorldSizeZ;
    public int ChunkSize;
    public float ChunkLoadIntervalSeconds;
    public WorldEnviromentsConfig EnviromentsConfig;
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

public class WorldConfigFile : BaseDataFile
{
    private WorldConfig Config;
    public static WorldConfigFile Instance = null;
    // Use this for initialization
    public override void Init ()
    {
        Instance = this;
        JsonFile = Resources.Load(ConstFilePath.TXT_WORLD_CONFIG_DATA) as TextAsset;
        JsonObject = new JSONObject(JsonFile.text);
        AccessData(JsonObject);
    }
    public WorldConfig GetConfig()
    {
        return Config;
    }
    protected override void AccessData(JSONObject jsonObj)
    {
        switch (jsonObj.type)
        {
            case JSONObject.Type.OBJECT:
                //to do
                var data = JsonObject.ToDictionary();
                string extractedValue;
                data.TryGetValue("ChunkLoadIntervalSeconds", out extractedValue);
                Config.ChunkLoadIntervalSeconds = float.Parse(extractedValue);
                //
                data.TryGetValue("SubWorldSizeX", out extractedValue);
                Config.SubWorldSizeX = int.Parse(extractedValue);
                //
                data.TryGetValue("SubWorldSizeY", out extractedValue);
                Config.SubWorldSizeY = int.Parse(extractedValue);
                //
                data.TryGetValue("SubWorldSizeZ", out extractedValue);
                Config.SubWorldSizeZ = int.Parse(extractedValue);
                //
                data.TryGetValue("OneTileUnit", out extractedValue);
                Config.OneTileUnit = float.Parse(extractedValue);
                //
                data.TryGetValue("ChunkSize", out extractedValue);
                Config.ChunkSize = int.Parse(extractedValue);
                //
                data.TryGetValue("SubWorld_Count_X_Axis_Per_WorldArea", out extractedValue);
                Config.SubWorld_Count_X_Axis_Per_WorldArea = int.Parse(extractedValue);
                //
                data.TryGetValue("SubWorld_Count_Y_Axis_Per_WorldArea", out extractedValue);
                Config.SubWorld_Count_Y_Axis_Per_WorldArea = int.Parse(extractedValue);
                //
                data.TryGetValue("SubWorld_Count_Z_Axis_Per_WorldArea", out extractedValue);
                Config.SubWorld_Count_Z_Axis_Per_WorldArea = int.Parse(extractedValue);
                break;
            case JSONObject.Type.ARRAY:
                break;
            default:
                Debug.Log("Json Level Data Sheet Access ERROR");
                break;
        }

    }
}
