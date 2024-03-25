using BrutalAPI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PYMN13
{
    public class MultiEffectCondition : EffectConditionSO
    {
        public EffectConditionSO[] conditions;
        public bool And = true;
        public override bool MeetCondition(IUnit caster, EffectInfo[] effects, int currentIndex)
        {
            foreach (EffectConditionSO condition in conditions)
            {
                bool flag = condition.MeetCondition(caster, effects, currentIndex);
                if (And && !flag)
                    return false;
                if (!And & flag)
                    return true;
            }
            return And;
        }
        public static MultiEffectCondition Create(EffectConditionSO[] cond, bool and = true)
        {
            MultiEffectCondition instance = CreateInstance<MultiEffectCondition>();
            instance.conditions = cond;
            instance.And = and;
            return instance;
        }
        public static MultiEffectCondition Create( EffectConditionSO first, EffectConditionSO second, bool and = true)
        {
            MultiEffectCondition instance = CreateInstance<MultiEffectCondition>();
            instance.conditions = new EffectConditionSO[] { first, second };
            instance.And = and;
            return instance;
        }
    }
    public class MultiEffectorCondition : EffectorConditionSO
    {
        public EffectorConditionSO[] conditions;
        public bool And = true;
        public override bool MeetCondition(IEffectorChecks effector, object args)
        {
            foreach (EffectorConditionSO condition in conditions)
            {
                bool flag = condition.MeetCondition(effector, args);
                if (And && !flag)
                    return false;
                if (!And & flag)
                    return true;
            }
            return And;
        }
        public static MultiEffectorCondition Create(EffectorConditionSO[] cond, bool and = true)
        {
            MultiEffectorCondition instance = CreateInstance<MultiEffectorCondition>();
            instance.conditions = cond;
            instance.And = and;
            return instance;
        }
        public static MultiEffectorCondition Create(EffectorConditionSO first, EffectorConditionSO second, bool and = true)
        {
            MultiEffectorCondition instance = CreateInstance<MultiEffectorCondition>();
            instance.conditions = new EffectorConditionSO[] { first, second };
            instance.And = and;
            return instance;
        }
    }
}
