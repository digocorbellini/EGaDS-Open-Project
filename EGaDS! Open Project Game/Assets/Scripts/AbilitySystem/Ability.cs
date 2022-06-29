using UnityEngine;

public abstract class Ability : ScriptableObject
{
    private int _slotIndex = -1;

    [SerializeField] private float _speedMultiplier = 1.0f;
    [SerializeField] private float _fallSpeedMultiplier = 1.0f;
    [SerializeField] private float _wallSlideMultiplier = 1.0f;
    [SerializeField] private float _jumpHeightAddend = 0.0f;
    [SerializeField] private int _airJumpAddend = 0;

    /// <summary>
    /// The slot index this ability is held in.
    /// If equal to -1, then the ability is not in a slot
    /// </summary>
    public int SlotIndex => _slotIndex;

    /// <summary>
    /// Multiplier applied to player left-right movement speed, in tiles per second.
    /// </summary>
    public float SpeedMultiplier 
    { 
        get => _speedMultiplier;
        protected set { _speedMultiplier = value; }
    }

    /// <summary>
    /// Multiplier applied to the maximum speed the player falls at, in tiles per second.
    /// </summary>
    public float FallSpeedMultiplier
    { 
        get => _fallSpeedMultiplier;
        protected set { _fallSpeedMultiplier = value; }
    }

    /// <summary>
    /// Multiplier applied to the speed the player slides down while on a wall, in tiles per second.
    /// </summary>
    public float WallSlideMultiplier
    { 
        get => _wallSlideMultiplier;
        protected set { _wallSlideMultiplier = value; }
    }

    /// <summary>
    /// Number of tiles added to player jump height.
    /// </summary>
    public float JumpHeightAddend
    {
        get => _jumpHeightAddend;
        protected set { _jumpHeightAddend = value; }
    }

    /// <summary>
    /// Number of additional air jumps the player can do
    /// </summary>
    public int AirJumpAddend
    {
        get => _airJumpAddend;
        protected set { AirJumpAddend = value; }
    }

    /// <summary>
    /// USED ONLY IN ABILITY MANAGER
    /// Called to signal that the ability has been added to the player
    /// and the ability can now be usable.
    /// </summary>
    /// <param name="player">The components of the player the ability has been added to.</param>
    /// <param name="slotIndex">The slot index the ability has been added to.</param>
    public void SetAdded(PlayerComponents player, int slotIndex) 
    {
        _slotIndex = slotIndex;
        AbilityStart(player);
    }

    /// <summary>
    /// USED ONLY IN ABILITY MANAGER
    /// Called to signal that the ability has been removed from the player
    /// </summary>
    /// <param name="player">The components of the player the ability has been removed from.</param>
    public void SetRemoved(PlayerComponents player)
    {
        _slotIndex = -1;
        AbilityEnd(player);
    }

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

    /// <summary>
    /// Called when the player loses the ability
    /// </summary>
    /// <param name="player">The components held by the player</param>
    public virtual void AbilityEnd(PlayerComponents player) {}
}