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
    public List<Actor> RegisteredActors { get; private set; } = new List<Actor>();
    public void Initialize()
    {
    }

    public void Register(GameObject obj)
    {

    }

    public void Register(Actor actor)
    {
        RegisteredActors.Add(actor);
    }

    public void UnRegister(InGameObjectType objectType, GameObject obj)
    {
        switch (objectType)
        {
            case InGameObjectType.OBJECT:
                break;
            case InGameObjectType.ACTOR:
                RegisteredActors.Remove(obj.GetComponent<Actor>());
                break;
        }
    }
}
