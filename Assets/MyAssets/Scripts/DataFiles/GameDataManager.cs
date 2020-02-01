using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 
 * Unity PC(Editor)상에서는 문제없이 한글로 인코딩 되는 문제가, 모바일 운영체제로 가면 안되는 문제가 발생한다.
이 문제를 해결하는 방법은 인코딩과 관련된 dll을 Plugins폴더로 복사하면 된다.
해당 dll은 유니티 설치 폴더인 Unity\Editor\Data\Mono\lib\mono\unity에 있으며,
파일 이름은 [ I18N.dll ], [ I18N.CJK.dll ]이다.
이 둘을 복사하면 아래와 같이 인코딩 값을 가져올 수 있다.
Encoding enkr = Encoding.GetEncoding(51949);
출처: https://202psj.tistory.com/1297 
 */

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
