using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    /// <summary>
    /// Holds the keycodes that activate each ability slot the player holds
    /// </summary>
    public static readonly KeyCode[] TRIGGERED_ABILITY_KEY_CODES = {
        KeyCode.H, KeyCode.J, KeyCode.K, KeyCode.L,
    };

    /// <summary>
    /// Holds the keycodes that are used to pick up and drop 
    /// each passive ability the player holds
    /// </summary>
    public static readonly KeyCode[] PASSIVE_ABILITY_KEY_CODES = {
        KeyCode.U, KeyCode.I, KeyCode.O, KeyCode.P,
    };

    private List<TriggeredAbility> _triggeredAbilities = new List<TriggeredAbility>();
    private List<PassiveAbility> _passiveAbilities = new List<PassiveAbility>();

    /// <summary>
    /// holds a reference to all player components (queried in <c>Start()</c>)
    /// </summary>
    private PlayerComponents _player;

    private Ability _focus;

    public IList<TriggeredAbility> TriggeredAbilities => _triggeredAbilities.AsReadOnly();
    public IList<PassiveAbility> PassiveAbilities => _passiveAbilities.AsReadOnly();

    /// <summary>
    /// The total multiplier applied to player left-right movement speed, in tiles per second
    /// </summary>
    public float SpeedMultiplier => 
        _triggeredAbilities.Aggregate(1.0f, (prod, ability) => prod * (ability?.SpeedMultiplier ?? 1)) *
        _passiveAbilities.Aggregate(1.0f, (prod, ability) => prod * (ability?.SpeedMultiplier ?? 1));

    /// <summary>
    /// The total multiplier applied to player max fall speed, in tiles per second
    /// </summary>
    public float FallSpeedMultiplier => 
        _triggeredAbilities.Aggregate(1.0f, (prod, ability) => prod * (ability?.FallSpeedMultiplier) ?? 1) *
        _passiveAbilities.Aggregate(1.0f, (prod, ability) => prod * (ability?.FallSpeedMultiplier ?? 1));

    /// <summary>
    /// The total multiplier applied to player wall slide speed, in tiles per second
    /// </summary>
    public float WallSlideMultiplier => 
        _triggeredAbilities.Aggregate(1.0f, (prod, ability) => prod * (ability?.WallSlideMultiplier) ?? 1) *
        _passiveAbilities.Aggregate(1.0f, (prod, ability) => prod * (ability?.WallSlideMultiplier ?? 1));

    /// <summary>
    /// The number of tiles added to player jump height
    /// </summary>
    public float JumpHeightAddend => 
        _triggeredAbilities.Aggregate(0.0f, (sum, ability) => sum + (ability?.JumpHeightAddend ?? 0)) +
        _passiveAbilities.Aggregate(0.0f, (sum, ability) => sum + (ability?.JumpHeightAddend ?? 0));

    /// <summary>
    /// The number of tiles added to player jump height
    /// </summary>
    public int AirJumpAddend => 
        _triggeredAbilities.Aggregate(0, (sum, ability) => sum + (ability?.AirJumpAddend ?? 0)) +
        _passiveAbilities.Aggregate(0, (sum, ability) => sum + (ability?.AirJumpAddend ?? 0));


    /// <summary>
    /// Acquire the focus on the passed in ability. When this is set, calls to 
    /// AbilityUpdate() and AbilityFixedUpdate() of all other abilities will be blocked.
    /// </summary>
    /// <param name="ability">The ability that is acquiring the focus</param>
    /// <returns>True if the ability successfuly acquired the focus and 
    /// false otherwise.</returns>
    public bool AcquireFocus(Ability ability)
    {
        bool focusAcquired = true;
        if (_focus == null)
            _focus = ability;
        else
            focusAcquired = false;

        return focusAcquired;
    }

    /// <summary>
    /// Unacquire the focus on the passed in ability. All abilities will be able to have their
    /// AbilityUpdate() and AbilityFixedUpdate() methods called by the ability manager.
    /// </summary>
    /// <param name="ability">The ability that is unacquiring the focus</param>
    /// <returns>True if the ability successfuly unacquired the focus and 
    /// false otherwise.</returns>
    public bool UnacquireFocus(Ability ability)
    {
        bool focusUnacquired = true;
        if (_focus == ability)
            _focus = null;
        else
            focusUnacquired = false;

        return focusUnacquired;
    }


    // Start is called before the first frame update
    void Start()
    {
        _player = new PlayerComponents(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (_focus == null) // if no focus, then call Update() of all abilities
        {
            foreach (Ability ability in _triggeredAbilities)
                ability?.AbilityUpdate(_player);

            foreach (Ability ability in _passiveAbilities)
                ability?.AbilityUpdate(_player);
        }
        else // otherwise only call Update() of focus
            _focus.AbilityUpdate(_player);
    }

    // called once every fixed frame-rate frame defined by physics engine
    void FixedUpdate()
    {
        if (_focus == null) // if no focus, then call FixedUpdate() of all abilities
        {
            foreach (Ability ability in _triggeredAbilities)
                ability?.AbilityFixedUpdate(_player);

            foreach (Ability ability in _passiveAbilities)
                ability?.AbilityFixedUpdate(_player);
        }
        else // otherwise only call FixedUpdate() of focus
            _focus.AbilityFixedUpdate(_player);
    }


    private bool AddAbility<T>(List<T> abilities, T ability, int slotIndex) where T : Ability 
    {
        // check if ability is null
        if (ability == null) return false;

        // check if slotIndex is out of bounds
        if (slotIndex < 0 || slotIndex >= abilities.Count) return false;

        // check if slotIndex is already occupied
        if (abilities[slotIndex] != null) return false;

        // check if player already has the triggered ability
        if (ability is TriggeredAbility)
            foreach (Ability a in abilities)
                if (ability.GetType() == a?.GetType())
                    return false;

        ability.SetAdded(_player, slotIndex);
        abilities[slotIndex] = ability;
        return true;
    }

    /// <summary>
    ///     Adds the ability to the player at the slot index
    /// </summary>
    /// <param name="ability">The ability to add to the player</param>
    /// <param name="slotIndex">The slot to add the ability in</param>
    /// <returns>True if the ability was successfully added</returns>
    public bool AddAbility(Ability ability, int slotIndex)
    {
        if (ability is TriggeredAbility) return AddAbility(_triggeredAbilities, ability as TriggeredAbility, slotIndex);
        else if (ability is PassiveAbility) return AddAbility(_passiveAbilities, ability as PassiveAbility, slotIndex);
        return false;
    }


    /// <summary>
    ///     Remove the ability at the given slot
    /// </summary>
    /// <param name="abilities">The list of abilities to remove from.</param>
    /// <param name="slotIndex">The slot to remove from</param>
    /// <returns>The ability that was removed, or null if no ability was removed</returns>
    private T RemoveAbility<T>(List<T> abilities, int slotIndex) where T : Ability
    {
        // check if slotIndex is out of bounds
        if (slotIndex < 0 || slotIndex >= abilities.Count) return null;

        T ability = abilities[slotIndex];
        ability?.SetRemoved(_player);
        abilities[slotIndex] = null;
        return ability;
    }

    /// <summary>Alias of <c>RemoveAbility(false, slotIndex)</c></summary>
    public TriggeredAbility RemoveTriggeredAbility(int slotIndex) => RemoveAbility(_triggeredAbilities, slotIndex);

    /// <summary>Alias of <c>RemoveAbility(true, slotIndex)</c></summary>
    public PassiveAbility RemovePassiveAbility(int slotIndex) => RemoveAbility(_passiveAbilities, slotIndex);


    /// <summary>
    /// Set the size of the abilities list
    /// </summary>
    /// <param name="abilities">The list of abilities to update</param>
    /// <param name="count">The size to update the list to</param>
    /// <returns></returns>
    private List<T> SetAbilityCount<T>(List<T> abilities, int count) where T : Ability
    {
        if (count < 0) count = 0;

        List<T> removed = new List<T>();

        if (count < abilities.Count) // remove abilities
        {
            while (abilities.Count > count)
            {
                T ability = abilities.Last();
                ability?.SetRemoved(_player);
                if (ability != null) removed.Insert(0, ability);
                abilities.RemoveAt(abilities.Count-1);
            }
        }
        else if (count > abilities.Count) // add empty ability slots
        {
            abilities.AddRange(new List<T>(new T[count - abilities.Count]));
        }
        return removed;
    }

    /// <summary>Alias of <c>SetAbilityCount(false, count)</c></summary>
    public List<TriggeredAbility> SetTriggeredAbilityCount(int count) => SetAbilityCount(_triggeredAbilities, count);

    /// <summary>Alias of <c>SetAbilityCount(true, count)</c></summary>
    public List<PassiveAbility> SetPassiveAbilityCount(int count) => SetAbilityCount(_passiveAbilities, count);
}
