using System.Collections.Generic;
using System.Collections.ObjectModel;

public partial class Config 
{
    public readonly ReadOnlyDictionary<int, NPCCityRelation> NPCCityRelation = new ReadOnlyDictionary<int, NPCCityRelation>(new Dictionary<int, NPCCityRelation>(12) 
    {

        {
            , new NPCCityRelation(){ 
                1000,
                2000,
                List<int> hour = new List<int>()
{
};

            }
        },

        {
            , new NPCCityRelation(){ 
                1000,
                2001,
                List<int> hour = new List<int>()
{
3,4,5
};

            }
        },

        {
            , new NPCCityRelation(){ 
                1000,
                2002,
                List<int> hour = new List<int>()
{
1,2,10
};

            }
        },

        {
            , new NPCCityRelation(){ 
                1001,
                2000,
                List<int> hour = new List<int>()
{
};

            }
        },

        {
            , new NPCCityRelation(){ 
                1001,
                2001,
                List<int> hour = new List<int>()
{
3,4,5
};

            }
        },

        {
            , new NPCCityRelation(){ 
                1001,
                2002,
                List<int> hour = new List<int>()
{
10,11
};

            }
        },

        {
            , new NPCCityRelation(){ 
                1002,
                2000,
                List<int> hour = new List<int>()
{
10,11,12
};

            }
        },

        {
            , new NPCCityRelation(){ 
                1002,
                2001,
                List<int> hour = new List<int>()
{
7,8,9,10
};

            }
        },

        {
            , new NPCCityRelation(){ 
                1002,
                2002,
                List<int> hour = new List<int>()
{
};

            }
        },

        {
            , new NPCCityRelation(){ 
                1003,
                2000,
                List<int> hour = new List<int>()
{
4,5,6,10,11
};

            }
        },

        {
            , new NPCCityRelation(){ 
                1003,
                2001,
                List<int> hour = new List<int>()
{
};

            }
        },

        {
            , new NPCCityRelation(){ 
                1003,
                2002,
                List<int> hour = new List<int>()
{
};

            }
        }
    });
};