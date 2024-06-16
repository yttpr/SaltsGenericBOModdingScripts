using BrutalAPI;
using UnityEngine;

namespace PYMN13
{
    public static class Passiver
    {
        public static FleetingPassiveAbility Fleeting(int amount)
        {
            FleetingPassiveAbility flee = Object.Instantiate(Passives.Fleeting3 as FleetingPassiveAbility);
            flee._passiveName = "Fleeting (" + amount.ToString() + ")";
            flee._characterDescription = "After " + amount.ToString() + " rounds this party member will flee... Coward.";
            flee._enemyDescription = "After " + amount.ToString() + " rounds this enemy will flee.";
            flee._turnsBeforeFleeting = amount;
            return flee;
        }
        public static BasePassiveAbilitySO Overexert(int amount)
        {
            IntegerReferenceOverEqualValueEffectorCondition con = ScriptableObject.CreateInstance<IntegerReferenceOverEqualValueEffectorCondition>();
            con.compareValue = amount;
            BasePassiveAbilitySO over = Object.Instantiate(LoadedAssetsHandler.GetEnemy("Scrungie_EN").passiveAbilities[2]);
            over._passiveName = "Overexert (" + amount.ToString() + ")";
            over._characterDescription = "Won't work with this version.";
            over._enemyDescription = "Upon receiving " + amount.ToString() + " or more direct damage, cancel 1 of this enemy's actions.";
            over.conditions = new EffectorConditionSO[] { con };
            return over;
        }
        public static PerformEffectPassiveAbility Leaky(int amount)
        {
            PerformEffectPassiveAbility leaky = ScriptableObject.CreateInstance<PerformEffectPassiveAbility>();
            leaky._passiveName = "Leaky (" + amount.ToString() + ")";
            leaky.passiveIcon = Passives.Leaky1.passiveIcon;
            leaky._enemyDescription = "Upon receiving direct damage, this enemy generates " + amount.ToString() + " extra pigment of its health color.";
            leaky._characterDescription = "Upon receiving direct damage, this character generates " + amount.ToString() + " extra pigment of its health color.";
            leaky.m_PassiveID = PassiveType_GameIDs.Leaky.ToString();
            leaky.doesPassiveTriggerInformationPanel = true;
            leaky._triggerOn = new TriggerCalls[] { TriggerCalls.OnDirectDamaged };
            leaky.effects = new EffectInfo[]
            {
                Effects.GenerateEffect(ScriptableObject.CreateInstance<GenerateCasterHealthManaEffect>(), amount, null)
            };
            return leaky;
        }
        public static BasePassiveAbilitySO Multiattack(int amount, bool fool = false)
        {
            if (!fool)
            {
                IntegerSetterPassiveAbility setterPassiveAbility = Object.Instantiate<IntegerSetterPassiveAbility>(Passives.Multiattack as IntegerSetterPassiveAbility);
                setterPassiveAbility._passiveName = "Multi Attack (" + amount.ToString() + ")";
                setterPassiveAbility._characterDescription = "won't work boowomp";
                setterPassiveAbility._enemyDescription = "This enemy will perform " + amount.ToString() + " actions each turn.";
                setterPassiveAbility.integerValue = amount - 1;
                return setterPassiveAbility;
            }
            PerformDoubleEffectPassiveAbility instance1 = ScriptableObject.CreateInstance<PerformDoubleEffectPassiveAbility>();
            ((BasePassiveAbilitySO)instance1)._passiveName = "MultiAttack (" + amount.ToString() + ")";
            ((BasePassiveAbilitySO)instance1).passiveIcon = Passives.Multiattack.passiveIcon;
            ((BasePassiveAbilitySO)instance1).type = (PassiveAbilityTypes)13;
            ((BasePassiveAbilitySO)instance1)._enemyDescription = "This shouldn't be on an enemy.";
            ((BasePassiveAbilitySO)instance1)._characterDescription = "This party member can perform " + amount.ToString() + " abilities per turn.";
            ((BasePassiveAbilitySO)instance1).specialStoredValue = (UnitStoredValueNames)77889;
            CasterSetStoredValueEffect instance2 = ScriptableObject.CreateInstance<CasterSetStoredValueEffect>();
            instance2._valueName = (UnitStoredValueNames)77889;
            ((BasePassiveAbilitySO)instance1)._triggerOn = new TriggerCalls[1]
            {
        (TriggerCalls) 21
            };
            instance1.effects = ExtensionMethods.ToEffectInfoArray(new Effect[1]
            {
        new Effect((EffectSO) instance2, amount - 1, new IntentType?(), Slots.Self)
            });
            RefreshIfStoredValueNotZero instance3 = ScriptableObject.CreateInstance<RefreshIfStoredValueNotZero>();
            instance3._valueName = (UnitStoredValueNames)77889;
            ScriptableObject.CreateInstance<CasterLowerStoredValueEffect>()._valueName = (UnitStoredValueNames)77889;
            instance1._secondTriggerOn = new TriggerCalls[1]
            {
        (TriggerCalls) 14
            };
            instance1._secondEffects = ExtensionMethods.ToEffectInfoArray(new Effect[1]
            {
        new Effect((EffectSO) instance3, 1, new IntentType?(), Slots.Self)
            });
            ((BasePassiveAbilitySO)instance1).doesPassiveTriggerInformationPanel = false;
            instance1._secondDoesPerformPopUp = false;
            return (BasePassiveAbilitySO)instance1;
        }
        public static PerformEffectPassiveAbility Inferno(int amount)
        {
            PerformEffectPassiveAbility instance = ScriptableObject.CreateInstance<PerformEffectPassiveAbility>();
            instance._passiveName = "Inferno (" + amount.ToString() + ")";
            instance.passiveIcon = Passives.Inferno.passiveIcon;
            instance._enemyDescription = "On turn start, this enemy inflicts " + amount.ToString() + " Fire on their position.";
            instance._characterDescription = "On turn start, this character inflicts " + amount.ToString() + " Fire on their position.";
            instance.type = PassiveAbilityTypes.Inferno;
            instance.doesPassiveTriggerInformationPanel = true;
            instance._triggerOn = new TriggerCalls[] { TriggerCalls.OnTurnStart };
            instance.effects = ExtensionMethods.ToEffectInfoArray(new Effect[1]
            {
                new Effect(ScriptableObject.CreateInstance<ApplyFireSlotEffect>(), amount, new IntentType?(), Slots.Self)
            });
            return instance;
        }
        public static BasePassiveAbilitySO Abomination => LoadedAssetsHandler.GetEnemy("OneManBand_EN").passiveAbilities[1];
    }
    public class RefreshIfStoredValueNotZero : EffectSO
    {
        [SerializeField]
        public bool _doesExhaustInstead;
        [SerializeField]
        public UnitStoredValueNames _valueName = (UnitStoredValueNames)2;

