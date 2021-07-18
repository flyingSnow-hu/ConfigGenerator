using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using flyingSnow;

public class Main : MonoBehaviour
{
    void Start()
    {
        var reader = new XMLReader();

        var npcSetting = reader.LoadSetting<NPCSetting>("NPCSetting");
        Debug.Log(npcSetting.assetPath);

        var npcTemplates = reader.LoadTemplate<NPCTemplate>("NPCTemplate");
        Debug.Log(npcTemplates[101].nationality);
    }

    void Update()
    {
        
    }
}
