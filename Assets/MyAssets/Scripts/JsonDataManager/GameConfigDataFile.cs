using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GameConfigData
{
    public int sub_world_x_size;
    public int sub_world_y_size;
    public int sub_world_z_size;
    public int chunk_size;
    public int ingame_font_size;
    public float one_tile_unit;
}

public class GameConfigDataFile : MonoBehaviour {
    private static GameConfigDataFile _singleton = null;
    public static GameConfigDataFile singleton
    {
        get
        {
            if (_singleton == null) KojeomLogger.DebugLog("GameConfigDataFile 제대로 초기화 되지 않았습니다", LOG_TYPE.ERROR);
            return _singleton;
        }
    }

    private JSONObject configFileJosnObj;
    private TextAsset configFile;
    private GameConfigData gameConfigData;

    public void Init()
    {
        _singleton = this;
        configFile = Resources.Load<TextAsset>("TextAsset/game_config");
        configFileJosnObj = new JSONObject(configFile.text);
        ExtractGameConfigData(configFileJosnObj);
    }

    public void ResetManager()
    {
    }

    public GameConfigData GetGameConfigData()
    {
        return gameConfigData;
    }

    private void ExtractGameConfigData(JSONObject jsonObj)
    {
        switch (jsonObj.type)
        {
            case JSONObject.Type.OBJECT:
                var data = jsonObj.ToDictionary();
                string extractedData;
                data.TryGetValue("sub_world_x_size", out extractedData);
                gameConfigData.sub_world_x_size = int.Parse(extractedData);
                //
                data.TryGetValue("sub_world_y_size", out extractedData);
                gameConfigData.sub_world_y_size = int.Parse(extractedData);
                //
                data.TryGetValue("sub_world_z_size", out extractedData);
                gameConfigData.sub_world_z_size = int.Parse(extractedData);
                //
                data.TryGetValue("one_tile_unit", out extractedData);
                gameConfigData.one_tile_unit = float.Parse(extractedData);
                //
                data.TryGetValue("ingame_font_size", out extractedData);
                gameConfigData.ingame_font_size = int.Parse(extractedData);
                //
                data.TryGetValue("chunk_size", out extractedData);
                gameConfigData.chunk_size = int.Parse(extractedData);
                break;
            case JSONObject.Type.ARRAY:
                break;
            default:
                break;
        }
    }
}
