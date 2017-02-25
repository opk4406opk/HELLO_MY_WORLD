using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, ActorController
{
    [SerializeField]
    private Animator animator;
    private CustomAABB aabb;
    private World containWorld;
    [SerializeField]
    private Transform minExtent;
    [SerializeField]
    private Transform maxExtent;

    void OnDrawGizmos()
    {
    }

    void ActorController.Move(Vector3 dir, float speed)
    {
        animator.Play("Running@loop", 0);
        Vector3 newPos = gameObject.transform.position;
        newPos += dir.normalized * Time.deltaTime * speed;
        gameObject.transform.position = newPos;
    }

    void ActorController.LookAt(Vector3 dir)
    {
        float theta = Vector3.Dot(dir, transform.forward) / (transform.forward.magnitude * dir.magnitude);
        float angle = Mathf.Acos(theta) * Mathf.Rad2Deg;
        Vector3 cross = Vector3.Cross(dir, transform.forward);
        if (cross.z < 0.0f) angle = 360.0f - angle;
        transform.Rotate(transform.up, angle);
    }

    void ActorController.Init(World world)
    {
        containWorld = world;
        StartCoroutine(SimpleGravityForce());
    }

    Transform ActorController.GetActorTransform()
    {
        return transform;
    }
    private IEnumerator SimpleGravityForce()
    {
        for (;;)
        {
            aabb.MakeAABB(minExtent.position, maxExtent.position);
            CollideInfo collideInfo = containWorld.customOctree.Collide(aabb);
            if (!collideInfo.isCollide)
            {
                transform.position = new Vector3(transform.position.x,
                    transform.position.y - 0.1f,
                    transform.position.z);
            }
            yield return null;
        }
    }
}
