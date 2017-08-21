using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstFilePath {

    // 상대주소. Json 형식의 파일 path 모음.
    public readonly static string TXT_NPC_DATAS = "TextAsset/ActorData/NPC_DATAS";
    public readonly static string TXT_CHARACTER_DATAS = "TextAsset/ChDatas/characterDatas";
    public readonly static string TXT_CRAFT_ITEM_LIST_DATAS = "TextAsset/ItemDatas/craftItemListDatas";
    public readonly static string TXT_ITEM_DATAS = "TextAsset/ItemDatas/itemDatas";
    public readonly static string TXT_TYPE_TO_ITEM_DATAS = "TextAsset/ItemDatas/typeToItemID";
    public readonly static string TXT_SUB_WORLD_DEFAULT_DATAS = "TextAsset/SubWorldDefaultData/subworld_default";
    public readonly static string TXT_TILE_DATAS = "TextAsset/TileDatas/tileDatas";

    //캐릭터 렌더링 텍스처 베이스 파일이 있는 path와 directory.
    public readonly static string CH_RT_BASE_FILE_WITH_EXT = "Assets/MyAssets/Resources/Texture(RT)/RT_base.renderTexture";
    public readonly static string CH_RT_BASE_FILE_DIR = "Assets/MyAssets/Resources/Texture(RT)/{0}.renderTexture";

    /// <summary>
    /// Resource.Load 사용시에 쓰이는 캐릭터 RT 리소스 Path.
    /// </summary>
    public readonly static string CH_RT_RESOURCE_PATH = "GamePrefabs/Ch_forRenderTexture/Texture(RT)/{0}";

    /// <summary>
    /// 캐릭터 프리팹이 위치한 path.
    /// </summary>
    public readonly static string PREFAB_CHARACTER = "GamePrefabs/Characters/user/{0}";

    /// <summary>
    ///  RenderTexture에 사용될 캐릭터 프리팹이 저장될 path.
    /// </summary>
    public readonly static string SAVE_PATH_FOR_RT_PREFAB = "Assets/MyAssets/Resources/GamePrefabs/Ch_forRenderTexture/{0}.prefab";
}
