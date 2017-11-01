using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    [SerializeField]
    private GameObject worldPrefab;
    [SerializeField]
    private GameObject chunkPrefab;
    [SerializeField]
    private Transform worldGroupTrans;


    private List<World> _worldList = new List<World>();
    public List<World> worldList
    {
        get { return _worldList; }
    }
    private int maxSubWorld = 0;

    public static WorldManager instance;

    public void Init()
    {
        CreateGameWorld();
        instance = this;
    }

    public void CreateGameWorld()
    {
        maxSubWorld = SubWorldDataFile.instance.maxSubWorld;
        for (int idx = 0; idx < maxSubWorld; ++idx)
        {
            int subWorldPosX = SubWorldDataFile.instance.GetPosValue(idx, "X") * GameConfig.subWorldX;
            int subWorldPosZ = SubWorldDataFile.instance.GetPosValue(idx, "Z") * GameConfig.subWorldZ;
            string subWorldName = SubWorldDataFile.instance.GetWorldName(idx, "WORLD_NAME");

            GameObject newSubWorld = Instantiate(worldPrefab, new Vector3(0, 0, 0),
                new Quaternion(0, 0, 0, 0)) as GameObject;
            newSubWorld.GetComponent<World>().chunkPrefab = chunkPrefab;
            newSubWorld.GetComponent<World>().playerTrans = PlayerManager.instance.gamePlayer.transform;
            newSubWorld.GetComponent<World>().Init(subWorldPosX, subWorldPosZ);
            newSubWorld.GetComponent<World>().worldName = subWorldName;
            newSubWorld.transform.parent = worldGroupTrans;
            //add world.
            _worldList.Add(newSubWorld.GetComponent<World>());
        }
    }
}
