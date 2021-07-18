using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

namespace flyingSnow
{
    public class XMLReader
    {
        public Dictionary<int, T> LoadTemplate<T>(string filename) where T : Template, new()
        {
            ConfigGeneratorSettings setting = ConfigGeneratorSettings.GetInstance();
            string filePath = Path.Combine(setting.GetTargetPath(), filename + ".xml");
            if (File.Exists(filePath))
            {
                XmlDocument xmlDoc = new XmlDocument();
                try
                {
                    xmlDoc.Load(filePath);

                    Dictionary<int, T> result = new Dictionary<int, T>();
                    XmlNode root = xmlDoc.ChildNodes[1];  // 0节点是xml声明
                    XmlNodeList nodelist = root.ChildNodes;

                    for (int i = 0; i < nodelist.Count; i++)
                    {
                        XmlNode xmlNode = nodelist[i];
                        T item = new T();
                        SetData(item, xmlNode);
                        result.Add(item.id, item);
                    }
                    return result;
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
            else
            {
                Debug.LogError("找不到配置文件:" + filePath);
            }

            return null;
        }

        public Dictionary<string, Dictionary<string, string>> LoadTable(string filename)
        {
            ConfigGeneratorSettings setting = ConfigGeneratorSettings.GetInstance();
            string filePath = Path.Combine(setting.GetTargetPath(), filename + ".xml");
            if (File.Exists(filePath))
            {
                XmlDocument xmlDoc = new XmlDocument();
                try
                {
                    xmlDoc.Load(filePath);

                    Dictionary<string, Dictionary<string, string>> tableData = new Dictionary<string, Dictionary<string, string>>();
                    XmlNode root = xmlDoc.ChildNodes[1];  // 0节点是xml声明
                    XmlNodeList rowList = root.ChildNodes;

                    for (int r = 0; r < rowList.Count; r++)
                    {
                        XmlNode rowNode = rowList[r];
                        XmlNodeList cellList = rowNode.ChildNodes;

                        string rowKey = rowNode.Attributes["value"].Value;

                        Dictionary<string, string> rowData = new Dictionary<string, string>();

                        for (int c = 0; c < cellList.Count; c++)
                        {
                            XmlNode cellNode = cellList[c];
                            string columnKey = cellNode.Attributes["value"].Value;
                            string cellValue = cellNode.InnerText;

                            rowData.Add(columnKey, cellValue);
                        }

                        tableData.Add(rowKey, rowData);
                    }
                    return tableData;
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
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
            string filePath = Path.Combine(setting.GetTargetPath(), filename + ".xml");
            if (File.Exists(filePath))
            {
                XmlDocument xmlDoc = new XmlDocument();
                try
                {
                    xmlDoc.Load(filePath);

                    List<T> result = new List<T>();
                    XmlNode root = xmlDoc.ChildNodes[1];  // 0节点是xml声明
                    XmlNodeList nodelist = root.ChildNodes;

                    for (int i = 0; i < nodelist.Count; i++)
                    {
                        XmlNode xmlNode = nodelist[i];
                        T item = new T();
                        SetData(item, xmlNode);
                        result.Add(item);
                    }
                    return result;
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
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
            string filePath = Path.Combine(setting.GetTargetPath(), filename + ".xml");
            if (File.Exists(filePath))
            {
                XmlDocument xmlDoc = new XmlDocument();
                try
                {
                    xmlDoc.Load(filePath);

                    T result = new T();
                    XmlNode xmlNode = xmlDoc.ChildNodes[1];  // 0节点是xml声明
                    SetData(result, xmlNode);
                    return result;
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
            else
            {
                Debug.LogError("找不到配置文件:" + filePath);
            }

            return null;
        }


        private void SetData<T>(T obj, XmlNode rootNode)
        {
            int id = 0;
            try
            {
                XmlAttribute idAttr = rootNode.Attributes["id"];
                if (idAttr != null)
                {
                    id = int.Parse(idAttr.Value);
                    obj.GetType().GetProperty("id").SetValue(obj, id, null);
                }
                for (int i = 0; i < rootNode.ChildNodes.Count; i++)
                {
                    XmlNode childNode = rootNode.ChildNodes[i];

                    string name = childNode.Name;
                    try
                    {
                        var parent = obj.GetType();
                        parent.GetProperty(name).SetValue(obj, GetNodeData(childNode,obj), null);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Config Error:[xml={rootNode.Name},id={id},name={name}]");
                        throw e;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(String.Format("Config Error:[xml={0},id={1}]", rootNode.Name, id) + e.Message + "\n" + e.StackTrace);
            }
        }

        private object GetNodeData(XmlNode node, object parent)
        {
            string type = node.Attributes["type"].Value;
            if (type.EndsWith("_id"))
            {
                type = "int";
            }
            switch (type)
            {
                case "int":
                    return string.IsNullOrEmpty(node.InnerText) ? 0 : int.Parse(node.InnerText);
                case "float":
                    return string.IsNullOrEmpty(node.InnerText) ? 0f : float.Parse(node.InnerText);
                case "string":
                    return node.InnerText;
                case "[]":
                    Type genericType = parent.GetType().GetProperty(node.Name).PropertyType;
                    object list = Activator.CreateInstance(genericType);

                    for (int i = 0; i < node.ChildNodes.Count; i++)
                    {
                        XmlNode childNode = node.ChildNodes[i];
                        list.GetType().GetMethod("Add").Invoke(list, new object[] { GetNodeData(childNode, parent) });
                    }
                    return list;
                default:
                    Type propertyType = parent.GetType().GetProperty(node.Name).PropertyType;
                    if (propertyType.IsSubclassOf(typeof(Enum)))
                    {
                        //枚举类型.
                        try
                        {
                            return Enum.Parse(propertyType, node.InnerText);
                        }
                        catch
                        {
                            Debug.LogError(string.Format("Undefined Enum:{0}.{1}", type, node.InnerText));
                            return parent.GetType().GetProperty(node.Name).GetValue(parent, null);
                        }
                    }
                    else
                    {
                        return GetCustomData(node);
                    }
            }
        }

        protected virtual object GetCustomData(XmlNode node) { return null; }
    }
}