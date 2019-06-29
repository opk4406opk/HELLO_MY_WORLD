using UnityEngine;

public struct GameConfigData
{
    public int ingame_font_size;
}

public class GameConfigDataFile : BaseDataFile
{
    private static GameConfigDataFile _Instance = null;
    public static GameConfigDataFile Instance
    {
        get
        {
            if (_Instance == null) KojeomLogger.DebugLog("GameConfigDataFile 제대로 초기화 되지 않았습니다", LOG_TYPE.ERROR);
            return _Instance;
        }
    }

    private GameConfigData GameConfigData;

    public override void Init()
    {
        _Instance = this;
        JsonFile = Resources.Load<TextAsset>(ConstFilePath.TXT_GAME_CONFIG_DATA);
        JsonObject = new JSONObject(JsonFile.text);
        AccessData(JsonObject);
    }

    public void ResetManager()
    {
    }

    public GameConfigData GetGameConfigData()
    {
        return GameConfigData;
    }

    protected override void AccessData(JSONObject jsonObj)
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
