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

    protected ActorController Controller;

    public delegate void del_OnClickActor(Actor actor);
    abstract public event del_OnClickActor OnClickedActor;
    abstract public void Init(ActorSpawnData spawnData, World world);
    abstract public void Show();
    abstract public void Hide();
    abstract public ACTOR_TYPE GetActorType();
    abstract public ActorController GetController();
    abstract public void Tick(float deltaTime);
}

abstract public class NPCActor : Actor
{
    protected NPC_TYPE NpcType;
}

abstract public class MonsterActor : Actor
{
    protected MONSTER_TYPE MonsterType;
}
