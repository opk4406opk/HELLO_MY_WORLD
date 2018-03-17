using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameNetworkSpawner : NetworkBehaviour {

    private static GameNetworkSpawner netSpawner;
    public static GameNetworkSpawner GetInstance()
    {
        if (netSpawner == null) netSpawner = new GameNetworkSpawner();
        return netSpawner;
    }
    [Command]
    public void CmdSpawnFromServer(GameObject spawnObj)
    {
        NetworkServer.Spawn(spawnObj);
    }
    [Command]
    public void CmdSpawnWithAuthoFromServer(GameObject spawnObj)
    {
        NetworkServer.Spawn(spawnObj);
    }
}
