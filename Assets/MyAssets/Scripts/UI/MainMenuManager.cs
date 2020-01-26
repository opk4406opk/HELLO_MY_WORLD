using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

/// <summary>
/// 첫 게임메인화면을 관리하는 클래스.
/// </summary>
public class MainMenuManager : MonoBehaviour {

    [Range(1, 300)]
    public int MaximumWaitSec = 2;
    [SerializeField]
    private UIButton btn_singleGameStart;
    [SerializeField]
    private UIButton btn_multiGameStart;
    [SerializeField]
    private UIButton btn_settings;
    [SerializeField]
    private UIButton btn_exit;

    public bool bSoundOn = false;

    private void Start()
    {
        if(bSoundOn == true)
        {
            GameSoundManager.GetInstnace().PlaySound(GAME_SOUND_TYPE.BGM_mainMenu);
        }
        GameResourceSupervisor.GetInstance();
    }

    /// <summary>
    /// 멀티플레이(p2p).
    /// </summary>
    public void OnClickMultiPlay()
    {
        //멀티플레이로 값 설정.
        GameStatus.CurrentGameModeState = GameModeState.MULTI;
        GameStartProcess();
    }

    private bool bSuccessLogin = false;
    /// <summary>
    /// 싱글플레이.
    /// </summary>
	public void OnClickSinglePlay()
	{
        // 싱글플레이-> sing host 플레이로 설정.
        GameStatus.CurrentGameModeState = GameModeState.SINGLE;
        GameStartProcess();
    }

    /// <summary>
    /// 게임시작
    /// -> 로그인 서버에 접속 후 실패/성공에 상관없이 캐릭터 선택창으로 이동.
    /// </summary>
    private void GameStartProcess()
    {
        EnableButtons(false);
        UIPopupSupervisor.OpenPopupUI(UI_POPUP_TYPE.WaitingConnect);
        StartCoroutine(LoginProcess());
    }

    private void EnableButtons(bool enable)
    {
        btn_exit.isEnabled = enable;
        btn_settings.isEnabled = enable;
        btn_multiGameStart.isEnabled = enable;
        btn_singleGameStart.isEnabled = enable;
    }

	private void PostLoginRequest(bool isSuccess)
	{
		if (isSuccess) bSuccessLogin = true;
		else bSuccessLogin = false;
	}
	private IEnumerator LoginProcess()
	{
        bool bTimeOut = false;
		int waitSec = 0;
		while (bSuccessLogin == false)
		{
			//waiting connect to login-server (http-Request).
			if (waitSec == MaximumWaitSec)
			{
				bTimeOut = true;
				break;
			}
			KojeomLogger.DebugLog("Waiting LoginServer...");
			yield return new WaitForSeconds(1.0f);
			waitSec++;
		}
		if(bTimeOut == false)
		{
			KojeomLogger.DebugLog("Success Login_server");
		}
		else
		{
            KojeomLogger.DebugLog("Waiting LoginServer TimeOut!", LOG_TYPE.ERROR);
		}
        // 일단 http 로그인 서버에 접속이 실패 or 성공에 상관없이 다음 화면으로 넘어간다.
        UIPopupSupervisor.ClosePopupUI(UI_POPUP_TYPE.WaitingConnect);
        GameSoundManager.GetInstnace().StopSound(GAME_SOUND_TYPE.BGM_mainMenu);
        GameSceneLoader.LoadGameSceneAsync(GameSceneLoader.SCENE_TYPE.SelectCharacter);
	}
	
    /// <summary>
    /// 싱글플레이 시에 사용될 기능.
    /// </summary>
    public void OnClickLoad()
    {
        if(CheckIsFile())
        {
            GameStatus.DetailSingleMode = DetailSingleMode.LOAD_GAME;
            GameSceneLoader.LoadGameSceneAsync(GameSceneLoader.SCENE_TYPE.GameLoading);
        }
        else
        {
            GameMessage.SetMessage("게임 로딩에 실패했습니다.", GameMessage.MESSAGE_TYPE.WORLD_LOAD_FAIL);
			KojeomLogger.DebugLog("GameLoading Failed", LOG_TYPE.ERROR);
            UIPopupSupervisor.OpenPopupUI(UI_POPUP_TYPE.GameMessage);
        }
    }
    
    public void OnClickSettings()
    {
        // to do
    }

    public void OnClickExit()
    {
        Application.Quit();
    }
     
    private bool CheckIsFile()
    {
        BinaryFormatter bf;
        FileStream fileStream;
        try
        {
            string filePath = Application.dataPath + "/GameSavefile.dat";
            //파일 생성.
            bf = new BinaryFormatter();
            fileStream = File.Open(filePath, FileMode.Open);
            if (fileStream != null) fileStream.Close();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
