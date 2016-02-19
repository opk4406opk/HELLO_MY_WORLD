using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using System.Text;
using Mono.Data.Sqlite;
using System.Data;

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

    private LZFCompress lzfCompress;

    public void Init()
    {
        filePath = Application.persistentDataPath + "/GameSavefile.dat";
        gameWorldList = gameManager.GetComponent<GameManager>().worldList;
        lzfCompress = new LZFCompress();
        CalcWorldDataSize();
    }

    public void Save()
    {
        //init
        mergeIdx = 0;
        mergeIdxOffset = 0;
        mergeWorldData = new byte[mergeWorldSize];

        for (int idx = 0; idx < gameWorldList.Count; ++idx)
            SubWorldToTotalWorld(idx);
        // 마지막 원소에 데이터의 끝을 알리는 구분자를 넣는다.
        mergeWorldData[mergeWorldSize - 1] = DELIMETER_END;

        // 압축.
        int compressedSize = 0;
        byte[] outputData = new byte[mergeWorldSize];
        compressedSize = lzfCompress.Compress(mergeWorldData, mergeWorldSize, outputData, mergeWorldSize);
        byte[] compressedData = new byte[compressedSize];

        for (int idx = 0; idx < compressedSize; ++idx)
            compressedData[idx] = outputData[idx];

        // 파일 생성.
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fileStream = File.Open(filePath, FileMode.OpenOrCreate);
        // 시리얼라이징.
        bf.Serialize(fileStream, compressedData);
        fileStream.Close();
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

    private delegate void del_GetMergeWorldSize();
    public void Load()
    {
        //init
        mergeIdx = 0;
        mergeIdxOffset = 0;

        del_GetMergeWorldSize GetMergeWorldSize = () =>
        {
            string conn = "URI=file:" + Application.dataPath +
              "/MyAssets/Resources/GameUserDB/userDB.db";

            IDbConnection dbconn = (IDbConnection)new SqliteConnection(conn);
            IDbCommand dbcmd = dbconn.CreateCommand();
            dbconn.Open(); //Open connection to the database.

            string sqlQuery = "SELECT size FROM MERGE_WORLD";
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();

            // 첫번 째 레코드의 size값만 가져온다.
            reader.Read();
            mergeWorldSize = reader.GetInt32(0);

            dbcmd.Dispose();
            dbcmd = null;

            dbconn.Close();
            dbconn = null;
        };
        GetMergeWorldSize();

        //파일 생성.
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fileStream = File.Open(filePath, FileMode.Open);
        // DeSerialzing ( decode..)
        byte[] compressedData = (byte[])bf.Deserialize(fileStream);
        int compressedSize = compressedData.Length;
        //압축 해제.
        mergeWorldData = new byte[mergeWorldSize];
        lzfCompress.Decompress(compressedData, compressedSize, mergeWorldData, mergeWorldSize);

        int idx = 0;
        while(mergeWorldData[mergeIdxOffset] != DELIMETER_END)
        {
            TotalWorldToSubWorld(idx);
            idx++;
        }

        foreach (World world in gameWorldList)
        {
            StartCoroutine(world.loadProcessRoutine);
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
    private delegate void del_GetRecordNum();
    private delegate void del_UpdateMergeWorldSize();
    private delegate void del_InSertMergeWorldSize();
    private void CalcWorldDataSize()
    {
        int subWorldSize = GameWorldConfig.worldX * GameWorldConfig.worldY * GameWorldConfig.worldZ;
        mergeWorldSize = (subWorldSize * gameWorldList.Count) + gameWorldList.Count;

        int recordNum = 0;
        del_GetRecordNum GetRecordNum = () =>
        {
            string conn = "URI=file:" + Application.dataPath +
               "/MyAssets/Resources/GameUserDB/userDB.db";

            IDbConnection dbconn = (IDbConnection)new SqliteConnection(conn);
            IDbCommand dbcmd = dbconn.CreateCommand();
            dbconn.Open(); //Open connection to the database.

            string sqlQuery = "SELECT * FROM MERGE_WORLD";
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();
            while (reader.Read()) { recordNum++; }

            dbcmd.Dispose();
            dbcmd = null;

            dbconn.Close();
            dbconn = null;
        };
        GetRecordNum();

        del_UpdateMergeWorldSize UpdateMergeWorldSize = () =>
        {
            string conn = "URI=file:" + Application.dataPath +
               "/MyAssets/Resources/GameUserDB/userDB.db";

            IDbConnection dbconn = (IDbConnection)new SqliteConnection(conn);
            IDbCommand dbcmd = dbconn.CreateCommand();
            dbconn.Open(); //Open connection to the database.

            string sqlQuery = "UPDATE MERGE_WORLD SET size=" + mergeWorldSize;
            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteNonQuery();

            dbcmd.Dispose();
            dbcmd = null;

            dbconn.Close();
            dbconn = null;
        };

        del_InSertMergeWorldSize InSertMergeWorldSize = () =>
        {
            string conn = "URI=file:" + Application.dataPath +
               "/MyAssets/Resources/GameUserDB/userDB.db";

            IDbConnection dbconn = (IDbConnection)new SqliteConnection(conn);
            IDbCommand dbcmd = dbconn.CreateCommand();
            dbconn.Open(); //Open connection to the database.

            string sqlQuery = "INSERT INTO MERGE_WORLD (size) VALUES (" + mergeWorldSize +");";
            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteNonQuery();

            dbcmd.Dispose();
            dbcmd = null;

            dbconn.Close();
            dbconn = null;
        };

        if (recordNum > 0) UpdateMergeWorldSize();
        else InSertMergeWorldSize();
    }

}
