using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace flyingSnow
{
    [Serializable]
    public class ConfigGeneratorSettings
    {
        public const string k_MyCustomSettingsPath = "ProjectSettings/ConfigGeneratorSettings.asset";

        [SerializeField] public string sourceDir;
        [SerializeField] public string targetDir;

        public static ConfigGeneratorSettings GetInstance()
        {
            ConfigGeneratorSettings settings;
            if (!File.Exists(k_MyCustomSettingsPath))
            {
                settings = new ConfigGeneratorSettings();
                settings.sourceDir = "Assets/ConfigSheets~";
                settings.targetDir = "Assets/StreamingAssets/Config";
                settings.Save();
            }else
            {
                using (var reader = new StreamReader(k_MyCustomSettingsPath))
                {
                    var content = reader.ReadToEnd();
                    settings = JsonUtility.FromJson<ConfigGeneratorSettings>(content);
                }
            }
            return settings;
        }

        public void Save()
        {
            using (var writer = new StreamWriter(k_MyCustomSettingsPath))
            {
                writer.Write(JsonUtility.ToJson(this));
            }
        }

        public string GetSourcePath()
        {
            return Path.Combine(Directory.GetParent(Application.dataPath).FullName, sourceDir);
        }

        public string GetTargetPath()
        {
            return Path.Combine(Directory.GetParent(Application.dataPath).FullName, targetDir);
        }
    }
}