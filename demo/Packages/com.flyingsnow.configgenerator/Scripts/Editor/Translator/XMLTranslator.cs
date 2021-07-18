using System;
using System.IO;
using System.Data;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Excel;
using System.Text;
using System.Xml;

namespace flyingSnow
{
    public class XMLTranslator:Translator<XmlElement, XmlElement>
    {
        private XmlDocument xmlDoc;
        // private string configName;
        private XmlElement root;

        protected override void CreateFile(string configName, string configType)
        {

            xmlDoc = new XmlDocument();
            // this.configName = configName;

            //XML的声明段落,<?xmlversion="1.0" encoding="utf-8"?>
            XmlDeclaration xmldecl;
            xmldecl = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);
            xmlDoc.AppendChild(xmldecl);

            //根元素
            root = xmlDoc.CreateElement("data");
            xmlDoc.AppendChild(root);
            root.SetAttribute("type", configType);
            root.SetAttribute("name", configName);
        }

        protected override void Save(string configName)
        {
            ConfigGeneratorSettings setting = ConfigGeneratorSettings.GetInstance();
            string fileName = Path.Combine(setting.GetTargetPath(), configName + ".xml");
            File.Delete(fileName);            
            xmlDoc.Save(fileName);
        }

        protected override XmlElement CreateRow(string configName)
        {
            return xmlDoc.CreateElement(configName);
        }
        
        protected override void SaveRow(XmlElement row)
        {
            root.AppendChild(row);
        }

        protected override string GetRowID(XmlElement row)
        {
            return row.GetAttribute("id");
        }

        protected override void AppendCell(XmlElement row, Cell cell)
        {
            var node = xmlDoc.CreateElement(cell.name);
            node.SetAttribute("type", cell.type);
            if (cell.itemType != null){
                node.SetAttribute("itemType", cell.itemType);                
            }

            if (cell.type == "[]")
            {
                if (!string.IsNullOrEmpty(cell.value))
                {
                    string[] valueList = cell.value.Split(';');
                    for (int i = 0; i < valueList.Length; i++)
                    {
                        TranslateCell(cell.itemType, cell.itemType, valueList[i], node);
                    }
                }
            }else
            {                
                node.InnerText = cell.value;
            }
            row.AppendChild(node);
        }

        protected override void SetRowId(XmlElement row, string id)
        {
            row.SetAttribute("id", id.ToString());
        }

        protected override void SetRowAttribute(XmlElement row, string key, string value)
        {
            row.SetAttribute(key, value);
        }
    }
}