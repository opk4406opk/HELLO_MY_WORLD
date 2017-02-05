using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorController : MonoBehaviour {

    [SerializeField]
    private Transform _curPos;
    public Transform curPos { get { return _curPos; } }

    public void Move(Vector3 dir, float speed)
    {
        Vector3 newPos = gameObject.transform.position;
        newPos += dir.normalized * Time.deltaTime * speed;
        gameObject.transform.position = newPos;
    }

}
