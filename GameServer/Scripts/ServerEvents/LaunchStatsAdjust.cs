using System;
using System.Reflection;
using DOL.Database;
using DOL.Events;
using DOL.GS.PacketHandler;
using DOL.GS.Realm;
using log4net;

#region LoginEvent
namespace DOL.GS.GameEvents
{
    public class LaunchRestartStatsScript
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        private static string StatsKey = "StatsAdjust";
        
        [GameServerStartedEvent]
        public static void OnServerStart(CoreEvent e, object sender, EventArgs arguments)
        {
            GameEventMgr.AddHandler(GamePlayerEvent.GameEntered, new CoreEventHandler(PlayerEntered));
        }

        /// <summary>
        /// Event handler fired when server is stopped
        /// </summary>
        [GameServerStoppedEvent]
        public static void OnServerStop(CoreEvent e, object sender, EventArgs arguments)
        {
            GameEventMgr.RemoveHandler(GamePlayerEvent.GameEntered, new CoreEventHandler(PlayerEntered));
        }
        
        /// <summary>
        /// Event handler fired when players enters the game
        /// </summary>
        /// <param name="e"></param>
        /// <param name="sender"></param>
        /// <param name="arguments"></param>
        private static void PlayerEntered(CoreEvent e, object sender, EventArgs arguments)
        {
            if (sender is not GamePlayer player) return;
            
            var secondLaunchDate = new DateTime(2022, 7,4,15,0,0);
            
            //Male chars created after 2nd launch day dont need to be reset.
            if (player.CreationDate > secondLaunchDate && player.Gender == EGender.Male) return;

            var femaleFixDate = new DateTime(2022, 8,9,7,0,0);

            //Characters created after Female Stat Fix Date dont need stats reset.
            if (player.CreationDate > femaleFixDate) return;
            
            var needsAdjustment = CoreDb<DOLCharactersXCustomParam>.SelectObject(DB.Column("DOLCharactersObjectId")
                .IsEqualTo(player.ObjectId).And(DB.Column("KeyName").IsEqualTo("StatsAdjust")));
            
            if (needsAdjustment != null) return;
            
            Log.Warn($"STATSADJUST - {player.Name} ({player.AccountName})");

            var stsMessage = $"STATSADJUST - {player.Name} PREV STATS ";
            stsMessage +=
                $"STR: {player.GetBaseStat(EStat.STR)} CON: {player.GetBaseStat(EStat.CON)} DEX: {player.GetBaseStat(EStat.DEX)} QUI: {player.GetBaseStat(EStat.QUI)} INT: {player.GetBaseStat(EStat.INT)} PIE: {player.GetBaseStat(EStat.PIE)} EMP: {player.GetBaseStat(EStat.EMP)} CHR: {player.GetBaseStat(EStat.CHR)}";
            Log.Warn(stsMessage);

            
            var rBaseStats = RaceStats.GetRaceStats(player.Race);

            var baseStr = (short)rBaseStats.Strength;
            var baseCon = (short)rBaseStats.Constitution;
            var baseDex = (short)rBaseStats.Dexterity;
            var baseQui = (short)rBaseStats.Quickness;
            var baseInt = (short)rBaseStats.Intelligence;
            var basePie = (short)rBaseStats.Piety;
            var baseEmp = (short)rBaseStats.Empathy;
            var baseCha = (short)rBaseStats.Charisma;

            player.ChangeBaseStat(EStat.STR, (short)-player.GetBaseStat(EStat.STR));
            player.ChangeBaseStat(EStat.EMP,(short)-player.GetBaseStat(EStat.EMP));
            player.ChangeBaseStat(EStat.CHR,(short)-player.GetBaseStat(EStat.CHR));
            player.ChangeBaseStat(EStat.PIE,(short)-player.GetBaseStat(EStat.PIE));
            player.ChangeBaseStat(EStat.INT,(short)-player.GetBaseStat(EStat.INT));
            player.ChangeBaseStat(EStat.QUI,(short)-player.GetBaseStat(EStat.QUI));
            player.ChangeBaseStat(EStat.DEX,(short)-player.GetBaseStat(EStat.DEX));
            player.ChangeBaseStat(EStat.CON,(short)-player.GetBaseStat(EStat.CON));
            
            player.ChangeBaseStat(EStat.STR,baseStr);
            player.ChangeBaseStat(EStat.CON,baseCon);
            player.ChangeBaseStat(EStat.DEX,baseDex);
            player.ChangeBaseStat(EStat.QUI,baseQui);
            player.ChangeBaseStat(EStat.INT,baseInt);
            player.ChangeBaseStat(EStat.PIE,basePie);
            player.ChangeBaseStat(EStat.EMP,baseEmp);
            player.ChangeBaseStat(EStat.CHR,baseCha);
            
            for (var i = 6; i <= player.Level ; i++)
            {
                if (player.CharacterClass.PrimaryStat != EStat.UNDEFINED)
                {
                    player.ChangeBaseStat(player.CharacterClass.PrimaryStat, +1);
                }
                if (player.CharacterClass.SecondaryStat != EStat.UNDEFINED && ((i - 6) % 2 == 0))
                {
                    player.ChangeBaseStat(player.CharacterClass.SecondaryStat, +1);
                }
                if (player.CharacterClass.TertiaryStat != EStat.UNDEFINED && ((i - 6) % 3 == 0))
                {
                    player.ChangeBaseStat(player.CharacterClass.TertiaryStat, +1);
                }
            }
            
            stsMessage = $"STATSADJUST - {player.Name} NEW STATS ";
            stsMessage +=
                $"STR: {player.GetBaseStat(EStat.STR)} CON: {player.GetBaseStat(EStat.CON)} DEX: {player.GetBaseStat(EStat.DEX)} QUI: {player.GetBaseStat(EStat.QUI)} INT: {player.GetBaseStat(EStat.INT)} PIE: {player.GetBaseStat(EStat.PIE)} EMP: {player.GetBaseStat(EStat.EMP)} CHR: {player.GetBaseStat(EStat.CHR)}";
            Log.Warn(stsMessage);
            
            Log.Warn($"STATSADJUST - {player.Name} Granted a stats respec");
            player.CustomisationStep = 3;
            player.SaveIntoDatabase();
            
            var adjusted = new DOLCharactersXCustomParam
            {
                DOLCharactersObjectId = player.ObjectId,
                KeyName = "StatsAdjust",
                Value = "1"
            };
            GameServer.Database.AddObject(adjusted);
            
            var message = "Your base stats have been adjusted and you have been granted a free stats respec.\n";
            player.Out.SendDialogBox(EDialogCode.SimpleWarning, 0, 0, 0, 0, EDialogType.Ok, true, message);
            message += "Logout and click on CUSTOMIZE to redistribute your 30 starting points.\n\n";
            
            player.Out.SendCharStatsUpdate();
            
            player.Out.SendMessage(message, EChatType.CT_Important, EChatLoc.CL_SystemWindow);
            player.Out.SendMessage(message, EChatType.CT_Staff, EChatLoc.CL_ChatWindow);
            
            message += "Your new base stats are:\n\n";
            message += $"STR: {player.GetBaseStat(EStat.STR)}\n" +
                       $"CON: {player.GetBaseStat(EStat.CON)}\n" +
                       $"DEX: {player.GetBaseStat(EStat.DEX)}\n" +
                       $"QUI: {player.GetBaseStat(EStat.QUI)}\n" +
                       $"INT: {player.GetBaseStat(EStat.INT)}\n" +
                       $"PIE: {player.GetBaseStat(EStat.PIE)}\n" +
                       $"EMP: {player.GetBaseStat(EStat.EMP)}\n" +
                       $"CHR: {player.GetBaseStat(EStat.CHR)}\n\n";
            
            message += "REMEMBER TO LOGOUT AND CLICK ON CUSTOMIZE TO DISTRIBUTE YOUR 30 STARTING POINTS.";

            player.Out.SendMessage(message, EChatType.CT_Important, EChatLoc.CL_PopupWindow);

        }
    }
}
#endregion