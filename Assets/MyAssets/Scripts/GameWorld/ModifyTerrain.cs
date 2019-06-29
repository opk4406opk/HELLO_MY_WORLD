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

    private World world;
    private int chunkSize = 0;
    public void Init()
    {
        var gameWorldConfig = WorldConfigFile.Instance.GetConfig();
        chunkSize = gameWorldConfig.chunk_size;
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
        foreach (var element in WorldManager.Instance.WholeWorldStates)
        {
            if (CustomAABB.IsInterSectPoint(element.Value.subWorldInstance.CustomOctreeInstance.RootMinBound,
                element.Value.subWorldInstance.CustomOctreeInstance.RootMaxBound, clickWorldPos))
            {
                world = element.Value.subWorldInstance;
                break;
            }
        }
    }

    private void RayCastingProcess(Ray ray, byte blockType, bool isCreate)
    {
        CollideInfo collideInfo = world.CustomOctreeInstance.Collide(ray);
        if (collideInfo.IsCollide)
        {
            int blockX, blockY, blockZ;
            blockX = (int)(collideInfo.HitBlockCenter.x);
            blockY = (int)(collideInfo.HitBlockCenter.y);
            blockZ = (int)(collideInfo.HitBlockCenter.z);
            var gameConfig = WorldConfigFile.Instance.GetConfig();
            blockX -= (int)world.WorldCoordinate.x * gameConfig.sub_world_x_size;
            blockY -= (int)world.WorldCoordinate.y * gameConfig.sub_world_y_size;
            blockZ -= (int)world.WorldCoordinate.z * gameConfig.sub_world_z_size;
            //-------------------------------------------------------------------------------
            if (isCreate)
            {
                //Vector3 createPosition = new Vector3(Mathf.Ceil(collideInfo.collisionPoint.x),
                //    Mathf.Ceil(collideInfo.collisionPoint.y),
                //    Mathf.Ceil(collideInfo.collisionPoint.z));
                // 임시코드
                world.CustomOctreeInstance.Add(collideInfo.HitBlockCenter + new Vector3(0, 1.0f, 0));
                SetBlockForAdd(blockX, blockY + 1, blockZ, blockType);
                world.WorldBlockData[blockX, blockY + 1, blockZ].bRendered = true;
            }
            else
            {
                world.CustomOctreeInstance.Delete(collideInfo.HitBlockCenter);
                SetBlockForDelete(blockX, blockY, blockZ, blockType);
                world.WorldBlockData[blockX, blockY, blockZ].bRendered = false;
            }
        }
    }

    private void SetBlockForAdd(int x, int y, int z, byte block)
    {
        var gameWorldConfig = WorldConfigFile.Instance.GetConfig();
        if ((x < gameWorldConfig.sub_world_x_size) &&
           (y < gameWorldConfig.sub_world_y_size) &&
           (z < gameWorldConfig.sub_world_z_size) &&
           (x >= 0) && (y >= 0) && (z >= 0)) 
        {
            world.WorldBlockData[x, y, z].Type = block;
            world.WorldBlockData[x, y, z].bRendered = true;
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
                    ItemInfo itemInfo = ItemDataFile.instance.GetItemData(itemID);
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

        var gameWorldConfig = WorldConfigFile.Instance.GetConfig();
        if ((x < gameWorldConfig.sub_world_x_size) &&
           (y < gameWorldConfig.sub_world_y_size) &&
           (z < gameWorldConfig.sub_world_z_size) &&
           (x >= 0) && (y >= 0) && (z >= 0))
        {
            UpdateUserItem(world.WorldBlockData[x, y, z].Type);
            world.WorldBlockData[x, y, z].Type = block;
            world.WorldBlockData[x, y, z].bRendered = false;
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
        var gameWorldConfig = WorldConfigFile.Instance.GetConfig();
        // world data 인덱스를 chunkGroup 인덱스로 변환한다. 
        int updateX, updateY, updateZ;
        updateX = Mathf.FloorToInt(x / chunkSize);
        updateY = Mathf.FloorToInt(y / chunkSize);
        updateZ = Mathf.FloorToInt(z / chunkSize);
        if (world.ChunkSlots[updateX, updateY, updateZ].Chunks[(int)ChunkType.COMMON] == null)
        {
            return;
        }
        world.ChunkSlots[updateX, updateY, updateZ].Chunks[(int)ChunkType.COMMON].Update = true;

        if (x - (chunkSize * updateX) == 0 && updateX != 0)
        {
            world.ChunkSlots[updateX - 1, updateY, updateZ].Chunks[(int)ChunkType.COMMON].Update = true;
        }

        if (x - (chunkSize * updateX) == gameWorldConfig.chunk_size && updateX != world.ChunkSlots.GetLength(0) - 1)
        {
            world.ChunkSlots[updateX + 1, updateY, updateZ].Chunks[(int)ChunkType.COMMON].Update = true;
        }

        if (y - (chunkSize * updateY) == 0 && updateY != 0)
        {
            world.ChunkSlots[updateX, updateY - 1, updateZ].Chunks[(int)ChunkType.COMMON].Update = true;
        }

        if (y - (chunkSize * updateY) == gameWorldConfig.chunk_size && updateY != world.ChunkSlots.GetLength(1) - 1)
        {
            world.ChunkSlots[updateX, updateY + 1, updateZ].Chunks[(int)ChunkType.COMMON].Update = true;
        }

        if (z - (chunkSize * updateZ) == 0 && updateZ != 0)
        {
            world.ChunkSlots[updateX, updateY, updateZ - 1].Chunks[(int)ChunkType.COMMON].Update = true;
        }

        if (z - (chunkSize * updateZ) == gameWorldConfig.chunk_size && updateZ != world.ChunkSlots.GetLength(2) - 1)
        {
            world.ChunkSlots[updateX, updateY, updateZ + 1].Chunks[(int)ChunkType.COMMON].Update = true;
        }
    }
}
