using System;
using System.Text;
using System.Reflection;
using DOL.Database;
using DOL.GS.Quests;

namespace DOL.GS.Behaviour
{
    public class BehaviourBuilder
    {
        
        //private MethodInfo addActionMethod;

        public BehaviourBuilder()
        {            
            //this.addActionMethod = questType.GetMethod("AddBehaviour", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);            
        }                

        public void AddBehaviour(QuestBehaviour questPart)
        {            
            //addActionMethod.Invoke(null, new object[] { questPart });
        }        

        public BaseBehaviour CreateBehaviour(GameNPC npc)
        {
            BaseBehaviour behaviour =  new BaseBehaviour(npc);            
            return behaviour;
        }
        
    }
}
