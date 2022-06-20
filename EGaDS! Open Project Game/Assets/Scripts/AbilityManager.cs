using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{

    private List<Ability> abilities = new List<Ability>();
    private PlayerComponents player;

    public Ability Focus { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        player = new PlayerComponents(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: call Update() of abilities/Focus
    }

    void FixedUpdate()
    {
        // TODO: call FixedUpdate() of abilities/Focus
    }

    public void AddAbility(Ability ability) {
        foreach (Ability a in abilities)
            if (ability.GetType() == a.GetType())
                return;
        abilities.Add(ability);
    }

    public void RemoveAbility(Ability ability) {
        for (int i = 0; i < abilities.Count; i++)
            if (ability.GetType() == abilities[i].GetType())
            {
                abilities.RemoveAt(i);
                return;
            }
    }

    public double GetSpeedMultiplier() => 
        abilities.Aggregate(1.0, (prod, ability) => prod * ability.GetSpeedMultiplier());

    public double GetJumpHeightMultiplier() => 
        abilities.Aggregate(1.0, (prod, ability) => prod * ability.GetJumpHeightMultiplier());

}
