using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class GamePlayer : NetworkBehaviour
{
    private PlayerController playerController;
    private int _characterType;
    private string _characterName;
    private GameCharacter _charInstance;
    public GameCharacter charInstance
    {
        get { return _charInstance; }
    }
    public void Init(GameCharacter charInst, int charType, string charName = null)
    {
        KojeomLogger.DebugLog(string.Format("GamePlayer Init start [CharName : {0}]", charName));
        _characterName = charName;
        _characterType = charType;
        _charInstance = charInst;
        // 캐릭터 인스턴스는 게임플레이어 하위종속으로 설정.
        _charInstance.transform.parent = gameObject.transform;
        _charInstance.transform.localPosition = new Vector3(0, 0, 0);
        playerController = gameObject.GetComponent<PlayerController>();
        playerController.Init(Camera.main, gameObject);
    }
    public PlayerController GetController()
    {
        return playerController;
    }

    public override void OnStartAuthority()
    {
        KojeomLogger.DebugLog("this gameplayer client with Authority", LOG_TYPE.NETWORK_CLIENT_INFO);
    }

    public override void OnStartClient()
    {
        KojeomLogger.DebugLog("this gameplayer client", LOG_TYPE.NETWORK_CLIENT_INFO);
    }
}
