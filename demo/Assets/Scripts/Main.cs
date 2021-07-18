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
    }

    void Update()
    {
        
    }
}
