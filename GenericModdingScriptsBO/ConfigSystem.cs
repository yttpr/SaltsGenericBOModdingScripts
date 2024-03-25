using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

/*HOW TO USE:
1) change the namespace
2) change the FolderName and the FileName
3) in your awake, call Check("string name") for every boolean you want to use.
  -  To add a new value to the config, just call Check("thevaluename")
  -  Alternatively if you want to set it to a specific value, you can use Set("thevaluename", boolean value);
  -  Once you have "checked" all of the values you want, call TryWriteConfig();
  -  There is an example of how to make use of this to create a functional config below.
 */

namespace PYMN13
{
    public static class Config
    {
        public const string FolderName = "PYMN13";//this is the name of the folder, you can have multiple configs in a folder just fine
        public const string FileName = "GameData";//this is the file name, change the file name for each config
        public const bool Default = true;//when generating new config values, it'll set them automatically to this value.

        public static void TryWriteConfig() => WriteConfig(SaveName);

        public static void ExampleAwake()
        {
            //pretend this is your real awake
            if (Check("AddMisterOne"))
            {
                //MisterOne.Add();
            }
            if (Check("AddMisterTwo"))
            {
                //MisterTwo.Add();
            }
            if (Check("AddMisterThree"))
            {
                //MisterThree.Add();
            }
            TryWriteConfig();
            //that's it.
        }

        public static Dictionary<string, bool> SaveConfigNames;

        public static void WriteConfig(string location)
        {
            StreamWriter text = File.CreateText(location);
            XmlDocument xmlDocument = new XmlDocument();
            string xml = "<config";
            foreach (string key in SaveConfigNames.Keys)
            {
                xml += " ";
                xml += key;
                xml += "='";
                xml += SaveConfigNames[key].ToString().ToLower();
                xml += "'";
            }
            xml += "> </config>";
            xmlDocument.LoadXml(xml);
            xmlDocument.Save((TextWriter)text);
            text.Close();
        }

        public static bool Check(string name)
        {
            if (SaveConfigNames == null)
            {
                SaveConfigNames = new Dictionary<string, bool>();
            }
            string l = SaveName;
            bool add = Default;
            FileStream inStream = File.Open(SaveName, FileMode.Open);
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load((Stream)inStream);
            if (xmlDocument.GetElementsByTagName("config").Count > 0)
            {

                if (xmlDocument.GetElementsByTagName("config")[0].Attributes[name] != null)
                {
                    add = bool.Parse(xmlDocument.GetElementsByTagName("config")[0].Attributes[name].Value);

                }
                if (!SaveConfigNames.Keys.Contains(name))
                    SaveConfigNames.Add(name, add);
                else
                    SaveConfigNames[name] = add;
            }
            inStream.Close();
            return add;
        }

        public static void Set(string name, bool value)
        {
            if (Check(name) != value)
            {
                SaveConfigNames[name] = value;
                WriteConfig(SaveName);
            }
        }

        static string pathPlus
        {
            get
            {
                return BepInEx.Paths.BepInExRootPath + "\\Plugins\\";
            }
        }
        public static string SavePath
        {
            get
            {
                if (!Directory.Exists(pathPlus + FolderName + "\\"))
                {
                    Directory.CreateDirectory(pathPlus + FolderName + "\\");
                }
                return pathPlus + FolderName + "\\";
            }
        }
        public static string SaveName
        {
            get
            {
                if (!File.Exists(SavePath + FileName + ".config"))
                {
                    WriteConfig(SavePath + FileName + ".config");
                }
                return SavePath + FileName + ".config";
            }
        }
    }
}
