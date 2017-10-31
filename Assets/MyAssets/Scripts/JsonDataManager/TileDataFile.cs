using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 블록 tile의 type 클래스.
/// </summary>
public class TileType
{
    public static readonly string TILE_TYPE_NONE = "NONE";
    public static readonly string TILE_TYPE_GRASS = "GRASS";
    public static readonly string TILE_TYPE_STONE_BIG = "STONE_BIG";
    public static readonly string TILE_TYPE_STONE_SMALL = "STONE_SMALL";
    public static readonly string TILE_TYPE_SAND = "SAND";
    public static readonly string TILE_TYPE_RED_STONE = "RED_STONE";
    public static readonly string TILE_TYPE_WOOD = "WOOD";
    public static readonly string TILE_TYPE_STONE_GOLD = "STONE_GOLD";
    public static readonly string TILE_TYPE_STONE_IRON = "STONE_IRON";
    public static readonly string TILE_TYPE_STONE_SILVER = "STONE_SILVER";
}
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

    private List<string> _tileNameList;
    public List<string> tileNameList
    {
        get { return _tileNameList; }
    }
    
    private TileInfo tileData;

    public static TileDataFile instance = null;

    public void Init ()
    {
        _tileNameList = new List<string>();
        typeToNameSheet = new Dictionary<int, string>();
        jsonDataSheet = new Dictionary<string, Dictionary<string, string>>();
        jsonFile = Resources.Load(ConstFilePath.TXT_TILE_DATAS) as TextAsset;
        tileDataJsonObj = new JSONObject(jsonFile.text);

        AccessData(tileDataJsonObj);
        SetTileNameList();

        if (instance == null) instance = this;
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

    private void SetTileNameList()
    {
        // 1번의 경우 NONE 이므로, 0번 인덱스는 제외.
        for(int idx = 1; idx < tileDataJsonObj.Count; ++idx)
        {
            _tileNameList.Add(tileDataJsonObj.keys[idx]);
        }
    }
}
