using BrutalAPI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PYMN13
{
    public static class StoredValueExtensions
    {
        public static int GetStoredValue(this IUnit unit, string value)
        {
            UnitStoreData_BasicSO unitStoredData = null;
            unitStoredData = LoadedDBsHandler.MiscDB.GetUnitStoreData(value);
            if (unitStoredData == null) return 0;
            if (unit.TryGetStoredData(unitStoredData, out var holder, false)) return holder.m_MainData;
            return 0;
        }

        public static void SetStoredValue(this IUnit unit, string value, int amount)
        {
            UnitStoreData_BasicSO unitStoredData = null;
            unitStoredData = LoadedDBsHandler.MiscDB.GetUnitStoreData(value);
            if (unitStoredData == null) return;
            unit.TryGetStoredData(unitStoredData, out var holder);
            holder.m_MainData = amount;
        }
    }
    public class CasterSetStoredValueEffect : EffectSO
    {
        public string _Value = "";
        public override bool PerformEffect(CombatStats stats, IUnit caster, TargetSlotInfo[] targets, bool areTargetSlots, int entryVariable, out int exitAmount)
        {
            exitAmount = entryVariable;
            caster.SetStoredValue(_Value, entryVariable);
            return true;
        }
    }
    public class TargetSetStoredValueEffect : EffectSO
    {
        public string _Value = "";
        public override bool PerformEffect(CombatStats stats, IUnit caster, TargetSlotInfo[] targets, bool areTargetSlots, int entryVariable, out int exitAmount)
        {
            exitAmount = 0;
            foreach (TargetSlotInfo target in targets)
            {
                if (target.HasUnit)
                {
                    target.Unit.SetStoredValue(_Value, entryVariable);
                    exitAmount += entryVariable;
                }
            }
            return exitAmount > 0;
        }
    }
    public class TargetChangeStoredValueEffect : EffectSO
    {
        [SerializeField]
        public string _valueName;
        [SerializeField]
        public bool Increase;
        [SerializeField]
        public int Minimum = 0;
        public override bool PerformEffect(CombatStats stats, IUnit caster, TargetSlotInfo[] targets, bool areTargetSlots, int entryVariable, out int exitAmount)
        {
            exitAmount = 0;
            foreach (TargetSlotInfo target in targets)
            {
                if (target.HasUnit)
                {
                    int orig = target.Unit.GetStoredValue(_valueName);
                    int set = Math.Max(Minimum, orig + (Increase ? entryVariable : (entryVariable * -1)));
                    caster.SetStoredValue(_valueName, set);
                    exitAmount += Math.Abs(orig - set);
                }
            }
            return exitAmount > 0;
        }
    }
}
