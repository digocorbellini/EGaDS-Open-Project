using UnityEngine;

public abstract class Ability : ScriptableObject
{
    private int _slotIndex = -1;

    /// <summary>
    /// The slot index this ability is held in.
    /// If equal to -1, then the ability is not in a slot
    /// </summary>
    public int SlotIndex => _slotIndex;

    /// <summary>
    /// Called to signal that the ability has been added to the player
    /// and the ability can now be usable.
    /// </summary>
    /// <param name="player">The components of the player the ability has been added to.</param>
    /// <param name="slotIndex">The slot index the ability has been added to.</param>
    public void AbilityStart(PlayerComponents player, int slotIndex) 
    {
        _slotIndex = slotIndex;
        AbilityStart(player);
    }

    /// <summary>
    /// Called to signal that the ability has been removed from the player
    /// </summary>
    public void End()
    {
        _slotIndex = -1;
    }

    /// <summary>
    /// Get key code corresponding to the slot this ability is in, 
    /// or KeyCode.None if not in a slot
    /// </summary>
    private KeyCode GetKeyCode() 
    {
        if (SlotIndex < 0) return KeyCode.None;
        if (IsPassive) return AbilityManager.PASSIVE_ABILITY_KEY_CODES[_slotIndex];
        else return AbilityManager.ABILITY_KEY_CODES[_slotIndex];
    }

    /// <summary>
    /// Returns true while the user 
    /// holds down the key corresponding to this ability.
    /// </summary>
    public bool GetKey() => Input.GetKey(GetKeyCode());

    /// <summary>
    /// Returns true during the frame the user 
    /// starts pressing down the key corresponding to this ability.
    /// </summary>
    public bool GetKeyDown() => Input.GetKeyDown(GetKeyCode());

    /// <summary>
    /// Returns true during the frame the user
    /// releases the key corresponding to this ability.
    /// </summary>
    public bool GetKeyUp() => Input.GetKeyUp(GetKeyCode());

    /// <summary>
    /// Abilities are grouped together depending on the value of IsPassive,
    /// with separate maximum number of abilities a player can hold per group.
    /// <para>Defaults to <c>false</c>.</para>
    /// </summary>
    public bool IsPassive = false;

    /// <summary>
    /// Multiplier applied to player left-right movement speed, in tiles per second.
    /// </summary>
    public double SpeedMultiplier = 1.0;

    /// <summary>
    /// Number of tiles added to player jump height.
    /// </summary>
    public double JumpHeightAddend = 0.0;

    /// <summary>
    /// Multiplier applied to the maximum speed the player falls at, in tiles per second.
    /// </summary>
    public double FallSpeedMultiplier = 1.0;

    /////////////////////////////////////////////////////////////////
    // Virtual methods start here

    /// <summary>
    /// Called when the player obtains the ability. 
    /// Once this is called, <c>Update()</c> and <c>FixedUpdate()</c> will begin to be called.
    /// </summary>
    /// <param name="player">The components held by the player</param>
    public virtual void AbilityStart(PlayerComponents player) {}

    /// <summary>
    /// Called every frame when the ability is held by the player.
    /// </summary>
    /// <param name="player">The components held by the player</param>
    public virtual void AbilityUpdate(PlayerComponents player) {}

    /// <summary>
    /// Called every fixed frame-rate frame defined by the physics system.
    /// Useful for performing operations that are required to be performed 
    /// at a fixed frequency not dependent on user frame-rate.
    /// </summary>
    /// <param name="player">The components held by the player</param>
    public virtual void AbilityFixedUpdate(PlayerComponents player) {}
}