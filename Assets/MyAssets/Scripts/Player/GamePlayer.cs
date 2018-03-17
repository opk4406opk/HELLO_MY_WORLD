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
    
}
