using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : ActorController
{
    [SerializeField]
    private Animator Animator;
    private World ContainWorld;

    public override void Move(Vector3 dir, float speed)
    {
        Animator.Play("Running@loop", 0);
        Vector3 newPos = gameObject.transform.position;
        newPos += dir.normalized * Time.deltaTime * speed;
        gameObject.transform.position = newPos;
    }
    //void OnDrawGizmos()
    //{
    //    Vector3 offsetPos = transform.position;
    //    offsetPos.y += 1.0f;
    //    Gizmos.DrawWireCube(offsetPos, new Vector3(obb.xRadius, obb.yRadius, obb.zRadius));
    //}

    public override void StartController()
    {
        StartCoroutine(SimpleGravityForce());
    }

    public override void StopController()
    {
        StopCoroutine(SimpleGravityForce());
    }

    public override void LookAt(Vector3 dir)
    {
        float theta = Vector3.Dot(dir, transform.forward) / (transform.forward.magnitude * dir.magnitude);
        float angle = Mathf.Acos(theta) * Mathf.Rad2Deg;
        Vector3 cross = Vector3.Cross(dir, transform.forward);
        if (cross.z < 0.0f) angle = 360.0f - angle;
        transform.Rotate(transform.up, angle);
    }

    public override void Init(World world)
    {
        ContainWorld = world;
        BoxCollider = gameObject.GetComponent<BoxCollider>();
    }

    public override Transform GetActorTransform()
    {
        return transform;
    }
    private IEnumerator SimpleGravityForce()
    {
        for (;;)
        {
            CollideInfo collideInfo = ContainWorld.CustomOctree.Collide(transform.position);
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
