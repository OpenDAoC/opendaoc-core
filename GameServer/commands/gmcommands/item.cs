/*
 * DAWN OF LIGHT - The first free open source DAoC server emulator
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */
using System;
using System.Linq;
using System.Collections.Generic;
using Atlas.DataLayer.Models;
using DOL.GS.PacketHandler;
using DOL.GS.PacketHandler.Client.v168;
using DOL.GS.Spells;
using DOL.Language;
using Microsoft.EntityFrameworkCore;

namespace DOL.GS.Commands
{
	[Cmd("&item",
	     ePrivLevel.GM,
	     "GMCommands.Item.Description",
	     "GMCommands.Item.Information",
	     "GMCommands.Item.Usage.Blank",
	     "GMCommands.Item.Usage.Info",
	     "GMCommands.Item.Usage.Create",
	     "GMCommands.Item.Usage.Count",
	     "GMCommands.Item.Usage.MaxCount",
	     "GMCommands.Item.Usage.PackSize",
	     "GMCommands.Item.Usage.Model",
	     "GMCommands.Item.Usage.Extension",
	     "GMCommands.Item.Usage.Color",
	     "GMCommands.Item.Usage.Effect",
	     "GMCommands.Item.Usage.Name",
	     "/item description <Description> <slot> - set the description of this item",
	     "GMCommands.Item.Usage.CrafterName",
	     "GMCommands.Item.Usage.Type",
	     "GMCommands.Item.Usage.Object",
	     "GMCommands.Item.Usage.Hand",
	     "GMCommands.Item.Usage.DamageType",
	     "GMCommands.Item.Usage.Emblem",
	     "GMCommands.Item.Usage.Price",
	     "GMCommands.Item.Usage.Condition",
	     "GMCommands.Item.Usage.Quality",
	     "GMCommands.Item.Usage.Durability",
	     "GMCommands.Item.Usage.isPickable",
	     "GMCommands.Item.Usage.IsNotLosingDUR",
	     "GMCommands.Item.Usage.IsIndestructible",
	     "GMCommands.Item.Usage.isDropable",
	     "GMCommands.Item.Usage.IsTradable",
	     "GMCommands.Item.Usage.IsStackable",
	     "GMCommands.Item.Usage.CanDropAsLoot",
	     "GMCommands.Item.Usage.Bonus",
	     "GMCommands.Item.Usage.mBonus",
	     "GMCommands.Item.Usage.Weight",
	     "GMCommands.Item.Usage.DPS_AF",
	     "GMCommands.Item.Usage.SPD_ABS",
	     "GMCommands.Item.Usage.Material",
	     "GMCommands.Item.Usage.Scroll",
	     "GMCommands.Item.Usage.Spell",
	     "GMCommands.Item.Usage.Spell1",
	     "GMCommands.Item.Usage.Proc",
	     "GMCommands.Item.Usage.Proc1",
	     "/item procchance <chance>",
	     "GMCommands.Item.Usage.Poison",
	     "GMCommands.Item.Usage.Realm",
	     "/item classtype <ClassType> <slot> - Set this items ClassType",
	     "/item packageid <PackageID> <slot> - Set this items PackageID",
	     "/item levelrequired <level> <slot> - Set the required level needed to use spells and procs on this item",
	     "/item bonuslevel <level> <slot> - Set the level required for item bonuses to effect player",
	     "/item flags <flags> <slot> - Set the flags for this item",
	     "/item classes <csv_allowed_classes> <slot> - Set and replace the Allowed Classes field (0 for everybody)",
	     "/item salvageid <SalvageYield ID> <slot> - Set the SalvageYieldID for this item",
	     "/item salvageinfo <SalvageYield ID> <slot> - Show the salvage yield for this item",
	     "/item update <slot> - Changes to this item will also be made to the ItemTemplate and can be saved in the DB.",
	     "/item save <TemplateID> [slot #]' - Create a new template or save an existing one",
	     "/item addunique <id_nb> <slot> - save item as an unique one",
	     "/item saveunique <id_nb> <slot> - update a unique item",
	     "GMCommands.Item.Usage.FindID",
	     "GMCommands.Item.Usage.FindName",
	     "/item load <id_nb> - Load an item from the DB and replace or add item to the ItemTemplate cache",
	     "/item loadartifacts - Re-load all the artifact entries from the DB.  ItemTemplates must be loaded separately and prior to loading artifacts.",
	     "/item loadpackage <packageid> | **all** - Load all the items in a package from the DB and replace or add to the ItemTemplate cache. **all** is loading all items [! SLOW !]",
	     "/item loadspells - Read each item spell from the database and update the global spell list")]
	public class ItemCommandHandler : AbstractCommandHandler, ICommandHandler
	{
		private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public void OnCommand(GameClient client, string[] args)
		{
			if (args.Length < 2)
			{
				DisplaySyntax(client);
				return;
			}

			try
			{
				switch (args[1].ToLower())
				{
						#region Blank
					case "blank":
						{
                            ItemTemplate newTemplate = new ItemTemplate
                            {
                                Name = "(blank item)",
                                KeyName = InventoryItem.BLANK_ITEM
                            };
                            GameInventoryItem item = new GameInventoryItem(newTemplate);
							if (client.Player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, item))
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Blank.ItemCreated"), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								InventoryLogging.LogInventoryAction(client.Player, client.Player, eInventoryActionType.Other, item.ItemTemplate, item.Count);
							}
							else
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Blank.CreationError"), eChatType.CT_System, eChatLoc.CL_SystemWindow);
							}
							break;
						}
						#endregion Blank
						#region Scroll
					case "scroll":
						{
							WorldInventoryItem scroll = ArtifactMgr.CreateScroll(Convert.ToInt32(args[2]), Convert.ToInt16(args[3]));
							if (scroll == null)
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Scroll.NotFound", args[3], args[2]), eChatType.CT_SpellResisted, eChatLoc.CL_SystemWindow);
								return;
							}
							if (client.Player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, scroll.Item))
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Scroll.Created", scroll.Item.Name), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								InventoryLogging.LogInventoryAction(client.Player, client.Player, eInventoryActionType.Other, scroll.Item.ItemTemplate, scroll.Item.Count);
							}
							break;
						}
						#endregion Scroll
						#region Classes
					case "classes":
						{
							int slot = (int)eInventorySlot.LastBackpack;

							if (args.Length >= 4)
							{
								slot = Convert.ToInt32(args[3]);
							}

							InventoryItem item = client.Player.Inventory.GetItem((eInventorySlot)slot);

							if (item == null)
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Count.NoItemInSlot", slot), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return;
							}
							
							item.ItemTemplate.AllowedClasses = args[2].Trim();
							break;
						}
						#endregion
						#region Create
					case "create":
						{
							ItemTemplate template = GameServer.Database.ItemTemplates.FirstOrDefault(x => x.Id.ToString() == args[2] || x.KeyName == args[2]);
							if (template == null)
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Create.NotFound", args[2]), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return;
							}
							else
							{
								int count = 1;
								if (args.Length >= 4)
								{
									try
									{
										count = Convert.ToInt32(args[3]);
										if (count < 1)
											count = 1;
									}
									catch (Exception)
									{
									}
								}

								InventoryItem item = GameInventoryItem.Create(template);
								if (item.ItemTemplate.IsStackable)
								{
									item.Count = count;
								}
								if (client.Player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, item))
								{
									client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Create.Created", item.ItemTemplate.Level, item.ItemTemplate.GetName(0, false), count), eChatType.CT_System, eChatLoc.CL_SystemWindow);
									InventoryLogging.LogInventoryAction(client.Player, client.Player, eInventoryActionType.Other, item.ItemTemplate, item.Count);
								}
							}
							break;
						}
						#endregion Create
						#region Count
					case "count":
						{
							int slot = (int)eInventorySlot.LastBackpack;

							if (args.Length >= 4)
							{
								slot = Convert.ToInt32(args[3]);
							}

							InventoryItem item = client.Player.Inventory.GetItem((eInventorySlot)slot);

							if (item == null)
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Count.NoItemInSlot", slot), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return;
							}
							if (!item.ItemTemplate.IsStackable)
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Count.NotStackable", item.ItemTemplate.GetName(0, true)), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return;
							}
							if (Convert.ToInt32(args[2]) < 1)
							{
								item.Count = 1;
							}
							else
							{
								item.Count = Convert.ToInt32(args[2]);
							}

							client.Out.SendInventoryItemsUpdate(new InventoryItem[] { item });
							client.Player.UpdateEncumberance();
							break;
						}
						#endregion Count
						#region MaxCount
					case "maxcount":
						{
							int slot = (int)eInventorySlot.LastBackpack;

							if (args.Length >= 4)
							{
								slot = Convert.ToInt32(args[3]);
							}

							InventoryItem item = client.Player.Inventory.GetItem((eInventorySlot)slot);

							if (item == null)
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Count.NoItemInSlot", slot), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return;
							}
							item.ItemTemplate.MaxCount = Convert.ToInt32(args[2]);
							if (item.ItemTemplate.MaxCount < 1)
								item.ItemTemplate.MaxCount = 1;
							break;
						}
						#endregion MaxCount
						#region PackSize
					case "packsize":
						{
							int slot = (int)eInventorySlot.LastBackpack;

							if (args.Length >= 4)
							{
								slot = Convert.ToInt32(args[3]);
							}

							InventoryItem item = client.Player.Inventory.GetItem((eInventorySlot)slot);

							if (item == null)
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Count.NoItemInSlot", slot), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return;
							}
							item.ItemTemplate.PackSize = Convert.ToInt32(args[2]);
							if (item.ItemTemplate.PackSize < 1)
								item.ItemTemplate.PackSize = 1;
							break;
						}
						#endregion PackSize
						#region Info
					case "info":
						{
							ItemTemplate obj = GameServer.Database.ItemTemplates.FirstOrDefault(x => x.Id.ToString() == args[2] || x.KeyName == args[2]);
							if (obj == null)
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Info.ItemTemplateUnknown", args[2]), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return;
							}
							GameInventoryItem invItem = GameInventoryItem.Create(obj);
							var objectInfo = new List<string>();
                            invItem.WriteTechnicalInfo(objectInfo, client);
							client.Out.SendCustomTextWindow(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Info.Informations", obj.Id), objectInfo);
							break;
						}
						#endregion Info
						#region Model
					case "model":
						{
							int slot = (int)eInventorySlot.LastBackpack;
							if (args.Length >= 4)
							{
								try
								{
									slot = Convert.ToInt32(args[3]);
								}
								catch
								{
									slot = (int)eInventorySlot.LastBackpack;
								}
							}
							InventoryItem item = client.Player.Inventory.GetItem((eInventorySlot)slot);
							if (item == null)
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Count.NoItemInSlot", slot), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return;
							}
							item.Model = Convert.ToUInt16(args[2]);
							client.Out.SendInventoryItemsUpdate(new InventoryItem[] { item });
							if (item.SlotPosition < (int)eInventorySlot.FirstBackpack)
								client.Player.UpdateEquipmentAppearance();
							break;
						}
						#endregion Model
						#region Extension
					case "extension":
						{
							int slot = (int)eInventorySlot.LastBackpack;
							if (args.Length >= 4)
							{
								try
								{
									slot = Convert.ToInt32(args[3]);
								}
								catch
								{
									slot = (int)eInventorySlot.LastBackpack;
								}
							}
							InventoryItem item = client.Player.Inventory.GetItem((eInventorySlot)slot);
							if (item == null)
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Count.NoItemInSlot", slot), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return;
							}
							item.Extension = Convert.ToByte(args[2]);

							if (item.ItemTemplate is ItemUnique || (item.ItemTemplate is ItemTemplate && (item.ItemTemplate as ItemTemplate).AllowUpdate))
							{
								item.ItemTemplate.Extension = item.Extension;
							}

							client.Out.SendInventoryItemsUpdate(new InventoryItem[] { item });
							if (item.SlotPosition < (int)eInventorySlot.FirstBackpack)
								client.Player.UpdateEquipmentAppearance();
							break;
						}
						#endregion Extension
						#region Color
					case "color":
						{
							int slot = (int)eInventorySlot.LastBackpack;
							if (args.Length >= 4)
							{
								try
								{
									slot = Convert.ToInt32(args[3]);
								}
								catch
								{
									slot = (int)eInventorySlot.LastBackpack;
								}
							}
							InventoryItem item = client.Player.Inventory.GetItem((eInventorySlot)slot);
							if (item == null)
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Count.NoItemInSlot", slot), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return;
							}
							item.Color = Convert.ToUInt16(args[2]);

							if (item.ItemTemplate is ItemUnique || (item.ItemTemplate is ItemTemplate && (item.ItemTemplate as ItemTemplate).AllowUpdate))
							{
								item.ItemTemplate.Color = item.Color;
							}

							client.Out.SendInventoryItemsUpdate(new InventoryItem[] { item });
							if (item.SlotPosition < (int)eInventorySlot.FirstBackpack)
								client.Player.UpdateEquipmentAppearance();
							break;
						}
						#endregion Color
						#region Effect
					case "effect":
						{
							int slot = (int)eInventorySlot.LastBackpack;
							if (args.Length >= 4)
							{
								try
								{
									slot = Convert.ToInt32(args[3]);
								}
								catch
								{
									slot = (int)eInventorySlot.LastBackpack;
								}
							}
							InventoryItem item = client.Player.Inventory.GetItem((eInventorySlot)slot);
							if (item == null)
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Count.NoItemInSlot", slot), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return;
							}
							item.ItemTemplate.Effect = Convert.ToUInt16(args[2]);
							client.Out.SendInventoryItemsUpdate(new InventoryItem[] { item });
							if (item.SlotPosition < (int)eInventorySlot.FirstBackpack)
								client.Player.UpdateEquipmentAppearance();
							break;
						}
						#endregion Effect
						#region Type
					case "type":
						{
							int slot = (int)eInventorySlot.LastBackpack;
							if (args.Length >= 4)
							{
								try
								{
									slot = Convert.ToInt32(args[3]);
								}
								catch
								{
									slot = (int)eInventorySlot.LastBackpack;
								}
							}
							InventoryItem item = client.Player.Inventory.GetItem((eInventorySlot)slot);
							if (item == null)
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Count.NoItemInSlot", slot), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return;
							}
							item.ItemTemplate.ItemType = Convert.ToInt32(args[2]);
							client.Out.SendInventoryItemsUpdate(new InventoryItem[] { item });
							break;
						}
						#endregion Type
						#region Object
					case "object":
						{
							int slot = (int)eInventorySlot.LastBackpack;
							if (args.Length >= 4)
							{
								try
								{
									slot = Convert.ToInt32(args[3]);
								}
								catch
								{
									slot = (int)eInventorySlot.LastBackpack;
								}
							}
							InventoryItem item = client.Player.Inventory.GetItem((eInventorySlot)slot);
							if (item == null)
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Count.NoItemInSlot", slot), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return;
							}
							item.ItemTemplate.ObjectType = Convert.ToInt32(args[2]);
							client.Out.SendInventoryItemsUpdate(new InventoryItem[] { item });
							break;
						}
						#endregion Object
						#region Hand
					case "hand":
						{
							int slot = (int)eInventorySlot.LastBackpack;
							if (args.Length >= 4)
							{
								try
								{
									slot = Convert.ToInt32(args[3]);
								}
								catch
								{
									slot = (int)eInventorySlot.LastBackpack;
								}
							}
							InventoryItem item = client.Player.Inventory.GetItem((eInventorySlot)slot);
							if (item == null)
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Count.NoItemInSlot", slot), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return;
							}
							item.ItemTemplate.Hand = Convert.ToInt32(args[2]);
							client.Out.SendInventoryItemsUpdate(new InventoryItem[] { item });
							break;
						}
						#endregion Hand
						#region DamageType
					case "damagetype":
						{
							int slot = (int)eInventorySlot.LastBackpack;
							if (args.Length >= 4)
							{
								try
								{
									slot = Convert.ToInt32(args[3]);
								}
								catch
								{
									slot = (int)eInventorySlot.LastBackpack;
								}
							}
							InventoryItem item = client.Player.Inventory.GetItem((eInventorySlot)slot);
							if (item == null)
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Count.NoItemInSlot", slot), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return;
							}
							item.ItemTemplate.TypeDamage = Convert.ToInt32(args[2]);
							client.Out.SendInventoryItemsUpdate(new InventoryItem[] { item });
							break;
						}
						#endregion DamageType
						#region Name
					case "name":
						{
							string name = args[2];
							int slot = (int)eInventorySlot.LastBackpack;

							if (int.TryParse(args[args.Length - 1], out slot))
							{
								name = string.Join(" ", args, 2, args.Length - 3);
							}
							else
							{
								name = string.Join(" ", args, 2, args.Length - 2);
								slot = (int)eInventorySlot.LastBackpack;
							}

							InventoryItem item = client.Player.Inventory.GetItem((eInventorySlot)slot);
							if (item == null)
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Count.NoItemInSlot", slot), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return;
							}

							item.Name = name;
							client.Out.SendInventoryItemsUpdate(new InventoryItem[] { item });
							break;
						}
						#endregion Name
						#region Description
					case "description":
						{
							string desc = args[2];
							int slot = (int)eInventorySlot.LastBackpack;

							if (int.TryParse(args[args.Length - 1], out slot))
							{
								desc = string.Join(" ", args, 2, args.Length - 3);
							}
							else
							{
								desc = string.Join(" ", args, 2, args.Length - 2);
								slot = (int)eInventorySlot.LastBackpack;
							}

							InventoryItem item = client.Player.Inventory.GetItem((eInventorySlot)slot);
							if (item == null)
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Count.NoItemInSlot", slot), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return;
							}

							item.ItemTemplate.Description = desc;
							client.Out.SendInventoryItemsUpdate(new InventoryItem[] { item });
							break;
						}
						#endregion Name
						#region CrafterName
					case "craftername":
						{
							int slot = (int)eInventorySlot.LastBackpack;
							if (args.Length >= 4)
							{
								try
								{
									slot = Convert.ToInt32(args[3]);
								}
								catch
								{
									slot = (int)eInventorySlot.LastBackpack;
								}
							}
							InventoryItem item = client.Player.Inventory.GetItem((eInventorySlot)slot);
							if (item == null)
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Count.NoItemInSlot", slot), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return;
							}
							item.IsCrafted = true;
							item.Creator = args[2];
							client.Out.SendInventoryItemsUpdate(new InventoryItem[] { item });
							break;
						}
						#endregion CrafterName
						#region Emblem
					case "emblem":
						{
							int slot = (int)eInventorySlot.LastBackpack;
							if (args.Length >= 4)
							{
								try
								{
									slot = Convert.ToInt32(args[3]);
								}
								catch
								{
									slot = (int)eInventorySlot.LastBackpack;
								}
							}
							InventoryItem item = client.Player.Inventory.GetItem((eInventorySlot)slot);
							if (item == null)
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Count.NoItemInSlot", slot), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return;
							}
							item.Emblem = Convert.ToInt32(args[2]);

							if (item.ItemTemplate is ItemUnique || (item.ItemTemplate is ItemTemplate && (item.ItemTemplate as ItemTemplate).AllowUpdate))
							{
								item.ItemTemplate.Emblem = item.Emblem;
							}

							client.Out.SendInventoryItemsUpdate(new InventoryItem[] { item });
							if (item.SlotPosition < (int)eInventorySlot.FirstBackpack)
								client.Player.UpdateEquipmentAppearance();
							break;
						}
						#endregion Emblem
						#region Level
					case "level":
						{
							int slot = (int)eInventorySlot.LastBackpack;
							if (args.Length >= 4)
							{
								try
								{
									slot = Convert.ToInt32(args[3]);
								}
								catch
								{
									slot = (int)eInventorySlot.LastBackpack;
								}
							}
							InventoryItem item = client.Player.Inventory.GetItem((eInventorySlot)slot);
							if (item == null)
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Count.NoItemInSlot", slot), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return;
							}
							item.ItemTemplate.Level = Convert.ToUInt16(args[2]);
							client.Out.SendInventoryItemsUpdate(new InventoryItem[] { item });
							break;
						}
						#endregion Level
						#region Price
					case "price":
						{
							int slot = (int)eInventorySlot.LastBackpack;
							if (args.Length >= 7)
							{
								try
								{
									slot = Convert.ToInt32(args[6]);
								}
								catch
								{
									slot = (int)eInventorySlot.LastBackpack;
								}
							}
							InventoryItem item = client.Player.Inventory.GetItem((eInventorySlot)slot);
							if (item == null)
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Count.NoItemInSlot", slot), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return;
							}
							item.ItemTemplate.Price = Money.GetMoney(0, (int)(Convert.ToInt16(args[2]) % 1000), (int)(Convert.ToInt16(args[3]) % 1000), (int)(Convert.ToByte(args[4]) % 100), (int)(Convert.ToByte(args[5]) % 100));
							client.Out.SendInventoryItemsUpdate(new InventoryItem[] { item });
							break;
						}
						#endregion Price
						#region Condition
					case "condition":
						{
							int slot = (int)eInventorySlot.LastBackpack;
							if (args.Length >= 5)
							{
								try
								{
									slot = Convert.ToInt32(args[4]);
								}
								catch
								{
									slot = (int)eInventorySlot.LastBackpack;
								}
							}
							InventoryItem item = client.Player.Inventory.GetItem((eInventorySlot)slot);
							if (item == null)
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Count.NoItemInSlot", slot), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return;
							}
							
							UpdateAllowed(item, client);
							int con = Convert.ToInt32(args[2]);
							int maxcon = Convert.ToInt32(args[3]);
							item.Condition = con;
							item.ItemTemplate.MaxCondition = maxcon;
							if (item.ItemTemplate is ItemUnique || (item.ItemTemplate is ItemTemplate && (item.ItemTemplate as ItemTemplate).AllowUpdate))
							{
								item.ItemTemplate.Condition = item.Condition;
							}
							client.Out.SendInventoryItemsUpdate(new InventoryItem[] { item });
							break;
						}
						#endregion Condition
						#region Durability
					case "durability":
						{
							int slot = (int)eInventorySlot.LastBackpack;
							if (args.Length >= 5)
							{
								try
								{
									slot = Convert.ToInt32(args[4]);
								}
								catch
								{
									slot = (int)eInventorySlot.LastBackpack;
								}
							}
							InventoryItem item = client.Player.Inventory.GetItem((eInventorySlot)slot);
							if (item == null)
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Count.NoItemInSlot", slot), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return;
							}
							
							UpdateAllowed(item, client);
							int Dur = Convert.ToInt32(args[2]);
							int MaxDur = Convert.ToInt32(args[3]);
							item.Durability = Dur;
							if (item.ItemTemplate is ItemUnique || (item.ItemTemplate is ItemTemplate && (item.ItemTemplate as ItemTemplate).AllowUpdate))
							{
								item.ItemTemplate.Durability = item.Durability;
							}
							item.ItemTemplate.MaxDurability = MaxDur;
							client.Out.SendInventoryItemsUpdate(new InventoryItem[] { item });
							break;
						}
						#endregion Durability
						#region Quality
					case "quality":
						{
							int slot = (int)eInventorySlot.LastBackpack;
							if (args.Length >= 4)
							{
								try
								{
									slot = Convert.ToInt32(args[3]);
								}
								catch
								{
									slot = (int)eInventorySlot.LastBackpack;
								}
							}
							InventoryItem item = client.Player.Inventory.GetItem((eInventorySlot)slot);
							if (item == null)
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Count.NoItemInSlot", slot), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return;
							}
							int Qua = Convert.ToInt32(args[2]);
							item.ItemTemplate.Quality = Qua;
							client.Out.SendInventoryItemsUpdate(new InventoryItem[] { item });
							break;
						}
						#endregion Quality
						#region Bonus
					case "bonus":
						{
							int slot = (int)eInventorySlot.LastBackpack;
							if (args.Length >= 4)
							{
								try
								{
									slot = Convert.ToInt32(args[3]);
								}
								catch
								{
									slot = (int)eInventorySlot.LastBackpack;
								}
							}
							InventoryItem item = client.Player.Inventory.GetItem((eInventorySlot)slot);
							if (item == null)
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Count.NoItemInSlot", slot), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return;
							}
							int Bonus = Convert.ToInt32(args[2]);
							item.ItemTemplate.ItemBonus = Bonus;
							client.Out.SendInventoryItemsUpdate(new InventoryItem[] { item });
							break;
						}
						#endregion Bonus
						#region mBonus
					case "mbonus":
						{
							int slot = (int)eInventorySlot.LastBackpack;
							int num = 0;
							int bonusType = 0;
							int bonusValue = 0;
							if (args.Length >= 6)
							{
								try
								{
									slot = Convert.ToInt32(args[5]);
								}
								catch
								{
									slot = (int)eInventorySlot.LastBackpack;
								}
							}
							InventoryItem item = client.Player.Inventory.GetItem((eInventorySlot)slot);
							if (item == null)
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Count.NoItemInSlot", slot), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return;
							}
							try
							{
								num = Convert.ToInt32(args[2]);
							}
							catch
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.mBonus.NonSetBonusNumber"), eChatType.CT_System, eChatLoc.CL_SystemWindow);
							}
							try
							{
								bonusType = Convert.ToInt32(args[3]);
								if (bonusType < 0 || bonusType >= (int)eProperty.MaxProperty)
								{
									client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.mBonus.TypeShouldBeInRange", (int)(eProperty.MaxProperty - 1)), eChatType.CT_System, eChatLoc.CL_SystemWindow);
									break;
								}
							}
							catch
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.mBonus.NonSetBonusType"), eChatType.CT_System, eChatLoc.CL_SystemWindow);
							}
							try
							{
								if (num < 0 || num > 10)
                                {
									client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.mBonus.UnknownBonusNumber", num), eChatType.CT_System, eChatLoc.CL_SystemWindow);
									return;
								}
									

								bonusValue = Convert.ToInt32(args[4]);

								var bonus = item.Bonuses.FirstOrDefault(x => x.BonusOrder == num);

								if (bonus == null)
                                {
									bonus = new ItemBonus() { BonusOrder = num };
									item.Bonuses.Add(bonus);
                                }
								bonus.BonusValue = bonusValue;
								bonus.BonusType = bonusType;
								
								if (item.SlotPosition < (int)eInventorySlot.FirstBackpack)
								{
									client.Out.SendCharStatsUpdate();
									client.Out.SendCharResistsUpdate();
								}
							}
							catch
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.mBonus.NotSetBonusValue"), eChatType.CT_System, eChatLoc.CL_SystemWindow);
							}
							break;
						}
						#endregion mBonus
						#region Weight
					case "weight":
						{
							int slot = (int)eInventorySlot.LastBackpack;
							if (args.Length >= 4)
							{
								try
								{
									slot = Convert.ToInt32(args[3]);
								}
								catch
								{
									slot = (int)eInventorySlot.LastBackpack;
								}
							}
							InventoryItem item = client.Player.Inventory.GetItem((eInventorySlot)slot);
							if (item == null)
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Count.NoItemInSlot", slot), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return;
							}
							item.ItemTemplate.Weight = Convert.ToInt32(args[2]);
							client.Out.SendInventoryItemsUpdate(new InventoryItem[] { item });
							break;
						}
						#endregion Weight
						#region DPS_AF - DPS - AF
					case "dps_af":
					case "dps":
					case "af":
						{
							int slot = (int)eInventorySlot.LastBackpack;
							if (args.Length >= 4)
							{
								try
								{
									slot = Convert.ToInt32(args[3]);
								}
								catch
								{
									slot = (int)eInventorySlot.LastBackpack;
								}
							}
							InventoryItem item = client.Player.Inventory.GetItem((eInventorySlot)slot);
							if (item == null)
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Count.NoItemInSlot", slot), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return;
							}
							item.ItemTemplate.DPS_AF = Convert.ToByte(args[2]);
							client.Out.SendInventoryItemsUpdate(new InventoryItem[] { item });
							break;
						}
						#endregion DPS_AF - DPS - AF
						#region SPD_ABS - SPD - ABS
					case "spd_abs":
					case "spd":
					case "abs":
						{
							int slot = (int)eInventorySlot.LastBackpack;
							if (args.Length >= 4)
							{
								try
								{
									slot = Convert.ToInt32(args[3]);
								}
								catch
								{
									slot = (int)eInventorySlot.LastBackpack;
								}
							}
							InventoryItem item = client.Player.Inventory.GetItem((eInventorySlot)slot);
							if (item == null)
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Count.NoItemInSlot", slot), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return;
							}
							item.ItemTemplate.SPD_ABS = Convert.ToByte(args[2]);
							client.Out.SendInventoryItemsUpdate(new InventoryItem[] { item });
							break;
						}
						#endregion SPD_ABS - SPD - ABS
						#region IsDropable
					case "isdropable":
						{
							int slot = (int)eInventorySlot.LastBackpack;
							if (args.Length >= 4)
							{
								try
								{
									slot = Convert.ToInt32(args[3]);
								}
								catch
								{
									slot = (int)eInventorySlot.LastBackpack;
								}
							}
							InventoryItem item = client.Player.Inventory.GetItem((eInventorySlot)slot);
							if (item == null)
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Count.NoItemInSlot", slot), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return;
							}
							item.ItemTemplate.IsDropable = Convert.ToBoolean(args[2]);
							break;
						}
						#endregion IsDropable
						#region IsPickable
					case "ispickable":
						{
							int slot = (int)eInventorySlot.LastBackpack;
							if (args.Length >= 4)
							{
								try
								{
									slot = Convert.ToInt32(args[3]);
								}
								catch
								{
									slot = (int)eInventorySlot.LastBackpack;
								}
							}
							InventoryItem item = client.Player.Inventory.GetItem((eInventorySlot)slot);
							if (item == null)
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Count.NoItemInSlot", slot), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return;
							}
							item.ItemTemplate.IsPickable = Convert.ToBoolean(args[2]);
							break;
						}
						#endregion IsPickable
						#region IsNotLosingDur
					case "isnotlosingdur":
						{
							int slot = (int)eInventorySlot.LastBackpack;
							if (args.Length >= 4)
							{
								try
								{
									slot = Convert.ToInt32(args[3]);
								}
								catch
								{
									slot = (int)eInventorySlot.LastBackpack;
								}
							}
							InventoryItem item = client.Player.Inventory.GetItem((eInventorySlot)slot);
							if (item == null)
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Count.NoItemInSlot", slot), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return;
							}
							item.ItemTemplate.IsNotLosingDur = Convert.ToBoolean(args[2]);
							break;
						}
						#endregion IsNotLosingDur
						#region IsIndestructible
					case "isindestructible":
						{
							int slot = (int)eInventorySlot.LastBackpack;
							if (args.Length >= 4)
							{
								try
								{
									slot = Convert.ToInt32(args[3]);
								}
								catch
								{
									slot = (int)eInventorySlot.LastBackpack;
								}
							}
							InventoryItem item = client.Player.Inventory.GetItem((eInventorySlot)slot);
							if (item == null)
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Count.NoItemInSlot", slot), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return;
							}
							item.ItemTemplate.IsIndestructible = Convert.ToBoolean(args[2]);
							break;
						}
						#endregion IsIndestructible
						#region IsTradable
					case "istradable":
						{
							int slot = (int)eInventorySlot.LastBackpack;
							if (args.Length >= 4)
							{
								try
								{
									slot = Convert.ToInt32(args[3]);
								}
								catch
								{
									slot = (int)eInventorySlot.LastBackpack;
								}
							}
							InventoryItem item = client.Player.Inventory.GetItem((eInventorySlot)slot);
							if (item == null)
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Count.NoItemInSlot", slot), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return;
							}
							item.ItemTemplate.IsTradable = Convert.ToBoolean(args[2]);
							break;
						}
						#endregion IsTradable
						#region CanDropAsLoot
					case "candropasloot":
						{
							int slot = (int)eInventorySlot.LastBackpack;
							if (args.Length >= 4)
							{
								try
								{
									slot = Convert.ToInt32(args[3]);
								}
								catch
								{
									slot = (int)eInventorySlot.LastBackpack;
								}
							}
							InventoryItem item = client.Player.Inventory.GetItem((eInventorySlot)slot);
							if (item == null)
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Count.NoItemInSlot", slot), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return;
							}
							item.ItemTemplate.CanDropAsLoot = Convert.ToBoolean(args[2]);
							break;
						}
						#endregion CanDropAsLoot
						#region Spell
					case "spell":
						{
							int slot = (int)eInventorySlot.LastBackpack;
							if (args.Length >= 6)
							{
								try
								{
									slot = Convert.ToInt32(args[5]);
								}
								catch
								{
									slot = (int)eInventorySlot.LastBackpack;
								}
							}
							InventoryItem item = client.Player.Inventory.GetItem((eInventorySlot)slot);
							if (item == null)
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Count.NoItemInSlot", slot), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return;
							}
							int Charges = Convert.ToInt32(args[2]);
							int MaxCharges = Convert.ToInt32(args[3]);
							int SpellID = Convert.ToInt32(args[4]);

							var spell = item.Spells.FirstOrDefault(x => x.ProcChance <= 0 && !x.IsPoison);

							if (spell == null)
                            {
								spell = new InventoryItemSpell();
								item.Spells.Add(spell);
                            }
							spell.Charges = Charges;
							spell.MaxCharges = MaxCharges;
							spell.SpellID = SpellID;

							if (item.ItemTemplate is ItemUnique || (item.ItemTemplate is ItemTemplate && (item.ItemTemplate as ItemTemplate).AllowUpdate))
							{
								var templateSpell = item.ItemTemplate.Spells.FirstOrDefault(x => x.ProcChance <= 0 && !x.IsPoison);

								if (templateSpell == null)
								{
									templateSpell = new ItemSpell();
									item.ItemTemplate.Spells.Add(templateSpell);
								}
								templateSpell.Charges = Charges;
								templateSpell.MaxCharges = MaxCharges;
								templateSpell.SpellID = SpellID;
							}
							
							client.Out.SendInventoryItemsUpdate(new InventoryItem[] { item });
							break;
						}
						#endregion Spell
						#region Spell1
					case "spell1":
						{
							int slot = (int)eInventorySlot.LastBackpack;
							if (args.Length >= 6)
							{
								try
								{
									slot = Convert.ToInt32(args[5]);
								}
								catch
								{
									slot = (int)eInventorySlot.LastBackpack;
								}
							}
							InventoryItem item = client.Player.Inventory.GetItem((eInventorySlot)slot);
							if (item == null)
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Count.NoItemInSlot", slot), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return;
							}
							int Charges = Convert.ToInt32(args[2]);
							int MaxCharges = Convert.ToInt32(args[3]);
							int SpellID = Convert.ToInt32(args[4]);

							var spell = item.Spells.Where(x => x.ProcChance <= 0 && !x.IsPoison).Skip(1).FirstOrDefault();

							if (spell == null)
							{
								spell = new InventoryItemSpell();
								item.Spells.Add(spell);
							}
							spell.Charges = Charges;
							spell.MaxCharges = MaxCharges;
							spell.SpellID = SpellID;

							if (item.ItemTemplate is ItemUnique || (item.ItemTemplate is ItemTemplate && (item.ItemTemplate as ItemTemplate).AllowUpdate))
							{
								var templateSpell = item.ItemTemplate.Spells.FirstOrDefault(x => x.ProcChance <= 0 && !x.IsPoison);

								if (templateSpell == null)
								{
									templateSpell = new ItemSpell();
									item.ItemTemplate.Spells.Add(templateSpell);
								}
								templateSpell.Charges = Charges;
								templateSpell.MaxCharges = MaxCharges;
								templateSpell.SpellID = SpellID;
							}
							client.Out.SendInventoryItemsUpdate(new InventoryItem[] { item });
							break;
						}
						#endregion Spell1
						#region Proc
					case "proc":
						{
							int slot = (int)eInventorySlot.LastBackpack;
							if (args.Length >= 4)
							{
								try
								{
									slot = Convert.ToInt32(args[3]);
								}
								catch
								{
									slot = (int)eInventorySlot.LastBackpack;
								}
							}
							InventoryItem item = client.Player.Inventory.GetItem((eInventorySlot)slot);
							if (item == null)
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Count.NoItemInSlot", slot), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return;
							}
							int spellID = Convert.ToInt32(args[2]);

							var spell = item.Spells.Where(x => x.ProcChance > 0 && !x.IsPoison).FirstOrDefault();

							if (spell == null)
							{
								spell = new InventoryItemSpell();
								item.Spells.Add(spell);
							}
							spell.SpellID = spellID;

							client.Out.SendInventoryItemsUpdate(new InventoryItem[] { item });
							break;
						}
						#endregion Proc
						#region Proc1
					case "proc1":
						{
							int slot = (int)eInventorySlot.LastBackpack;
							if (args.Length >= 4)
							{
								try
								{
									slot = Convert.ToInt32(args[3]);
								}
								catch
								{
									slot = (int)eInventorySlot.LastBackpack;
								}
							}
							InventoryItem item = client.Player.Inventory.GetItem((eInventorySlot)slot);
							if (item == null)
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Count.NoItemInSlot", slot), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return;
							}

							int spellID = Convert.ToInt32(args[2]);

							var spell = item.Spells.Where(x => x.ProcChance > 0 && !x.IsPoison).Skip(1).FirstOrDefault();

							if (spell == null)
							{
								spell = new InventoryItemSpell();
								item.Spells.Add(spell);
							}
							spell.SpellID = spellID;

							client.Out.SendInventoryItemsUpdate(new InventoryItem[] { item });
							break;
						}
						#endregion Proc1
						#region ProcChance
					case "procchance":
						{
							int slot = (int)eInventorySlot.LastBackpack;
							if (args.Length >= 4)
							{
								try
								{
									slot = Convert.ToInt32(args[3]);
								}
								catch
								{
									slot = (int)eInventorySlot.LastBackpack;
								}
							}
							InventoryItem item = client.Player.Inventory.GetItem((eInventorySlot)slot);
							if (item == null)
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Count.NoItemInSlot", slot), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return;
							}

							var spell = item.Spells.Where(x => x.ProcChance > 0 && !x.IsPoison).Skip(1).FirstOrDefault();

							if (spell == null)
							{
								spell = new InventoryItemSpell();
								item.Spells.Add(spell);
							}

							spell.ProcChance = Convert.ToByte(args[2]);
							client.Out.SendInventoryItemsUpdate(new InventoryItem[] { item });
							break;
						}
						#endregion ProcChance
						#region Poison
					case "poison":
						{
							int slot = (int)eInventorySlot.LastBackpack;
							if (args.Length >= 6)
							{
								try
								{
									slot = Convert.ToInt32(args[5]);
								}
								catch
								{
									slot = (int)eInventorySlot.LastBackpack;
								}
							}
							InventoryItem item = client.Player.Inventory.GetItem((eInventorySlot)slot);
							if (item == null)
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Count.NoItemInSlot", slot), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return;
							}
							int Charges = Convert.ToInt32(args[2]);
							int MaxCharges = Convert.ToInt32(args[3]);
							int SpellID = Convert.ToInt32(args[4]);

							var spell = item.Spells.Where(x => x.IsPoison).FirstOrDefault();

							if (spell == null)
							{
								spell = new InventoryItemSpell();
								item.Spells.Add(spell);
							}
							spell.Charges = Charges;
							spell.MaxCharges = MaxCharges;
							spell.SpellID = SpellID;

							if (item.ItemTemplate is ItemUnique || (item.ItemTemplate is ItemTemplate && (item.ItemTemplate as ItemTemplate).AllowUpdate))
							{
								var templateSpell = item.ItemTemplate.Spells.FirstOrDefault(x => x.ProcChance <= 0 && !x.IsPoison);

								if (templateSpell == null)
								{
									templateSpell = new ItemSpell();
									item.ItemTemplate.Spells.Add(templateSpell);
								}
								templateSpell.Charges = Charges;
								templateSpell.MaxCharges = MaxCharges;
								templateSpell.SpellID = SpellID;
							}

							client.Out.SendInventoryItemsUpdate(new InventoryItem[] { item });
							break;
						}
						#endregion Poison
						#region Realm
					case "realm":
						{
							int slot = (int)eInventorySlot.LastBackpack;
							if (args.Length >= 4)
							{
								try
								{
									slot = Convert.ToInt32(args[3]);
								}
								catch
								{
									slot = (int)eInventorySlot.LastBackpack;
								}
							}
							InventoryItem item = client.Player.Inventory.GetItem((eInventorySlot)slot);
							if (item == null)
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Count.NoItemInSlot", slot), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return;
							}
							item.ItemTemplate.Realm = int.Parse(args[2]);
							break;
						}
						#endregion Realm
						#region Level Required
					case "levelrequired":
						{
							int slot = (int)eInventorySlot.LastBackpack;
							if (args.Length >= 4)
							{
								try
								{
									slot = Convert.ToInt32(args[3]);
								}
								catch
								{
									slot = (int)eInventorySlot.LastBackpack;
								}
							}
							InventoryItem item = client.Player.Inventory.GetItem((eInventorySlot)slot);
							if (item == null)
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Count.NoItemInSlot", slot), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return;
							}
							int setting = Convert.ToInt32(args[2]);
							item.ItemTemplate.LevelRequirement = setting;
							client.Out.SendInventoryItemsUpdate(new InventoryItem[] { item });
							break;
						}
						#endregion Level Required
						#region Bonus Level
					case "bonuslevel":
						{
							int slot = (int)eInventorySlot.LastBackpack;
							if (args.Length >= 4)
							{
								try
								{
									slot = Convert.ToInt32(args[3]);
								}
								catch
								{
									slot = (int)eInventorySlot.LastBackpack;
								}
							}
							InventoryItem item = client.Player.Inventory.GetItem((eInventorySlot)slot);
							if (item == null)
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Count.NoItemInSlot", slot), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return;
							}
							int setting = Convert.ToInt32(args[2]);
							item.ItemTemplate.BonusLevel = setting;
							client.Out.SendInventoryItemsUpdate(new InventoryItem[] { item });
							break;
						}
						#endregion Bonus Level
						#region ClassType
					case "classtype":
						{
							string classType = args[2];
							int slot = (int)eInventorySlot.LastBackpack;

							if (int.TryParse(args[args.Length - 1], out slot) == false)
							{
								slot = (int)eInventorySlot.LastBackpack;
							}

							InventoryItem item = client.Player.Inventory.GetItem((eInventorySlot)slot);
							if (item == null)
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Count.NoItemInSlot", slot), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return;
							}

							item.ItemTemplate.ClassType = classType;
							client.Out.SendInventoryItemsUpdate(new InventoryItem[] { item });
							break;
						}
						#endregion ClassType
						#region PackageID
					case "packageid":
						{
							string packageID = args[2];
							int slot = (int)eInventorySlot.LastBackpack;

							if (int.TryParse(args[args.Length - 1], out slot) == false)
							{
								slot = (int)eInventorySlot.LastBackpack;
							}

							InventoryItem item = client.Player.Inventory.GetItem((eInventorySlot)slot);
							if (item == null)
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Count.NoItemInSlot", slot), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return;
							}

							item.ItemTemplate.PackageID = packageID;
							client.Out.SendInventoryItemsUpdate(new InventoryItem[] { item });
							break;
						}
						#endregion PackageID
						#region Flags
					case "flags":
						{
							int flags = Convert.ToInt32(args[2]);
							int slot = (int)eInventorySlot.LastBackpack;

							if (args.Length == 4)
							{
								slot = Convert.ToInt32(args[3]);
							}

							InventoryItem item = client.Player.Inventory.GetItem((eInventorySlot)slot);
							if (item == null)
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Count.NoItemInSlot", slot), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return;
							}

							item.ItemTemplate.Flags = flags;
							client.Out.SendInventoryItemsUpdate(new InventoryItem[] { item });
							break;
						}
						#endregion Flags
						#region Salvage
					case "salvageid":
						{
							int salvageID = Convert.ToInt32(args[2]);
							int slot = (int)eInventorySlot.LastBackpack;

							if (args.Length == 4)
							{
								slot = Convert.ToInt32(args[3]);
							}

							InventoryItem item = client.Player.Inventory.GetItem((eInventorySlot)slot);
							if (item == null)
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Count.NoItemInSlot", slot), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return;
							}

							item.ItemTemplate.SalvageYieldID = salvageID;
							client.Out.SendInventoryItemsUpdate(new InventoryItem[] { item });
							break;
						}
					case "salvageinfo":
						{
							int slot = (int)eInventorySlot.LastBackpack;

							if (args.Length == 3)
							{
								slot = Convert.ToInt32(args[2]);
							}

							InventoryItem item = client.Player.Inventory.GetItem((eInventorySlot)slot);
							if (item == null)
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Count.NoItemInSlot", slot), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return;
							}

							List<string> list = new List<string>();
														
							bool calculated = true;

							int salvageLevel = CraftingMgr.GetItemCraftLevel(item) / 100;
							if (salvageLevel > 9) salvageLevel = 9; // max 9

							var query = GameServer.Database.SalvageYields.AsQueryable();

							if (!item.ItemTemplate.SalvageYieldID.HasValue || item.ItemTemplate.SalvageYieldID <= 0)
							{
								query = query.Where(x => x.ObjectType == item.ItemTemplate.ObjectType && x.SalvageLevel == salvageLevel);
							}
							else
							{
								query = query.Where(x => x.Id == item.ItemTemplate.SalvageYieldID.Value);
								calculated = false;
							}

							if (ServerProperties.Properties.USE_SALVAGE_PER_REALM)
							{
								query = query.Where(x => x.Realm == (int)eRealm.None || x.Realm == item.ItemTemplate.Realm);
							}

							var salvageYield = query.FirstOrDefault();

							SalvageYield yield = null;

							if (salvageYield != null)
							{
								yield = salvageYield.Clone() as SalvageYield;
							}

							if (yield == null || yield.PackageID == SalvageYield.LEGACY_SALVAGE_ID)
							{
								if (calculated == false)
								{
									list.Add("SalvageYield ID " + item.ItemTemplate.SalvageYieldID + " specified but not found!");
								}
								else if (ServerProperties.Properties.USE_NEW_SALVAGE)
								{
									list.Add("Calculated Values (USE_NEW_SALVAGE = True)");
								}
								else
								{
									list.Add("Calculated Values (USE_NEW_SALVAGE = False)");
								}
							}
							else
							{
								list.Add("Using SalvageYield ID: " + yield.Id);
							}

							list.Add(" ");

							ItemTemplate material = GameServer.Database.ItemTemplates.Find(yield.ItemTemplateID);
							string materialName = yield.ItemTemplate.KeyName;

							if (material != null)
							{
								materialName = material.Name + " (" + materialName + ")";
							}
							else
							{
								materialName = "Not Found! (" + materialName + ")";
							}

							if (calculated == false)
							{
								if (yield != null)
								{
									list.Add("SalvageYield ID: " + yield.Id);
									list.Add("       Material: " + materialName);
									list.Add("          Count: " + yield.Count);
									list.Add("          Realm: " + (yield.Realm == 0 ? "Any" : GlobalConstants.RealmToName((eRealm)yield.Realm)));
									list.Add("      PackageID: " + yield.PackageID);
								}
							}
							else
							{
								list.Add("SalvageYield ID: " + yield.Id);
								list.Add("     ObjectType: " + yield.ObjectType);
								list.Add("   SalvageLevel: " + yield.SalvageLevel);
								list.Add("       Material: " + materialName);
								list.Add("          Count: " + Salvage.GetMaterialYield(client.Player, item, yield, material));
								list.Add("          Realm: " + (yield.Realm == 0 ? "Any" : GlobalConstants.RealmToName((eRealm)yield.Realm)));
								list.Add("      PackageID: " + yield.PackageID);
							}

							client.Out.SendCustomTextWindow("Salvage info for " + item.Name, list);
							break;
						}
						#endregion Flags
						#region Update
					case "update":
					case "updatetemplate":
						{
							int slot = (int)eInventorySlot.LastBackpack;
							if (args.Length == 3)
							{
								slot = Convert.ToInt32(args[2]);
							}

							InventoryItem item = client.Player.Inventory.GetItem((eInventorySlot)slot);
							if (item == null)
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Count.NoItemInSlot", slot), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return;
							}

							UpdateAllowed(item, client);
							break;
						}
						#endregion Update
						#region SaveUnique
					case "saveunique":
						{
							int slot = (int)eInventorySlot.LastBackpack;
							string idnb = string.Empty;
							if (args.Length >= 4)
							{
								try
								{
									slot = Convert.ToInt32(args[3]);
								}
								catch
								{
									slot = (int)eInventorySlot.LastBackpack;
								}
								if (slot > (int)eInventorySlot.LastBackpack)
								{
									slot = (int)eInventorySlot.LastBackpack;
								}
								if (slot < 0)
								{
									slot = 0;
								}

								idnb = args[2];
							}
							else if (args.Length >= 3)
							{
								idnb = args[2];
							}
							else if (args.Length < 2)
							{
								DisplaySyntax(client);
								return;
							}

							InventoryItem item = client.Player.Inventory.GetItem((eInventorySlot)slot);
							if (item == null)
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Count.NoItemInSlot", slot), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return;
							}

							if (item.ItemTemplate is ItemUnique)
							{
								ItemUnique itemUnique = item.ItemTemplate as ItemUnique;
								Log.Debug("update ItemUnique " + item.ItemTemplate.Id);
								GameServer.Instance.SaveDataObject(itemUnique);
								client.Out.SendMessage(string.Format("ItemUnique {0} updated!", itemUnique.Id), eChatType.CT_System, eChatLoc.CL_SystemWindow);
							}
							else
							{
								DisplayMessage(client, "This is not an ItemUnique.  To create a new ItemUnique use addunique");
								return;
							}
						}
						break;
						#endregion SaveUnique
						#region Save / AddUnique
					case "save":
					case "addunique":
						{
							int slot = (int)eInventorySlot.LastBackpack;
							string idnb = string.Empty;
							if (args.Length >= 4)
							{
								try
								{
									slot = Convert.ToInt32(args[3]);
								}
								catch
								{
									slot = (int)eInventorySlot.LastBackpack;
								}
								if (slot > (int)eInventorySlot.LastBackpack)
								{
									slot = (int)eInventorySlot.LastBackpack;
								}
								if (slot < 0)
								{
									slot = 0;
								}

								idnb = args[2];
							}
							else if (args.Length >= 3)
							{
								idnb = args[2];
							}
							else if (args.Length < 2)
							{
								DisplaySyntax(client);
								return;
							}

							InventoryItem item = client.Player.Inventory.GetItem((eInventorySlot)slot);
							if (item == null)
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Count.NoItemInSlot", slot), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return;
							}

							// if a blank item was created then AllowAdd will be false here
							if (idnb == string.Empty && (item.AllowAdd == false || item.ItemTemplate.KeyName == InventoryItem.BLANK_ITEM || args[1].ToLower() == "addunique"))
							{
								DisplayMessage(client, "You need to provide a new id_nb for this item.");
								return;
							}
							else if (idnb == string.Empty)
							{
								if (args[1].ToLower() == "save" && item.ItemTemplate is ItemUnique)
								{
									DisplayMessage(client, "You need to provide a new id_nb to save this ItemUnique as an ItemTemplate.  Use saveunique to save this ItemUnique.");
									return;
								}

								idnb = item.ItemTemplate.KeyName;
							}

							ItemTemplate temp = null;
							if (args[1].ToLower() == "save")
							{
								// if the item is allready in the database
								temp = GameServer.Database.ItemTemplates.FirstOrDefault(x => x.Id.ToString() == idnb || x.KeyName == idnb);
							}

							// save the new item
							if (temp == null)
							{
								if (args[1].ToLower() == "save")
								{
									try
									{
										client.Player.Inventory.RemoveItem(item);
										ItemTemplate itemTemplate = (ItemTemplate)item.ItemTemplate.Clone();
										itemTemplate.KeyName = idnb;
                                        GameServer.Instance.SaveDataObject(itemTemplate);
										Log.Debug("Added New Item Template: " + itemTemplate.Id);
										DisplayMessage(client, "Added New Item Template: " + itemTemplate.Id);
										GameInventoryItem newItem = GameInventoryItem.Create(itemTemplate);
										if (client.Player.Inventory.AddItem((eInventorySlot)slot, newItem))
											InventoryLogging.LogInventoryAction(client.Player, client.Player, eInventoryActionType.Other, newItem.ItemTemplate, newItem.Count);
									}
									catch (Exception ex)
									{
										DisplayMessage(client, "Error adding template: " + ex.Message);
										return;
									}
								}
								else //addunique
								{
									try
									{
										client.Player.Inventory.RemoveItem(item);
										ItemUnique unique = (ItemUnique)item.ItemTemplate.Clone();
										unique.KeyName = idnb;

										GameServer.Instance.SaveDataObject(unique);
										Log.Debug("Added New ItemUnique: " + unique.Id + " (" + unique.Id + ")");
										DisplayMessage(client, "Added New ItemUnique: " + unique.Id + " (" + unique.Id + ")");
										GameInventoryItem newItem = GameInventoryItem.Create(unique);
										if (client.Player.Inventory.AddItem((eInventorySlot)slot, newItem))
											InventoryLogging.LogInventoryAction(client.Player, client.Player, eInventoryActionType.Other, newItem.ItemTemplate, newItem.Count);
									}
									catch (Exception ex)
									{
										DisplayMessage(client, "Error adding unique: " + ex.Message);
										return;
									}
								}
							}
							else // update the item
							{
								GameServer.Instance.SaveDataObject(item.ItemTemplate);
								DisplayMessage(client, "Updated Inventory Item: " + item.Id);

								if (item.ItemTemplate is ItemTemplate && (item.ItemTemplate as ItemTemplate).AllowUpdate)
								{
									Log.Debug("Updated ItemTemplate: " + item.ItemTemplate.Id);
									DisplayMessage(client, "++ Source ItemTemplate Updated!");
								}
							}

						}
						break;
						#endregion Save / AddUnique
						#region FindID
					case "findid":
						{
							string name = string.Join(" ", args, 2, args.Length - 2);
							if (name != "")
							{
								var items = GameServer.Database.ItemTemplates.Where(x => x.Id.ToString().Contains(name) || x.KeyName.ToLower().Contains(name.ToLower())).ToList();
								DisplayMessage(client, LanguageMgr.GetTranslation("EN", "GMCommands.Item.FindID.MatchingIDsForX", name, items.Count), new object[] { });
								foreach (ItemTemplate item in items)
								{
									DisplayMessage(client, item.Id + " (" + item.Name + ")", new object[] { });
								}
							}
							break;
						}
						#endregion FindID
						#region FindName
					case "findname":
						{
							string name = string.Join(" ", args, 2, args.Length - 2);
							if (name != "")
							{
								var items = GameServer.Database.ItemTemplates.Where(x => x.Name.ToLower().Contains(name.ToLower())).ToList();
								DisplayMessage(client, LanguageMgr.GetTranslation("EN", "GMCommands.Item.FindName.MatchingNamesForX", name, items.Count), new object[] { });
								foreach (ItemTemplate item in items)
								{
									DisplayMessage(client, item.Name + "  (" + item.Id + ")", new object[] { });
								}
							}
							break;
						}
						#endregion FindName
						#region Load
					case "load":
						{
							var item = GameServer.Database.ItemTemplates.FirstOrDefault(x => x.Id.ToString() == args[2] || x.KeyName == args[2]);
							if (item != null)
							{
								Log.DebugFormat("Item {0} updated or added to ItemTemplate cache.", args[2]);
								DisplayMessage(client, "Item {0} updated or added to ItemTemplate cache.", args[2]);
							}
							else
							{
								Log.DebugFormat("Item {0} not found.", args[2]);
								DisplayMessage(client, "Item {0} not found.", args[2]);
							}
							break;
						}
						#endregion Load
						#region LoadPackage
					case "loadpackage":
						{
							if (args[2] != "")
							{
								if (args[2] == "**all**") args[2] = String.Empty;

								var packageItems = GameServer.Database.ItemTemplates.Where(x => x.PackageID == args[2]).ToList();

								if (packageItems != null)
								{
									int count = 0;

									Log.DebugFormat("{0} items updated or added to the ItemTemplate cache.", count);
									DisplayMessage(client, "{0} items updated or added to the ItemTemplate cache.", count);
								}
								else
								{
									DisplayMessage(client, "No items found for package {0}.", args[2]);
								}
							}
							break;
						}
						#endregion LoadPackage
						#region LoadArtifacts
					case "loadartifacts":
						{
							DisplayMessage(client, "{0} Artifacts re-loaded.", DOL.GS.ArtifactMgr.LoadArtifacts());
						}
						break;
						#endregion LoadArtifacts
						#region LoadSpells
					case "loadspells":
						{
							int slot = (int)eInventorySlot.LastBackpack;
							if (args.Length >= 4)
							{
								try
								{
									slot = Convert.ToInt32(args[3]);
								}
								catch
								{
									slot = (int)eInventorySlot.LastBackpack;
								}
							}
							InventoryItem item = client.Player.Inventory.GetItem((eInventorySlot)slot);
							if (item == null)
							{
								client.Out.SendMessage(LanguageMgr.GetTranslation(client.Account.Language, "GMCommands.Item.Count.NoItemInSlot", slot), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return;
							}

							foreach (var spell in item.Spells)
                            {
								LoadSpell(client, spell.SpellID);
                            }
							break;
						}
						#endregion LoadSpells
				}
			}
			catch
			{
				DisplaySyntax(client);
			}
		}
		
		private void UpdateAllowed(InventoryItem item, GameClient client)
		{
			if (item.ItemTemplate is ItemUnique)
			{
				DisplayMessage(client, "This command is only applicable for items based on an ItemTemplate");
				return;
			}
			else
			{
				(item.ItemTemplate as ItemTemplate).AllowUpdate = true;
				client.Out.SendMessage("** When this item is saved all changes will also be made to the source ItemTemplate: " + item.ItemTemplate.Id, eChatType.CT_Staff, eChatLoc.CL_SystemWindow);
				DisplayMessage(client, "** When this item is saved all changes will also be made to the source ItemTemplate: " + item.ItemTemplate.Id);
			}
		}
		
		private void LoadSpell(GameClient client, int spellID)
		{
			if (spellID != 0)
			{
				if (SkillBase.UpdateSpell(spellID))
				{
					Log.DebugFormat("Spell ID {0} added / updated in the global spell list", spellID);
					DisplayMessage(client, "Spell ID {0} added / updated in the global spell list", spellID);
				}
			}
		}
	}
}