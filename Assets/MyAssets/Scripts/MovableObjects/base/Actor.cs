using UnityEngine;

public enum ACTOR_TYPE
{
    NPC,
    MONSTER,
    COUNT
}

public enum NPC_TYPE
{
    MERCHANT,
    GUARD,
    COUNT
}

public enum MONSTER_TYPE
{
    // to do
    COUNT
}

public abstract class ActorSpawnData
{
    public int HP;
    public int MP;
    public int AP;
    public string NAME;
    public ACTOR_TYPE ActorType;
    public string ResourceID;
    public int UniqueID;
}

public class NPCSpawnData : ActorSpawnData
{
    public NPC_TYPE NpcType;
}

public class MonsterSpawnData : ActorSpawnData
{
    public MONSTER_TYPE MonsterType;
}


abstract public class Actor : MonoBehaviour
{
    protected ACTOR_TYPE ActorType;
    protected int HealthPoint;
    protected int MagicaPoint;
    protected int AttackPoint;
    protected string Name;
    /// <summary>
    /// Actor가 가지고 있는 리소스 식별자.
    /// </summary>
    protected string ResourceID;
    /// <summary>
    /// Actor가 월드에 생성되면 발급받는 식별자.
    /// </summary>
    protected int SpawnID;
    /// <summary>
    /// Actor가 가지고 있는 유일한 Key 식별자. 
    /// (같은 리소스이면서 다른 이름을 가진 객체를 구분하기 위해 사용)
    /// </summary>
    protected int UniqueID;

    protected ActorController Controller;


    public delegate void del_OnClickActor(Actor actor);
    abstract public event del_OnClickActor OnClickedActor;
    abstract public void Init(ActorSpawnData spawnData, World world, int spawnID);
    abstract public void Show();
    abstract public void Hide();
    abstract public ActorController GetController();
    abstract public void Tick(float deltaTime);

    public ACTOR_TYPE GetActorType()
    {
        return ActorType;
    }
    public int GetSpawnID()
    {
        return SpawnID;
    }
    public string GetResourceID()
    {
        return ResourceID;
    }
    public int GetUniqueID()
    {
        return UniqueID;
    }
}

abstract public class NPCActor : Actor
{
    protected NPC_TYPE NpcType;
}

abstract public class MonsterActor : Actor
{
    protected MONSTER_TYPE MonsterType;
}
