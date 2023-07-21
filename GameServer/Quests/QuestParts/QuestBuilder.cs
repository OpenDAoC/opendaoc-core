using System;
using System.Text;
using System.Reflection;
using DOL.Database;

namespace DOL.GS.Quests
{
    public class QuestBuilder
    {
        private Type questType;

        private MethodInfo addActionMethod;        

        public Type QuestType
        {
            get { return questType; }
            set { questType = value; }
        }

        public QuestBuilder(Type questType)
        {
            this.questType = questType;
            this.addActionMethod = questType.GetMethod("AddBehaviour", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);            
        }                

        public void AddBehaviour(QuestBehaviour questPart)
        {            
            addActionMethod.Invoke(null, new object[] { questPart });
        }        

        public QuestBehaviour CreateBehaviour(GameNPC npc)
        {
            QuestBehaviour questPart =  new QuestBehaviour(questType, npc);            
            return questPart;
        }

        public QuestBehaviour CreateBehaviour(GameNPC npc, int maxExecutions)
        {
            QuestBehaviour questPart = new QuestBehaviour(questType, npc,maxExecutions);
            return questPart;
        }
    }
}
