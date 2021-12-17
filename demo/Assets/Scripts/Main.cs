using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using flyingSnow;

public class Main : MonoBehaviour
{
    public void OnClick()
    {
        var dict = new DualKeyDictionary<string, string, string>(3);
        dict.Set("Apple", "Argentina", "AA");
        dict.Set("Apple", "Brasil", "AB");
        dict.Set("Apple", "China", "AC");
        dict.Set("Banana", "Argentina", "BA");
        dict.Set("Banana", "Brasil", "BB");
        dict.Set("Banana", "China", "BC");
        dict.Set("Coconut", "Argentina", "CA");
        dict.Set("Coconut", "Brasil", "CB");
        dict.Set("Coconut", "China", "CC");
        dict.Print();
        Debug.Log("完");
    }
}
