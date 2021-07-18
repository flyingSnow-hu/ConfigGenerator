using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System;

[Serializable]
class ConfigGeneratorSettings
{
    public const string k_MyCustomSettingsPath = "ProjectSettings/ConfigGeneratorSettings.asset";


    [SerializeField] internal string configDir;

    internal static ConfigGeneratorSettings GetOrCreateSettings()
    {
        ConfigGeneratorSettings settings;
        if (!File.Exists(k_MyCustomSettingsPath))
        {
            settings = new ConfigGeneratorSettings();
            settings.configDir = "Assets/ConfigSheets~";
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

    internal void Save()
    {
        using (var writer = new StreamWriter(k_MyCustomSettingsPath))
        {
            writer.Write(JsonUtility.ToJson(this));
        }
    }

    internal string GetPath()
    {
        return Path.Combine(Directory.GetParent(Application.dataPath).FullName, configDir);
    }
}

static class ConfigGeneratorSettingsIMGUIRegister
{
    [SettingsProvider]
    public static SettingsProvider CreateConfigGeneratorSettingsProvider()
    {
        var provider = new SettingsProvider("Project/ConfigGeneratorSettings", SettingsScope.Project)
        {
            label = "ConfigGenerator Settings",

            guiHandler = (searchContext) =>
            {
                var settings = ConfigGeneratorSettings.GetOrCreateSettings();     
                settings.configDir = EditorGUILayout.TextField("Path of Excel Files", settings.configDir);
                if(GUI.changed)
                {
                    settings.Save();
                }
            },

            keywords = new HashSet<string>(new[] { "Number", "Some String" })
        };

        return provider;
    }
}