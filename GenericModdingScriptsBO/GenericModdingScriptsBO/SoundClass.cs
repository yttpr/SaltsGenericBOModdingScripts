using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.IO;

namespace PYMN13
{
    public static class SoundClass
    {
        public static void CreateSoundBankFile(string resourceName, bool onlyIfNotExist = false)
        {
            CreateResourceFile(resourceName, Application.dataPath + "/StreamingAssets", resourceName + ".bank", onlyIfNotExist);
            //call this like, CreateSoundBankFile("FunnyGuyHitNoise");
        }

        public static void CreateResourceFile(string resourceName, string path, string outputName, bool onlyIfNotExist = false)
        {
            byte[] resource = new byte[0] { };
            try
            {
                resource = ResourceLoader.ResourceBinary(resourceName);
            }
            catch (Exception ex)
            {
                Debug.Log("YOUR FILE DOES NOT EXIST MOTHERFUCKER");
            }
            if (resource.Length > 0 && !(onlyIfNotExist && File.Exists(path + "/" + outputName)))
            {
                File.WriteAllBytes(path + "/" + outputName, resource);
            }
        }

        public static void Setup()
        {
            CreateSoundBankFile("HawthorneBank");
            CreateSoundBankFile("HawthorneBank.strings");
            CreateSoundBankFile("Friendship");
            CreateSoundBankFile("Friendship.strings");

            FMODUnity.RuntimeManager.LoadBank("HawthorneBank", true);
            FMODUnity.RuntimeManager.LoadBank("HawthorneBank.strings", true);
            FMODUnity.RuntimeManager.LoadBank("Friendship", true);
            FMODUnity.RuntimeManager.LoadBank("Friendship.strings", true);

        }
    }
}
