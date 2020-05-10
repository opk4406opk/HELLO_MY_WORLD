using UnityEngine;

/*
 * 
 * 대분류 타입은 대문자로, 소분류 타입은 소문자로
 * // 동물의 경우 카테고리별로 세부적인 구분을 한다.
 * 
 */

public enum ACTOR_TYPE
{
    NPC,
    MONSTER,
    ANIMAL,
    COUNT
}

public enum NPC_TYPE
{
    Merchant,
    Guard,
    COUNT
}

public enum MONSTER_TYPE
{
    // to do
    Cyclopes,
    Fiery,
    COUNT
}

public enum ANIMAL_CATEGORY
{
    None,
    Herbivore, // 초식 동물.
    FleshEating, // 육식 동물.
    Polyphagia // 잡식 동물.
}

public enum ANIMAL_TYPE
{
    Chick,
    Chiken,
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

public class AnimalSpawnData : ActorSpawnData
{
    public ANIMAL_TYPE AnimalType;
    public ANIMAL_CATEGORY AnimalCategory;
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
    /// Actor가 월드에 생성되면 발급받는 식별자. ( 클라이언트에서 사용되는 ID )
    /// </summary>
    protected int SpawnID;
    /// <summary>
    /// 네트워크 상에서 부여받는 Actor ID 값.
    /// </summary>
    protected int NetID;
    /// <summary>
    /// Actor가 가지고 있는 유일한 Key 식별자. 
    /// (같은 리소스이면서 다른 이름을 가진 객체를 구분하기 위해 사용)
    /// </summary>
    protected int UniqueID;
    protected ActorController Controller;

    public delegate void del_OnClickActor(Actor actor);
    abstract public event del_OnClickActor OnClickedActor;
    abstract public void Init(ActorSpawnData spawnData, SubWorld world, int spawnID);
    abstract public ActorController GetController();
    abstract public void Update();

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
    public int GetNetID()
    {
        return NetID;
    }
    public void Show()
    {
        gameObject.SetActive(true);
        Controller.StartController();
    }
    public void Hide()
    {
        gameObject.SetActive(false);
        Controller.StopController();
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

abstract public class AnimalActor : Actor
{
    protected ANIMAL_TYPE AnimalType;
    protected ANIMAL_CATEGORY AnimalCategory;
}
