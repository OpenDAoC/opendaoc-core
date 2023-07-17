﻿
using System;
using DOL.GS.PacketHandler;
using DOL.GS.RealmAbilities;

namespace DOL.GS.Commands
{
	[Command(
		"&resetability",
		EPrivLevel.GM,
		"/resetability - <self|target|group|cg|bg>")]
	
	public class ResetAbilityCommand : AbstractCommandHandler, ICommandHandler
	{
		public void OnCommand(GameClient client, string[] args)
		{
			GamePlayer target = client.Player.TargetObject as GamePlayer;
			BattleGroup bg = (BattleGroup)client.Player.TempProperties.getProperty<object>(BattleGroup.BATTLEGROUP_PROPERTY, null);
			ChatGroup cg = (ChatGroup)client.Player.TempProperties.getProperty<object>(ChatGroup.CHATGROUP_PROPERTY, null);


			if (args.Length < 2)
			{
				DisplaySyntax(client);
				return;
			}

			switch (args[1].ToLower())
			{
					#region group
				case "group":
					{
						if (target != null)
						{
							if (target.Group != null)
							{
								foreach (GamePlayer groupedplayers in client.Player.Group.GetMembersInTheGroup())
								{

									groupedplayers.ResetDisabledSkills();
									groupedplayers.Out.SendMessage(client.Player.Name +" has reset your ability and spell timers!", eChatType.CT_Spell, eChatLoc.CL_ChatWindow);
								}
							}
						}
						else
							client.Player.ResetDisabledSkills();
						client.Player.Out.SendMessage("Target does not have a group so, ability and spell timers have been reset for you!", eChatType.CT_Spell, eChatLoc.CL_ChatWindow);
						break;
					}
					#endregion

					#region chatgrp
				case "cg":
					{
						if (cg != null)
						{
							foreach (GamePlayer cgplayers in cg.Members.Keys)
							{
								cgplayers.ResetDisabledSkills();
								cgplayers.Out.SendMessage(client.Player.Name + " has reset your ability and spell timers!", eChatType.CT_Spell, eChatLoc.CL_ChatWindow);
							}
						}
						else
							client.Player.ResetDisabledSkills();
						client.Player.Out.SendMessage("Target does not have a chatgroup so, ability and spell timers have been reset for you!", eChatType.CT_Spell, eChatLoc.CL_ChatWindow);
						break;
					}
					#endregion

					#region target
				case "target":
					{
						if (target == null)
							target = (GamePlayer)client.Player;
						target.ResetDisabledSkills();
						target.Out.SendMessage("Your ability and spell timers have been reset!", eChatType.CT_Spell, eChatLoc.CL_ChatWindow);
					}
					break;
					#endregion

					#region self
				case "self":
					{
						client.Player.ResetDisabledSkills();
						client.Player.Out.SendMessage("Your ability and spell timers have been reset!", eChatType.CT_Spell, eChatLoc.CL_ChatWindow);
					}
					break;
					#endregion

					#region battlegroup
				case "bg":
					{
						if (target != null)
						{
							if (bg != null)
							{
								foreach (GamePlayer bgplayers in bg.Members.Keys)
								{
									bgplayers.ResetDisabledSkills();
									bgplayers.Out.SendMessage(client.Player.Name + " has reset your ability and spell timers!", eChatType.CT_Spell, eChatLoc.CL_ChatWindow);
								}
							}
						}
						else
							client.Player.ResetDisabledSkills();
						client.Player.Out.SendMessage("Target does not have a battlegroup so, ability and spell timers have been reset for you!", eChatType.CT_Spell, eChatLoc.CL_ChatWindow);
						break;
					}
					#endregion
				default:
					{
						client.Out.SendMessage("'" + args[1] + "' is not a valid arguement.", eChatType.CT_Important, eChatLoc.CL_SystemWindow);
					}
					break;
			}
		}
	}
}