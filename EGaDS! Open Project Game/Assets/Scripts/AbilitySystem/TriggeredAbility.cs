using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TriggeredAbility : Ability
{
    /// <summary>
    /// Get key code corresponding to the slot this ability is in, 
    /// or KeyCode.None if not in a slot
    /// </summary>
    protected KeyCode GetKeyCode() 
    {
        if (SlotIndex < 0 || SlotIndex > AbilityManager.TRIGGERED_ABILITY_KEY_CODES.Length) 
            return KeyCode.None;
        else return AbilityManager.TRIGGERED_ABILITY_KEY_CODES[SlotIndex];
    }

    /// <summary>
    /// Returns true while the user 
    /// holds down the key corresponding to this ability.
    /// </summary>
    protected bool GetKey() => Input.GetKey(GetKeyCode());

    /// <summary>
    /// Returns true during the frame the user 
    /// starts pressing down the key corresponding to this ability.
    /// </summary>
    protected bool GetKeyDown() => Input.GetKeyDown(GetKeyCode());

    /// <summary>
    /// Returns true during the frame the user
    /// releases the key corresponding to this ability.
    /// </summary>
    protected bool GetKeyUp() => Input.GetKeyUp(GetKeyCode());
}
