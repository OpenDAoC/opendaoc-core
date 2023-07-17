﻿/*
*
* Atlas - Dismisses novelty pet
*
*/
using DOL.GS.PacketHandler;
using System.Collections;
using System.Collections.Generic;
using DOL.AI.Brain;
using DOL.Language;
using DOL.GS.Keeps;
using DOL.GS.ServerRules;

namespace DOL.GS.Commands
{
	[Command(
	   "&dismisspet",
	   EPrivLevel.Player,
		 "Dismiss the novelty pet", "/dismisspet")]
	public class DismissPetCommandHandler : AbstractCommandHandler, ICommandHandler
	{
		public void OnCommand(GameClient client, string[] args)
		{
			if (IsSpammingCommand(client.Player, "dismisspet"))
				return;

			if (client.Player.TempProperties.getProperty<bool>(NoveltyPetBrain.HAS_PET, false))
			{
				foreach (GameSummonedPet pet in client.Player.GetNPCsInRadius(500))
				{
					if (pet.Brain is NoveltyPetBrain)
					{
						if (pet.Owner == client.Player)
						{
							pet.RemoveFromWorld();
							client.Player.TempProperties.removeProperty(NoveltyPetBrain.HAS_PET);
							client.Player.MessageToSelf("You have dismissed your companion pet.",EChatType.CT_Spell);
						}
		
					}
				}
			}
			else
			{
				client.Player.MessageToSelf("You have no companion pet.",EChatType.CT_SpellResisted);
			}
		}

	}
}
