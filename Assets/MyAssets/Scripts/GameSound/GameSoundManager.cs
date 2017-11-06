using UnityEngine;
using System.Collections.Generic;
public enum GAME_SOUND_TYPE
{
    BGM_InGame = 0,
    BGM_mainMenu = 1
}

public class GameSoundManager : MonoBehaviour {

    private Dictionary<GAME_SOUND_TYPE, FMOD.Studio.EventInstance> soundInstances;
    public static GameSoundManager instance;

    public void Init()
    {
        FMODUnity.RuntimeManager.LoadBank("hello_my_world_bank");
        FMODUnity.RuntimeManager.StudioSystem.setNumListeners(1);
        soundInstances = new Dictionary<GAME_SOUND_TYPE, FMOD.Studio.EventInstance>();
        // 배경음악은 2d 사운드.
        // 3d사운드의 경우 리스너와 사운드와의 거리설정이 있는데 이부분은 좀더 레퍼런스 문서를 찾아봐야한다.
        soundInstances.Add(GAME_SOUND_TYPE.BGM_mainMenu, FMODUnity.RuntimeManager.CreateInstance("event:/bgm/bgm_mainmenu_2d"));
        soundInstances.Add(GAME_SOUND_TYPE.BGM_InGame, FMODUnity.RuntimeManager.CreateInstance("event:/bgm/bgm_ingame_2d"));
        instance = this;
    }

    public void PlaySound(GAME_SOUND_TYPE soundType)
    {
        FMOD.Studio.EventInstance soundInst;
        soundInstances.TryGetValue(soundType, out soundInst);
        FMOD.RESULT ret = soundInst.start();
        KojeomLogger.DebugLog(string.Format("play sound is {0}", ret));
    }
    public void StopSound(GAME_SOUND_TYPE soundType)
    {
        FMOD.Studio.EventInstance soundInst;
        soundInstances.TryGetValue(soundType, out soundInst);
        FMOD.RESULT ret = soundInst.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        KojeomLogger.DebugLog(string.Format("play sound is {0}", ret));
    }
}
