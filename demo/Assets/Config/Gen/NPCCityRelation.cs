using System.Collections.Generic;
using flyingSnow;

public partial class Config
{
    public readonly DualKeyDictionary<int, int, NPCCityRelation> NPCCityRelation = new (12)
    {
               {
        1000, 2000, new NPCCityRelation(){npcID=1000,
                cityID=2000,
                hour = new List<int>()
{
}
}
        },
       {
        1000, 2001, new NPCCityRelation(){npcID=1000,
                cityID=2001,
                hour = new List<int>()
{
3,4,5
}
}
        },
       {
        1000, 2002, new NPCCityRelation(){npcID=1000,
                cityID=2002,
                hour = new List<int>()
{
1,2,10
}
}
        },
       {
        1001, 2000, new NPCCityRelation(){npcID=1001,
                cityID=2000,
                hour = new List<int>()
{
}
}
        },
       {
        1001, 2001, new NPCCityRelation(){npcID=1001,
                cityID=2001,
                hour = new List<int>()
{
3,4,5
}
}
        },
       {
        1001, 2002, new NPCCityRelation(){npcID=1001,
                cityID=2002,
                hour = new List<int>()
{
10,11
}
}
        },
       {
        1002, 2000, new NPCCityRelation(){npcID=1002,
                cityID=2000,
                hour = new List<int>()
{
10,11,12
}
}
        },
       {
        1002, 2001, new NPCCityRelation(){npcID=1002,
                cityID=2001,
                hour = new List<int>()
{
7,8,9,10
}
}
        },
       {
        1002, 2002, new NPCCityRelation(){npcID=1002,
                cityID=2002,
                hour = new List<int>()
{
}
}
        },
       {
        1003, 2000, new NPCCityRelation(){npcID=1003,
                cityID=2000,
                hour = new List<int>()
{
4,5,6,10,11
}
}
        },
       {
        1003, 2001, new NPCCityRelation(){npcID=1003,
                cityID=2001,
                hour = new List<int>()
{
}
}
        },
       {
        1003, 2002, new NPCCityRelation(){npcID=1003,
                cityID=2002,
                hour = new List<int>()
{
}
}
        }
    };
}