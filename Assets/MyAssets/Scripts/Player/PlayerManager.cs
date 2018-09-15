using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine.Networking;
/// <summary>
/// 게임내 사용자(캐릭터)를 관리하는 클래스.
/// </summary>
public class PlayerManager : MonoBehaviour {

    public GamePlayer myGamePlayer;
    private GamePlayerController myPlayerController;
    public static PlayerManager instance;
    private GameObject gamePlayerPrefab;

    public void Init()
    {
        gamePlayerPrefab = GameNetworkManager.singleton.playerPrefab;
        instance = this;
        //
        myGamePlayer = GameNetworkManager.GetNetworkManagerInstance().GetMyGamePlayer();
        myGamePlayer.GetController().Init(Camera.main, myGamePlayer);
        //Player Manager 하위 종속으로 변경.
        myGamePlayer.transform.parent = gameObject.transform;
        myPlayerController = myGamePlayer.GetController();
    }

    public static Vector3 GetGamePlayerInitPos()
    {
        //임시로 정해진 값.
        return new Vector3(0.0f, 48.0f, 0.0f);
    }
    /// <summary>
    /// 플레이어 컨트롤러를 시작합니다.
    /// </summary>
    public void StartController()
    {
        myPlayerController.StartControllProcess();
    }
    /// <summary>
    /// 플레이어 컨트롤러를 중지합니다.
    /// </summary>
    public void StopController()
    {
        myPlayerController.StopControllProcess();
    }

}
