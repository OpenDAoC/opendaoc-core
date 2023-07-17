using System;
using System.Text;
using DOL.Events;
using DOL.Database;
using log4net;
using System.Reflection;
using DOL.GS.Behaviour.Attributes;
using DOL.GS.Behaviour;

namespace DOL.GS.Behaviour.Triggers
{	
    /// <summary>
    /// A trigger defines the circumstances under which a certain QuestAction is fired.
    /// This can be eTriggerAction.Interact, eTriggerAction.GiveItem, eTriggerAction.Attack, etc...
    /// Additional there are two variables to add the needed parameters for the triggertype (Item to give for GiveItem, NPC to interact for Interact, etc...). To fire a QuestAction at least one of the added triggers must be fulfilled. 
    /// </summary>
    [TriggerAttribute(Global = true,TriggerType = ETriggerType.GiveItem, DefaultValueK = EDefaultValueConstants.NPC)]
    public class GiveItemTrigger : AbstractTrigger<GameNPC,DbItemTemplates>
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
        /// Creates a new questtrigger and does some simple triggertype parameter compatibility checking
		/// </summary>
		/// <param name="defaultNPC"></param>
		/// <param name="notifyHandler"></param>
		/// <param name="k"></param>
		/// <param name="i"></param>
        public GiveItemTrigger(GameNPC defaultNPC, CoreEventHandler notifyHandler,  Object k, Object i)
            : base(defaultNPC,notifyHandler, ETriggerType.GiveItem, k, i)
        { }

        /// <summary>
        /// Creates a new questtrigger and does some simple triggertype parameter compatibility checking
        /// </summary>
        /// <param name="defaultNPC"></param>
        /// <param name="notifyHandler"></param>
        /// <param name="k"></param>
        /// <param name="i"></param>
        public GiveItemTrigger(GameNPC defaultNPC, CoreEventHandler notifyHandler, GameNPC k, DbItemTemplates i)
            : this(defaultNPC,notifyHandler,  (object)k,(object) i)
        { }

        /// <summary>
        /// Checks the trigger, this method is called whenever a event associated with this questparts quest
        /// or a manualy associated eventhandler is notified.
        /// </summary>
        /// <param name="e">DolEvent of notify call</param>
        /// <param name="sender">Sender of notify call</param>
        /// <param name="args">EventArgs of notify call</param>
        /// <returns>true if QuestPart should be executes, else false</returns>
        public override bool Check(CoreEvent e, object sender, EventArgs args)
        {
            bool result = false;
            GamePlayer player = BehaviorUtils.GuessGamePlayerFromNotify(e, sender, args);

            if (e == GamePlayerEvent.ReceiveItem)
            {
                ReceiveItemEventArgs gArgs = (ReceiveItemEventArgs)args;
                
                result = (gArgs.Source == player && gArgs.Item.Name == I.Name);
            }
            
            return result;
        }

		/// <summary>
		/// Registers the needed EventHandler for this Trigger
		/// </summary>
		/// <remarks>
		/// This method will be called multiple times, so use AddHandlerUnique to make
		/// sure only one handler is actually registered
		/// </remarks>
        public override void Register()
        {			
            GameEventMgr.AddHandler(K,GamePlayerEvent.ReceiveItem,NotifyHandler);
        }

		/// <summary>
		/// Unregisters the needed EventHandler for this Trigger
		/// </summary>
		/// <remarks>
		/// Don't remove handlers that will be used by other triggers etc.
		/// This is rather difficult since we don't know which events other triggers use.
		/// </remarks>
        public override void Unregister()
        {
            GameEventMgr.RemoveHandler(K, GamePlayerEvent.ReceiveItem, NotifyHandler);
        }
    }
}
