using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, ActorController
{
    [SerializeField]
    private Animator animator;
    private CustomOBB obb = new CustomOBB();
    private World containWorld;
    [SerializeField]
    private Transform maxExtent;
    [SerializeField]
    private Transform minExtent;

    Vector3 ActorController.GetMinExtent()
    {
        return minExtent.position;
    }

    Vector3 ActorController.GetMaxExtent()
    {
        return maxExtent.position;
    }

    CustomOBB ActorController.GetOBB()
    {
        return obb;
    }

    void ActorController.Move(Vector3 dir, float speed)
    {
        animator.Play("Running@loop", 0);
        Vector3 newPos = gameObject.transform.position;
        newPos += dir.normalized * Time.deltaTime * speed;
        gameObject.transform.position = newPos;
    }
    void OnDrawGizmos()
    {
        Vector3 offsetPos = transform.position;
        offsetPos.y += 1.0f;
        Gizmos.DrawWireCube(offsetPos, new Vector3(obb.xRadius, obb.yRadius, obb.zRadius));
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
        obb.Init(transform, maxExtent);
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
            CollideInfo collideInfo = containWorld.customOctree.Collide(transform.position);
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
