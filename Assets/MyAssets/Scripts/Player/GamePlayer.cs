using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class GamePlayer : MonoBehaviour
{
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
    }
    
}
