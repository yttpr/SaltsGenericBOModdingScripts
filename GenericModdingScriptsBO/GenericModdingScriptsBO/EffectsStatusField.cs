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
                            foreach (IFieldEffect field in new List<IFieldEffect>(slot.FieldEffects))
                            {
                                if (!field.IsPositive)
                                {
                                    int amount = slot.TryRemoveFieldEffect(field.FieldID);
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
                            foreach (IFieldEffect field in new List<IFieldEffect>(slot.FieldEffects))
                            {
                                if (!field.IsPositive)
                                {
                                    int amount = slot.TryRemoveFieldEffect(field.FieldID);
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
                            int amount = target.Unit.TryRemoveStatusEffect(status.StatusID);
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
            if (casterIncluded)
            {
                if (caster is IStatusEffector effector)
                {
                    foreach (IStatusEffect status in new List<IStatusEffect>(effector.StatusEffects))
                    {
                        if (status is StatusEffect_Holder holder)
                        {
                            if (caster.ApplyStatusEffect(holder._Status, holder.StatusContent + holder.Restrictor * 4))
                            {
                                if (holder.TryUseNumberOnPopUp) exitAmount += holder.StatusContent + holder.Restrictor * 4;
                                else exitAmount++;
                            }
                        }
                    }
                }
            }
            foreach (TargetSlotInfo target in targets)
            {
                if (target.HasUnit && target.Unit is IStatusEffector effector)
                {
                    foreach (IStatusEffect status in new List<IStatusEffect>(effector.StatusEffects))
                    {
                        if (status is StatusEffect_Holder holder)
                        {
                            if (caster.ApplyStatusEffect(holder._Status, holder.StatusContent + holder.Restrictor * 4))
                            {
                                if (holder.TryUseNumberOnPopUp) exitAmount += holder.StatusContent + holder.Restrictor * 4;
                                else exitAmount++;
                            }
                        }
                    }
                }
            }
            return exitAmount > 0;
        }
    }
    public class ReduceAllNegativeStatusEffect : EffectSO
    {
        [SerializeField]
        public List<string> Exclude = new List<string>();
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
                            if (!status.IsPositive && !Exclude.Contains(status.StatusID))
                            {
                                if (status.StatusContent > Math.Abs(entryVariable))
                                {
                                    int start = status.StatusContent;
                                    if (status.TryAddContent(entryVariable, 0))
                                    {
                                        effector.StatusEffectValuesChanged(status.StatusID, entryVariable);
                                        if (effector.IsStatusEffectorCharacter) CombatManager.Instance.AddUIAction(new CharacterStatusEffectPopupUIAction(targetSlotInfo.Unit.ID, status.EffectInfo, status.DisplayText, entryVariable, start > 0));
                                        else CombatManager.Instance.AddUIAction(new EnemyStatusEffectPopupUIAction(targetSlotInfo.Unit.ID, status.EffectInfo, status.DisplayText, entryVariable, start > 0));
                                        exitAmount += Math.Abs(entryVariable);
                                    }
                                }
                                else
                                {
                                    exitAmount += targetSlotInfo.Unit.TryRemoveStatusEffect(status.StatusID);
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
                            foreach (IFieldEffect field in new List<IFieldEffect>(slot.FieldEffects))
                            {
                                if (!field.IsPositive)
                                {
                                    //WHY IS THIS BUGGED
                                    field.TryAddContent(entryVariable, 0);
                                    if (field is FieldEffect_Holder holder && holder._Field.TryRemoveFieldEffect(holder)) exitAmount++;
                                    else
                                    {
                                        slot.FieldEffectValuesChanged(field.FieldID, useSpecialSound: false, entryVariable);
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
                            foreach (IFieldEffect field in new List<IFieldEffect>(slot.FieldEffects))
                            {
                                if (!field.IsPositive)
                                {
                                    field.TryAddContent(entryVariable, 0);
                                    if (field is FieldEffect_Holder holder && holder._Field.TryRemoveFieldEffect(holder)) exitAmount++;
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
