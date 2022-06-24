using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Abilities/ExampleAbility")]
public class ExampleAbility : Ability
{
    [SerializeField]
    private float _cooldownTime = 1.0f;

    [SerializeField]
    private float _riseSpeed = 5.0f;

    private float _timer;
    private float _savedGravity;

    public override void AbilityStart(PlayerComponents player)
    {
        // Do not need to call base.AbilityStart(player) as base method has no logic implemented
        // (same is true for AbilityUpdate and AbilityFixedUpdate)

        // perform any setup logic for when the player first obtains the ability
        _timer = -1;
    }

    public override void AbilityUpdate(PlayerComponents player)
    {
        // Use GetKeyDown() to check if the player pressed the key corresponding to this ability
        if (_timer == -1 && GetKeyDown()) 
        {
            // set focus so no other ability can be activated while this one is
            if (player.abilityManager.AcquireFocus(this))
            {
                // if focus was successfully acquired:
                _timer = _cooldownTime;
                _savedGravity = player.rigidbody.gravityScale;
                player.rigidbody.gravityScale = 0;
                player.rigidbody.velocity = Vector2.zero;
            }
            
        }

        if (_timer > 0) 
        {
            // make player move up for 1 second until timer reaches 0
            player.transform.position += new Vector3(0, _riseSpeed*Time.deltaTime);
            _timer -= Time.deltaTime;
            if (_timer <= 0) 
            {
                // unset focus so other abilities can be activated again
                if (player.abilityManager.UnacquireFocus(this))
                {
                    // if focus was successfully unacquired:
                    _timer = -1;
                    player.rigidbody.gravityScale = _savedGravity;
                }
            }
        }
    }
}