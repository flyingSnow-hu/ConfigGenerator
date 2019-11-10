using System;
using System.IO;
using System.Data;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Excel;
using System.Text;
using System.Xml;

class ExcelTranslator
{
    public static readonly string SHEETS_PATH = Application.dataPath + "/ConfigSheets~";

    [MenuItem("Config/Translate Config")]
    static void TranslateAll()
    {
        DirectoryInfo folder = new DirectoryInfo(SHEETS_PATH);
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

    static void TranslateXLS(FileInfo fileInfo)
    {
        using (FileStream stream = File.Open(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            using (IExcelDataReader r = ExcelReaderFactory.CreateBinaryReader(stream))
            {
                TranslateFile(fileInfo, r);
            }
        }        
    }

    static void TranslateXLSX(FileInfo fileInfo)
    {
        using (FileStream stream = File.Open(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            using (IExcelDataReader r = ExcelReaderFactory.CreateOpenXmlReader(stream))
            {
                TranslateFile(fileInfo, r);
            }
        }        

    }

    static void TranslateFile(FileInfo fileInfo, IExcelDataReader excelReader)
    {        
        DataSet dataSet = excelReader.AsDataSet();

        string rootPath = Application.streamingAssetsPath + "/Config/";

        for (int t = 0; t < dataSet.Tables.Count; t++)
        {
            DataTable table = dataSet.Tables[t];
            if (table.Rows.Count < 3)
            {
                Debug.LogWarning(table.TableName + "表不足三行,未导出");
                continue;
            }

            //建立xml文件.
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

            string fileName = rootPath + configName + ".xml";
            DeleteFile(fileName);

            XmlDocument xmlDoc = new XmlDocument();
            //XML的声明段落,<?xmlversion="1.0" encoding="utf-8"?>
            XmlDeclaration xmldecl;
            xmldecl = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);
            xmlDoc.AppendChild(xmldecl);
            //根元素
            XmlElement root = xmlDoc.CreateElement("data");
            xmlDoc.AppendChild(root);
            root.SetAttribute("type", configType);
            root.SetAttribute("name", configName);

            Debug.Log("Exporting :" + fileName);
            Debug.Log("类型：" + configType);

            switch (configType)
            {
                case "setting":
                    TranslateConfig(table, configName, fileName, xmlDoc, root);
                    break;
                case "relation":
                    TranslateRelation(table, configName, fileName, xmlDoc, root);
                    break;
                default:
                    TranslateTemplate(table, configName, fileName, xmlDoc, root);
                    break;
            }

            xmlDoc.Save(fileName);
        }
    }

    private static void TranslateTemplate(DataTable table, string configName, string fileName, XmlDocument xmlDoc, XmlElement root)
    {
        DataRow varNameRow = table.Rows[0];
        DataRow varTypeRow = table.Rows[1];

        for (int r = 3; r < table.Rows.Count; r++)
        {
            DataRow row = table.Rows[r];
            XmlElement line = xmlDoc.CreateElement(configName);
            for (int c = 0; c < table.Columns.Count; c++)
            {
                string name = varNameRow[c].ToString();
                string type = varTypeRow[c].ToString();
                string value = row[c].ToString();

                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(type))
                {
                    TranslateCell(xmlDoc, line, fileName, name, type, value);
                }
            }
            if (string.IsNullOrEmpty(line.GetAttribute("id")))
            {
                Debug.LogWarningFormat("Invalid ID in {0}, Skip", fileName);
            }else
            {
                root.AppendChild(line);
            }
        }
    }

    private static void TranslateRelation(DataTable table, string configName, string fileName, XmlDocument xmlDoc, XmlElement root)
    {
        DataRow varNameRow = table.Rows[0];
        DataRow varTypeRow = table.Rows[1];

        for (int r = 3; r < table.Rows.Count; r++)
        {
            DataRow row = table.Rows[r];
            XmlElement line = xmlDoc.CreateElement(configName);
            for (int c = 0; c < table.Columns.Count; c++)
            {
                string name = varNameRow[c].ToString();
                string type = varTypeRow[c].ToString();
                string value = row[c].ToString();

                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(type))
                {
                    TranslateCell(xmlDoc, line, fileName, name, type, value);
                }
            }
            root.AppendChild(line);
        }
    }

