using System;
using System.Text;
using System.Reflection;
using DOL.Database;
using DOL.GS.Quests;

namespace DOL.GS.Behaviour
{
    public class BehaviorBuilder
    {
        
        //private MethodInfo addActionMethod;

        public BehaviorBuilder()
        {            
            //this.addActionMethod = questType.GetMethod("AddBehaviour", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);            
        }                

        public void AddBehavior(QuestBehaviour questPart)
        {            
            //addActionMethod.Invoke(null, new object[] { questPart });
        }        

        public BaseBehavior CreateBehavior(GameNPC npc)
        {
            BaseBehavior behaviour =  new BaseBehavior(npc);            
            return behaviour;
        }
        
    }
}
