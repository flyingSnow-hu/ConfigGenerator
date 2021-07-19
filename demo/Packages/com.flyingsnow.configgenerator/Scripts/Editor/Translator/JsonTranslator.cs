using System.IO;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace flyingSnow
{
    public class JsonTranslator : Translator<dynamic, dynamic>
    {
        private string configType;
        dynamic root;
        protected override void AppendCell(dynamic row, Cell cell)
        {
            switch(cell.type)
            {
                case "string":
                default:
                    row[cell.name] = cell.value; 
                    break;
                case "float":
                    row[cell.name] = float.Parse(cell.value); 
                    break;
                case "int":
                    row[cell.name] = int.Parse(cell.value); 
                    break;
                case "[]":
                    if (!string.IsNullOrEmpty(cell.value))
                    {
                        JArray list = new JArray();
                        string[] valueList = cell.value.Split(';');
                        for (int i = 0; i < valueList.Length; i++)
                        {
                            var value = valueList[i];
                            switch(cell.itemType)
                            {
                                case "string":
                                default:
                                    list.Add(value); 
                                    break;
                                case "float":
                                    list.Add(float.Parse(value)); 
                                    break;
                                case "int":
                                    list.Add(int.Parse(value)); 
                                    break;
                            }
                        }
                        row[cell.name] = list;
                    }
                    break;
            }
        }

        protected override void CreateFile(string configName, string configType)
        {
            this.configType = configType;
            if (configType == "relation")
            {
                root = new JArray();
            }else
            {
                root = new JObject();
            }
        }

        protected override dynamic CreateRow(string configName)
        {
           return new JObject();
        }

        protected override string GetRowID(dynamic row)
        {
            return row["id"].ToString();
        }

        protected override void Save(string configName)
        {
            ConfigGeneratorSettings setting = ConfigGeneratorSettings.GetInstance();
            string fileName = Path.Combine(setting.GetTargetPath(), configName + ".json");
            using(var fs = File.Open(fileName, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                JsonSerializerSettings jsetting = new JsonSerializerSettings();
                jsetting.NullValueHandling = NullValueHandling.Ignore;
                var content = JsonConvert.SerializeObject(root, Formatting.Indented, jsetting);
                byte[] bytes = new UTF8Encoding(true).GetBytes(content);

                fs.Write(bytes, 0, bytes.Length);
            }
        }

        protected override void SaveRow(dynamic row)
        {
            switch(configType)
            {
                case "relation":
                    root.Add(row);
                    break;
                case "setting":
                    root = row;
                    break;
                case "template":                
                    root[row["id"].ToString()] = row;
                    break;
            }
        }

        protected override void SetRowAttribute(dynamic row, string key, string value)
        {
            row[key] = value;
        }

        protected override void SetRowId(dynamic row, string id)
        {
            row["id"] = id;
        }
    }
}