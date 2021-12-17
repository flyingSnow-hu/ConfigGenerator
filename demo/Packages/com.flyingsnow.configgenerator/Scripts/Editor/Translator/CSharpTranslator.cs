using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace flyingSnow
{
    public class CSharpRow
    {
        public string id;
        public List<string> lines;
    }

    public class CSharpTranslator : Translator<CSharpRow, string>
    {
        private const string TemplateTableFormat = @"using System.Collections.Generic;
using System.Collections.ObjectModel;

public partial class Config 
{{
    public readonly ReadOnlyDictionary<int, {0}> {0} = new ReadOnlyDictionary<int, {0}>(new Dictionary<int, {0}>({1}) 
    {{
{2}
    }});
}};";

        private const string TemplateFormat = @"
        {{
            {1}, new {0}(){{ 
                {2}
            }}
        }}";

        private const string SettingTableFormat = @"using System.Collections.Generic;
public partial class Config {{
    public readonly {0} {0} = new {0}() 
    {{
{2}
    }};
}};";

        private const string SettingFormat = "        {2}";

        private const string RelationTableFormat = @"using System.Collections.Generic;
public partial class Config
{{
    public readonly Dictionary<int, Dictionary<int, {0}>> {0} = new Dictionary<int, Dictionary<int, {0}>>({1})
    {{
    }};
}}";

        private const string RelationFormat = @"
        {{
            {0},
            new()
            {{
                {{
                    {1},
                    new()
                    {{
{2}
                    }}
            }}
        }}";

        private string configType;
        List<CSharpRow> root;

        protected override void AppendCell(CSharpRow row, Cell cell)
        {
            switch (cell.type)
            {
                default:
                    row.lines.Add($"{cell.name} = {GetCsForm(cell.type, cell.value)}");
                    break;
                case "[]":
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine($"{cell.name} = new List<{cell.itemType}>()");
                    sb.AppendLine("{");

                    if (!string.IsNullOrEmpty(cell.value))
                    {
                        string[] valueList = cell.value.Split(';');
                        sb.AppendLine(string.Join(",", valueList.Select(s=>GetCsForm(cell.itemType, s))));
                    }
                    sb.AppendLine("}");
                    row.lines.Add(sb.ToString());
                    break;
            }
        }

        private string GetCsForm(string baseType, string value)
        {
            switch (baseType)
            {
                case "string":
                    return $"\"{value}\"";
                case "int":
                    return value;
                case "float":
                    return $"{value}f";
                default:
                    // 枚举
                    return $"{baseType}.{value}";
            }
        }

        protected override void CreateFile(string configName, string configType)
        {
            this.configType = configType;
            root = new List<CSharpRow>();
        }

        protected override CSharpRow CreateRow(string configName)
        {
            return new CSharpRow(){ lines=new List<string>()};
        }

        protected override string GetRowID(CSharpRow row)
        {
            return row.id;
        }

        protected override void Save(string configName)
        {
            ConfigGeneratorSettings setting = ConfigGeneratorSettings.GetInstance();
            string fileName = Path.Combine(setting.GetTargetPath(), configName + ".cs");
            var tableTemplate = "";
            var rowTemplate = "";
            var lineIntedent = "    ";
            switch (configType)
            {
                case "setting":
                    tableTemplate = SettingTableFormat;
                    rowTemplate = SettingFormat;
                    lineIntedent = "        ";
                    break;
                case "template":
                    tableTemplate = TemplateTableFormat;
                    rowTemplate = TemplateFormat;
                    lineIntedent = "                ";
                    break;
                case "relation":
                    tableTemplate = RelationTableFormat;
                    rowTemplate = RelationFormat;
                    lineIntedent = "                ";
                    break;
                default:
                    return;
            }

                //{
                using (var fs = File.Open(fileName, FileMode.Create, FileAccess.Write, FileShare.Read))
                {
                    byte[] bytes = new UTF8Encoding(true).GetBytes(
                        string.Format(tableTemplate, configName, root.Count,
                            string.Join(",\r\n",
                                root.Select(
                                    row =>
                                    string.Format(rowTemplate, configName, row.id,
                                        string.Join(",\r\n" + lineIntedent, row.lines.ToArray())
                                    )
                                )
                            )
                        )
                    );

                    fs.Write(bytes, 0, bytes.Length);
                }
            //}
        }

        protected override void SaveRow(CSharpRow row)
        {
            root.Add(row);
        }

        protected override void SetRowAttribute(CSharpRow row, string key, string value)
        {
            row.lines.Add(value);
        }

        protected override void SetRowId(CSharpRow row, string id)
        {
            row.id = id;
        }
    }
}