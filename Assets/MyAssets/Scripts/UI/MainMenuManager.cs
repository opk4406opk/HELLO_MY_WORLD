using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using System.Text;

/// <summary>
/// 첫 게임메인화면을 관리하는 클래스.
/// </summary>
public class MainMenuManager : MonoBehaviour {

	private bool isSuccessLogin = false;
	public void OnClickStart()
	{
		GameNetworkManager.PostHttpRequest += PostLoginRequest;
		GameNetworkManager.ConnectLoginServer();
		StartCoroutine(LoginProcess());
    }
	private void PostLoginRequest(bool isSuccess)
	{
		if (isSuccess) isSuccessLogin = true;
		else isSuccessLogin = false;
	}
	private IEnumerator LoginProcess()
	{
		while (!isSuccessLogin)
		{
			//to do.
			//waiting connect to login-server (http-Request).
			yield return null;
		}
		Debug.Log("Success Login_server");
		SceneManager.LoadSceneAsync("SelectCharacter");
	}
	
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
            UIPopupManager.OpenGameMessage();
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
