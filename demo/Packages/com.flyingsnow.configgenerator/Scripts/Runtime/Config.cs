using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;


public class Config
{
    public static readonly Config Instance = new Config();

	// public GlobalSetting globalSetting { get; private set; }
	// public Dictionary<int,EventTemplate> events;
	// public Dictionary<int,DemandTemplate> demands;
	// public Dictionary<int,DemandGroupTemplate> demandGroups;

    // public NPCSetting globalConfig { get; private set; }
    
    public float LoadedPercentage { get; private set; }

    private Config() { }
    ~Config() { }
    public void Preload()
    {
        // globalSetting = LoadConfig<GlobalSetting>("GameConstConfig");
		// events = LoadTemplate<EventTemplate>("EventTemplate");
		// demands = LoadTemplate<DemandTemplate>("DemandTemplate");
		// demandGroups = LoadTemplate<DemandGroupTemplate>("DemandGroupTemplate");
        // globalConfig = LoadConfig<NPCSetting>("NPCSetting");
    }

    private Dictionary<int, T> LoadTemplate<T>(string filename) where T : XMLReader, new()
    {
        string filePath = Application.streamingAssetsPath + @"/Config/" + filename + ".xml";
        if (File.Exists(filePath))
        {
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(filePath);

                Dictionary<int, T> result = new Dictionary<int, T>();
                XmlNode root = xmlDoc.ChildNodes[1];//0节点是xml声明
                XmlNodeList nodelist = root.ChildNodes;

                for (int i = 0; i < nodelist.Count; i++)
                {
                    XmlNode xmlNode = nodelist[i];
                    T item = new T();
                    item.SetData(xmlNode);
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

    private Dictionary<string, Dictionary<string, string>> LoadTable(string filename)
    {
        string filePath = Application.streamingAssetsPath + @"/Config/" + filename + ".xml";
        if (File.Exists(filePath))
        {
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(filePath);

                Dictionary<string, Dictionary<string, string>> tableData = new Dictionary<string, Dictionary<string, string>>();
                XmlNode root = xmlDoc.ChildNodes[1];//0节点是xml声明
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

                        rowData.Add(columnKey,cellValue);
                    }

                    tableData.Add(rowKey,rowData);
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

    private List<T> LoadList<T>(string filename) where T : XMLReader, new()
    {
        string filePath = Application.streamingAssetsPath + @"/Config/" + filename + ".xml";
        if (File.Exists(filePath))
        {
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(filePath);

                List<T> result = new List<T>();
                XmlNode root = xmlDoc.ChildNodes[1];//0节点是xml声明
                XmlNodeList nodelist = root.ChildNodes;

                for (int i = 0; i < nodelist.Count; i++)
                {
                    XmlNode xmlNode = nodelist[i];
                    T item = new T();
                    item.SetData(xmlNode);
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

    private T LoadConfig<T>(string filename) where T :XMLReader, new()
    {
        string filePath = Application.streamingAssetsPath + @"/Config/" + filename + ".xml";
        if (File.Exists(filePath))
        {
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(filePath);

                T result = new T();
                XmlNode xmlNode = xmlDoc.ChildNodes[1];//0节点是xml声明
                result.SetData(xmlNode);
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
}