    private static void TranslateConfig(DataTable table, string configName, string fileName, XmlDocument xmlDoc, XmlElement root)
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
        XmlElement line = root;

        for (int c = 0; c < table.Columns.Count; c++)
        {
            string name = varNameRow[c].ToString();
            string type = varTypeRow[c].ToString();
            string value = row[c].ToString();

            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(type))
            {
                TranslateCell(xmlDoc, line, fileName, name, type, value);
            }
        }
    }
    private static void TranslateTable(DataTable table, string configName, string fileName, XmlDocument xmlDoc, XmlElement root)
    {
        DataRow varColumeNameRow = table.Rows[0];
        object type = table.Rows[0][0];
        string[] typeStr = type.ToString().Split(new char[] { '\\' });
        if (typeStr.Length != 2)
        {
            Debug.LogWarning(string.Format("ERROR in {0}:表结构的第一行第一格需要写明数据格式如：int\\float", configName));
            return;
        }

        string rowType = typeStr[0];
        string columnType = typeStr[1];

        for (int r = 1; r < table.Rows.Count; r++)
        {
            DataRow row = table.Rows[r];
            string rowTitle = row[0].ToString();

            if (string.IsNullOrEmpty(rowTitle)) continue;

            XmlElement line = xmlDoc.CreateElement("row");// AppendChild(xmlDoc, root, fileName, rowType, rowType,rowTitle);
            line.SetAttribute("type", rowType);
            line.SetAttribute("value", rowTitle);
            root.AppendChild(line);

            for (int c = 1; c < table.Columns.Count; c++)
            {
                string columnTitle = varColumeNameRow[c].ToString();

                if (!string.IsNullOrEmpty(columnTitle))
                {
                    var cell = TranslateCell(xmlDoc, line, fileName, "cell", columnType, row[c].ToString());
                    cell.SetAttribute("value", columnTitle);
                }
            }
        }
    }

    private static XmlElement TranslateCell(XmlDocument xmlDoc, XmlElement parent, string fileName, string name, string type, string value)
    {
        XmlElement node = null;
        switch (type)
        {
            case "string":
                node = xmlDoc.CreateElement(name);
                node.SetAttribute("type", type);
                node.InnerText = value;
                parent.AppendChild(node);
                break;
            case "float":
                node = xmlDoc.CreateElement(name);
                node.SetAttribute("type", type);
                node.InnerText = value;
                parent.AppendChild(node);
                break;
            case "int":
                node = xmlDoc.CreateElement(name);
                node.SetAttribute("type", type);
                node.InnerText = value;
                parent.AppendChild(node);
                break;
            case "id":
                int id;
                if (int.TryParse(value, out id))
                {                        
                    parent.SetAttribute("id", value);
                }
                break;
            default:
                if (type.EndsWith("[]"))
                {
                    string itemType = type.Remove(type.LastIndexOf("[]"));

                    node = xmlDoc.CreateElement(name);
                    node.SetAttribute("type", "[]");
                    node.SetAttribute("itemType", itemType);

                    if (!string.IsNullOrEmpty(value))
                    {
                        string[] valueList = value.Split(';');
                        for (int i = 0; i < valueList.Length; i++)
                        {
                            TranslateCell(xmlDoc, node, fileName, itemType, itemType, valueList[i]);
                        }
                    }
                    parent.AppendChild(node);
                }
                else if (type.EndsWith("_id"))
                {                    
                    parent.SetAttribute(type, value);
                    break;
                }
                else
                {
                    node = xmlDoc.CreateElement(name);
                    node.SetAttribute("type", type);
                    node.InnerText = value;
                    parent.AppendChild(node);
                }
                break;
        }
        return node;
    }

    private static void DeleteFile(string path)
    {
        File.Delete(path);
    }

    //	private static void AppendFile(string path,string content){
    //		File.AppendAllText (path, content, Encoding.UTF8);
    //	}
}