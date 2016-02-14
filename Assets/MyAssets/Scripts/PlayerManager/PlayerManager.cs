using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour {

    [SerializeField]
    private GameObject gang_Prefab;
    [SerializeField]
    private GameObject fireFighter_Prefab;
    [SerializeField]
    private GameObject police_Prefab;
    [SerializeField]
    private GameObject sheriff_Prefab;
    [SerializeField]
    private GameObject trucker_Prefab;

    private SceneToScene_Data sceneToSceneData;

    private GameObject _gamePlayer;
    public GameObject gamePlayer
    {
        get { return _gamePlayer; }
    }

    public Vector3 initPosition;

    public void Init()
    {
        sceneToSceneData = GameObject.Find("SceneToScene_datas").GetComponent<SceneToScene_Data>();
        CreateProcess();
    }

    private void CreateProcess()
    {
        string chName;
        sceneToSceneData.gameChDatas.TryGetValue("chName", out chName);
        switch(chName)
        {
            case "FireFighter":
                InstanceCharacter(fireFighter_Prefab);
                break;
            case "Gang":
                InstanceCharacter(gang_Prefab);
                break;
            case "Police":
                InstanceCharacter(police_Prefab);
                break;
            case "Sheriff":
                InstanceCharacter(sheriff_Prefab);
                break;
            case "Trucker":
                InstanceCharacter(trucker_Prefab);
                break;
            default:
                break;
        }
    }
   
    private void InstanceCharacter(GameObject _prefab)
    {
        _gamePlayer = Instantiate(_prefab,
            initPosition,
            new Quaternion(0, 0, 0, 0)) as GameObject;
    }
}
