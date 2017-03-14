using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Actor : MonoBehaviour
{
    abstract public void Init(Vector3 pos, World world);
    abstract public ActorController GetActorController();

}
