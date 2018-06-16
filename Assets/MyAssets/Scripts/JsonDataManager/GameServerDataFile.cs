using UnityEngine;

public struct GameServerData
{
    public string login_server_ip;
    public int login_server_port;
}

public class GameServerDataFile {
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

    private JSONObject serverDataFileJosnObj;
    private TextAsset serverDataFile;
    private GameServerData gameServerData;

    private GameServerDataFile()
    {
        serverDataFile = Resources.Load<TextAsset>("TextAsset/game_server_data");
        serverDataFileJosnObj = new JSONObject(serverDataFile.text);
        ExtractServerData(serverDataFileJosnObj);
    }

    public GameServerData GetGameServerData()
    {
        return gameServerData;
    }

    private void ExtractServerData(JSONObject jsonObj)
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
                break;
            default:
                break;
        }
    }
}
