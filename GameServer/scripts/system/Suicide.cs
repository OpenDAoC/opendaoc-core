﻿using System;
using System.Text;
using DOL.Language;
using DOL.GS.PacketHandler;

namespace DOL.GS.Commands
{
    /// <summary>
    /// Command handler for the /killself command
    /// </summary>
    [Command(
        "&suicide",
        EPrivLevel.Admin,
        "Kill yourself. You can't suicide while in combat!")]
    public class KillselfCommandHandler : AbstractCommandHandler, ICommandHandler
    {
        public void OnCommand(GameClient client, string[] args)
        {
            if (client.Player.InCombat)
            {
                DisplayMessage(client, "You can't kill yourself while in combat!");
                return;
            } else if (client.Player.CurrentZone.IsRvR)
            {
                DisplayMessage(client, "There are other ways to die in the Frontiers.");
                return;
            }
            else if (!client.Player.IsAlive)
            {
                DisplayMessage(client, "You are already dead!");
                return;
            }
            else
            {
                client.Out.SendCustomDialog("Do you want kill yourself?", new CustomDialogResponse(SuicideResponceHandler));
            }
        }
        protected virtual void SuicideResponceHandler(GamePlayer player, byte response)
        {
            //int amount = 10000;

            if (response == 1)
            {
                {
                    player.Emote(eEmote.SpellGoBoom);
                    player.TakeDamage(player, EDamageType.Natural, player.MaxHealth, 0);
                }
            }
            else
            {
                return;
            }

        }
    }
}
