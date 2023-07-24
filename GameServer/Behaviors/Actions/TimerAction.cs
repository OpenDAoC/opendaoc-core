using System;
using System.Collections.Generic;
using System.Text;
using DOL.GS.PacketHandler;
using DOL.Events;
using DOL.GS.Behaviour.Attributes;using DOL.GS.Behaviour;
using DOL.Database;

namespace DOL.GS.Behaviour.Actions
{

    [ActionAttribute(ActionType = EActionType.Timer)]
    public class TimerAction : AbstractAction<string,int>
    {
        /// <summary>
        /// Constant used to store timerid in RegionTimer.Properties
        /// </summary>
        const string TIMER_ID = "timerid";
        /// <summary>
        /// Constant used to store GameLiving Source in RegionTimer.Properties
        /// </summary>
        const string TIMER_SOURCE = "timersource";


        public TimerAction(GameNpc defaultNPC,  Object p, Object q)
            : base(defaultNPC, EActionType.Timer, p, q)
        { 
            
        }


        public TimerAction(GameNpc defaultNPC,   string timerID, int delay)
            : this(defaultNPC, (object)timerID,(object) delay) { }
        


        public override void Perform(CoreEvent e, object sender, EventArgs args)
        {
            GamePlayer player = BehaviorUtils.GuessGamePlayerFromNotify(e, sender, args);

            ECSGameTimer timer = new ECSGameTimer(player, new ECSGameTimer.ECSTimerCallback(QuestTimerCallBack));
            timer.Properties.setProperty(TIMER_ID, P);
            timer.Properties.setProperty(TIMER_SOURCE, player);
            timer.Start(Q);
        }

        /// <summary>
        /// Callback for quest internal timers used via eActionType.Timer and eTriggerType.Timer
        /// </summary>
        /// <param name="callingTimer"></param>
        /// <returns>0</returns>
        private static int QuestTimerCallBack(ECSGameTimer callingTimer)
        {
            string timerid = callingTimer.Properties.getProperty<object>(TIMER_ID, null) as string;
            if (timerid == null)
                throw new ArgumentNullException("TimerId out of Range", "timerid");

            GameLiving source = callingTimer.Properties.getProperty<object>(TIMER_SOURCE, null) as GameLiving;
            if (source == null)
                throw new ArgumentNullException("TimerSource null", "timersource");


            TimerEventArgs args = new TimerEventArgs(source, timerid);
            source.Notify(GameLivingEvent.Timer, source, args);

            return 0;
        }
    }
}
