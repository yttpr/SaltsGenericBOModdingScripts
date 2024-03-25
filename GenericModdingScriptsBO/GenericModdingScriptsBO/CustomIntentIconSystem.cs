using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace PYMN13
{
    public static class CustomIntentIconSystem
    {
        public static Dictionary<string, IntentInfo> Intents = new Dictionary<string, IntentInfo>();
        public static void TryAddIntent(string name, IntentInfo intent)
        {
            if (Intents.Keys.Contains(name)) return;
            else Intents.Add(name, intent);
        }
        public static IntentType GetIntent(string name)
        {
            if (Intents.TryGetValue(name, out IntentInfo intentInfo))
            {
                return intentInfo.Type;
            }
            Debug.LogError("IntentType for: " + name + " does not exist. Did you not add it in the correct order?");
            return IntentType.Misc_Hidden;
        }
        public static void HookInIntent(IntentHandlerSO self, IntentInfo info)
        {
            if (info is CustomIntentInfo custom)
                if (self._intentDB.Keys.Contains(custom.GetSoundFrom))
                    info._sound = self._intentDB[custom.GetSoundFrom]._sound;
                else Debug.LogError("IntentInfo: " + custom.Name + " cannot pull sound from: " + custom.GetSoundFrom.ToString() + " because it does not exist.");
            if (!self._intentDB.TryGetValue(info.Type, out IntentInfo other))
                self._intentDB.Add(info.Type, info);
            else Debug.LogWarning("Intent for IntentType: " + info.Type.ToString() + " already exists!?");
        }
        public static void AddIntentsGeneral(Action<IntentHandlerSO> orig, IntentHandlerSO self)
        {
            orig(self);
            foreach (IntentInfo info in Intents.Values) HookInIntent(self, info);
        }
        static bool AlreadySet = false;
        public static void Setup()
        {
            if (AlreadySet) return;
            AlreadySet = true;
            IDetour idetour = new Hook(typeof(IntentHandlerSO).GetMethod(nameof(IntentHandlerSO.Initialize), ~BindingFlags.Default), typeof(CustomIntentIconSystem).GetMethod(nameof(AddIntentsGeneral), ~BindingFlags.Default));
        }
    }
    public class CustomIntentInfo : IntentInfo
    {
        public string Name;
        public CustomIntentInfo(string name, IntentType type, Sprite sprite, IntentType SoundSouce)
        {
            this.Name = name;
            this._type = type;
            this._sprite = sprite;
            this._color = Color.white;
            this.GetSoundFrom = SoundSouce;
            SetupInternal();
        }
        public CustomIntentInfo(string name, IntentType type, Sprite sprite, IntentType SoundSouce, Color color)
        {
            this.Name = name;
            this._type = type;
            this._sprite = sprite;
            this._color = color;
            this.GetSoundFrom = SoundSouce;
            SetupInternal();
        }
        public void SetupInternal()
        {
            CustomIntentIconSystem.Setup();
            CustomIntentIconSystem.TryAddIntent(Name, this);
        }
        public IntentType GetSoundFrom;
        public override Sprite GetSprite(bool selector)
        {
            return _sprite;
        }
        public override Color GetColor(bool selector)
        {
            return _color;
        }
    }
}
