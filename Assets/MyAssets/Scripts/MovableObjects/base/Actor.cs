using UnityEngine;

public enum ACTOR_TYPE
{
    NPC = 0,
    MONSTER = 1
}
abstract public class Actor : MonoBehaviour
{
    protected ACTOR_TYPE actorType;
    protected ActorController controller;

    public delegate void del_OnClickActor(Actor actor);
    abstract public event del_OnClickActor OnClickedActor;
    abstract public void Init(Vector3 pos, World world, ACTOR_TYPE actorType);
    abstract public ACTOR_TYPE GetActorType();
    abstract public ActorController GetController();

}
