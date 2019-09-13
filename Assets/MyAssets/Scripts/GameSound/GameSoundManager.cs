using System.Collections.Generic;

public enum GAME_SOUND_TYPE
{
    BGM_InGame = 0,
    BGM_mainMenu = 1
}

public class GameSoundManager {
    private static GameSoundManager instance;
    public static GameSoundManager GetInstnace()
    {
        if (instance == null) instance = new GameSoundManager();
        return instance;
    }
    private GameSoundManager()
    {
       
    }

    public void PlaySound(GAME_SOUND_TYPE soundType)
    {

    }
    public void StopSound(GAME_SOUND_TYPE soundType)
    {
      
    }

    public void Release()
    {
        
    }
}
