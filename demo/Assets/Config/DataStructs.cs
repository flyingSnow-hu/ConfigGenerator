using System.Collections.Generic;

public class NPCSetting
{
    public float timeFactor;
    public int npcCount;
    public string assetPath;
}

public enum Nationality
{
    CHINA = 1,
    JAPAN = 2,
    KOREA = 3
}


public class NPCTemplate:Template
{    
    public string name;
    public Nationality nationality;
}

public class NPCCityRelation
{
    public int npcID;
    public int cityID;
    public List<int> hour;
}