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
    private GameCharacterInstance CharacterInstance;
    [SerializeField]
    private Transform CamPivotTransform;
    public bool bInitProcessFinish { get; private set; } = false;

    public void Initialize(int charType, string charName, Vector3 initPos)
    {
        gameObject.name = string.Format("GamePlayer_{0}",charName);
        //
        KojeomLogger.DebugLog("게임플레이어 PostInit 시작", LOG_TYPE.INFO);
        CharacterName = charName;
        CharacterType = charType;
        CharacterInstance = MakeGameChararacter(GameResourceSupervisor.GetInstance().GetCharacterPrefab(charType));
        // 카메라 피벗은 캐릭터 인스턴스 하위종속으로 설정.
        CamPivotTransform.name = "Camera_Pivot";
        CamPivotTransform.parent = CharacterInstance.transform;
        // 캐릭터 인스턴스는 게임플레이어의 하위종속으로 설정.
        CharacterInstance.transform.parent = transform;
        CharacterInstance.transform.localPosition = new Vector3(0, 0, 0);
        // 카메라는 캐릭터 인스턴스 하위종속으로 등록.
        GamePlayerCameraManager.Instance.AttachTo(CamPivotTransform);
        GamePlayerCameraManager.Instance.GetPlayerCamera().transform.position = CharacterInstance.GetPosition() + new Vector3(0.0f, 2.0f, 0.0f);
        // ECM 컴포넌트에서 Awake() 타이밍에 하위 컴포넌트들을 통해 초기화 하는 작업이 있음.
        // 따라서, 여기서에서 다시한번 호출해서 제대로 정상 동작하도록 한다.
        CharacterInstance.ECM_BaseCharController.ManualAwake(GamePlayerCameraManager.Instance.GetPlayerCamera());
        //
        Controller = gameObject.GetComponent<GamePlayerController>();
        Controller.Init(this, CharacterInstance);
        Controller.EnableTick(true);
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
        //GameObject가 아닌 트랜스폼으로 자식노드들을 가져와야 정상 동작.
        var childObjects = KojeomUtility.GetChilds<Transform>(CharacterInstance.gameObject);
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

    public Vector3 GetPosition()
    {
        return Controller.GetPosition();
    }
}
