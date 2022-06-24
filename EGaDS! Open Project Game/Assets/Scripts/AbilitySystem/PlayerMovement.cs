using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float MovementSpeed = 1;
    public float JumpHeight = 1;
    [SerializeField] private LayerMask groundLayerMask;

    private Rigidbody2D rigidbody;
    private BoxCollider2D collider;
    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
    }

    
    private void Update()
    {
        
        var movement = Input.GetAxisRaw("Horizontal");
        // be wary of setting the player's position directly. Can cause clipping and getting into unwanted areas.
        // better to set the player's position.
        transform.position += new Vector3(movement, 0, 0) * Time.deltaTime * MovementSpeed;

        bool isGrounded() 
        {
            RaycastHit2D raycastHit = Physics2D.Raycast(collider.bounds.center, Vector2.down, collider.bounds.extents.y*1.2f, groundLayerMask);

            
            if (raycastHit.collider == null)
            {
                return (false);
            }
            else
            {
                return (true);
            }
                
        }

        if (Input.GetButtonDown("Jump") && isGrounded()) 
        {
            // For platformers I have personally found that it is better to stay away from forces. It feels more sluggish since
            // it takes a little bit for the players to get up to speed. 
            rigidbody.AddForce(new Vector2(0, JumpHeight), ForceMode2D.Impulse);
        }
    }
}
