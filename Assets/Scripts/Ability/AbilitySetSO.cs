using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ability Set", menuName = "Special Abilities / Ability Set", order = 1)]
public class AbilitySetSO : ScriptableObject
{
    public AbilitySO[] abilities;
}
