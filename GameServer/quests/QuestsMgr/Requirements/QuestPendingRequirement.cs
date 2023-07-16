using System;
using System.Text;
using DOL.Events;
using DOL.Database;
using log4net;
using System.Reflection;
using DOL.GS.Behaviour.Attributes;
using DOL.GS.Behaviour;

namespace DOL.GS.Quests.Requirements
{

	/// <summary>
	/// Requirements describe what must be true to allow a QuestAction to fire.
	/// Level of player, Step of Quest, Class of Player, etc... There are also some variables to add
	/// additional parameters. To fire a QuestAction ALL requirements must be fulfilled.         
	/// </summary>
    [RequirementAttribute(RequirementType=eRequirementType.QuestPending)]
	public class QuestPendingRequirement : AbstractRequirement<Type,Unused>
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
        /// Creates a new QuestRequirement and does some basich compativilite checks for the parameters
		/// </summary>
		/// <param name="defaultNPC"></param>
		/// <param name="n"></param>
		/// <param name="v"></param>
		/// <param name="comp"></param>
        public QuestPendingRequirement(GameNPC defaultNPC, Object n, Object v, eComparator comp)
            : base(defaultNPC,eRequirementType.QuestPending, n, v, comp)
		{   			
		}

        /// <summary>
        /// Creates a new QuestRequirement and does some basich compativilite checks for the parameters
        /// </summary>
        /// <param name="defaultNPC"></param>
        /// <param name="questType"></param>
        /// <param name="comp"></param>
        public QuestPendingRequirement(GameNPC defaultNPC, Type questType, eComparator comp)
            : this(defaultNPC, (object)questType, (object)null, comp)
		{   			
		}

		/// <summary>
        /// Checks the added requirement whenever a trigger associated with this questpart fires.(returns true)
		/// </summary>
		/// <param name="e"></param>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public override bool Check(DOLEvent e, object sender, EventArgs args)
		{
			bool result = true;
            GamePlayer player = BehaviourUtils.GuessGamePlayerFromNotify(e, sender, args);
            
            if (Comparator == eComparator.Not)
                result = player.IsDoingQuest(N) == null;
            else
                result = player.IsDoingQuest(N) != null;

			return result;
		}

		
    }
}
