﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임에서 사용되는 여러가지 파일 경로.
/// <br> 1) 유니티 예약 폴더 'Resoruces' 에서는 파일 확장자명은 필요없음. </br>
/// </summary>
public class ConstFilePath
{
    // 상대경로. Json 형식의 데이터 파일 리소스 경로 모음.
    public readonly static string TXT_RESOURCE_NPC_DATAS = "TextAsset/ActorData/NPCDatas";
    public readonly static string TXT_RESOURCE_ANIMAL_DATAS = "TextAsset/ActorData/AnimalDatas";
    public readonly static string TXT_RESOURCE_CHARACTER_DATAS = "TextAsset/ChDatas/CharacterDatas";
    public readonly static string TXT_RESOURCE_CRAFT_ITEM_LIST_DATAS = "TextAsset/ItemDatas/CraftItemListDatas";
    public readonly static string TXT_RESOURCE_WORLD_MAP_DATAS = "TextAsset/GameWorld/WorldMapData";
    public readonly static string TXT_RESOURCE_WORLD_CONFIG_DATA = "TextAsset/GameWorld/WorldConfigData";
    public readonly static string TXT_RESOURCE_BLOCK_TILE_DATAS = "TextAsset/TileDatas/BlockTileDatas";
    public readonly static string TXT_RESOURCE_GAME_CONFIG_DATA = "TextAsset/GameConfigData";
    public readonly static string TXT_RESOURCE_GAME_SERVER_DATA = "TextAsset/GameServerData";
    // 데이터 Table path.
    public readonly static string TXT_RESOURCE_RAW_ELEMENT_TABLE_PATH = "TextAsset/Tables/RawElementTable";
    public readonly static string TXT_RESOURCE_BLOCK_PRODUCT_TABLE_PATH = "TextAsset/Tables/BlockProductTable";
    public readonly static string TXT_RESOURCE_ITEM_TABLE_PATH = "TextAsset/Tables/ItemTable";

    //캐릭터 렌더링 텍스처 베이스 파일이 있는 상대경로 directory.
    public readonly static string CH_RT_BASE_FILE_WITH_EXT = "Assets/MyAssets/Resources/Texture(RT)/userCharacters/CharRTBase.renderTexture";
    public readonly static string CH_RT_BASE_FILE_DIR = "Assets/MyAssets/Resources/Texture(RT)/userCharacters/{0}.renderTexture";
    public readonly static string CH_RT_FILES_PATH = "Texture(RT)/userCharacters/";

    // ..\ 현재 폴더의 상위폴더
    // .\ 현재 폴더.
    /// <summary>
    /// 윈도우 탐색기경로로 표현한. 캐릭터 데이타 파일 상대경로. ( Editor only )
    /// </summary>
    public readonly static string WINDOW_PATH_CHARACTER_DATAS_FILE = ".\\Assets\\MyAssets\\Resources\\TextAsset\\ChDatas\\characterDatas.json";

    /// <summary>
    /// Resource.Load 사용시에 쓰이는 캐릭터 RT 리소스 상대경로.
    /// </summary>
    public readonly static string SELECT_CHARS_RT_RESOURCE_PATH = "Texture(RT)/userCharacters/{0}";

    /// <summary>
    /// 캐릭터 프리팹이 위치한 path.
    /// </summary>
    public readonly static string PREFAB_CHARACTER_RESOURCE_PATH = "GamePrefabs/Characters/user/";

    /// <summary>
    ///  RenderTexture에 사용될 캐릭터 프리팹이 저장될 path. ( Editor only )
    /// </summary>
    public readonly static string SAVE_PATH_FOR_SELECT_CHARS_PREFAB = "Assets/MyAssets/Resources/GamePrefabs/SelectChars/{0}.prefab";
    /// <summary>
    /// 렌더텍스처 생성에 사용되는 SelectCharacters 프리팹 리소스 path. 
    /// </summary>
    public readonly static string SELECT_CHARS_PREFAB_RESOURCE_PATH = "GamePrefabs/SelectChars/SelectCharacters";
    /// <summary>
    /// SelectCharacters 템플릿 프리팹.
    /// </summary>
    public readonly static string SELECT_CHARS_TEMPLATE_PREFAB = "GamePrefabs/SelectChars/TemplatePrefab";

    /// <summary>
    /// 
    /// </summary>
    public readonly static string EDITOR_SCENE_ENV_LIGHT_PREFAB = "Editor/EditorSceneEnvLight";

    /// <summary>
    /// N개의 서브월드 block 데이터가 저장될 path.
    /// </summary>
    public readonly static string RAW_SUB_WORLD_DATA_PATH = ".\\SubWorldDatas\\";

    /// <summary>
    /// 네트워크 게임에서 사용될 플레이어 프리팹 path.
    /// </summary>
    public readonly static string GAME_NET_PLAYER_PREFAB_RESOURCE_PATH = "GamePrefabs/Network/GamePlayer";

    /// <summary>
    /// 캐릭터 얼굴 Texture2D 파일 path.
    /// </summary>
    public readonly static string CHAR_TEXTURE2D_PATH = "Assets/MyAssets/Resources/CharacterTexture2D/";
    /// <summary>
    /// 캐릭터 얼굴 리소스 폴더 파일 path.
    /// </summary>
    public readonly static string CHAR_TEXTURE2D_RESOURCES_PATH = "CharacterTexture2D/";

