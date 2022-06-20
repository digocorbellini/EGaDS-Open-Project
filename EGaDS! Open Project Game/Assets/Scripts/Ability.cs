using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : ScriptableObject
{
    public virtual void Start(PlayerComponents player) {}

    public virtual void Update(PlayerComponents player) {}

    public virtual void FixedUpdate(PlayerComponents player) {}

    public virtual double GetSpeedMultiplier() => 1.0;
    public virtual double GetJumpHeightMultiplier() => 1.0;
    // TODO: add more multiplier modifier functions
}
