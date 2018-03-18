using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameNetworkSpawner : NetworkBehaviour {
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
