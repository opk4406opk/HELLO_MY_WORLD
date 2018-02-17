using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(NetworkTransform))]
public class GameCharacter : NetworkBehaviour {

    [SerializeField]
    private PlayerController controller;
    public PlayerController GetController()
    {
        return controller;
    }
    public void SetController(PlayerController _controller)
    {
        controller = _controller;
    }
}
