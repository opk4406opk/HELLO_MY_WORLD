using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using System.Data;
using System;


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

    private World world;
    private int chunkSize = 0;
    void Start()
    {
        chunkSize = GameConfig.chunkSize;
    }

    public void ReplaceBlockCursor(Ray ray, Vector3 clickWorldPos, byte blockType)
    {
        DeleteBlockAt(ray, clickWorldPos, blockType);
    }

    public void AddBlockCursor(Ray ray, Vector3 clickWorldPos, byte blockType)
    {
        AddBlockAt(ray, clickWorldPos, blockType);
    }

    private void DeleteBlockAt(Ray ray, Vector3 clickWorldPos, byte blockType)
    {
        SelectWorld(clickWorldPos);
        RayCastingProcess(ray, blockType, false);
    }
    private void AddBlockAt(Ray ray, Vector3 clickWorldPos, byte blockType)
    {
        SelectWorld(clickWorldPos);
        RayCastingProcess(ray, blockType, true);
    }
    private void SelectWorld(Vector3 clickWorldPos)
    {
        foreach (World w in gameMgr.worldList)
        {
            if (CustomAABB.IsInterSectPoint(w.customOctree.rootMinBound,
                w.customOctree.rootMaxBound, clickWorldPos))
            {
                world = w;
                break;
            }
        }
    }

    private void RayCastingProcess(Ray ray, byte blockType, bool isCreate)
    {
        CollideInfo collideInfo = world.customOctree.Collide(ray);
        if (collideInfo.isCollide)
        {
            int blockX, blockY, blockZ;
            blockX = (int)(collideInfo.hitBlockCenter.x);
            blockY = (int)(collideInfo.hitBlockCenter.y);
            blockZ = (int)(collideInfo.hitBlockCenter.z);
            blockX -= world.worldOffsetX;
            blockZ -= world.worldOffsetZ;
            //-------------------------------------------------------------------------------
            if (isCreate)
            {
                world.customOctree.Add(collideInfo.hitBlockCenter + new Vector3(0, 1.0f, 0));
                SetBlockForAdd(blockX, blockY + 1, blockZ, blockType);
                world.worldBlockData[blockX, blockY + 1, blockZ].isRendered = true;
            }
            else
            {
                world.customOctree.Delete(collideInfo.hitBlockCenter);
                SetBlockForDelete(blockX, blockY, blockZ, blockType);
                world.worldBlockData[blockX, blockY, blockZ].isRendered = false;
            }
        }
    }

    private void SetBlockForAdd(int x, int y, int z, byte block)
    {
      
        if((x < GameConfig.worldX) &&
           (y < GameConfig.worldY) &&
           (z < GameConfig.worldZ) &&
           (x >= 0) && (y >= 0) && (z >= 0)) 
        {
            world.worldBlockData[x, y, z].type = block;
            world.worldBlockData[x, y, z].isRendered = true;
            UpdateChunkAt(x, y, z, block);
        }
        else
        {
            GameMessage.SetGameMsgType = GameMessage.MESSAGE_TYPE.CANT_CREATE_BLOCK;
            GameMessage.SetMessage("허용 범위를 벗어나 블록 생성이 불가능합니다. (구조적 문제로 인해 수정중입니다. 기다려주세요.)");
            UIPopupManager.OpenGameMessage();
        }
    }
    private void SetBlockForDelete(int x, int y, int z, byte block)
    {
        Action<byte> UpdateUserItem = (byte blockType) =>
        {
            string conn = "URI=file:" + Application.dataPath +
              "/StreamingAssets/GameUserDB/userDB.db";

            IDbConnection dbconn;
            IDbCommand dbcmd;
            using (dbconn = (IDbConnection)new SqliteConnection(conn))
            {
                using (dbcmd = dbconn.CreateCommand())
                {
                    string itemID;
                    itemID = lootingSystem.GetTypeToItemID(blockType.ToString());
                    string type;
                    ItemInfo itemInfo = itemDataFile.GetItemData(itemID);
                    type = itemInfo.type;
                    try
                    {
                        dbconn.Open(); //Open connection to the database.
                        string sqlQuery = "INSERT INTO USER_ITEM (name, type, amount, id) VALUES("
                                           + "'" + itemInfo.name + "'" + "," + "'" + 
                                           type + "'" + "," + "1," + itemID +")";
                        dbcmd.CommandText = sqlQuery;
                        dbcmd.ExecuteNonQuery();

                        dbconn.Close();
                    }
                    catch (SqliteException e) // 인벤토리에 중복된 아이템이 있다면, 수량증가를 해야한다.
                    {
                        if (SQLiteErrorCode.Constraint == e.ErrorCode)
                        {
                            string sqlQuery = "SELECT amount FROM USER_ITEM WHERE id = "
                                        + "'" + itemID + "'";
                            dbcmd.CommandText = sqlQuery;
                            IDataReader reader = dbcmd.ExecuteReader();
                            reader.Read();
                            int itemAmount = reader.GetInt32(0);
                            itemAmount++;
                            reader.Close();

                            sqlQuery = "UPDATE USER_ITEM SET amount = " + "'" + itemAmount + "'" +
                                        " WHERE id = " + "'" + itemID + "'";
                            dbcmd.CommandText = sqlQuery;
                            dbcmd.ExecuteNonQuery();

                            dbconn.Close();
                        }
                    }
                }
                dbconn.Close();
            }
        };

        if ((x < GameConfig.worldX) &&
           (y < GameConfig.worldY) &&
           (z < GameConfig.worldZ) &&
           (x >= 0) && (y >= 0) && (z >= 0))
        {
            UpdateUserItem(world.worldBlockData[x, y, z].type);
            world.worldBlockData[x, y, z].type = block;
            world.worldBlockData[x, y, z].isRendered = false;
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

        world.chunkGroup[updateX, updateY, updateZ].update = true;

        if (x - (chunkSize * updateX) == 0 && updateX != 0)
        {
            world.chunkGroup[updateX - 1, updateY, updateZ].update = true;
        }

        if (x - (chunkSize * updateX) == GameConfig.chunkSize && updateX != world.chunkGroup.GetLength(0) - 1)
        {
            world.chunkGroup[updateX + 1, updateY, updateZ].update = true;
        }

        if (y - (chunkSize * updateY) == 0 && updateY != 0)
        {
            world.chunkGroup[updateX, updateY - 1, updateZ].update = true;
        }

        if (y - (chunkSize * updateY) == GameConfig.chunkSize && updateY != world.chunkGroup.GetLength(1) - 1)
        {
            world.chunkGroup[updateX, updateY + 1, updateZ].update = true;
        }

        if (z - (chunkSize * updateZ) == 0 && updateZ != 0)
        {
            world.chunkGroup[updateX, updateY, updateZ - 1].update = true;
        }

        if (z - (chunkSize * updateZ) == GameConfig.chunkSize && updateZ != world.chunkGroup.GetLength(2) - 1)
        {
            world.chunkGroup[updateX, updateY, updateZ + 1].update = true;
        }

    }

}
