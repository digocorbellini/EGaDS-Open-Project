using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is used to add an ability to the player it is attached to
// in an actual case, it is ideal to create a collectable that adds the ability
// to the player when the player gets close to it
public class AbilityAdder : MonoBehaviour
{
    [SerializeField]
    private Ability ability; // the ability to add

    // Start is called before the first frame update
    void Start()
    {
        AbilityManager abilityManager = gameObject.GetComponent<AbilityManager>();
        abilityManager.SetTriggeredAbilityCount(1); // ability manager can now hold at most 1 ability
        abilityManager.AddAbility(ability, 0);
    }
}
