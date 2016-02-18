using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using System.Text;

public class SaveAndLoadManager : MonoBehaviour {

    private List<World> gameWorldList;
    private byte[] mergeWorldData;
    private int mergeWorldSize = 0;
    private int mergeIdx = 0;
    private int mergeIdxOffset = 0;

    private readonly byte DELIMETER_END = 200;
    private readonly byte DELIMETER_AND = 199;

    private string filePath;

    [SerializeField]
    private GameManager gameManager;

    public void Init()
    {
        filePath = Application.persistentDataPath + "/GameSavefile.dat";
        gameWorldList = gameManager.GetComponent<GameManager>().worldList;
    }

    public void Save()
    {
        SaveProcessInit();
        for (int idx = 0; idx < gameWorldList.Count; ++idx)
            SubWorldToTotalWorld(idx);
        // 마지막 원소에 데이터의 끝을 알리는 구분자를 넣는다.
        mergeWorldData[mergeWorldSize - 1] = DELIMETER_END;

        // 파일 생성.
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fileStream = File.Open(filePath, FileMode.OpenOrCreate);
        // 시리얼라이징.
        bf.Serialize(fileStream, mergeWorldData);
        fileStream.Close();
    }
    private void SaveProcessInit()
    {
        //init
        mergeIdx = 0;
        mergeIdxOffset = 0;

        CalcWorldDataSize();
        mergeWorldData = new byte[mergeWorldSize];
    }

    private void SubWorldToTotalWorld(int subWorldIdx)
    {
        for (int x = 0; x < GameWorldConfig.worldX; ++x)
            for (int y = 0; y < GameWorldConfig.worldY; ++y)
                for (int z = 0; z < GameWorldConfig.worldZ; ++z)
                {
                    mergeIdx = (x * GameWorldConfig.worldY * GameWorldConfig.worldZ) + (y * GameWorldConfig.worldZ) + z;
                    mergeWorldData[mergeIdx + mergeIdxOffset] = gameWorldList[subWorldIdx].worldBlockData[x, y, z];
                }
        
        // 데이터 입력이 끝나면, 구분자를 삽입한다.
        if(subWorldIdx != (gameWorldList.Count-1)) 
            mergeWorldData[mergeIdx + mergeIdxOffset + 1] = DELIMETER_AND;
        // Delimeter 포함하여 +2를 해줘야한다.
        mergeIdxOffset += (mergeIdx+2);
    }

    public void Load()
    {
        //init
        mergeIdx = 0;
        mergeIdxOffset = 0;

        //파일 생성.
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fileStream = File.Open(filePath, FileMode.Open);
        // DeSerialzing ( decode..)
        mergeWorldData = (byte[])bf.Deserialize(fileStream);

        int idx = 0;
        while(mergeWorldData[mergeIdxOffset] != DELIMETER_END)
        {
            TotalWorldToSubWorld(idx);
            idx++;
        }
    }

    private void TotalWorldToSubWorld(int subWorldIdx)
    {
        for (int x = 0; x < GameWorldConfig.worldX; ++x)
            for (int y = 0; y < GameWorldConfig.worldY; ++y)
                for (int z = 0; z < GameWorldConfig.worldZ; ++z)
                {
                    mergeIdx = (x * GameWorldConfig.worldY * GameWorldConfig.worldZ) + (y * GameWorldConfig.worldZ) + z;
                    gameWorldList[subWorldIdx].worldBlockData[x, y, z] = mergeWorldData[mergeIdx + mergeIdxOffset];
                }

        if((subWorldIdx != (gameWorldList.Count-1)) &&
           (mergeWorldData[mergeIdx + mergeIdxOffset + 1] == DELIMETER_AND))
        {
            mergeIdx += 2;
            mergeIdxOffset += mergeIdx;
        }
        else
        {
            mergeIdx += 1;
            mergeIdxOffset += mergeIdx;
        } 
    }

    /// <summary>
    /// 전체 게임 월드 크기를 구합니다.
    /// - (서브월드크기 * 서브월드개수) + 서브월드 개수만큼의 구분자
    /// </summary>
    private void CalcWorldDataSize()
    {
        int subWorldSize = GameWorldConfig.worldX * GameWorldConfig.worldY * GameWorldConfig.worldZ;
        mergeWorldSize = (subWorldSize * gameWorldList.Count) + gameWorldList.Count;
    }

}
