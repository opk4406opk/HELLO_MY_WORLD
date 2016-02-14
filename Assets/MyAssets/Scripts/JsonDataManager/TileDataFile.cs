using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Tile 데이터를 주고받기 위한 내부 통신용 구조체.
/// </summary>
public struct TileInfo
{
    public string name;
    public int posX;
    public int posY;
    public int type;
}

/// <summary>
/// 블록 수정 또는 삭제, 월드 생성시에 쓰이는 TileDataFile.
/// </summary>
public class TileDataFile : MonoBehaviour {

    private JSONObject tileDataJsonObj;
    private TextAsset jsonFile;
    private Dictionary<string, Dictionary<string, string>> jsonDataSheet;
    private Dictionary<int, string> typeToNameSheet;

    private TileInfo tileData;

    public void Init ()
    {
        typeToNameSheet = new Dictionary<int, string>();
        jsonDataSheet = new Dictionary<string, Dictionary<string, string>>();
        jsonFile = Resources.Load("TextAsset/TileDatas/tileDatas") as TextAsset;
        tileDataJsonObj = new JSONObject(jsonFile.text);
        AccessData(tileDataJsonObj);
    }

    public TileInfo GetTileData(string _tileName)
    {
        if (_tileName == null) Debug.Log("ERROR_tileName is NULL");

        Dictionary<string, string> coordinate;
        jsonDataSheet.TryGetValue(_tileName, out coordinate);

        string posX, posY, type;
        coordinate.TryGetValue("posX", out posX);
        coordinate.TryGetValue("posY", out posY);
        coordinate.TryGetValue("type", out type);

        tileData.name = _tileName;
        tileData.posX = int.Parse(posX);
        tileData.posY = int.Parse(posY);
        tileData.type = int.Parse(type);

        return tileData;
    }

    /// <summary>
    /// tile type 값으로 name을 구한다.
    /// </summary>
    /// <param name="_type"></param>
    /// <returns></returns>
    public string GetTileName(int _type)
    {
        string name;
        typeToNameSheet.TryGetValue(_type, out name);

        return name;
    }

    private void AccessData(JSONObject jsonObj)
    {
        switch (jsonObj.type)
        {
            case JSONObject.Type.OBJECT:
                for (int idx = 0; idx < jsonObj.Count; ++idx)
                {
                    jsonDataSheet.Add(jsonObj.keys[idx],
                        jsonObj.list[idx].ToDictionary());

                    typeToNameSheet.Add(idx, jsonObj.keys[idx]);
                }
                break;
            case JSONObject.Type.ARRAY:
                // to do
                break;
            default:
                Debug.Log("Json Level Data Sheet Access ERROR");
                break;
        }
    }
}
