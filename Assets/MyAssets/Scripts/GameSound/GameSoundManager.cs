using UnityEngine;
using System.Collections.Generic;
public enum GAME_SOUND_TYPE
{
    BGM_InGame = 0,
    BGM_mainMenu = 1
}

public class GameSoundManager : MonoBehaviour {
    private static string bankFileName = "hello_my_world_bank";
    private static Dictionary<GAME_SOUND_TYPE, FMOD.Studio.EventInstance> soundInstances;
    private static GameSoundManager instance;
    public static GameSoundManager GetInstnace()
    {
        if (instance == null) instance = new GameSoundManager();
        return instance;
    }
    private GameSoundManager()
    {
        FMODUnity.RuntimeManager.LoadBank(bankFileName);
        FMODUnity.RuntimeManager.StudioSystem.setNumListeners(1);
        soundInstances = new Dictionary<GAME_SOUND_TYPE, FMOD.Studio.EventInstance>();
        // 배경음악은 2d 사운드.
        // 3d사운드의 경우 리스너와 사운드와의 거리설정이 있는데 이부분은 좀더 레퍼런스 문서를 찾아봐야한다.
        soundInstances.Add(GAME_SOUND_TYPE.BGM_mainMenu, FMODUnity.RuntimeManager.CreateInstance("event:/bgm/bgm_mainmenu_2d"));
        soundInstances.Add(GAME_SOUND_TYPE.BGM_InGame, FMODUnity.RuntimeManager.CreateInstance("event:/bgm/bgm_ingame_2d"));
    }

    public void PlaySound(GAME_SOUND_TYPE soundType)
    {
        FMOD.Studio.EventInstance soundInst;
        soundInstances.TryGetValue(soundType, out soundInst);
        FMOD.RESULT ret = soundInst.start();
        KojeomLogger.DebugLog(string.Format("play sound is {0}", ret));
    }
    public void StopSound(GAME_SOUND_TYPE soundType,
        FMOD.Studio.STOP_MODE stopMode = FMOD.Studio.STOP_MODE.ALLOWFADEOUT)
    {
        FMOD.Studio.EventInstance soundInst;
        soundInstances.TryGetValue(soundType, out soundInst);
        FMOD.RESULT ret = soundInst.stop(stopMode);
        KojeomLogger.DebugLog(string.Format("stop sound is {0}", ret));
    }

    public void Release()
    {
        FMODUnity.RuntimeManager.UnloadBank(bankFileName);
        foreach(var sound in soundInstances)
        {
            sound.Value.release();
        }
    }
}
