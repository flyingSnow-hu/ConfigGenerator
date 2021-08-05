using System.Collections.Generic;
using System.Collections.ObjectModel;

public partial class Config 
{
    public readonly ReadOnlyDictionary<int, NPCTemplate> NPCTemplate = new ReadOnlyDictionary<int, NPCTemplate>(new Dictionary<int, NPCTemplate>(4) 
    {

        {
            100, new NPCTemplate(){ 
                name = "王小二",
                nationality = Nationality.CHINA
            }
        },

        {
            101, new NPCTemplate(){ 
                name = "李大狗",
                nationality = Nationality.CHINA
            }
        },

        {
            102, new NPCTemplate(){ 
                name = "Tanaka",
                nationality = Nationality.JAPAN
            }
        },

        {
            103, new NPCTemplate(){ 
                name = "赵七炫",
                nationality = Nationality.KOREA
            }
        }
    });
};