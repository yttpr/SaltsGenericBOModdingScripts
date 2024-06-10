using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace PYMN13
{
    public class RemoveAllNegativeFieldEffect : EffectSO
    {
        public bool JustTypes;
        public override bool PerformEffect(CombatStats stats, IUnit caster, TargetSlotInfo[] targets, bool areTargetSlots, int entryVariable, out int exitAmount)
        {
            exitAmount = 0;
            foreach (TargetSlotInfo target in targets)
            {
                if (target.IsTargetCharacterSlot)
                {
                    foreach (CombatSlot slot in stats.combatSlots.CharacterSlots)
                    {
                        if (slot.SlotID == target.SlotID)
                        {
                            foreach (ISlotStatusEffect field in new List<ISlotStatusEffect>(slot.StatusEffects))
                            {
                                if (!field.IsPositive)
                                {
                                    int amount = slot.TryRemoveSlotStatusEffect(field.EffectType);
                                    exitAmount += JustTypes ? 1 : amount;
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (CombatSlot slot in stats.combatSlots.EnemySlots)
                    {
                        if (slot.SlotID == target.SlotID)
                        {
                            foreach (ISlotStatusEffect field in new List<ISlotStatusEffect>(slot.StatusEffects))
                            {
                                if (!field.IsPositive)
                                {
                                    int amount = slot.TryRemoveSlotStatusEffect(field.EffectType);
                                    exitAmount += JustTypes ? 1 : amount;
                                }
                            }
                        }
                    }
                }
            }
            return exitAmount > 0;
        }
    }
    public class RemoveAllNegativeStatusEffect : EffectSO
    {
        public bool JustTypes;
        public override bool PerformEffect(CombatStats stats, IUnit caster, TargetSlotInfo[] targets, bool areTargetSlots, int entryVariable, out int exitAmount)
        {
            exitAmount = 0;
            foreach (TargetSlotInfo target in targets)
            {
                if (target.HasUnit && target.Unit is IStatusEffector unit)
                {
                    foreach (IStatusEffect status in new List<IStatusEffect>(unit.StatusEffects))
                    {
                        if (!status.IsPositive)
                        {
                            int amount = target.Unit.TryRemoveStatusEffect(status.EffectType);
                            exitAmount += JustTypes ? 1 : amount;
                        }
                    }
                }
            }
            return exitAmount > 0;
        }
    }
    public class CopyStatusOntoCasterEffect : EffectSO
    {
        public override bool PerformEffect(CombatStats stats, IUnit caster, TargetSlotInfo[] targets, bool areTargetSlots, int entryVariable, out int exitAmount)
        {
            exitAmount = 0;
            bool casterIncluded = false;
            foreach (TargetSlotInfo target in targets)
            {
                if (target.Unit == caster) casterIncluded = true;
            }
            stats.statusEffectDataBase.TryGetValue(StatusEffectType.Spotlight, out var spot);
            bool ApplySpotlight = false;
            if (casterIncluded)
            {
                if (caster is IStatusEffector effector)
                {
                    List<IStatusEffect> statuses = new List<IStatusEffect>(effector.StatusEffects);
                    for (int i = 0; i < statuses.Count; i++)
                    {
                        IStatusEffect toCopy = statuses[i];
                        if (toCopy.EffectType == StatusEffectType.Spotlight)
                        {
                            ApplySpotlight = true;
                            continue;
                        }
                        ConstructorInfo[] constructors = toCopy.GetType().GetConstructors();
                        IStatusEffect letsGo = null;
                        foreach (ConstructorInfo build in constructors)
                        {
                            if (build.GetParameters().Length == 0)
                            {
                                letsGo = (IStatusEffect)Activator.CreateInstance(toCopy.GetType());
                            }
                            else if (build.GetParameters().Length == 1)
                            {
                                letsGo = (IStatusEffect)Activator.CreateInstance(toCopy.GetType(), 0);
                            }
                            else if (build.GetParameters().Length == 2)
                            {
                                letsGo = (IStatusEffect)Activator.CreateInstance(toCopy.GetType(), toCopy.StatusContent + (toCopy.Restrictor * 4), 0);
                            }
                        }
                        if (letsGo != null)
                        {
                            letsGo.SetEffectInformation(toCopy.EffectInfo);
                            bool hasNum = letsGo.DisplayText != "";
                            int amount = hasNum ? letsGo.StatusContent : 0;
                            if (caster.ApplyStatusEffect(letsGo, amount)) exitAmount += Math.Max(letsGo.StatusContent, 1);
                        }
                    }
                }
            }
            foreach (TargetSlotInfo target in targets)
            {
                if (target.HasUnit && target.Unit != caster && target.Unit is IStatusEffector effector)
                {
                    List<IStatusEffect> statuses = new List<IStatusEffect>(effector.StatusEffects);
                    for (int i = 0; i < statuses.Count; i++)
                    {
                        IStatusEffect toCopy = statuses[i];
                        if (toCopy.EffectType == StatusEffectType.Spotlight)
                        {
                            ApplySpotlight = true;
                            continue;
                        }
                        ConstructorInfo[] constructors = toCopy.GetType().GetConstructors();
                        IStatusEffect letsGo = null;
                        foreach (ConstructorInfo build in constructors)
                        {
                            if (build.GetParameters().Length == 0)
                            {
                                letsGo = (IStatusEffect)Activator.CreateInstance(toCopy.GetType());
                            }
                            else if (build.GetParameters().Length == 1)
                            {
                                letsGo = (IStatusEffect)Activator.CreateInstance(toCopy.GetType(), 0);
                            }
                            else if (build.GetParameters().Length == 2)
                            {
                                letsGo = (IStatusEffect)Activator.CreateInstance(toCopy.GetType(), toCopy.StatusContent + (toCopy.Restrictor * 4), 0);
                            }
                        }
                        if (letsGo != null)
                        {
                            letsGo.SetEffectInformation(toCopy.EffectInfo);
                            bool hasNum = letsGo.DisplayText != "";
                            int amount = hasNum ? letsGo.StatusContent : 0;
                            if (caster.ApplyStatusEffect(letsGo, amount)) exitAmount += Math.Max(letsGo.StatusContent, 1);
                        }
                    }
                }
            }
            if (ApplySpotlight)
            {
                Spotlight_StatusEffect spotlight_StatusEffect = new Spotlight_StatusEffect();
                spotlight_StatusEffect.SetEffectInformation(spot);
                if (caster.ApplyStatusEffect(spotlight_StatusEffect, 0))
                {
                    exitAmount++;
                }
            }
            return exitAmount > 0;
        }
    }
    public class ReduceAllNegativeStatusEffect : EffectSO
    {
        [SerializeField]
        public List<StatusEffectType> Exclude = new List<StatusEffectType>();
        public override bool PerformEffect(CombatStats stats, IUnit caster, TargetSlotInfo[] targets, bool areTargetSlots, int entryVariable, out int exitAmount)
        {
            exitAmount = 0;
            entryVariable = -1 * Math.Abs(entryVariable);
            foreach (TargetSlotInfo targetSlotInfo in targets)
            {
                if (targetSlotInfo.HasUnit)
                {
                    if (targetSlotInfo.Unit is IStatusEffector effector)
                    {
                        foreach (IStatusEffect status in new List<IStatusEffect>(effector.StatusEffects))
                        {
                            if (!status.IsPositive && !Exclude.Contains(status.EffectType))
                            {
                                if (status.StatusContent > Math.Abs(entryVariable))
                                {
                                    int start = status.StatusContent;
                                    if (status.TryAddContent(entryVariable))
                                    {
                                        effector.StatusEffectValuesChanged(status.EffectType, entryVariable);
                                        if (effector.IsStatusEffectorCharacter) CombatManager.Instance.AddUIAction(new CharacterStatusEffectPopupUIAction(targetSlotInfo.Unit.ID, status.EffectInfo, status.DisplayText, entryVariable, start > 0));
                                        else CombatManager.Instance.AddUIAction(new EnemyStatusEffectPopupUIAction(targetSlotInfo.Unit.ID, status.EffectInfo, status.DisplayText, entryVariable, start > 0));
                                        exitAmount += Math.Abs(entryVariable);
                                    }
                                }
                                else
                                {
                                    exitAmount += targetSlotInfo.Unit.TryRemoveStatusEffect(status.EffectType);
                                }
                            }
                        }
                    }
                }
            }
            return exitAmount > 0;
        }
    }
    public class PerformRandomEffectsAmongEffects : EffectSO
    {
        public Dictionary<string, string[]> List;
        public bool UsePreviousExitValueForNewEntry;
        public List<EffectSO> Effects;
        public static List<PerformRandomEffectsAmongEffects> Selves = new List<PerformRandomEffectsAmongEffects>();
        public bool Specific;
        public static void GO()
        {
            foreach (PerformRandomEffectsAmongEffects effect in Selves) effect.Actually();
        }
        public void Setup()
        {
            Selves.Add(this);
            Actually();
        }
        public void Actually()
        {
            if (List == null) return;
            if (Effects == null) Effects = new List<EffectSO>();
            Type[] types = EZExtensions.GetAllDerived(typeof(EffectSO));
            List<string> remove = new List<string>();
            foreach (string name in List.Keys)
            {
                bool skip = false;
                foreach (EffectSO e in Effects) if (e.GetType().Name == name) { skip = true; break; }
                if (skip)
                {
                    UnityEngine.Debug.LogWarning("already has " + name);
                    continue;
                }
                List<Type> test = new List<Type>();
                foreach (Type type in types)
                {
                    /*if (type.Name.Contains(name) || name.Contains(type.Name))
                    {
                        Debug.Log("found near : " + type.Name + " , " + type.Namespace); 
                    }*/
                    if (type.Name == name)
                    {
                        //Debug.Log(type.Namespace);
                        if (Specific)
                        {
                            if (List[name].Contains(type.Namespace)) test.Add(type);
                            if (List[name].Length <= 0)
                            {
                                test.Add(type);
                                break;
                            }
                        }
                        else
                        {
                            if (List[name].Contains("PrayerFool_BOMOD") || type.Namespace != "PrayerFool_BOMOD") test.Add(type);
                            if (List[name].Length <= 0) break;
                            else if (List[name].Contains(type.Namespace)) break;
                        }
                    }
                }
                if (test.Count > 0)
                {
                    Effects.Add(ScriptableObject.CreateInstance(test[test.Count - 1]) as EffectSO);
                    remove.Add(name);
                    Debug.Log("added effectSO " + name + " from " + test[test.Count - 1].Namespace);
                }
            }
            foreach (string g in remove) List.Remove(g);
        }
        public EffectSO GrabRand()
        {
            if (Effects == null || Effects.Count <= 0) return null;
            return Effects[UnityEngine.Random.Range(0, Effects.Count)];
        }
        public override bool PerformEffect(CombatStats stats, IUnit caster, TargetSlotInfo[] targets, bool areTargetSlots, int entryVariable, out int exitAmount)
        {
            exitAmount = 0;
            int effectsRan = 0;
            if (Effects == null || Effects.Count <= 0) return false;
            foreach (TargetSlotInfo target in targets)
            {
                for (int i = 0; i < entryVariable; i++)
                {
                    try
                    {
                        EffectSO run = GrabRand();
                        if (run != null)
                        {
                            if (run.PerformEffect(stats, caster, new TargetSlotInfo[] { target }, areTargetSlots, UsePreviousExitValueForNewEntry ? PreviousExitValue : 1, out int exi))
                                exitAmount += exi;
                            effectsRan++;
                        }
                    }
                    catch
                    {
                        Debug.LogError("of course its fucking this");
                    }
                }
            }
            return effectsRan > 0;
        }
    }
    public class ReduceAllNegativeFieldEffect : EffectSO
    {
        public override bool PerformEffect(CombatStats stats, IUnit caster, TargetSlotInfo[] targets, bool areTargetSlots, int entryVariable, out int exitAmount)
        {
            exitAmount = 0;
            entryVariable = -1 * Math.Abs(entryVariable);
            foreach (TargetSlotInfo target in targets)
            {
                if (target.IsTargetCharacterSlot)
                {
                    foreach (CombatSlot slot in stats.combatSlots.CharacterSlots)
                    {
                        if (slot.SlotID == target.SlotID)
                        {
                            foreach (ISlotStatusEffect field in new List<ISlotStatusEffect>(slot.StatusEffects))
                            {
                                if (!field.IsPositive)
                                {
                                    //WHY IS THIS BUGGED
                                    field.TryAddContent(entryVariable);
                                    if (field.TryRemoveSlotStatusEffect()) exitAmount++;
                                    else
                                    {
                                        slot.SlotStatusEffectValuesChanged(field.EffectType, useSpecialSound: false, entryVariable);
                                        //CombatManager.Instance.AddUIAction(new SlotStatusEffectAppliedUIAction(slot.SlotID, slot.IsCharacter, field.DisplayText, field.EffectInfo, entryVariable, true));
                                        exitAmount++;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (CombatSlot slot in stats.combatSlots.EnemySlots)
                    {
                        if (slot.SlotID == target.SlotID)
                        {
                            foreach (ISlotStatusEffect field in new List<ISlotStatusEffect>(slot.StatusEffects))
                            {
                                if (!field.IsPositive)
                                {
                                    field.TryAddContent(entryVariable);
                                    if (field.TryRemoveSlotStatusEffect()) exitAmount++;
                                    else
                                    {
                                        CombatManager.Instance.AddUIAction(new SlotStatusEffectAppliedUIAction(slot.SlotID, slot.IsCharacter, field.DisplayText, field.EffectInfo, entryVariable, true));
                                        exitAmount++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return exitAmount > 0;
        }
    }
    public class CharacterStatusEffectPopupUIAction : CombatAction
    {
        public int _id;

        public StatusEffectInfoSO _effectInfo;

        public string _effectText;

        public int _amount;

        public bool _asPositivity;

        public CharacterStatusEffectPopupUIAction(int id, StatusEffectInfoSO effectInfo, string effectText, int amount, bool asPositivity)
        {
            _id = id;
            _effectInfo = effectInfo;
            _effectText = effectText;
            _amount = amount;
            _asPositivity = asPositivity;
        }

        public override IEnumerator Execute(CombatStats stats)
        {
            CombatVisualizationController ui = stats.combatUI;
            if (ui._charactersInCombat.TryGetValue(_id, out var character))
            {
                Vector3 characterPosition = ui._characterZone.GetCharacterPosition(character.FieldID);
                ui.PlaySoundOnPosition(_effectInfo.UpdatedSoundEvent, characterPosition);
                yield return ui._popUpHandler3D.StartStatusFieldShowcase(isFieldText: false, characterPosition, (_asPositivity ? StatusFieldEffectPopUpType.Sign : StatusFieldEffectPopUpType.Number), _amount, _effectInfo.icon);
            }
        }
    }
    public class EnemyStatusEffectPopupUIAction : CombatAction
    {
        public int _id;

        public StatusEffectInfoSO _effectInfo;

        public string _effectText;

        public int _amount;

        public bool _asPositivity;

        public EnemyStatusEffectPopupUIAction(int id, StatusEffectInfoSO effectInfo, string effectText, int amount, bool asPositivity)
        {
            _id = id;
            _effectInfo = effectInfo;
            _effectText = effectText;
            _amount = amount;
            _asPositivity = asPositivity;
        }

        public override IEnumerator Execute(CombatStats stats)
        {
            CombatVisualizationController ui = stats.combatUI;
            if (ui._enemiesInCombat.TryGetValue(_id, out var enemy))
            {
                Vector3 enemyposition = ui._enemyZone.GetEnemyPosition(enemy.FieldID);
                ui.PlaySoundOnPosition(_effectInfo.UpdatedSoundEvent, enemyposition);
                yield return ui._popUpHandler3D.StartStatusFieldShowcase(isFieldText: false, enemyposition, (_asPositivity ? StatusFieldEffectPopUpType.Sign : StatusFieldEffectPopUpType.Number), _amount, _effectInfo.icon);
            }
        }
    }
}
