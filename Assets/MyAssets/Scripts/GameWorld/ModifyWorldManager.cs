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
public class ModifyWorldManager : MonoBehaviour
{
    private SubWorld SelectWorldInstance;
    private int chunkSize = 0;
    public void Init()
    {
        var gameWorldConfig = WorldConfigFile.Instance.GetConfig();
        chunkSize = gameWorldConfig.ChunkSize;
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
        foreach (var element in WorldAreaManager.Instance.ContainedWorldArea(clickWorldPos).SubWorldStates)
        {
            if (CustomAABB.IsInterSectPoint(element.Value.SubWorldInstance.CustomOctreeInstance.RootMinBound,
                element.Value.SubWorldInstance.CustomOctreeInstance.RootMaxBound, clickWorldPos))
            {
                SelectWorldInstance = element.Value.SubWorldInstance;
                break;
            }
        }
    }

    private void RayCastingProcess(Ray ray, byte blockType, bool bCreate)
    {
        CollideInfo collideInfo = SelectWorldInstance.CustomOctreeInstance.Collide(ray);
        if (collideInfo.IsCollide)
        {
            int blockX = (int)(collideInfo.HitBlockCenter.x);
            int blockY = (int)(collideInfo.HitBlockCenter.y);
            int blockZ = (int)(collideInfo.HitBlockCenter.z);

            var gameConfig = WorldConfigFile.Instance.GetConfig();
            blockX -= (int)SelectWorldInstance.OffsetCoordinate.x * gameConfig.SubWorldSizeX * (int)SelectWorldInstance.GetWorldAreaOffset().x;
            blockY -= (int)SelectWorldInstance.OffsetCoordinate.y * gameConfig.SubWorldSizeY * (int)SelectWorldInstance.GetWorldAreaOffset().y;
            blockZ -= (int)SelectWorldInstance.OffsetCoordinate.z * gameConfig.SubWorldSizeZ * (int)SelectWorldInstance.GetWorldAreaOffset().z;
            KojeomLogger.DebugLog(string.Format("RayCasting blockX {0} blockY {1} blockZ {2}", blockX, blockY, blockZ));
            //-------------------------------------------------------------------------------
            if (bCreate == true)
            {
                //Vector3 createPosition = new Vector3(Mathf.Ceil(collideInfo.collisionPoint.x),
                //    Mathf.Ceil(collideInfo.collisionPoint.y),
                //    Mathf.Ceil(collideInfo.collisionPoint.z));
                // 임시코드
                SelectWorldInstance.CustomOctreeInstance.Add(collideInfo.HitBlockCenter + new Vector3(0, 1.0f, 0));
                SetBlockForAdd(blockX, blockY + 1, blockZ, blockType);
                SelectWorldInstance.WorldBlockData[blockX, blockY + 1, blockZ].bRendered = true;
            }
            else
            {
                SelectWorldInstance.CustomOctreeInstance.Delete(collideInfo.HitBlockCenter);
                SetBlockForDelete(blockX, blockY, blockZ, blockType);
                SelectWorldInstance.WorldBlockData[blockX, blockY, blockZ].bRendered = false;
            }
        }
    }

    private void SetBlockForAdd(int x, int y, int z, byte block)
    {
        var gameWorldConfig = WorldConfigFile.Instance.GetConfig();
        if ((x < gameWorldConfig.SubWorldSizeX) &&
           (y < gameWorldConfig.SubWorldSizeY) &&
           (z < gameWorldConfig.SubWorldSizeZ) &&
           (x >= 0) && (y >= 0) && (z >= 0)) 
        {
            SelectWorldInstance.WorldBlockData[x, y, z].Type = block;
            SelectWorldInstance.WorldBlockData[x, y, z].bRendered = true;
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
            conn.AppendFormat(GameDBManager.GetInstance().GetDBConnectionPath(), Application.dataPath);

            IDbConnection dbconn;
            IDbCommand dbcmd;
            using (dbconn = (IDbConnection)new SqliteConnection(conn.ToString()))
            {
                using (dbcmd = dbconn.CreateCommand())
                {
                    string itemID = blockType.ToString();
                    ItemInfo itemInfo = ItemTableReader.GetInstance().GetItemInfo(itemID);
                    string type = itemInfo.Type.ToString();
                    try
                    {
                        dbconn.Open(); //Open connection to the database.
                        StringBuilder sqlQuery = new StringBuilder();
                        sqlQuery.AppendFormat("INSERT INTO USER_ITEM (name, type, amount, id) VALUES('{0}', '{1}', 1, {2} )",
                            itemInfo.Name, type, itemID);
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
        if ((x < gameWorldConfig.SubWorldSizeX) &&
           (y < gameWorldConfig.SubWorldSizeY) &&
           (z < gameWorldConfig.SubWorldSizeZ) &&
           (x >= 0) && (y >= 0) && (z >= 0))
        {
            // 블록을 삭제할때마다, DB를 접속해서 쿼리 날리고 갱신시키는건 너무 무거운 작업.
            // 비동기로 처리하는게 좋을 듯 싶다.
            //UpdateUserItem(world.WorldBlockData[x, y, z].Type);
            SelectWorldInstance.WorldBlockData[x, y, z].Type = block;
            SelectWorldInstance.WorldBlockData[x, y, z].bRendered = false;
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
        if (SelectWorldInstance.ChunkSlots[updateX, updateY, updateZ].Chunks[(int)ChunkType.TERRAIN] == null)
        {
            return;
        }
        SelectWorldInstance.ChunkSlots[updateX, updateY, updateZ].Chunks[(int)ChunkType.TERRAIN].Update = true;

        if (x - (chunkSize * updateX) == 0 && updateX != 0)
        {
            SelectWorldInstance.ChunkSlots[updateX - 1, updateY, updateZ].Chunks[(int)ChunkType.TERRAIN].Update = true;
        }

        if (x - (chunkSize * updateX) == gameWorldConfig.ChunkSize && updateX != SelectWorldInstance.ChunkSlots.GetLength(0) - 1)
        {
            SelectWorldInstance.ChunkSlots[updateX + 1, updateY, updateZ].Chunks[(int)ChunkType.TERRAIN].Update = true;
        }

        if (y - (chunkSize * updateY) == 0 && updateY != 0)
        {
            SelectWorldInstance.ChunkSlots[updateX, updateY - 1, updateZ].Chunks[(int)ChunkType.TERRAIN].Update = true;
        }

        if (y - (chunkSize * updateY) == gameWorldConfig.ChunkSize && updateY != SelectWorldInstance.ChunkSlots.GetLength(1) - 1)
        {
            SelectWorldInstance.ChunkSlots[updateX, updateY + 1, updateZ].Chunks[(int)ChunkType.TERRAIN].Update = true;
        }

        if (z - (chunkSize * updateZ) == 0 && updateZ != 0)
        {
            SelectWorldInstance.ChunkSlots[updateX, updateY, updateZ - 1].Chunks[(int)ChunkType.TERRAIN].Update = true;
        }

        if (z - (chunkSize * updateZ) == gameWorldConfig.ChunkSize && updateZ != SelectWorldInstance.ChunkSlots.GetLength(2) - 1)
        {
            SelectWorldInstance.ChunkSlots[updateX, updateY, updateZ + 1].Chunks[(int)ChunkType.TERRAIN].Update = true;
        }
    }
}
