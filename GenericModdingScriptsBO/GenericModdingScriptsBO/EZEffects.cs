using BrutalAPI;
using UnityEngine;

namespace PYMN13
{
    public static class EZEffects
    {
        public static PreviousEffectCondition DidThat<T>(bool success, int prev = 1) where T : PreviousEffectCondition
        {
            PreviousEffectCondition instance = ScriptableObject.CreateInstance<T>();
            instance.wasSuccessful = success;
            instance.previousAmount = prev;
            return instance;
        }
        public static AnimationVisualsEffect GetVisuals<T>(
          string visuals,
          bool isChara,
          BaseCombatTargettingSO targets)
          where T : AnimationVisualsEffect
        {
            AnimationVisualsEffect instance = ScriptableObject.CreateInstance<T>();
            instance._visuals = !isChara ? LoadedAssetsHandler.GetEnemyAbility(visuals).visuals : LoadedAssetsHandler.GetCharacterAbility(visuals).visuals;
            instance._animationTarget = targets;
            return instance;
        }
        public static Targetting_ByUnit_Side TargetSide<T>(bool allies, bool allSlots, bool ignoreCast = false) where T : Targetting_ByUnit_Side
        {
            Targetting_ByUnit_Side instance = ScriptableObject.CreateInstance<T>();
            instance.ignoreCastSlot = ignoreCast;
            instance.getAllies = allies;
            instance.getAllUnitSlots = allSlots;
            return instance;
        }
        public static SwapToOneSideEffect GoSide<T>(bool right) where T : SwapToOneSideEffect
        {
            SwapToOneSideEffect instance = ScriptableObject.CreateInstance<T>();
            instance._swapRight = right;
            return instance;
        }
    }
    public struct Effect
    {
        public EffectSO _effect;

        public int _entryVariable;

        public string? _intent;

        public BaseCombatTargettingSO _target;

        public EffectConditionSO _condition;

        public Effect(EffectSO effect, int entryVariable, string? intent, BaseCombatTargettingSO target, EffectConditionSO condition = null)
        {
            _effect = effect;
            _entryVariable = entryVariable;
            _intent = intent;
            _target = target;
            _condition = condition;
        }

        public Effect(Effect effect)
        {
            _effect = effect._effect;
            _entryVariable = effect._entryVariable;
            _intent = effect._intent;
            _target = effect._target;
            _condition = effect._condition;
        }
    }
}
