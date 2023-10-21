using DOL.GS.PacketHandler;

namespace DOL.GS.Trainer;

[NpcGuildScript("Druid Trainer", ERealm.Hibernia)]		// this attribute instructs DOL to use this script for all "Druid Trainer" NPC's in Albion (multiple guilds are possible for one script)
public class DruidTrainer : GameTrainer
{
	public override EPlayerClass TrainedClass
	{
		get { return EPlayerClass.Druid; }
	}

	public const string ARMOR_ID1 = "druid_item";

	public DruidTrainer() : base()
	{
	}

	/// <summary>
	/// Interact with trainer
	/// </summary>
	/// <param name="player"></param>
	/// <returns></returns>
	public override bool Interact(GamePlayer player)
	{
		if (!base.Interact(player)) return false;
		
		// check if class matches.
		if (player.PlayerClass.ID == (int) TrainedClass)
		{
			player.Out.SendMessage(this.Name + " says, \"I shall impart all that I know, young Druid.\"", EChatType.CT_Say, EChatLoc.CL_ChatWindow);
		}
		else
		{
			// perhaps player can be promoted
			if (CanPromotePlayer(player))
			{
				player.Out.SendMessage(this.Name + " says, \"Do you wish to walk the Path of Harmony and learn the ways of the [Druid]?\"", EChatType.CT_System, EChatLoc.CL_PopupWindow);
				if (!player.IsLevelRespecUsed)
				{
					OfferRespecialize(player);
				}
			}
			else
			{
				CheckChampionTraining(player);
			}
		}
		return true;
	}

	/// <summary>
	/// Talk to trainer
	/// </summary>
	/// <param name="source"></param>
	/// <param name="text"></param>
	/// <returns></returns>
	public override bool WhisperReceive(GameLiving source, string text)
	{
		if (!base.WhisperReceive(source, text)) return false;
		GamePlayer player = source as GamePlayer;
		
		switch (text) {
			case "Druid":
				// promote player to other class
				if (CanPromotePlayer(player)) {
					PromotePlayer(player, (int)EPlayerClass.Druid, "The path of the Druid suits you, " + source.GetName(0, false) + ". Welcome. Take this, " + source.GetName(0, false) + ". You are a Druid now. Stick to our ways, and you shall go far.", null);
					player.ReceiveItem(this,ARMOR_ID1);
				}
				break;
		}
		return true;
	}
}