    /// <summary>
    /// 로그파일이 저장되는 윈도우 탐색기 경로.
    /// </summary>
    public readonly static string LOG_FILE_ROOT_WIN_PATH = ".\\GameLog\\";

    /// <summary>
    /// 지형 청크 프리팹 경로.
    /// </summary>
    public readonly static string TERRAIN_CHUNK_PREFAB_RESOURCE_PATH = "GamePrefabs/TerrainChunk";
    /// <summary>
    /// 워터 청크 프리팹 경로.
    /// </summary>
    public readonly static string WATER_CHUNK_PREFAB_RESOURCE_PATH = "GamePrefabs/WaterChunk";
    /// <summary>
    /// 환경 청크 프리팹 경로 ( 나무, 풀...)
    /// </summary>
    public readonly static string ENVIROMENT_CHUNK_PREFAB_RESOURCE_PATH = "GamePrefabs/EnviromentChunk";

    /// <summary>
    /// 서브월드 프리팹 경로.
    /// </summary>
    public readonly static string SUB_WORLD_PREFAB = "GamePrefabs/SubWorld";
    /// <summary>
    /// WorldArea 프리팹 경로.
    /// </summary>
    public readonly static string WORLD_AREA_PREFAB = "GamePrefabs/WorldArea";

    public readonly static string NPC_PREFABS_ASSET_PATH = "Assets/MyAssets/Resources/GamePrefabs/Actor/NPC";
    public readonly static string NPC_PREFABS_RESOURCE_PATH = "GamePrefabs/Actor/NPC";
    //
    public readonly static string MONSTER_PREFABS_ASSET_PATH = "Assets/MyAssets/Resources/GamePrefabs/Actor/MONSTER";
    public readonly static string MONSTER_PREFABS_RESOURCE_PATH = "GamePrefabs/Actor/MONSTER";
    //
    public readonly static string ANIMAL_PREFABS_ASSET_PATH = "Assets/MyAssets/Resources/GamePrefabs/Actor/ANIMAL";
    public readonly static string ANIMAL_PREFABS_RESOURCE_PATH = "GamePrefabs/Actor/ANIMAL";
    /// <summary>
    /// Actor 공용 프리팹.
    /// </summary>
    public readonly static string ACTOR_COMMONS_NAME_RESOURCE_PATH = "GamePrefabs/Actor/Commons/txt_mesh_Name";

    /// <summary>
    /// 플레이어 메인 카메라 프리팹.
    /// </summary>
    public readonly static string PLAYER_CAMERA_PREFAB = "GamePrefabs/Camera/PlayerCamera";

    public readonly static string IN_PREPARE_STATE_CAMERA_PREFAB = "GamePrefabs/Camera/CameraInPrepareState";

    /// <summary>
    /// 게임에서 사용되는 파티클 프리팹 에셋 경로.
    /// </summary>
    public readonly static string GAME_FX_ASSET_PATH = "Assets/MyAssets/Resources/GamePrefabs/FX";

    /// <summary>
    /// FX 에셋 파일 경로 리스트. ( Editor only )
    /// </summary>
    public readonly static string FX_ASSET_LIST_FILE_PATH = "Assets/MyAssets/Resources/TextAsset/AssetPathList/FX_AssetPathList";
    /// <summary>
    /// FX 에셋 리소스 경로 리스트.
    /// </summary>
    public readonly static string TXT_RESOURCE_FX_ASSET_LIST_PATH = "TextAsset/AssetPathList/FX_AssetPathList";

    /// <summary>
    /// Monster 에셋 파일 경로 리스트. ( Editor only )
    /// </summary>
    public readonly static string MONSTER_ASSET_LIST_FILE_PATH = "Assets/MyAssets/Resources/TextAsset/AssetPathList/Mon_AssetPathList";
    /// <summary>
    /// Monster 에셋 리소스 경로 리스트.
    /// </summary>
    public readonly static string TXT_RESOURCE_MONSTER_ASSET_LIST_PATH = "TextAsset/AssetPathList/Mon_AssetPathList";

    /// <summary>
    /// ANIMAL 에셋 파일 경로 리스트. ( Editor only )
    /// </summary>
    public readonly static string ANIMAL_ASSET_LIST_FILE_PATH = "Assets/MyAssets/Resources/TextAsset/AssetPathList/Animal_AssetPathList";
    /// <summary>
    /// ANIMAL 에셋 리소스 경로 리스트.
    /// </summary>
    public readonly static string TXT_RESOURCE_ANIMAL_ASSET_LIST_PATH = "TextAsset/AssetPathList/Animal_AssetPathList";

    /// <summary>
    /// NPC 에셋 파일 경로 리스트. ( Editor only )
    /// </summary>
    public readonly static string NPC_ASSET_LIST_FILE_PATH = "Assets/MyAssets/Resources/TextAsset/AssetPathList/Npc_AssetPathList";
    /// <summary>
    /// NPC 에셋 리소스 경로 리스트.
    /// </summary>
    public readonly static string TXT_RESOURCE_NPC_ASSET_LIST_PATH = "TextAsset/AssetPathList/Npc_AssetPathList";
}


