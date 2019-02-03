using UnityEngine;

public struct GameConfigData
{
    public int ingame_font_size;
}

public class GameConfigDataFile {
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
                data.TryGetValue("ingame_font_size", out extractedData);
                gameConfigData.ingame_font_size = int.Parse(extractedData);
                break;
            case JSONObject.Type.ARRAY:
                break;
            default:
                break;
        }
    }
}
