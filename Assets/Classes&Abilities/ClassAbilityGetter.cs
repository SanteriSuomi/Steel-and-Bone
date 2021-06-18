using UnityEngine;

public class ClassAbilityGetter : Singleton<ClassAbilityGetter>
{
    [SerializeField]
    private WarriorClass warriorClass = default;
    [SerializeField]
    private WarriorAbility warriorAbility = default;
    public (WarriorClass, WarriorAbility) Warrior => (warriorClass, warriorAbility);

    [SerializeField]
    private RogueClass rogueClass = default;
    [SerializeField]
    private RogueAbility rogueAbility = default;
    public (RogueClass, RogueAbility) Rogue => (rogueClass, rogueAbility);

    [SerializeField]
    private RangedClass rangedClass = default;
    [SerializeField]
    private RangedAbility rangedAbility = default;
    public (RangedClass, RangedAbility) Ranged => (rangedClass, rangedAbility);
}