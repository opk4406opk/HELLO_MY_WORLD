using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameParticeEffectCategory
{
    Action, // 때리기, 피격당했을때 등..
    Enviroment, // 환경.
    Skill, // 스킬에서 발생하는 이펙트.
    Buff, // 버프에서 사용되는 이펙트.
    COUNT
}

public class GameParticleEffectManager : MonoBehaviour
{
    public static GameParticleEffectManager Instance = null;
    public void Init()
    {
        Instance = this;

    }
}
