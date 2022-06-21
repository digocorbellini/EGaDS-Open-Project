using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float MovementSpeed = 1;
    public float JumpHeight = 1;

    private Rigidbody2D _rigidbody;
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    
    private void Update()
    {
        // for player movement, expecially on a keyboard, it is usually better to use "Input.GetAxisRaw()" since
        // "GetAxis()" smooths the value between 1 and 0 or -1 and 0 while "GetAxisRaw()" is either 1, 0, or -1. 
        // This is better for platformers since your inputs will be more responsive and for keyboard you don't have
        // to worry about something like how far right a joystick is. But very good that you used GetAxis instead of
        // "Input.GetKeyDown()", which is much messier. 
        var movement = Input.GetAxis("Horizontal");
        // be wary of setting the player's position directly. Can cause clipping and getting into unwanted areas.
        // better to set the player's position.
        transform.position += new Vector3(movement, 0, 0) * Time.deltaTime * MovementSpeed;

        // Be wary of using velocity to see if player is grounded. 
        // Might cause bugs like players being able to jump at the peak of their jump or when they are sliding down a wall.
        // Ground checkers are usually more consistent and allow you to determine what is the "floor."
        if (Input.GetButtonDown("Jump") && Mathf.Abs(_rigidbody.velocity.y) < 0.001f) 
        {
            // For platformers I have personally found that it is better to stay away from forces. It feels more sluggish since
            // it takes a little bit for the players to get up to speed. 
            _rigidbody.AddForce(new Vector2(0, JumpHeight), ForceMode2D.Impulse);
        }
    }
}
