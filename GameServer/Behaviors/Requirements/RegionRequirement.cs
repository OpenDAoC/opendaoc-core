using System;
using System.Text;
using DOL.Events;
using DOL.Database;
using log4net;
using System.Reflection;
using DOL.GS.Behaviour.Attributes;
using DOL.GS.Behaviour;

namespace DOL.GS.Behaviour.Requirements
{

	/// <summary>
	/// Requirements describe what must be true to allow a QuestAction to fire.
	/// Level of player, Step of Quest, Class of Player, etc... There are also some variables to add
	/// additional parameters. To fire a QuestAction ALL requirements must be fulfilled.         
	/// </summary>
    [RequirementAttribute(RequirementType=eRequirementType.Region)]
	public class RegionRequirement : AbstractRequirement<int,int>
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
        /// Creates a new QuestRequirement and does some basich compativilite checks for the parameters
		/// </summary>
		/// <param name="defaultNPC"></param>
		/// <param name="n"></param>
		/// <param name="v"></param>
		/// <param name="comp"></param>
        public RegionRequirement(GameNpc defaultNPC,  Object n, Object v, eComparator comp)
            : base(defaultNPC, eRequirementType.Region, n, v, comp)
		{   			
		}

        /// <summary>
        /// Creates a new QuestRequirement and does some basich compativilite checks for the parameters
        /// </summary>
        /// <param name="defaultNPC"></param>
        /// <param name="n"></param>
        /// <param name="v"></param>
        public RegionRequirement(GameNpc defaultNPC, int n, int v)
            : this(defaultNPC, (object)n, (object)v, eComparator.None)
		{
		}

		/// <summary>
        /// Checks the added requirement whenever a trigger associated with this defaultNPC fires.(returns true)
		/// </summary>
		/// <param name="e"></param>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public override bool Check(CoreEvent e, object sender, EventArgs args)
		{
			bool result = true;
            GamePlayer player = BehaviorUtils.GuessGamePlayerFromNotify(e, sender, args);
    
            //Dinberg on Instances:
            //I've changed this to skin. This is so that instances can cause zone 
            //requirements on quests to be triggered too! It's unlikely they will
            //ever get the right ID otherwise.
            result = (player.CurrentRegion.Skin == V && player.CurrentZone.ZoneSkinID == N);

			return result;
		}

		
    }
}
