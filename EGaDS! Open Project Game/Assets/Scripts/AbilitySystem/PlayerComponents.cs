using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerComponents
{
    public PlayerMovement Movement { get; }
    public Collider2D Collider { get; }
    public Rigidbody2D Rigidbody { get; }
    public AbilityManager AbilityManager { get; }
    public Transform PlayerTransform { get; }
    public GameObject PlayerObject { get;  }
    // TODO: add sprite renderer and animator

    public PlayerComponents(GameObject player) 
    {
        Movement = player.GetComponent<PlayerMovement>();
        Collider = player.GetComponent<Collider2D>();
        Rigidbody = player.GetComponent<Rigidbody2D>();
        AbilityManager = player.GetComponent<AbilityManager>();
        PlayerTransform = player.transform;
        PlayerObject = player;
    }
}
