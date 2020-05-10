using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class GamePlayer : MonoBehaviour
{
    public GamePlayerController Controller { get; private set; }

    [SerializeField]
    private int CharacterType;
    [SerializeField]
    private string CharacterName;
    private GameCharacterInstance CharInstance;
    public bool bInitProcessFinish { get; private set; } = false;

    public void Initialize(int charType, string charName, Vector3 initPos)
    {
        // init position.
        //gameObject.transform.position = initPos;
        gameObject.name = string.Format("GamePlayer_{0}",charName);
        //
        KojeomLogger.DebugLog("게임플레이어 PostInit 시작", LOG_TYPE.INFO);
        CharacterName = charName;
        CharacterType = charType;
        CharInstance = MakeGameChararacter(GameResourceSupervisor.GetInstance().GetCharacterPrefab(charType));
        // 캐릭터 인스턴스는 게임플레이어 하위종속으로 설정.
        CharInstance.transform.parent = gameObject.transform;
        CharInstance.transform.localPosition = new Vector3(0, 0, 0);
        //
        Controller = gameObject.GetComponent<GamePlayerController>();
        Controller.Init(this, CharInstance);
        Controller.EnableTick(false); // test
        Controller.SetPosition(initPos);
        SetObjectLayer(true);
        //
        KojeomLogger.DebugLog("게임플레이어 PostInit 완료. ", LOG_TYPE.INFO);
    }

    private void SetObjectLayer(bool isMine)
    {
        int layer = -999;
        if (isMine == true) layer = LayerMask.NameToLayer("PlayerCharacter");
        else layer = LayerMask.NameToLayer("OtherPlayerCharacter");

        gameObject.layer = layer;
        // GameObject가 아닌 트랜스폼으로 자식노드들을 가져와야 정상 동작.
        var childObjects = KojeomUtility.GetChilds<Transform>(CharInstance.gameObject);
        foreach (var child in childObjects)
        {
            child.gameObject.layer = layer;
        }
    }

    private GameCharacterInstance MakeGameChararacter(GameObject _prefab)
    {
        GameObject characterObject = Instantiate(_prefab, new Vector3(0, 0, 0),
            new Quaternion(0, 0, 0, 0)) as GameObject;
        //
        GameCharacterInstance gameChar = characterObject.GetComponent<GameCharacterInstance>();
        gameChar.Init();
        return gameChar;
    }
}
