using System.IO;
using UnityEditor;
using UnityEngine;
using Excel;
using System.Data;
using System;

namespace flyingSnow
{
    public class Cell
    {
        public string name;
        public string type;
        public string itemType;
        public string value;
    }

    public static class TranslatorMenu
    {
        
        [MenuItem("Config/Translate Config")]
        internal static void Start()
        {
            ConfigGeneratorSettings setting = ConfigGeneratorSettings.GetInstance();
            DirectoryInfo folder = new DirectoryInfo(setting.GetSourcePath());
            switch (setting.targetFormat)
            {
                case ConfigGeneratorSettings.TargetFormat.XML:
                    var tx = new XMLTranslator();
                    tx.Translate();
                    break;
                default:
                    var tj = new JsonTranslator();
                    tj.Translate();
                    break;
            }
            AssetDatabase.Refresh();
        }
    }


    public abstract class Translator<TRow, TCell>
    {        
        internal void Translate()
        {
            ConfigGeneratorSettings setting = ConfigGeneratorSettings.GetInstance();
            DirectoryInfo folder = new DirectoryInfo(setting.GetSourcePath());
            foreach (FileInfo fileInfo in folder.GetFiles())
            {
                if (fileInfo.Extension == ".xlsx")
                {
                    Debug.Log("exporing " + fileInfo.FullName);
                    TranslateXLSX(fileInfo);
                }else if (fileInfo.Extension == ".xls")
                {
                    Debug.Log("exporing " + fileInfo.FullName);
                    TranslateXLS(fileInfo);
                }
            }
            AssetDatabase.Refresh();
        }

        void TranslateXLS(FileInfo fileInfo)
        {
            using (FileStream stream = File.Open(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (IExcelDataReader r = ExcelReaderFactory.CreateBinaryReader(stream))
                {
                    TranslateFile(fileInfo, r);
                }
            }        
        }

        void TranslateXLSX(FileInfo fileInfo)
        {
            using (FileStream stream = File.Open(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (IExcelDataReader r = ExcelReaderFactory.CreateOpenXmlReader(stream))
                {
                    TranslateFile(fileInfo, r);
                }
            }        

        }

        void TranslateFile(FileInfo fileInfo, IExcelDataReader excelReader)
        {        
            DataSet dataSet = excelReader.AsDataSet();

            ConfigGeneratorSettings setting = ConfigGeneratorSettings.GetInstance();
            string targetPath = setting.GetTargetPath();

            for (int t = 0; t < dataSet.Tables.Count; t++)
            {
                DataTable table = dataSet.Tables[t];
                if (table.Rows.Count < 3)
                {
                    Debug.LogWarning(table.TableName + "表不足三行,未导出");
                    continue;
                }

                //建立文件.
                string configName = table.TableName;

                //以 ~ 结尾的表不导出
                if (configName.EndsWith("~")) continue;

                string configType = "";

                if (configName.EndsWith("Relation"))
                {
                    configType = "relation";
                }
                else if (configName.EndsWith("Setting"))
                {
                    configType = "setting";
                }
                else if (configName.EndsWith("Template"))
                {
                    configType = "template";
                }

                CreateFile(configName, configType);
                
                Debug.Log("Exporting :" + configName);
                Debug.Log("类型：" + configType);

                switch (configType)
                {
                    case "setting":
                        TranslateSetting(table, configName);
                        break;
                    case "relation":
                        TranslateRelation(table, configName);
                        break;
                    default:
                        TranslateTemplate(table, configName);
                        break;
                }
                Save(configName);
            }
        }

        protected abstract void CreateFile(string configName, string configType);
        protected abstract void Save(string configName);
        protected abstract TRow CreateRow(string configName);
        protected abstract void SaveRow(TRow row);
        protected abstract string GetRowID(TRow row);

        private void TranslateTemplate(DataTable table, string configName)
        {
            DataRow varNameRow = table.Rows[0];
            DataRow varTypeRow = table.Rows[1];

            for (int r = 3; r < table.Rows.Count; r++)
            {
                DataRow row = table.Rows[r];
                TRow line = CreateRow(configName);
                for (int c = 0; c < table.Columns.Count; c++)
                {
                    string name = varNameRow[c].ToString();
                    string type = varTypeRow[c].ToString();
                    string value = row[c].ToString();

                    if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(type))
                    {
                        TranslateCell(name, type, value, line);
                    }
                }
                if (string.IsNullOrEmpty(GetRowID(line)))
                {
                    Debug.LogWarningFormat("Invalid ID in {0}, Skip", configName);
                }else
                {
                    SaveRow(line);
                }
            }
        }

        private void TranslateRelation(DataTable table, string configName)
        {
            DataRow varNameRow = table.Rows[0];
            DataRow varTypeRow = table.Rows[1];

            for (int r = 3; r < table.Rows.Count; r++)
            {
                DataRow row = table.Rows[r];
                TRow line = CreateRow(configName);
                for (int c = 0; c < table.Columns.Count; c++)
                {
                    string name = varNameRow[c].ToString();
                    string type = varTypeRow[c].ToString();
                    string value = row[c].ToString();

                    if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(type))
                    {
                        TranslateCell(name, type, value, line);
                    }
                }
                SaveRow(line);
            }
        }
        
        private void TranslateSetting(DataTable table, string configName)
        {
            DataRow varNameRow = table.Rows[0];
            DataRow varTypeRow = table.Rows[1];

            if (table.Rows.Count != 4)
            {
                Debug.LogWarning(string.Format("ERROR in {0}：配置文件应该有且只有一行数据", configName));
                return;
            }
            int r = 3;
            DataRow row = table.Rows[r];
            TRow line = CreateRow(configName);

            for (int c = 0; c < table.Columns.Count; c++)
            {
                string name = varNameRow[c].ToString();
                string type = varTypeRow[c].ToString();
                string value = row[c].ToString();

                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(type))
                {
                    TranslateCell(name, type, value, line);
                }
            }
            SaveRow(line);
        }

        protected Cell TranslateCell(string name, string type, string value, TRow row)
        {
            Cell node = null;
            switch (type)
            {
                case "string":
                case "float":
                case "int":
                    node = new Cell(){name=name, type=type,value=value};
                    AppendCell(row, node);
                    break;
                case "id":               
                    SetRowId(row, value);
                    break;
                default:
                    if (type.EndsWith("[]"))
                    {
                        string itemType = type.Remove(type.LastIndexOf("[]"));
                        node = new Cell(){name=name, type="[]",value=value, itemType=itemType};
                        AppendCell(row, node);
                    }
                    else if (type.EndsWith("_id"))
                    {                    
                        SetRowAttribute(row, type, value);
                        break;
                    }
                    else
                    {
                        node = new Cell(){name=name, type=type,value=value};
                        AppendCell(row, node);
                    }
                    break;
            }
            return node;
        }

        protected abstract void AppendCell(TRow row, Cell cell);
        protected abstract void SetRowId(TRow row, string id);
        protected abstract void SetRowAttribute(TRow row, string key, string value);
    }
}