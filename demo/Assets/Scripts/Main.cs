using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using flyingSnow;

public class Main : MonoBehaviour
{
    public void OnClick()
    {
        var reader = new XMLReader();

        // var npcSetting = reader.LoadSetting<NPCSetting>("NPCSetting");
        // Debug.Log(npcSetting.assetPath);

        var npcTemplates = reader.LoadTemplate<NPCTemplate>("NPCTemplate");
        Debug.Log(npcTemplates[101].name);
        Debug.Log((Nationality)npcTemplates[101].nationality);
    }
}
