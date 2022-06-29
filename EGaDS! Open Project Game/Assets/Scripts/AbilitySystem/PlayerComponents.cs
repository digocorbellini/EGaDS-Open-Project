using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerComponents
{
    public PlayerController controller { get; }
    public Collider2D collider { get; }
    public Rigidbody2D rigidbody { get; }
    public AbilityManager abilityManager { get; }
    public Transform transform { get; }
    public GameObject gameObject { get;  }
    // TODO: add sprite renderer and animator

    public PlayerComponents(GameObject player) 
    {
        controller = player.GetComponent<PlayerController>();
        collider = player.GetComponent<Collider2D>();
        rigidbody = player.GetComponent<Rigidbody2D>();
        abilityManager = player.GetComponent<AbilityManager>();
        transform = player.transform;
        gameObject = player;
    }
}
