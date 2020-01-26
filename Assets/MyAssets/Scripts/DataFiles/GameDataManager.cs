using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataManager
{
    #region data file.
    // data file parser.
    private WorldConfigFile WorldConfigDataFileInstance = new WorldConfigFile();
    private WorldMapDataFile SubWorldDataFileInstance = new WorldMapDataFile();
    private BlockTileDataFile TileDataFileInstance = new BlockTileDataFile();
    private CraftItemListDataFile CraftItemListDataFileInstance = new CraftItemListDataFile();
    private NPCDataFile NpcDataFileInstance = new NPCDataFile();
    private AnimalDataFile AnimalDataFileInstance = new AnimalDataFile();
    private GameConfigDataFile GameConfigDataFileInstance = new GameConfigDataFile();
    // data tables.
    private RawElementTableReader RawElementTableReaderInstance = new RawElementTableReader();
    private BlockProductTableReader BlockProductTableReaderInstance = new BlockProductTableReader();
    private ItemTableReader ItemTableReaderInstance = new ItemTableReader();
    //
    #endregion
    public void Initialize()
    {
        KojeomLogger.DebugLog("게임 데이터 파일 초기화 시작.");
        // init tables.
        RawElementTableReaderInstance.Initialize(ConstFilePath.TXT_RESOURCE_RAW_ELEMENT_TABLE_PATH);
        BlockProductTableReaderInstance.Initialize(ConstFilePath.TXT_RESOURCE_BLOCK_PRODUCT_TABLE_PATH);
        ItemTableReaderInstance.Initialize(ConstFilePath.TXT_RESOURCE_ITEM_TABLE_PATH);
        //GameDataFiles Init
        WorldConfigDataFileInstance.Init();
        GameConfigDataFileInstance.Init();
        TileDataFileInstance.Init();
        SubWorldDataFileInstance.Init();
        CraftItemListDataFileInstance.Init();
        NpcDataFileInstance.Init();
        AnimalDataFileInstance.Init();
        KojeomLogger.DebugLog("게임 데이터 파일 초기화 완료.");
    }
}
