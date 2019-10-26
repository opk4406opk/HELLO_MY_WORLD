using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayerCameraManager : MonoBehaviour
{
    public static GamePlayerCameraManager Instance = null;

    private Camera PlayerCamera;
    public void Init()
    {
        Instance = this;
        var camInstance = Instantiate<GameObject>(GameResourceSupervisor.GetInstance().GamePlayerCameraPrefab.LoadSynchro());
        PlayerCamera = camInstance.GetComponent<Camera>();
    }

    public Camera GetPlayerCamera()
    {
        return PlayerCamera;
    }
}
