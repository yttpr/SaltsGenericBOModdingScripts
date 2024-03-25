using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace PYMN13
{
    public static class FieldEffectFixHook
    {
        public static bool ApplySlotStatusEffect(Func<CombatSlot, ISlotStatusEffect, int, bool> orig, CombatSlot self, ISlotStatusEffect statusEffect, int amount)
        {
            try
            {
                return orig(self, statusEffect, amount);
            }
            catch
            {
                Debug.LogWarning("field effect fix hook hello.");
            }
            bool flag = false;
            int index1 = -1;
            for (int index2 = 0; index2 < self.StatusEffects.Count; ++index2)
            {
                if (self.StatusEffects[index2].EffectType == statusEffect.EffectType && self.StatusEffects[index2].GetType() != statusEffect.GetType())
                {
                    flag = true;
                    index1 = index2;
                    break;
                }
            }
            if (flag)
            {
                foreach (MethodBase constructor in self.StatusEffects[index1].GetType().GetConstructors())
                {
                    ParameterInfo[] parameters = constructor.GetParameters();
                    if (parameters.Length == 4 && parameters[0].ParameterType == typeof(int) && parameters[1].ParameterType == typeof(int) && parameters[2].ParameterType == typeof(bool) && parameters[3].ParameterType == typeof(int))
                        statusEffect = (ISlotStatusEffect)Activator.CreateInstance(self.StatusEffects[index1].GetType(), self.SlotID, amount, self.IsCharacter, statusEffect.Restrictor);
                    else if (parameters.Length == 3 && parameters[0].ParameterType == typeof(int) && parameters[1].ParameterType == typeof(int) && parameters[2].ParameterType == typeof(int))
                        statusEffect = (ISlotStatusEffect)Activator.CreateInstance(self.StatusEffects[index1].GetType(), self.SlotID, amount, statusEffect.Restrictor);
                }
            }
            try
            {
                return orig(self, statusEffect, amount);
            }
            catch
            {
                Debug.LogError("super epic field effect compatibility failure!");
                return false;
            }
        }

        public static void Setup()
        {
            IDetour idetour = new Hook(typeof(CombatSlot).GetMethod(nameof(CombatSlot.ApplySlotStatusEffect), ~BindingFlags.Default), typeof(FieldEffectFixHook).GetMethod(nameof(ApplySlotStatusEffect), ~BindingFlags.Default));
        }
    }
    public static class StatusEffectFixHook
    {
        public static bool ApplyStatusEffect(Func<IUnit, IStatusEffect, int, bool> orig, IUnit self, IStatusEffect statusEffect, int amount)
        {
            try
            {
                return orig(self, statusEffect, amount);
            }
            catch
            {
                Debug.LogWarning("status effect fix hook hello.");
            }
            if (self is IStatusEffector effector)
            {
                bool hasItAlready = false;
                int thisIndex = 999;
                for (int i = 0; i < effector.StatusEffects.Count; i++)
                {
                    if (effector.StatusEffects[i].EffectType == statusEffect.EffectType)
                    {
                        thisIndex = i;
                        hasItAlready = true;
                    }
                }
                if (hasItAlready == true && statusEffect.GetType() != effector.StatusEffects[thisIndex].GetType())
                {
                    ConstructorInfo[] constructors = effector.StatusEffects[thisIndex].GetType().GetConstructors();
                    foreach (ConstructorInfo constructor in constructors)
                    {
                        if (constructor.GetParameters().Length == 2)
                        {
                            statusEffect = (IStatusEffect)Activator.CreateInstance(effector.StatusEffects[thisIndex].GetType(), statusEffect.StatusContent, statusEffect.Restrictor);
                        }
                        else if (constructor.GetParameters().Length == 1)
                        {
                            statusEffect = (IStatusEffect)Activator.CreateInstance(effector.StatusEffects[thisIndex].GetType(), statusEffect.Restrictor);
                        }
                        else if (constructor.GetParameters().Length == 0)
                        {
                            statusEffect = (IStatusEffect)Activator.CreateInstance(effector.StatusEffects[thisIndex].GetType());
                        }
                    }
                }
            }
            try
            {
                return orig(self, statusEffect, amount);
            }
            catch
            {
                Debug.LogError("epic status effect failure.");
                return false;
            }
        }
        public static void Setup()
        {
            IDetour idetour1 = new Hook(typeof(CharacterCombat).GetMethod(nameof(CharacterCombat.ApplyStatusEffect), ~BindingFlags.Default), typeof(StatusEffectFixHook).GetMethod(nameof(ApplyStatusEffect), ~BindingFlags.Default));
            IDetour idetour2 = new Hook(typeof(EnemyCombat).GetMethod(nameof(EnemyCombat.ApplyStatusEffect), ~BindingFlags.Default), typeof(StatusEffectFixHook).GetMethod(nameof(ApplyStatusEffect), ~BindingFlags.Default));
        }
    }
}
