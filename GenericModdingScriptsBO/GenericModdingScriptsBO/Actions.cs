using BrutalAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PYMN13
{
    public class CasterRootActionEffect : EffectSO
    {
        public Effect[] effects;
        public override bool PerformEffect(CombatStats stats, IUnit caster, TargetSlotInfo[] targets, bool areTargetSlots, int entryVariable, out int exitAmount)
        {
            EffectInfo[] effectInfoArray = ExtensionMethods.ToEffectInfoArray(this.effects);
            exitAmount = 0;
            CombatManager.Instance.AddRootAction(new EffectAction(effectInfoArray, caster, 0));
            return true;
        }
        public static CasterRootActionEffect Create(Effect[] e)
        {
            CasterRootActionEffect instance = CreateInstance<CasterRootActionEffect>();
            instance.effects = e;
            return instance;
        }
    }
    public class CasterSubActionEffect : EffectSO
    {
        public Effect[] effects;
        public override bool PerformEffect(CombatStats stats, IUnit caster, TargetSlotInfo[] targets, bool areTargetSlots, int entryVariable, out int exitAmount)
        {
            EffectInfo[] effectInfoArray = ExtensionMethods.ToEffectInfoArray(this.effects);
            exitAmount = 0;
            CombatManager.Instance.AddSubAction(new EffectAction(effectInfoArray, caster, 0));
            return true;
        }
        public static CasterSubActionEffect Create(Effect[] e)
        {
            CasterSubActionEffect instance = CreateInstance<CasterSubActionEffect>();
            instance.effects = e;
            return instance;
        }
    }
    public class RootActionEffect : EffectSO
    {
        public Effect[] effects;

        public override bool PerformEffect(CombatStats stats, IUnit caster, TargetSlotInfo[] targets, bool areTargetSlots, int entryVariable, out int exitAmount)
        {
            EffectInfo[] effectInfoArray = ExtensionMethods.ToEffectInfoArray(this.effects);
            exitAmount = 0;
            foreach (TargetSlotInfo target in targets)
            {
                if (target.HasUnit)
                {
                    CombatManager.Instance.AddRootAction(new EffectAction(effectInfoArray, target.Unit, 0));
                    ++exitAmount;
                }
            }
            return exitAmount > 0;
        }

        public static RootActionEffect Create(Effect[] e)
        {
            RootActionEffect instance = CreateInstance<RootActionEffect>();
            instance.effects = e;
            return instance;
        }
    }
    public class SubActionEffect : EffectSO
    {
        public Effect[] effects;

        public override bool PerformEffect(CombatStats stats, IUnit caster, TargetSlotInfo[] targets, bool areTargetSlots, int entryVariable, out int exitAmount)
        {
            EffectInfo[] effectInfoArray = ExtensionMethods.ToEffectInfoArray(this.effects);
            exitAmount = 0;
            foreach (TargetSlotInfo target in targets)
            {
                if (target.HasUnit)
                {
                    CombatManager.Instance.AddSubAction(new EffectAction(effectInfoArray, target.Unit, 0));
                    ++exitAmount;
                }
            }
            return exitAmount > 0;
        }

        public static SubActionEffect Create(Effect[] e)
        {
            SubActionEffect instance = CreateInstance<SubActionEffect>();
            instance.effects = e;
            return instance;
        }
    }
    public class RootActionAction : CombatAction
    {
        public CombatAction ex;
        public RootActionAction(CombatAction a) => ex = a;

        public override IEnumerator Execute(CombatStats stats)
        {
            CombatManager.Instance.AddRootAction(ex);
            yield return null;
        }
    }
    public class SubActionAction : CombatAction
    {
        public CombatAction ex;

        public SubActionAction(CombatAction a) => ex = a;

        public override IEnumerator Execute(CombatStats stats)
        {
            CombatManager.Instance.AddSubAction(ex);
            yield return null;
        }
    }
    public class CarryExitValueEffect : EffectSO
    {
        public override bool PerformEffect(CombatStats stats, IUnit caster, TargetSlotInfo[] targets, bool areTargetSlots, int entryVariable, out int exitAmount)
        {
            exitAmount = PreviousExitValue;
            return true;
        }
    }
}
