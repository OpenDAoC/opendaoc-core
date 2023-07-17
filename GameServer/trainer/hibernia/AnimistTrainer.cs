using System;
using DOL.GS.PacketHandler;
using DOL.Language;

namespace DOL.GS.Trainer
{
	/// <summary>
	/// Animist Trainer
	/// </summary>
	[NPCGuildScript("Animist Trainer", ERealm.Hibernia)]		// this attribute instructs DOL to use this script for all "Animist Trainer" NPC's in Albion (multiple guilds are possible for one script)
	public class AnimistTrainer : GameTrainer
	{
		public override ECharacterClass TrainedClass
		{
			get { return ECharacterClass.Animist; }
		}

		public const string WEAPON_ID1 = "animist_item";

		public AnimistTrainer() : base()
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
				player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "AnimistTrainer.Interact.Text2", this.Name, player.GetName(0, false)), eChatType.CT_Say, eChatLoc.CL_ChatWindow);
			}
			else
			{
				// perhaps player can be promoted
				if (CanPromotePlayer(player))
				{
					player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "AnimistTrainer.Interact.Text1", this.Name), eChatType.CT_System, eChatLoc.CL_PopupWindow);
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
			String lowerCase = text.ToLower();

			if (lowerCase == LanguageMgr.GetTranslation(player.Client.Account.Language, "AnimistTrainer.WhisperReceiveCase.Text1"))
			{
				// promote player to other class
				if (CanPromotePlayer(player))
				{
					PromotePlayer(player, (int)ECharacterClass.Animist, LanguageMgr.GetTranslation(player.Client.Account.Language, "AnimistTrainer.WhisperReceive.Text1", player.GetName(0, false)), null);
					player.ReceiveItem(this, WEAPON_ID1);
				}
			}
			return true;
		}
	}
}
