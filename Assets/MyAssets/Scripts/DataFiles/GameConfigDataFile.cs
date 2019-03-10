using UnityEngine;

public struct GameConfigData
{
    public int ingame_font_size;
}

public class GameConfigDataFile {
    private static GameConfigDataFile _Instance = null;
    public static GameConfigDataFile Instance
    {
        get
        {
            if (_Instance == null) KojeomLogger.DebugLog("GameConfigDataFile 제대로 초기화 되지 않았습니다", LOG_TYPE.ERROR);
            return _Instance;
        }
    }

    private JSONObject ConfigFileJosnObj;
    private TextAsset ConfigFile;
    private GameConfigData GameConfigData;

    public void Init()
    {
        _Instance = this;
        ConfigFile = Resources.Load<TextAsset>("TextAsset/game_config");
        ConfigFileJosnObj = new JSONObject(ConfigFile.text);
        ExtractGameConfigData(ConfigFileJosnObj);
    }

    public void ResetManager()
    {
    }

    public GameConfigData GetGameConfigData()
    {
        return GameConfigData;
    }

    private void ExtractGameConfigData(JSONObject jsonObj)
    {
        switch (jsonObj.type)
        {
            case JSONObject.Type.OBJECT:
                var data = jsonObj.ToDictionary();
                string extractedData;
                data.TryGetValue("ingame_font_size", out extractedData);
                GameConfigData.ingame_font_size = int.Parse(extractedData);
                break;
            case JSONObject.Type.ARRAY:
                break;
            default:
                break;
        }
    }
}
