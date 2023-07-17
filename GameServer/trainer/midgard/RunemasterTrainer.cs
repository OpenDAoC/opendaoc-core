using System;
using DOL.GS.PacketHandler;
using DOL.Language;

namespace DOL.GS.Trainer
{
	/// <summary>
	/// Runemaster Trainer
	/// </summary>
	[NpcGuild("Runemaster Trainer", ERealm.Midgard)]		// this attribute instructs DOL to use this script for all "Runemaster Trainer" NPC's in Albion (multiple guilds are possible for one script)
	public class RunemasterTrainer : GameTrainer
	{
		public override ECharacterClass TrainedClass
		{
			get { return ECharacterClass.Runemaster; }
		}

		public const string WEAPON_ID = "runemaster_item";

		public RunemasterTrainer() : base()
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
			if (player.CharacterClass.ID == (int)TrainedClass)
			{
				OfferTraining(player);
			}
			else
			{
				// perhaps player can be promoted
				if (CanPromotePlayer(player))
				{
					player.Out.SendMessage(this.Name + " says, \"Do you desire to [join the House of Odin] and defend our realm as a Runemaster?\"",eChatType.CT_Say,eChatLoc.CL_PopupWindow);
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
				case "join the House of Odin":
					// promote player to other class
					if (CanPromotePlayer(player)) {
						PromotePlayer(player, (int)ECharacterClass.Runemaster, "Welcome young Runemaster! May your time in Midgard army be rewarding!", null);
						player.ReceiveItem(this, WEAPON_ID);
					}
					break;
			}
			return true;
		}
	}
}
