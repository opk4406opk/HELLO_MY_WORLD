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
    public void CmdSpawnWithAuthoFromServer(GameObject spawnObj, NetworkIdentity netIdentity)
    {
        bool isSuccess = NetworkServer.SpawnWithClientAuthority(spawnObj, netIdentity.connectionToClient);
        if (isSuccess) KojeomLogger.DebugLog(string.Format("netIdentity : {0} 가 권한을 가진 오브젝트 스폰완료.", netIdentity), LOG_TYPE.NETWORK_SERVER_INFO);
        else KojeomLogger.DebugLog(string.Format("netIdentity : {0} 가 권한을 가진 오브젝트 스폰 실패.", netIdentity), LOG_TYPE.NETWORK_SERVER_INFO);
    }

    public void CreateNetGamePlayer(NetworkConnection conn, bool isMyPlayer, int charType, string playerName)
    {
        GameObject instance = Instantiate(GameNetworkManager.GetInstance().playerPrefab,
            new Vector3(0, 0, 0), Quaternion.identity);
        GamePlayer gamePlayer = instance.GetComponent<GamePlayer>();
        gamePlayer.Init(charType);
        //
        GameNetworkManager.GetInstance().netUserList.Add(new GameNetUser(playerName, conn, gamePlayer));
        if (isMyPlayer)
        {
            CmdSpawnWithAuthoFromServer(instance, gamePlayer.GetNetworkIdentity());
        }
        else
        {
            CmdSpawnFromServer(instance);
        }
    }
}
