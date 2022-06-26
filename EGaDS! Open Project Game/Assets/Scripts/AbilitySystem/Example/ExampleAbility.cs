using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/ExampleAbility")]
public class ExampleAbility : TriggeredAbility
{
    [SerializeField]
    private float _riseHeight = 8.0f; // in tiles

    [SerializeField]
    private float _riseSpeed = 5.0f; // in tiles per second

    [SerializeField]
    private float _cooldownTime = 0.5f; // in seconds

    private bool _isRising;
    private bool _isInCooldown;
    private float _timer;
    private float _savedFallSpeedMultiplier;

    public override void AbilityStart(PlayerComponents player)
    {
        _isInCooldown = false;
        _isRising = false;
        _timer = -1;
    }

    public override void AbilityUpdate(PlayerComponents player) 
    {
        // check that ability isn't in cooldown
        if (!_isInCooldown && !_isRising && GetKeyDown() && player.abilityManager.AcquireFocus(this))
        {
            Debug.Log("start rising");
            _isRising = true;
            _savedFallSpeedMultiplier = FallSpeedMultiplier;
            FallSpeedMultiplier = 0;

            _timer = _riseHeight / _riseSpeed;
        }
        if (_isRising)
        {
            player.transform.position += new Vector3(0, _riseSpeed*Time.deltaTime);

            _timer -= Time.deltaTime;
            if (_timer < 0)
            {
                if (player.abilityManager.UnacquireFocus(this))
                {
                    Debug.Log("end rising");
                    _isRising = false;
                    FallSpeedMultiplier = _savedFallSpeedMultiplier;

                    // start cooldown
                    _isInCooldown = true;
                    _timer = _cooldownTime;
                }
            }
        }
        if (_isInCooldown)
        {
            // progress timer and end cooldown
            _timer -= Time.deltaTime;
            if (_timer < 0)
            {
                Debug.Log("end cooldown");
                _isInCooldown = false;
            }
        }
    }

    public override void AbilityEnd(PlayerComponents player)
    {
        if (_isRising)
        {
            // allow the player to start falling again
            FallSpeedMultiplier = _savedFallSpeedMultiplier;
            player.abilityManager.UnacquireFocus(this);
        }
    }
}
