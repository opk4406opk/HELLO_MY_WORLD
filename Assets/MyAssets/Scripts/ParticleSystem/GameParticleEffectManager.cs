using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameParticeEffectCategory
{
    None,
    Action, // 때리기, 피격당했을때 등..
    Enviroment, // 환경.
    Skill, // 스킬에서 발생하는 이펙트.
    Buff, // 버프에서 사용되는 이펙트.
    COUNT
}

public enum GameParticleType
{
    None,
    // Action.
    HitBlock,
    BloodSplatter,
    // Enviroment.
    DirtSplatter,
    Dust01,
    Dust02,
    Dust03,
    Fire,
    FireworksBlueLarge,
    FireworksBlueSmall,
    FireworksGreenLarge,
    FireworksGreenSmall,
    FireworksYellowLarge,
    FireworksYellowSmall,
    Smoke,
    Steam,
    TireSmoke,
    WaterSplash,
    WaterSplatter,
    //
    COUNT,
}

public struct ParticleEffectSpawnParams
{
    public Vector3 SpawnLocation;
    public Quaternion SpawnRotation;
    public GameParticleType ParticleType;
    public bool bLooping;
    public bool bStart;
}

public class GameParticleEffectManager : MonoBehaviour
{
    public static GameParticleEffectManager Instance = null;
    public void Init()
    {
        Instance = this;
    }

    public ParticleSystem SpawnParticleEffect(ParticleEffectSpawnParams spawnParams)
    {
        GameResourceSupervisor.GetInstance().ParticleEffectPrefabs[(int)GetCategory(spawnParams.ParticleType)].Resources.TryGetValue(spawnParams.ParticleType, out SoftObjectPtr objectPtr);
        GameObject newInstance = Instantiate<GameObject>(objectPtr.LoadSynchro(), spawnParams.SpawnLocation, spawnParams.SpawnRotation);
        //
        ParticleSystem ps = newInstance.GetComponent<ParticleSystem>();
        if (spawnParams.bStart == true) ps.Play();
        var module = ps.main;
        module.loop = spawnParams.bLooping;
        module.stopAction = ParticleSystemStopAction.Destroy;
        //
        return ps;
    }

    private GameParticeEffectCategory GetCategory(GameParticleType particleType)
    {
        switch (particleType)
        {
            case GameParticleType.HitBlock:
            case GameParticleType.BloodSplatter:
                return GameParticeEffectCategory.Action;
            case GameParticleType.DirtSplatter:
            case GameParticleType.Dust01:
            case GameParticleType.Dust02:
            case GameParticleType.Dust03:
            case GameParticleType.Fire:
            case GameParticleType.FireworksBlueLarge:
            case GameParticleType.FireworksBlueSmall:
            case GameParticleType.FireworksGreenLarge:
            case GameParticleType.FireworksGreenSmall:
            case GameParticleType.FireworksYellowLarge:
            case GameParticleType.FireworksYellowSmall:
            case GameParticleType.Smoke:
            case GameParticleType.Steam:
            case GameParticleType.TireSmoke:
            case GameParticleType.WaterSplash:
            case GameParticleType.WaterSplatter:
                return GameParticeEffectCategory.Enviroment;
        }
        return GameParticeEffectCategory.None;
    }
}
