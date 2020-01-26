using UnityEngine;

public struct GameServerData
{
    public string login_server_ip;
    public int login_server_port;
    public string gamelog_server_ip;
    public int gamelog_server_port;
}

public class GameServerDataFile : BaseDataFile
{
    private static GameServerDataFile _singleton = null;
    public static GameServerDataFile singleton
    {
        get
        {
            if (_singleton == null)
            {
                _singleton = new GameServerDataFile();
            }
            return _singleton;
        }
    }

    private GameServerData gameServerData;

    private GameServerDataFile()
    {
        JsonFile = Resources.Load<TextAsset>(ConstFilePath.TXT_RESOURCE_GAME_SERVER_DATA);
        JsonObject = new JSONObject(JsonFile.text);
        AccessData(JsonObject);
    }

    public GameServerData GetGameServerData()
    {
        return gameServerData;
    }

    protected override void AccessData(JSONObject jsonObj)
    {
        switch (jsonObj.type)
        {
            case JSONObject.Type.OBJECT:
                var data = jsonObj.ToDictionary();
                string extractedData;
                data.TryGetValue("login_server_ip", out extractedData);
                gameServerData.login_server_ip = extractedData;
                //
                data.TryGetValue("login_server_port", out extractedData);
                gameServerData.login_server_port = int.Parse(extractedData);
                //
                data.TryGetValue("gamelog_server_ip", out extractedData);
                gameServerData.gamelog_server_ip = extractedData;
                //
                data.TryGetValue("gamelog_server_port", out extractedData);
                gameServerData.gamelog_server_port = int.Parse(extractedData);
                break;
            default:
                break;
        }
    }

    public override void Init()
    {
        AccessData(JsonObject);
    }
}
