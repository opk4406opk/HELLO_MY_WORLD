using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using System.Data;


/// <summary>
/// 게임내 사용자가 월드 블록을 수정/삭제를 관리하는 클래스.
/// </summary>
/// 
public class ModifyTerrain : MonoBehaviour
{
    [SerializeField]
    private LootingSystem lootingSystem;
    [SerializeField]
    private ItemDataFile itemDataFile;
    [SerializeField]
    private GameManager gameMgr;

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

    public void ReplaceBlockCursor(Ray ray, byte blockType)
    {
        DeleteBlockAt(ray, blockType);
    }

    public void AddBlockCursor(Ray ray, byte blockType)
    {
        AddBlockAt(ray, blockType);
    }

    private void DeleteBlockAt(Ray ray, byte blockType)
    {
        _world = gameMgr.worldList[0];
        RayCastingProcess(ray, blockType, false);
    }
    private void AddBlockAt(Ray ray, byte blockType)
    {
        _world = gameMgr.worldList[0];
        RayCastingProcess(ray, blockType, true);
    }

    private void RayCastingProcess(Ray ray, byte blockType, bool isCreate)
    {
        CollideInfo collideInfo = _world.customOctree.Collide(ray);
        if (collideInfo.isCollide)
        {
            int blockX, blockY, blockZ;
            blockX = (int)(collideInfo.hitBlockCenter.x);
            blockY = (int)(collideInfo.hitBlockCenter.y);
            blockZ = (int)(collideInfo.hitBlockCenter.z);
            //gameMgr.GetYuKoNPC().ActivePathFindNPC(blockX, blockZ);
            if (isCreate)
            {
                _world.customOctree.Add(collideInfo.hitBlockCenter + new Vector3(0, 1.0f, 0));
                SetBlockForAdd(blockX, blockY + 1, blockZ, blockType);
            }
            else
            {
                _world.customOctree.Delete(collideInfo.hitBlockCenter);
                SetBlockForDelete(blockX, blockY, blockZ, blockType);
                gameMgr.worldList[0].worldBlockData[blockX, blockY, blockZ].aabb.isEnable = false;
            }
        }
    }

    //void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    int lenX = gameMgr.worldList[0].worldBlockData.GetLength(0);
    //    int lenY = gameMgr.worldList[0].worldBlockData.GetLength(1);
    //    int lenZ = gameMgr.worldList[0].worldBlockData.GetLength(2);
    //    for (int ix = 0; ix < lenX; ix++)
    //        for (int iy = 0; iy < lenY; iy++)
    //            for (int iz = 0; iz < lenZ; iz++)
    //            {
    //                Vector3 min = gameMgr.worldList[0].worldBlockData[ix, iy, iz].aabb.minExtent;
    //                Vector3 max = gameMgr.worldList[0].worldBlockData[ix, iy, iz].aabb.maxExtent;
    //                Gizmos.DrawWireCube(gameMgr.worldList[0].worldBlockData[ix, iy, iz].center, max - min);
    //                Gizmos.DrawLine(min, max);
    //            }
    //}
    private void SetBlockForAdd(int x, int y, int z, byte block)
    {
      
        if((x < GameWorldConfig.worldX) &&
           (y < GameWorldConfig.worldY) &&
           (z < GameWorldConfig.worldZ) &&
           (x >= 0) && (y >= 0) && (z >= 0)) 
        {
            _world.worldBlockData[x, y, z].type = block;
            _world.worldBlockData[x, y, z].isRendered = true;
            _world.worldBlockData[x, y, z].aabb.isEnable = true;
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
            _world.worldBlockData[x, y, z].isRendered = false;
            _world.worldBlockData[x, y, z].aabb.isEnable = false;
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
