using BrutalAPI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PYMN13
{
    public class DoubleEffectItem : Item
    {
        public Effect[] firstEffects = new Effect[0];
        public Effect[] secondEffects = new Effect[0];
        public bool _firsteEffectImmediate = false;
        public bool _secondImmediateEffect = false;
        public TriggerCalls[] SecondTrigger = new TriggerCalls[0];
        public TriggerCalls[] FirstTrigger = new TriggerCalls[0];
        public bool firstPopUp = true;
        public bool secondPopUp = true;
        public EffectorConditionSO[] secondTriggerConditions = new EffectorConditionSO[0];

        public override BaseWearableSO Wearable()
        {
            CustomDoublePerformEffectWearable instance = ScriptableObject.CreateInstance<CustomDoublePerformEffectWearable>();
            instance.BaseWearable(this);
            instance._firstEffects = ExtensionMethods.ToEffectInfoArray(firstEffects);
            instance._firstImmediateEffect = _firsteEffectImmediate;
            instance.doesItemPopUp = firstPopUp;
            instance._secondEffects = ExtensionMethods.ToEffectInfoArray(secondEffects);
            instance._secondImmediateEffect = _firsteEffectImmediate;
            instance._secondPerformTriggersOn = SecondTrigger;
            instance._secondDoesPerformItemPopUp = secondPopUp;
            instance._secondPerformConditions = secondTriggerConditions;
            return instance;
        }
    }
}
