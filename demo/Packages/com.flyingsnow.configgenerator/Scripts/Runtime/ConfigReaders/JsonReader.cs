using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace flyingSnow
{
    public class JsonReader
    {
        public Dictionary<int, T> LoadTemplate<T>(string filename) where T : Template, new()
        {
            ConfigGeneratorSettings setting = ConfigGeneratorSettings.GetInstance();
            string filePath = Path.Combine(setting.GetTargetPath(), filename + ".json");
            if (File.Exists(filePath))
            {
                string content = File.ReadAllText(filePath);
                var jDic = JObject.Parse(content);
                var temp = JsonConvert.DeserializeObject<Dictionary<string,T>>(content);//new Dictionary<int, T>(jDic.Count);
                var ret = new Dictionary<int, T>(temp.Count);
                foreach (var kv in temp)
                {
                    ret[int.Parse(kv.Key)] = kv.Value;
                }
                return ret;
            }
            else
            {
                Debug.LogError("找不到配置文件:" + filePath);
            }

            return null;
        }

        public List<T> LoadList<T>(string filename) where T : class, new()
        {
            ConfigGeneratorSettings setting = ConfigGeneratorSettings.GetInstance();
            string filePath = Path.Combine(setting.GetTargetPath(), filename + ".json");
            if (File.Exists(filePath))
            {
            }
            else
            {
                Debug.LogError("找不到配置文件:" + filePath);
            }

            return null;
        }

        public T LoadSetting<T>(string filename) where T : class, new()
        {
            ConfigGeneratorSettings setting = ConfigGeneratorSettings.GetInstance();
            string filePath = Path.Combine(setting.GetTargetPath(), filename + ".json");
            if (File.Exists(filePath))
            {
            }
            else
            {
                Debug.LogError("找不到配置文件:" + filePath);
            }

            return null;
        }
    }
}