using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PYMN13
{
    public static class CustomVisuals
    {
        public static Dictionary<string, AttackVisualsSO> Visuals;
        public static void Prepare()
        {
            Visuals = new Dictionary<string, AttackVisualsSO>();
        }
        public static void LoadVisuals(string name, AssetBundle bundle, string path, string sound, bool full = false)
        {
            try
            {
                AttackVisualsSO ret = ScriptableObject.CreateInstance<AttackVisualsSO>();
                ret.name = name;
                ret.animation = bundle.LoadAsset<AnimationClip>(path);
                ret.audioReference = sound;
                ret.isAnimationFullScreen = full;
                if (Visuals == null) Prepare();
                if (!Visuals.ContainsKey(name)) Visuals.Add(name, ret);
                else Debug.LogWarning("animation for " + name + " already exists!");
            }
            catch
            {
                Debug.LogError("visuals failed to load: " + name);
                Debug.LogError("asset path: " + path);
                Debug.LogError("audio path: " + sound);
            }
        }
        public static AttackVisualsSO GetVisuals(string name)
        {
            if (Visuals == null) Prepare();
            if (Visuals.TryGetValue(name, out AttackVisualsSO ret)) return ret;
            else Debug.LogWarning("missing animation for " + name);
            return null;
        }
        public static void Duplicate(string newname, string oldname, string audio)
        {
            try
            {
                AttackVisualsSO old = GetVisuals(oldname);
                if (old == null) return;
                AttackVisualsSO ret = ScriptableObject.CreateInstance<AttackVisualsSO>();
                ret.name = newname;
                ret.animation = old.animation;
                ret.audioReference = audio;
                ret.isAnimationFullScreen = old.isAnimationFullScreen;
                if (Visuals == null) Prepare();
                if (!Visuals.ContainsKey(newname)) Visuals.Add(newname, ret);
                else Debug.LogWarning("animation for " + newname + " already exists!");
            }
            catch
            {
                Debug.LogError("visuals failed to load: " + newname);
                Debug.LogError("failed to copy off: " + oldname);
            }
        }

        public static void Setup()
        {
            //LoadVisuals("Salt/Gaze", SaltEnemies.assetBundle, "assets/pretty/EyeScare.anim", "event:/Hawthorne/Attack/EyeScare");
        }
    }
}
