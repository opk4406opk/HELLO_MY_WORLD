using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InGameObjectType
{
    OBJECT,
    ACTOR,
    COUNT
}

public class InGameObjectRegister
{
    private World world;
    public void Initialize(World world)
    {
        this.world = world;
    }

    public void Register(GameObject obj)
    {

    }

    public void Register(Actor actor)
    {
        world.RegisteredActors.Add(actor);
    }

    public void UnRegister(InGameObjectType objectType, GameObject obj)
    {
        switch (objectType)
        {
            case InGameObjectType.OBJECT:
                break;
            case InGameObjectType.ACTOR:
                world.RegisteredActors.Remove(obj.GetComponent<Actor>());
                break;
        }
    }
}
