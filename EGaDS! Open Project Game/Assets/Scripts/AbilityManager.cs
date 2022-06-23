using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    /// <summary>
    /// Holds the keycodes that activate each ability slot the player holds
    /// </summary>
    public static readonly KeyCode[] ABILITY_KEY_CODES = {
        KeyCode.H, KeyCode.J, KeyCode.K, KeyCode.L,
    };

    /// <summary>
    /// Holds the keycodes that are used to pick up and drop 
    /// each passive ability the player holds
    /// </summary>
    public static readonly KeyCode[] PASSIVE_ABILITY_KEY_CODES = {
        KeyCode.U, KeyCode.I, KeyCode.O, KeyCode.P,
    };

    private List<Ability> _abilities = new List<Ability>();
    private List<Ability> _passiveAbilities = new List<Ability>();

    /// <summary>
    /// holds a reference to all player components (queried in <c>Start()</c>)
    /// </summary>
    private PlayerComponents player;

    private Ability _focus;

    /// <summary>
    /// The ability to focus on. When this is set, calls to Update() and 
    /// FixedUpdate() of all other abilities will be blocked.
    /// </summary>
    public Ability Focus 
    { 
        get => _focus; 
        // don't allow focus to change when there is currently a focus
        // in order to prevent unexpected behavior with abilities
        set { if (_focus == null) _focus = value; }
    }


    // Start is called before the first frame update
    void Start()
    {
        player = new PlayerComponents(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (Focus == null) // if no focus, then call Update() of all abilities
            foreach (Ability ability in _abilities)
                ability?.Update(player);

        else // otherwise only call Update() of focus
            Focus.Update(player);
    }

    // called once every fixed frame-rate frame defined by physics engine
    void FixedUpdate()
    {
        if (Focus == null) // if no focus, then call FixedUpdate() of all abilities
            foreach (Ability ability in _abilities)
                ability?.FixedUpdate(player);

        else // otherwise only call FixedUpdate() of focus
            Focus.FixedUpdate(player);
    }


    /// <summary>
    ///     Adds the ability to the player at the slot index
    /// </summary>
    /// <param name="ability">The ability to add to the player</param>
    /// <param name="slotIndex">The slot to add the ability in</param>
    /// <returns>True if the ability was successfully added</returns>
    public bool AddAbility(Ability ability, int slotIndex) 
    {
        // determine which list to add to
        List<Ability> abilities = ability.IsPassive ? _passiveAbilities : _abilities;

        // check if ability is null
        if (ability == null) return false;

        // check if slotIndex is out of bounds
        if (slotIndex < 0 || slotIndex >= abilities.Count) return false;

        // check if slotIndex is already occupied
        if (abilities[slotIndex] != null) return false;

        // check if player already has the ability
        foreach (Ability a in abilities)
            if (ability.GetType() == a.GetType())
                return false;

        ability.Start(player, slotIndex);
        abilities.Add(ability);
        return true;
    }


    /// <summary>
    ///     Remove the ability at the given slot
    /// </summary>
    /// <param name="passive">
    ///     If true, then remove from the passive abilities list.
    ///     Otherwise remove from the normal abilities list.
    /// </param>
    /// <param name="slotIndex">The slot to remove from</param>
    /// <returns>The ability that was removed, or null if no ability was removed</returns>
    public Ability RemoveAbility(bool passive, int slotIndex) 
    {
        // determine which list to remove from
        List<Ability> abilities = passive ? _passiveAbilities : _abilities;

        // check if slotIndex is out of bounds
        if (slotIndex < 0 || slotIndex >= abilities.Count) return null;

        Ability ability = abilities[slotIndex];
        ability?.End();
        abilities[slotIndex] = null;
        return ability;
    }

    /// <summary>Alias of <c>RemoveAbility(false, slotIndex)</c></summary>
    public Ability RemoveAbility(int slotIndex) => RemoveAbility(false, slotIndex);

    /// <summary>Alias of <c>RemoveAbility(true, slotIndex)</c></summary>
    public Ability RemovePassiveAbility(int slotIndex) => RemoveAbility(true, slotIndex);


    /// <summary>
    /// Set the size of the abilities list
    /// </summary>
    /// <param name="passive">
    ///     If true, then update the passive abilities list.
    ///     Otherwise update the normal abilities list.
    /// </param>
    /// <param name="count">The size to update the list to</param>
    /// <returns></returns>
    public List<Ability> SetAbilityCount(bool passive, int count)
    {
        // determine which list to update
        List<Ability> abilities = passive ? _passiveAbilities : _abilities;

        if (count < abilities.Count) // remove abilities
        {
            List<Ability> removed = abilities.GetRange(count, abilities.Count - count);
            foreach (Ability ability in removed) ability?.End();
            abilities.RemoveRange(count, abilities.Count - count);
            return removed;
        }
        else if (count > abilities.Count) // add empty ability slots
        {
            abilities.AddRange(new List<Ability>(new Ability[abilities.Count - count]));
            return new List<Ability>();
        }
        return new List<Ability>();
    }

    /// <summary>Alias of <c>SetAbilityCount(false, count)</c></summary>
    public List<Ability> SetAbilityCount(int count) => SetAbilityCount(false, count);

    /// <summary>Alias of <c>SetAbilityCount(true, count)</c></summary>
    public List<Ability> SetPassiveAbilityCount(int count) => SetAbilityCount(true, count);


    /// <summary>
    /// Get the total multiplier applied to player left-right movement speed, in tiles per second
    /// </summary>
    public double GetSpeedMultiplier() => 
        _abilities.Aggregate(1.0, (prod, ability) => prod * ability.SpeedMultiplier) *
        _passiveAbilities.Aggregate(1.0, (prod, ability) => prod * ability.SpeedMultiplier);

    /// <summary>
    /// Get the number of tiles added to player jump height
    /// </summary>
    public double GetJumpHeightAddend() => 
        _abilities.Aggregate(0.0, (sum, ability) => sum + ability.JumpHeightAddend) +
        _passiveAbilities.Aggregate(0.0, (sum, ability) => sum + ability.JumpHeightAddend);

    /// <summary>
    /// Get the total multiplier applied to player max fall speed, in tiles per second
    /// </summary>
    public double FallSpeedMultiplier() => 
        _abilities.Aggregate(1.0, (prod, ability) => prod * ability.FallSpeedMultiplier) *
        _passiveAbilities.Aggregate(1.0, (prod, ability) => prod * ability.FallSpeedMultiplier);

}
