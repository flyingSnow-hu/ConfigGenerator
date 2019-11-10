using System;
using System.Xml;
using System.Collections.Generic;
using UnityEngine;

public abstract class XMLReader
{        
	public int id{get;set;}
	public XMLReader(){}
	public void SetData(XmlNode rootNode){
		try
		{
            XmlAttribute idAttr = rootNode.Attributes["id"];
            if (idAttr != null)
            {
                id = int.Parse(idAttr.Value);
            }
			for(int i=0;i<rootNode.ChildNodes.Count;i++){
				XmlNode childNode = rootNode.ChildNodes[i];

				string name = childNode.Name;
				try
				{
					this.GetType().GetProperty(name).SetValue(this,GetNodeData(childNode),null);
				}catch(Exception e)
				{
					Debug.LogError($"Config Error:[xml={rootNode.Name},id={id},name={name}]");
					throw e;
				}
			}
		}
		catch (Exception e) {
			Debug.LogError(String.Format("Config Error:[xml={0},id={1}]",rootNode.Name,id)+e.Message+"\n"+e.StackTrace);
		}
	}

	protected object GetNodeData(XmlNode node){
		string type = node.Attributes["type"].Value;
        if (type.EndsWith("_id")) {
            type = "int";
        }
		switch(type){
		case "int":
			 return string.IsNullOrEmpty(node.InnerText)?0:int.Parse(node.InnerText);
		case "float":
			return string.IsNullOrEmpty(node.InnerText)?0f:float.Parse(node.InnerText);
		case "string":
			 return node.InnerText;
		case "list":
			Type genericType = this.GetType().GetProperty(node.Name).PropertyType;
			object list = Activator.CreateInstance(genericType);
			
			for(int i=0;i<node.ChildNodes.Count;i++){
				XmlNode childNode = node.ChildNodes[i];
				list.GetType().GetMethod("Add").Invoke(list,new object[]{GetNodeData(childNode)});
			}
			return list;
		default:
			Type propertyType = this.GetType().GetProperty(node.Name).PropertyType;
			if(propertyType.IsSubclassOf(typeof(Enum))){
				//枚举类型.
				try{
				return Enum.Parse(propertyType,node.InnerText);
				}catch{
					Debug.LogError(string.Format("Undefined Enum:{0}.{1} [template id={2}]",type,node.InnerText,id));
					return this.GetType().GetProperty(node.Name).GetValue(this,null);
				}
			}else{
				return GetCustomData(node);
			}
		}
	}

	public virtual object GetCustomData(XmlNode node){ return null; }
}
