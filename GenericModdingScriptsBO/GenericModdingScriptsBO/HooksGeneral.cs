using MonoMod.RuntimeDetour;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace PYMN13
{
    public static class HooksGeneral
    {
        public static void Setup()
        {
            IDetour idetour1 = new Hook(typeof(CharacterCombat).GetMethod(nameof(CharacterCombat.Damage), ~BindingFlags.Default), typeof(HooksGeneral).GetMethod(nameof(Damage), ~BindingFlags.Default));
            IDetour idetour2 = new Hook(typeof(EnemyCombat).GetMethod(nameof(EnemyCombat.Damage), ~BindingFlags.Default), typeof(HooksGeneral).GetMethod(nameof(Damage), ~BindingFlags.Default));
            IDetour idetour3 = new Hook(typeof(CharacterCombat).GetMethod(nameof(CharacterCombat.WillApplyDamage), ~BindingFlags.Default), typeof(HooksGeneral).GetMethod(nameof(WillApplyDamage), ~BindingFlags.Default));
            IDetour idetour4 = new Hook(typeof(EnemyCombat).GetMethod(nameof(EnemyCombat.WillApplyDamage), ~BindingFlags.Default), typeof(HooksGeneral).GetMethod(nameof(WillApplyDamage), ~BindingFlags.Default));
            IDetour idetour5 = new Hook(typeof(MainMenuController).GetMethod(nameof(MainMenuController.Start), ~BindingFlags.Default), typeof(HooksGeneral).GetMethod(nameof(StartMenu), ~BindingFlags.Default));
            IDetour idetour6 = new Hook(typeof(CombatManager).GetMethod(nameof(CombatManager.InitializeCombat), ~BindingFlags.Default), typeof(HooksGeneral).GetMethod(nameof(InitializeCombat), ~BindingFlags.Default));
            IDetour idetour7 = new Hook(typeof(CombatStats).GetMethod(nameof(CombatStats.PlayerTurnStart), ~BindingFlags.Default), typeof(HooksGeneral).GetMethod(nameof(PlayerTurnStart), ~BindingFlags.Default));
            IDetour idetour8 = new Hook(typeof(CombatStats).GetMethod(nameof(CombatStats.PlayerTurnEnd), ~BindingFlags.Default), typeof(HooksGeneral).GetMethod(nameof(PlayerTurnEnd), ~BindingFlags.Default));
            IDetour idetour9 = new Hook(typeof(CombatManager).GetMethod(nameof(CombatManager.PostNotification), ~BindingFlags.Default), typeof(HooksGeneral).GetMethod(nameof(PostNotification), ~BindingFlags.Default));
            IDetour idetour10 = new Hook(typeof(EffectAction).GetMethod(nameof(EffectAction.Execute), ~BindingFlags.Default), typeof(HooksGeneral).GetMethod(nameof(EffectActionExecute), ~BindingFlags.Default));
            IDetour idetour11 = new Hook(typeof(TooltipTextHandlerSO).GetMethod(nameof(TooltipTextHandlerSO.ProcessStoredValue), ~BindingFlags.Default), typeof(HooksGeneral).GetMethod(nameof(AddStoredValue), ~BindingFlags.Default));
            IDetour idetour12 = new Hook(typeof(OverworldManagerBG).GetMethod(nameof(OverworldManagerBG.Awake), ~BindingFlags.Default), typeof(HooksGeneral).GetMethod(nameof(AwakeOverworld), ~BindingFlags.Default));
            IDetour idetour13 = new Hook(typeof(MainMenuController).GetMethod(nameof(MainMenuController.LoadOldRun), ~BindingFlags.Default), typeof(HooksGeneral).GetMethod(nameof(LoadRun), ~BindingFlags.Default));
            IDetour idetour14 = new Hook(typeof(MainMenuController).GetMethod(nameof(MainMenuController.OnEmbarkPressed), ~BindingFlags.Default), typeof(HooksGeneral).GetMethod(nameof(LoadRun), ~BindingFlags.Default));
            IDetour idetour15 = new Hook(typeof(CharacterCombat).GetMethod(nameof(CharacterCombat.UseAbility), ~BindingFlags.Default), typeof(HooksGeneral).GetMethod(nameof(UseAbility), ~BindingFlags.Default));
            IDetour idetour16 = new Hook(typeof(EnemyCombat).GetMethod(nameof(EnemyCombat.UseAbility), ~BindingFlags.Default), typeof(HooksGeneral).GetMethod(nameof(UseAbility), ~BindingFlags.Default));

        }

        public static DamageInfo Damage(Func<IUnit, int, IUnit, DeathType, int, bool, bool, bool, DamageType, DamageInfo> orig, IUnit self, int amount, IUnit killer, DeathType deathType, int targetSlotOffset = -1, bool addHealthMana = true, bool directDamage = true, bool ignoresShield = false, DamageType specialDamage = DamageType.None)
        {
            DamageInfo ret = orig(self, amount, killer, deathType, targetSlotOffset, addHealthMana, directDamage, ignoresShield, specialDamage);

            return ret;
        }
        public static int WillApplyDamage(Func<IUnit, int, IUnit, int> orig, IUnit self, int amount, IUnit targetUnit)
        {
            int ret = orig(self, amount, targetUnit);

            return ret;
        }
        public static void StartMenu(Action<MainMenuController> orig, MainMenuController self)
        {
            orig(self);
        }
        public static void InitializeCombat(Action<CombatManager> orig, CombatManager self)
        {
            orig(self);
        }
        public static void PlayerTurnStart(Action<CombatStats> orig, CombatStats self)
        {
            orig(self);
        }
        public static void PlayerTurnEnd(Action<CombatStats> orig, CombatStats self)
        {
            orig(self);
        }
        public static void PostNotification( Action<CombatManager, string, object, object> orig, CombatManager self, string call, object sender, object args)
        {
            orig(self, call, sender, args);
        }
        public static IEnumerator EffectActionExecute( Func<EffectAction, CombatStats, IEnumerator> orig, EffectAction self, CombatStats stats)
        {
            return orig(self, stats);
        }
        public static string AddStoredValue(Func<TooltipTextHandlerSO, UnitStoredValueNames, int, string> orig, TooltipTextHandlerSO self, UnitStoredValueNames storedValue, int value)
        {
            string str1;
            if (storedValue == (UnitStoredValueNames)77889 && value > 0)
            {
                string str2 = "Multiattack" + string.Format(" +{0}", value);
                string str3 = "<color=#" + ColorUtility.ToHtmlStringRGB(self._positiveSTColor) + ">";
                string str4 = "</color>";
                str1 = str3 + str2 + str4;
            }
            else
                str1 = orig(self, storedValue, value);
            return str1;
        }
        public static void AwakeOverworld(Action<OverworldManagerBG> orig, OverworldManagerBG self)
        {
            orig(self);
        }
        public static void LoadRun(Action<MainMenuController> orig, MainMenuController self)
        {
            orig(self);
        }
        public static void UseAbility(Action<IUnit, int, FilledManaCost[]> orig, IUnit self, int abilityID, FilledManaCost[] filledCost)
        {
            orig(self, abilityID, filledCost);
        }
    }
}
