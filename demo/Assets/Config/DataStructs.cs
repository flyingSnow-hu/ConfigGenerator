public class NPCSetting
{
    public float timeFactor { get; private set; }
    public int npcCount { get; private set; }
    public string assetPath { get; private set; }
}

public enum Nationality
{
    CHINA = 1,
    JAPAN = 2,
    KOREA = 3
}
public class NPCTemplate:Template
{    
    public string name { get; private set; }
    public Nationality nationality { get; private set; }
}