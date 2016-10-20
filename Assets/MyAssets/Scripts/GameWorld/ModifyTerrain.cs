using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;
using System.Data;
/// <summary>
/// 게임내 사용자가 월드 블록을 수정/삭제를 관리하는 클래스.
/// </summary>
public class ModifyTerrain : MonoBehaviour
{
    [SerializeField]
    private LootingSystem lootingSystem;
    [SerializeField]
    private ItemDataFile itemDataFile;

    private World _world;
    public World world
    {
        set { _world = value; }
    }
    
    private int chunkSize = 0;

    void Start()
    {
        chunkSize = GameWorldConfig.chunkSize;
    }

    public void ReplaceBlockCursor(RaycastHit hit, byte block)
    {
        DeleteBlockAt(hit, block);
    }

    public void AddBlockCursor(RaycastHit hit, byte block)
    {
        AddBlockAt(hit, block);
    }

    private void DeleteBlockAt(RaycastHit hit, byte block)
    {
        //removes a block at these impact coordinates, you can raycast against the terrain and call this with the hit.point
        Vector3 position = hit.point;
        position += (hit.normal * -0.5f);
        // 기본 청크 위치 : x' = x * chunkSize;
        // 오프셋 적용한 청크 위치 : x'' = (x + offset) * chunkSize;
        // 따라서, 기본 Chunk 위치와 오프셋이 적용된 Chunk의 차이는
        //  x'' = x' - (offset * chunkSize)
        int x, y, z;
        x = Mathf.RoundToInt(position.x - (_world.worldOffsetX * chunkSize));
        y = Mathf.RoundToInt(position.y);
        z = Mathf.RoundToInt(position.z - (_world.worldOffsetZ * chunkSize));

        // test code.
        if ((_world.worldBlockData[x, y, z].aabb.IsInterSectPoint(new Vector3(x, y, z))) ||
            (_world.worldBlockData[x, y - 1, z].aabb.IsInterSectPoint(new Vector3(x, y, z))))
            SetBlockForDelete(x, y, z, block);
    }

    private void AddBlockAt(RaycastHit hit, byte block)
    {
        //adds the specified block at these impact coordinates, you can raycast against the terrain and call this with the hit.point
        Vector3 position = hit.point;
        position += (hit.normal * 0.5f);
        // 기본 청크 위치 : x' = x * chunkSize;
        // 오프셋 적용한 청크 위치 : x'' = (x + offset) * chunkSize;
        // 따라서, 기본 Chunk 위치와 오프셋이 적용된 Chunk의 차이는
        //  x'' = x' - (offset * chunkSize)
        int x, y, z;
        x = Mathf.RoundToInt(position.x - (_world.worldOffsetX * chunkSize));
        y = Mathf.RoundToInt(position.y);
        z = Mathf.RoundToInt(position.z - (_world.worldOffsetZ * chunkSize));
        // test code.
        if((_world.worldBlockData[x, y, z].aabb.IsInterSectPoint(new Vector3(x, y, z))) ||
            (_world.worldBlockData[x, y-1, z].aabb.IsInterSectPoint(new Vector3(x, y, z))))
            SetBlockForAdd(x, y, z, block);
    }
    

