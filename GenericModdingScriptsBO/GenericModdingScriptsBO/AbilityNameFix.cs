using BrutalAPI;
using MonoMod.RuntimeDetour;
using System;
using System.Reflection;

namespace PYMN13
{
    public static class AbilityNameFix
    {
        public static CharacterAbility CharacterAbility(Func<Ability, CharacterAbility> orig, Ability self)
        {
            CharacterAbility characterAbility = orig(self);
            characterAbility.ability._abilityName = self.name;
            characterAbility.ability._description = self.description;
            characterAbility.ability.name = self.name;
            return characterAbility;
        }

        public static EnemyAbilityInfo EnemyAbility(Func<Ability, EnemyAbilityInfo> orig, Ability self)
        {
            EnemyAbilityInfo enemyAbilityInfo = orig(self);
            enemyAbilityInfo.ability._abilityName = self.name;
            enemyAbilityInfo.ability._description = self.description;
            enemyAbilityInfo.ability.name = self.name;
            return enemyAbilityInfo;
        }

        public static void Setup()
        {
            IDetour idetour1 = new Hook(typeof(Ability).GetMethod(nameof(Ability.CharacterAbility), ~BindingFlags.Default), typeof(AbilityNameFix).GetMethod(nameof(CharacterAbility), ~BindingFlags.Default));
            IDetour idetour2 = new Hook(typeof(Ability).GetMethod(nameof(Ability.EnemyAbility), ~BindingFlags.Default), typeof(AbilityNameFix).GetMethod(nameof(EnemyAbility), ~BindingFlags.Default));
        }
    }
}
