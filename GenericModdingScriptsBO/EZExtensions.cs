using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace PYMN13
{
    public static class EZExtensions
    {
        public static T GetRandom<T>(this T[] array)
        {
            return array.Length == 0 ? default : array[UnityEngine.Random.Range(0, array.Length)];
        }
        public static T GetRandom<T>(this List<T> list)
        {
            return list.Count <= 0 ? default : list[UnityEngine.Random.Range(0, list.Count)];
        }
        public static T[] SelfArray<T>(this T self)
        {
            return new T[1] { self };
        }
        public static int GetStatus(this IUnit self, StatusEffectType type)
        {
            if (self is IStatusEffector istatusEffector)
            {
                foreach (IStatusEffect statusEffect in istatusEffector.StatusEffects)
                {
                    if (statusEffect.EffectType == type)
                        return statusEffect.StatusContent;
                }
            }
            return 0;
        }
        public static Type[] GetAllDerived(Type baze)
        {
            List<Type> typeList = new List<Type>();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (baze.IsAssignableFrom(type) && !typeList.Contains(type) && type != baze)
                        typeList.Add(type);
                }
            }
            return typeList.ToArray();
        }
        public static bool PCall(Action orig, string name = null)
        {
            try
            {
                orig();
            }
            catch (Exception ex)
            {
                Debug.LogError(name != null ? (object)(name + " failed") : (object)(orig.ToString() + " failed"));
                Debug.LogError(ex.ToString() + ex.Message + ex.StackTrace);
                return false;
            }
            return true;
        }
    }
}
