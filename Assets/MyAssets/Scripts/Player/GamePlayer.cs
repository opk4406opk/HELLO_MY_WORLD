using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
[RequireComponent(typeof(NetworkTransform))]
public class GamePlayer : NetworkBehaviour
{
    [SyncVar]
    public int characterType;
    [SyncVar]
    public string characterName;
    private GameObject _charInstance;
    public GameObject charInstance
    {
        get { return _charInstance; }
    }
    public void Init(GameObject charInst, int charType)
    {
        characterType = charType;
        _charInstance = charInst;
    }
    public override void OnStartLocalPlayer()
    {
        KojeomLogger.DebugLog("Start Local Player");
        base.OnStartLocalPlayer();
    }
}
