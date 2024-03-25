using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace PYMN13
{
    public static class ResourceLoader
    {
        public static Texture2D LoadTexture(string name)
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            Texture2D texture2D1 = new Texture2D(0, 0, TextureFormat.ARGB32, false);
            texture2D1.anisoLevel = 1;
            texture2D1.filterMode = FilterMode.Point;
            Texture2D texture2D2 = texture2D1;
            try
            {
                string name1 = (executingAssembly.GetManifestResourceNames()).First(r => r.Contains(name));
                Stream manifestResourceStream = executingAssembly.GetManifestResourceStream(name1);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    byte[] buffer = new byte[16384];
                    int count;
                    while ((count = manifestResourceStream.Read(buffer, 0, buffer.Length)) > 0)
                        memoryStream.Write(buffer, 0, count);
                    ImageConversion.LoadImage(texture2D2, memoryStream.ToArray());
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Missing Texture \"" + name + "\"");
                if (name != "PassivePlaceholder.png") return LoadTexture("PassivePlaceholder.png");
                return null;
            }
            return texture2D2;
        }

        public static Sprite LoadSprite(string name, int ppu = 32, Vector2? pivot = null)
        {
            if (!pivot.HasValue)
                pivot = new Vector2?(new Vector2(0.5f, 0.5f));
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            Sprite sprite;
            try
            {
                string name1 = (executingAssembly.GetManifestResourceNames()).First((r => r.Contains(name)));
                Stream manifestResourceStream = executingAssembly.GetManifestResourceStream(name1);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    byte[] buffer = new byte[16384];
                    int count;
                    while ((count = manifestResourceStream.Read(buffer, 0, buffer.Length)) > 0)
                        memoryStream.Write(buffer, 0, count);
                    Texture2D texture2D = new Texture2D(0, 0, TextureFormat.ARGB32, false);
                    texture2D.anisoLevel = 1;
                    texture2D.filterMode = FilterMode.Point;
                    Texture2D texture = texture2D;
                    ImageConversion.LoadImage(texture, memoryStream.ToArray());
                    sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), pivot.Value, ppu);
                }
            }
            catch (InvalidOperationException ex)
            {
                Debug.LogError("Missing Texture \"" + name + "\"! Check for typos when using ResourceLoader.LoadSprite() and that all of your textures have their build action as Embedded Resource.");
                if (name != "PassivePlaceholder.png") return LoadSprite("PassivePlaceholder.png");
                return null;
            }
            return sprite;
        }

        public static byte[] ResourceBinary(string name)
        {
            try
            {
                Assembly executingAssembly = Assembly.GetExecutingAssembly();
                string name1 = (executingAssembly.GetManifestResourceNames()).First(r => r.Contains(name));
                using (Stream manifestResourceStream = executingAssembly.GetManifestResourceStream(name1))
                {
                    if (manifestResourceStream == null)
                        return new byte[0];
                    byte[] buffer = new byte[manifestResourceStream.Length];
                    manifestResourceStream.Read(buffer, 0, buffer.Length);
                    return buffer;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(("Missing Resource \"" + name + "\""));
                throw ex;
            }
        }
    }
}
