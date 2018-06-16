using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

/// <summary>
/// 첫 게임메인화면을 관리하는 클래스.
/// </summary>
public class MainMenuManager : MonoBehaviour {

    [Range(1, 15)]
    public int maximumWaitSec = 8;

    private void Start()
    {
        GameSoundManager.GetInstnace().PlaySound(GAME_SOUND_TYPE.BGM_mainMenu);
        PrefabStorage.GetInstance();
        CreateNetWorkManager();
    }

    /// <summary>
    /// 네트워크 매니저를 생성.
    /// (single, multi 게임에 상관없이 생성한다.)
    /// </summary>
    private void CreateNetWorkManager()
    {
        GameObject netMgr = Resources.Load<GameObject>(ConstFilePath.GAME_NET_MGR_PREFAB);
        if (netMgr == null)
        {
            KojeomLogger.DebugLog("GameNetworkManager creation is Failed.", LOG_TYPE.ERROR);
        }
        else
        {
            //init create netmgr.
            Instantiate(netMgr, new Vector3(0, 0, 0), Quaternion.identity);
            KojeomLogger.DebugLog("Success Create GameNetworkManager, and then select Characters scene.");
        }
    }

    /// <summary>
    /// 멀티플레이(p2p).
    /// </summary>
    public void OnClickMultiPlay()
    {
        //멀티플레이로 값 설정.
        GameStatus.isMultiPlay = true;
        //
        GameStartProcess();
    }

    private bool isSuccessProcessRun = false;
    private bool isSuccessLogin = false;
    /// <summary>
    /// 싱글플레이.
    /// </summary>
	public void OnClickSinglePlay()
	{
        //싱글플레이 스테이터스 설정.
        GameStatus.isMultiPlay = false;
        // 싱글플레이-> sing host 플레이로 설정.
        GameStatus.isSingleHostPlay = true;
        //
        GameStartProcess();
    }

    /// <summary>
    /// 게임시작
    /// -> 로그인 서버에 접속 후 실패/성공에 상관없이 캐릭터 선택창으로 이동.
    /// </summary>
    private void GameStartProcess()
    {
        GameNetworkManager.PostHttpRequest += PostLoginRequest;
        GameNetworkManager.ConnectLoginServer();
        if (isSuccessProcessRun == false) StartCoroutine(LoginProcess());
    }

	private void PostLoginRequest(bool isSuccess)
	{
		if (isSuccess) isSuccessLogin = true;
		else isSuccessLogin = false;
	}
	private IEnumerator LoginProcess()
	{
        isSuccessProcessRun = true;

        bool isTimeOut = false;
		int waitSec = 0;
		while (!isSuccessLogin)
		{
			//to do.
			//waiting connect to login-server (http-Request).
			if (waitSec == maximumWaitSec)
			{
				isTimeOut = true;
				break;
			}
			KojeomLogger.DebugLog("Waiting LoginServer...");
			yield return new WaitForSeconds(1.0f);
			waitSec++;
		}
		if(isTimeOut == false)
		{
			KojeomLogger.DebugLog("Success Login_server");
		}
		else
		{
			KojeomLogger.DebugLog("Waiting LoginServer TimeOut!", LOG_TYPE.ERROR);
		}
        // 일단 http 로그인 서버에 접속이 실패 or 성공에 상관없이 다음 화면으로 넘어간다.
        GameSoundManager.GetInstnace().StopSound(GAME_SOUND_TYPE.BGM_mainMenu);
        GameSceneLoader.LoadGameSceneAsync(GameSceneLoader.SCENE_TYPE.SELECT_CHARACTERS);
	}
	
    /// <summary>
    /// 싱글플레이 시에 사용될 기능.
    /// </summary>
    public void OnClickLoad()
    {
        if(ChkIsFile())
        {
            GameStatus.isLoadGame = true;
            SceneManager.LoadSceneAsync("GameLoading");
        }
        else
        {
            GameMessage.SetGameMsgType = GameMessage.MESSAGE_TYPE.WORLD_LOAD_FAIL;
            GameMessage.SetMessage("게임 로딩에 실패했습니다.");
			KojeomLogger.DebugLog("GameLoading Failed", LOG_TYPE.ERROR);
            UIPopupSupervisor.OpenPopupUI(POPUP_TYPE.gameMessage);
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
     
    private bool ChkIsFile()
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
