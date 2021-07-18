using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace flyingSnow
{
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
                    var settings = ConfigGeneratorSettings.GetInstance();     
                    settings.sourceDir = EditorGUILayout.TextField("Path of Excel Files", settings.sourceDir);
                    settings.targetDir = EditorGUILayout.TextField("Path of Exported Files", settings.targetDir);
                    if(GUI.changed)
                    {
                        settings.Save();
                    }
                },

                keywords = new HashSet<string>(new[] { "Excel", "Export"})
            };

            return provider;
        }
    }
}