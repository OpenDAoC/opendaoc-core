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
    [RequirementAttribute(RequirementType=eRequirementType.InventoryItem, DefaultValueV = 1)]
	public class InventoryItemRequirement : AbstractRequirement<DbItemTemplates,int>
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
        /// Creates a new QuestRequirement and does some basich compativilite checks for the parameters
		/// </summary>
		/// <param name="defaultNPC"></param>
		/// <param name="n"></param>
		/// <param name="v"></param>
		/// <param name="comp"></param>
        public InventoryItemRequirement(GameNPC defaultNPC,  Object n, Object v, eComparator comp)
            : base(defaultNPC, eRequirementType.InventoryItem, n, v, comp)
		{   			
		}

        /// <summary>
		/// Creates a new QuestRequirement and does some basich compativilite checks for the parameters
		/// </summary>
		/// <param name="defaultNPC">Parent defaultNPC of this Requirement</param>		
		/// <param name="n">First Requirement Variable, meaning depends on RequirementType</param>
		/// <param name="v">Second Requirement Variable, meaning depends on RequirementType</param>
		/// <param name="comp">Comparator used if some values are veeing compared</param>
        public InventoryItemRequirement(GameNPC defaultNPC, DbItemTemplates n, int v, eComparator comp)
            : this(defaultNPC,  (object)n, (object)v, comp)
		{   			
		}

		/// <summary>
        /// Checks the added requirement whenever a trigger associated with this defaultNPC fires.(returns true)
		/// </summary>
		/// <param name="e"></param>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public override bool Check(DOLEvent e, object sender, EventArgs args)
		{
			bool result = true;
            GamePlayer player = BehaviorUtils.GuessGamePlayerFromNotify(e, sender, args);
	        					    
		    int count =0;
		    foreach (InventoryItem invItem in player.Inventory.AllItems)
		    {
			    if (invItem.Name == N.Name)
			    {
				    if (invItem.IsStackable)
				    {
					    count += invItem.Count;
				    }
				    else
				    {
					    count++;
				    }
			    }
		    }
		    result = compare(count,V,Comparator);

			return result;
		}

		
    }
}
