using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActorController : MonoBehaviour {
    abstract public void Move(Vector3 dir, float speed);
    abstract public void LookAt(Vector3 dir);
    abstract public void Init(World world);
    abstract public void StartController();
    abstract public void StopController();
    abstract public Transform GetActorTransform();

    protected BoxCollider BoxCollider;
}
