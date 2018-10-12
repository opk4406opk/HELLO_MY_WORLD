using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.Text;

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
    public void Init()
    {
        var gameConfig = GameConfigDataFile.singleton.GetGameConfigData();
        chunkSize = gameConfig.chunk_size;
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
        foreach (var element in WorldManager.instance.wholeWorldStates)
        {
            if (CustomAABB.IsInterSectPoint(element.Value.subWorldInstance.customOctree.rootMinBound,
                element.Value.subWorldInstance.customOctree.rootMaxBound, clickWorldPos))
            {
                world = element.Value.subWorldInstance;
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
        var gameConfig = GameConfigDataFile.singleton.GetGameConfigData();
        if((x < gameConfig.sub_world_x_size) &&
           (y < gameConfig.sub_world_y_size) &&
           (z < gameConfig.sub_world_z_size) &&
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
            UIPopupSupervisor.OpenPopupUI(POPUP_TYPE.gameMessage);
        }
    }
    private void SetBlockForDelete(int x, int y, int z, byte block)
    {
        Action<byte> UpdateUserItem = (byte blockType) =>
        {
            StringBuilder conn = new StringBuilder();
            conn.AppendFormat(GameDBHelper.GetInstance().GetDBConnectionPath(), Application.dataPath);

            IDbConnection dbconn;
            IDbCommand dbcmd;
            using (dbconn = (IDbConnection)new SqliteConnection(conn.ToString()))
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
                        StringBuilder sqlQuery = new StringBuilder();
                        sqlQuery.AppendFormat("INSERT INTO USER_ITEM (name, type, amount, id) VALUES('{0}', '{1}', 1, {2} )",
                            itemInfo.name, type, itemID);
                        dbcmd.CommandText = sqlQuery.ToString();
                        dbcmd.ExecuteNonQuery();

                        dbconn.Close();
                    }
                    catch (SqliteException e) // 인벤토리에 중복된 아이템이 있다면, 수량증가를 해야한다.
                    {
                        if (SQLiteErrorCode.Constraint == e.ErrorCode)
                        {
                            StringBuilder sqlQuery = new StringBuilder();
                            sqlQuery.AppendFormat("SELECT amount FROM USER_ITEM WHERE id = '{0}'", itemID);
                            dbcmd.CommandText = sqlQuery.ToString();
                            IDataReader reader = dbcmd.ExecuteReader();
                            reader.Read();
                            int itemAmount = reader.GetInt32(0);
                            itemAmount++;
                            reader.Close();

                            sqlQuery.Remove(0, sqlQuery.Length);
                            sqlQuery.AppendFormat("UPDATE USER_ITEM SET amount = '{0}' WHERE id = '{1}'",
                                itemAmount, itemID);
                            dbcmd.CommandText = sqlQuery.ToString();
                            dbcmd.ExecuteNonQuery();

                            dbconn.Close();
                        }
                    }
                }
                dbconn.Close();
            }
        };

        var gameConfig = GameConfigDataFile.singleton.GetGameConfigData();
        if ((x < gameConfig.sub_world_x_size) &&
           (y < gameConfig.sub_world_y_size) &&
           (z < gameConfig.sub_world_z_size) &&
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
            UIPopupSupervisor.OpenPopupUI(POPUP_TYPE.gameMessage);
        }
        
    }

    private void UpdateChunkAt(int x, int y, int z, byte block)
    {
        var gameConfig = GameConfigDataFile.singleton.GetGameConfigData();
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

        if (x - (chunkSize * updateX) == gameConfig.chunk_size && updateX != world.chunkGroup.GetLength(0) - 1)
        {
            world.chunkGroup[updateX + 1, updateY, updateZ].update = true;
        }

        if (y - (chunkSize * updateY) == 0 && updateY != 0)
        {
            world.chunkGroup[updateX, updateY - 1, updateZ].update = true;
        }

        if (y - (chunkSize * updateY) == gameConfig.chunk_size && updateY != world.chunkGroup.GetLength(1) - 1)
        {
            world.chunkGroup[updateX, updateY + 1, updateZ].update = true;
        }

        if (z - (chunkSize * updateZ) == 0 && updateZ != 0)
        {
            world.chunkGroup[updateX, updateY, updateZ - 1].update = true;
        }

        if (z - (chunkSize * updateZ) == gameConfig.chunk_size && updateZ != world.chunkGroup.GetLength(2) - 1)
        {
            world.chunkGroup[updateX, updateY, updateZ + 1].update = true;
        }

    }

}