    private void SetBlockForAdd(int x, int y, int z, byte block)
    {
      
        if((x < GameWorldConfig.worldX) &&
           (y < GameWorldConfig.worldY) &&
           (z < GameWorldConfig.worldZ) &&
           (x >= 0) && (y >= 0) && (z >= 0)) 
        {
            _world.worldBlockData[x, y, z].type = block;
            UpdateChunkAt(x, y, z, block);
        }
        else
        {
            GameMessage.SetGameMsgType = GameMessage.MESSAGE_TYPE.CANT_CREATE_BLOCK;
            GameMessage.SetMessage("허용 범위를 벗어나 블록 생성이 불가능합니다. (구조적 문제로 인해 수정중입니다. 기다려주세요.)");
            UIPopupManager.OpenGameMessage();
        }
    }
    private delegate void del_UpdateUserItem(byte blockType);
    private void SetBlockForDelete(int x, int y, int z, byte block)
    {
        
        del_UpdateUserItem UpdateUserItem = (byte blockType) =>
        {
            string conn = "URI=file:" + Application.dataPath +
              "/StreamingAssets/GameUserDB/userDB.db";

            IDbConnection dbconn = (IDbConnection)new SqliteConnection(conn);
            IDbCommand dbcmd = dbconn.CreateCommand();

            string itemName;
            itemName = lootingSystem.GetTypeToItemName(blockType.ToString());
            string type;
            ItemInfo itemInfo = itemDataFile.GetItemData(itemName);
            type = itemInfo.type;
            try
            {
                dbconn.Open(); //Open connection to the database.
                string sqlQuery = "INSERT INTO USER_ITEM (name, type, amount) VALUES("
                                   + "'"+ itemName +"'" + "," + "'" +type + "'" + "," + "1)";
                dbcmd.CommandText = sqlQuery;
                dbcmd.ExecuteNonQuery();

                dbcmd.Dispose();
                dbcmd = null;

                dbconn.Close();
                dbconn = null;
            }
            catch // 인벤토리에 중복된 아이템이 있다면, 수량증가를 해야한다.
            {
                string sqlQuery = "SELECT amount FROM USER_ITEM WHERE name = "
                                  + "'" + itemName +"'";
                dbcmd.CommandText = sqlQuery;
                IDataReader reader = dbcmd.ExecuteReader();
                reader.Read();
                int itemAmount = reader.GetInt32(0);
                itemAmount++;
                reader.Close();

                sqlQuery = "UPDATE USER_ITEM SET amount = " + "'" + itemAmount + "'" +
                            " WHERE name = " + "'" + itemName + "'" ;
                dbcmd.CommandText = sqlQuery;
                dbcmd.ExecuteNonQuery();

                dbcmd.Dispose();
                dbcmd = null;

                dbconn.Close();
                dbconn = null;
            }
        };

        if ((x < GameWorldConfig.worldX) &&
           (y < GameWorldConfig.worldY) &&
           (z < GameWorldConfig.worldZ) &&
           (x >= 0) && (y >= 0) && (z >= 0))
        {
            UpdateUserItem(_world.worldBlockData[x, y, z].type);
            _world.worldBlockData[x, y, z].type = block;
            UpdateChunkAt(x, y, z, block);
        }
        else
        {
            GameMessage.SetGameMsgType = GameMessage.MESSAGE_TYPE.CANT_CREATE_BLOCK;
            GameMessage.SetMessage("허용 범위를 벗어나 블록 생성이 불가능합니다. (구조적 문제로 인해 수정중입니다. 기다려주세요.)");
            UIPopupManager.OpenGameMessage();
        }
        
    }

    private void UpdateChunkAt(int x, int y, int z, byte block)
    {
        // world data 인덱스를 chunkGroup 인덱스로 변환한다. 
        int updateX, updateY, updateZ;
        updateX = Mathf.FloorToInt(x / chunkSize);
        updateY = Mathf.FloorToInt(y / chunkSize);
        updateZ = Mathf.FloorToInt(z / chunkSize);
       
        //print("Updating: " + updateX + ", " + updateY + ", " + updateZ);

        _world.chunkGroup[updateX, updateY, updateZ].update = true;

        if (x - (chunkSize * updateX) == 0 && updateX != 0)
        {
            _world.chunkGroup[updateX - 1, updateY, updateZ].update = true;
        }

        if (x - (chunkSize * updateX) == GameWorldConfig.chunkSize && updateX != _world.chunkGroup.GetLength(0) - 1)
        {
            _world.chunkGroup[updateX + 1, updateY, updateZ].update = true;
        }

        if (y - (chunkSize * updateY) == 0 && updateY != 0)
        {
            _world.chunkGroup[updateX, updateY - 1, updateZ].update = true;
        }

        if (y - (chunkSize * updateY) == GameWorldConfig.chunkSize && updateY != _world.chunkGroup.GetLength(1) - 1)
        {
            _world.chunkGroup[updateX, updateY + 1, updateZ].update = true;
        }

        if (z - (chunkSize * updateZ) == 0 && updateZ != 0)
        {
            _world.chunkGroup[updateX, updateY, updateZ - 1].update = true;
        }

        if (z - (chunkSize * updateZ) == GameWorldConfig.chunkSize && updateZ != _world.chunkGroup.GetLength(2) - 1)
        {
            _world.chunkGroup[updateX, updateY, updateZ + 1].update = true;
        }

    }

}