        public override bool PerformEffect(
          CombatStats stats,
          IUnit caster,
          TargetSlotInfo[] targets,
          bool areTargetSlots,
          int entryVariable,
          out int exitAmount)
        {
            exitAmount = 0;
            if (caster.GetStoredValue(this._valueName) != 0)
            {
                for (int index = 0; index < targets.Length; ++index)
                {
                    if (targets[index].HasUnit && (this._doesExhaustInstead ? targets[index].Unit.ExhaustAbilityUse() : targets[index].Unit.RefreshAbilityUse()))
                    {
                        ++exitAmount;
                        int num = caster.GetStoredValue(this._valueName) - entryVariable;
                        if (num < 0)
                            num = 0;
                        caster.SetStoredValue(this._valueName, num);
                    }
                }
            }
            return exitAmount > 0;
        }
    }
    public class CasterLowerStoredValueEffect : EffectSO
    {
        [SerializeField]
        public UnitStoredValueNames _valueName = (UnitStoredValueNames)2;

        public override bool PerformEffect(
          CombatStats stats,
          IUnit caster,
          TargetSlotInfo[] targets,
          bool areTargetSlots,
          int entryVariable,
          out int exitAmount)
        {
            exitAmount = 0;
            int num = caster.GetStoredValue(this._valueName) - entryVariable;
            if (num < 0)
                num = 0;
            caster.SetStoredValue(this._valueName, num);
            return exitAmount > 0;
        }
    }
}
