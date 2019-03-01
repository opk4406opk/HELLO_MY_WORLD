using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class P2PNetworkSpawner : NetworkBehaviour {
    [Command]
    public void CmdSpawnFromServer(GameObject spawnObj)
    {
        NetworkServer.Spawn(spawnObj);
        KojeomLogger.DebugLog("spawn object from server", LOG_TYPE.P2P_NETWORK_SERVER_INFO);
    }
    [Command]
    public void CmdSpawnWithAuthoFromServer(GameObject spawnObj, NetworkIdentity netIdentity)
    {
        bool isSuccess = NetworkServer.SpawnWithClientAuthority(spawnObj, netIdentity.connectionToClient);
        if (isSuccess) KojeomLogger.DebugLog(string.Format("netIdentity : {0} 가 권한을 가진 오브젝트 스폰완료.", netIdentity), LOG_TYPE.P2P_NETWORK_SERVER_INFO);
        else KojeomLogger.DebugLog(string.Format("netIdentity : {0} 가 권한을 가진 오브젝트 스폰 실패.", netIdentity), LOG_TYPE.P2P_NETWORK_SERVER_INFO);
    }
}
