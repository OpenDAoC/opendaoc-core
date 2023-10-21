using System;
using System.Collections.Generic;
using System.Reflection;
using Core.Base;
using Core.Database;
using Core.Database.Tables;
using Core.GS.Database;
using Core.GS.Enums;
using Core.GS.Expansions.Foundations;
using Core.GS.GameUtils;
using Core.GS.Languages;
using Core.GS.ServerProperties;
using log4net;

namespace Core.GS.Packets.Clients;

[PacketHandler(EPacketHandlerType.TCP, EClientPackets.HousePlaceItem, "Handles things like placing indoor/outdoor items.", EClientStatus.PlayerInGame)]
public class HousingPlaceItemHandler : IPacketHandler
{
	private const string DeedWeak = "deedItem";
	private const string TargetHouse = "targetHouse";
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
	private int _position;

	public void HandlePacket(GameClient client, GsPacketIn packet)
	{
		try
		{
			int unknow1 = packet.ReadByte(); // 1=Money 0=Item (?)
			int slot = packet.ReadByte(); // Item/money slot
			ushort housenumber = packet.ReadShort(); // N� of house
			int unknow2 = (byte)packet.ReadByte();
			_position = (byte)packet.ReadByte();
			int method = packet.ReadByte(); // 2=Wall 3=Floor
			int rotation = packet.ReadByte(); // garden items only
			var xpos = (short)packet.ReadShort(); // x for inside objs
			var ypos = (short)packet.ReadShort(); // y for inside objs.
			//Log.Info("U1: " + unknow1 + " - U2: " + unknow2);

			ChatUtil.SendDebugMessage(client, string.Format("HousingPlaceItem: slot: {0}, position: {1}, method: {2}, xpos: {3}, ypos: {4}", slot, _position, method, xpos, ypos));

			if (client.Player == null)
				return;

			// house must exist
			House house = HouseMgr.GetHouse(client.Player.CurrentRegionID, housenumber);
			if (house == null)
			{
				client.Player.Out.SendInventorySlotsUpdate(null);
				return;
			}


			if ((slot >= 244) && (slot <= 248)) // money
			{
				// check that player has permission to pay rent
				if (!house.CanPayRent(client.Player))
				{
					client.Out.SendInventorySlotsUpdate(new[] { slot });
					return;
				}

				long moneyToAdd = _position;
				switch (slot)
				{
					case 248:
						moneyToAdd *= 1;
						break;
					case 247:
						moneyToAdd *= 100;
						break;
					case 246:
						moneyToAdd *= 10000;
						break;
					case 245:
						moneyToAdd *= 10000000;
						break;
					case 244:
						moneyToAdd *= 10000000000;
						break;
				}

				client.Player.TempProperties.SetProperty(HousingConstants.MoneyForHouseRent, moneyToAdd);
				client.Player.TempProperties.SetProperty(HousingConstants.HouseForHouseRent, house);
				client.Player.Out.SendInventorySlotsUpdate(null);
				client.Player.Out.SendHousePayRentDialog("Housing07");

				return;
			}

			// make sure the item dropped still exists
			DbInventoryItem orgitem = client.Player.Inventory.GetItem((EInventorySlot)slot);
			if (orgitem == null)
			{
				client.Player.Out.SendInventorySlotsUpdate(null);
				return;
			}

			if (orgitem.Id_nb == "house_removal_deed")
			{
				client.Out.SendInventorySlotsUpdate(null);

				// make sure player has owner permissions
				if (!house.HasOwnerPermissions(client.Player))
				{
					ChatUtil.SendSystemMessage(client.Player, "You don't own this house!");
					return;
				}

				client.Player.TempProperties.SetProperty(DeedWeak, new WeakRef(orgitem));
				client.Player.TempProperties.SetProperty(TargetHouse, house);
				client.Player.Out.SendCustomDialog(LanguageMgr.GetTranslation(client.Account.Language, "WARNING: You are about to delete this house and all indoor and outdoor items attached to it!"), HouseRemovalDialog);

				return;
			}

			if (orgitem.Id_nb.Contains("cottage_deed") || orgitem.Id_nb.Contains("house_deed") ||
				orgitem.Id_nb.Contains("villa_deed") || orgitem.Id_nb.Contains("mansion_deed"))
			{
				client.Out.SendInventorySlotsUpdate(null);

				// make sure player has owner permissions
				if (!house.HasOwnerPermissions(client.Player))
				{
					ChatUtil.SendSystemMessage(client, "You may not change other peoples houses");

					return;
				}

				client.Player.TempProperties.SetProperty(DeedWeak, new WeakRef(orgitem));
				client.Player.TempProperties.SetProperty(TargetHouse, house);
				client.Player.Out.SendMessage("Warning:\n This will remove *all* items from your current house!", EChatType.CT_System, EChatLoc.CL_PopupWindow);
				client.Player.Out.SendCustomDialog("Are you sure you want to upgrade your House?", HouseUpgradeDialog);

				return;
			}

			if (orgitem.Name == "deed of guild transfer")
			{
				// player needs to be in a guild to xfer a house to a guild
				if (client.Player.Guild == null)
				{
					client.Out.SendInventorySlotsUpdate(new[] { slot });
					ChatUtil.SendSystemMessage(client, "You must be a member of a guild to do that");
					return;
				}

				// player needs to own the house to be able to xfer it
				if (!house.HasOwnerPermissions(client.Player))
				{
					client.Out.SendInventorySlotsUpdate(new[] { slot });
					ChatUtil.SendSystemMessage(client, "You do not own this house.");
					return;
				}

				// guild can't already have a house
				if (client.Player.Guild.GuildOwnsHouse)
				{
					client.Out.SendInventorySlotsUpdate(new[] { slot });
					ChatUtil.SendSystemMessage(client, "Your Guild already owns a house.");
					return;
				}

				// player needs to be a GM in the guild to xfer his personal house to the guild
				if (!client.Player.Guild.HasRank(client.Player, EGuildRank.Leader))
				{
					client.Out.SendInventorySlotsUpdate(new[] { slot });
					ChatUtil.SendSystemMessage(client, "You are not the leader of a guild.");
					return;
				}

				if (HouseMgr.HouseTransferToGuild(client.Player, house))
				{
					// This will still take the item even if player answers NO to confirmation.
 					// I'm fixing consignment, not housing, and frankly I'm sick of fixing stuff!  :)  - tolakram
					client.Player.Inventory.RemoveItem(orgitem);
					InventoryLogging.LogInventoryAction(client.Player, "(HOUSE;" + housenumber + ")", EInventoryActionType.Other, orgitem.Template, orgitem.Count);
					client.Player.Guild.UpdateGuildWindow();
				}
				return;
			}

			if (house.CanChangeInterior(client.Player, EDecorationPermissions.Remove))
			{
				if (orgitem.Name == "interior banner removal")
				{
					house.IndoorGuildBanner = false;
					ChatUtil.SendSystemMessage(client.Player, "Scripts.Player.Housing.InteriorBannersRemoved", null);
					return;
				}

				if (orgitem.Name == "interior shield removal")
				{
					house.IndoorGuildShield = false;
					ChatUtil.SendSystemMessage(client.Player, "Scripts.Player.Housing.InteriorShieldsRemoved", null);
					return;
				}

				if (orgitem.Name == "carpet removal")
				{
					house.Rug1Color = 0;
					house.Rug2Color = 0;
					house.Rug3Color = 0;
					house.Rug4Color = 0;
					ChatUtil.SendSystemMessage(client.Player, "Scripts.Player.Housing.CarpetsRemoved", null);
					return;
				}
			}

			if (house.CanChangeExternalAppearance(client.Player))
			{
				if (orgitem.Name == "exterior banner removal")
				{
					house.OutdoorGuildBanner = false;
					ChatUtil.SendSystemMessage(client.Player, "Scripts.Player.Housing.OutdoorBannersRemoved", null);
					return;
				}

				if (orgitem.Name == "exterior shield removal")
				{
					house.OutdoorGuildShield = false;
					ChatUtil.SendSystemMessage(client.Player, "Scripts.Player.Housing.OutdoorShieldsRemoved", null);
					return;
				}
			}

			int objType = orgitem.Object_Type;
			if (objType == 49) // Garden items 
			{
				method = 1;
			}
			else if (orgitem.Id_nb == "housing_porch_deed" || orgitem.Id_nb == "housing_porch_remove_deed" || orgitem.Id_nb == "housing_consignment_deed")
			{
				method = 4;
			}
			else if (objType >= 59 && objType <= 64) // Outdoor Roof/Wall/Door/Porch/Wood/Shutter/awning Material item type
			{
				ChatUtil.SendSystemMessage(client.Player, "Scripts.Player.Housing.HouseUseMaterials", null);
				return;
			}
			else if (objType == 56 || objType == 52 || (objType >= 69 && objType <= 71)) // Indoor carpets 1-4
			{
				ChatUtil.SendSystemMessage(client.Player, "Scripts.Player.Housing.HouseUseCarpets", null);
				return;
			}
			else if (objType == 57 || objType == 58 // Exterior banner/shield
					 || objType == 66 || objType == 67) // Interior banner/shield
			{
				method = 6;
			}
			else if (objType == 53 || objType == 55 || objType == 68)
			{
				method = 5;
			}
			else if (objType == (int)EObjectType.HouseVault)
			{
				method = 7;
			}

			ChatUtil.SendDebugMessage(client, string.Format("Place Item: method: {0}", method));

			int pos;
			switch (method)
			{
				case 1: // GARDEN OBJECT
					{
						if (client.Player.InHouse)
						{
							client.Out.SendInventorySlotsUpdate(new[] { slot });
							return;
						}

						// no permissions to add to the garden, return
						if (!house.CanChangeGarden(client.Player, EDecorationPermissions.Add))
						{
							client.Out.SendInventorySlotsUpdate(new[] { slot });
							return;
						}


						// garden is already full, return
						if (house.OutdoorItems.Count >= Properties.MAX_OUTDOOR_HOUSE_ITEMS)
						{
							ChatUtil.SendSystemMessage(client, "Scripts.Player.Housing.GardenMaxObjects", null);
							client.Out.SendInventorySlotsUpdate(new[] { slot });

							return;
						}

						// create an outdoor item to represent the item being placed
						var oitem = new OutdoorItem
										{
											BaseItem = GameServer.Database.FindObjectByKey<DbItemTemplate>(orgitem.Id_nb),
											Model = orgitem.Model,
											Position = (byte)_position,
											Rotation = (byte)rotation
										};

						//add item in db
						pos = GetFirstFreeSlot(house.OutdoorItems.Keys);
						DbHouseOutdoorItem odbitem = oitem.CreateDBOutdoorItem(housenumber);
						oitem.DatabaseItem = odbitem;

						GameServer.Database.AddObject(odbitem);

						// remove the item from the player's inventory
						client.Player.Inventory.RemoveItem(orgitem);
						InventoryLogging.LogInventoryAction(client.Player, "(HOUSE;" + housenumber + ")", EInventoryActionType.Other, orgitem.Template, orgitem.Count);

						//add item to outdooritems
						house.OutdoorItems.Add(pos, oitem);

						ChatUtil.SendSystemMessage(client, "Scripts.Player.Housing.GardenItemPlaced",
												   Properties.MAX_OUTDOOR_HOUSE_ITEMS - house.OutdoorItems.Count);
						ChatUtil.SendSystemMessage(client, "Scripts.Player.Housing.GardenItemPlacedName", orgitem.Name);

						// update all nearby players
						foreach (GamePlayer player in WorldMgr.GetPlayersCloseToSpot(house.RegionID, house, WorldMgr.OBJ_UPDATE_DISTANCE))
						{
							player.Out.SendGarden(house);
						}

						// save the house
						house.SaveIntoDatabase();
						break;
					}
				case 2: // WALL OBJECT
				case 3: // FLOOR OBJECT
					{
						if (client.Player.InHouse == false)
						{
							client.Out.SendInventorySlotsUpdate(new[] { slot });
							return;
						}

						// no permission to add to the interior, return
						if (!house.CanChangeInterior(client.Player, EDecorationPermissions.Add))
						{
							client.Out.SendInventorySlotsUpdate(new[] { slot });
							return;
						}

						// not a wall object, return
						if (!IsSuitableForWall(orgitem) && method == 2)
						{
							ChatUtil.SendSystemMessage(client, "Scripts.Player.Housing.NotWallObject", null);
							client.Out.SendInventorySlotsUpdate(new[] { slot });
							return;
						}

						// not a floor object, return
						if (objType != 51 && method == 3)
						{
							ChatUtil.SendSystemMessage(client, "Scripts.Player.Housing.NotFloorObject", null);
							client.Out.SendInventorySlotsUpdate(new[] { slot });
							return;
						}

						// interior already has max items, return
						if (house.IndoorItems.Count >= GetMaxIndoorItemsForHouse(house.Model))
						{
							ChatUtil.SendSystemMessage(client, "Scripts.Player.Housing.IndoorMaxItems", GetMaxIndoorItemsForHouse(house.Model));
							client.Out.SendInventorySlotsUpdate(new[] { slot });
							return;
						}

						// create an indoor item to represent the item being placed
						var iitem = new IndoorItem
										{
											Model = orgitem.Model,
											Color = orgitem.Color,
											Emblem = orgitem.Emblem,
											X = xpos,
											Y = ypos,
											Size = orgitem.DPS_AF > 3 ? orgitem.DPS_AF : 100, // max size is 255
											Position = _position,
											PlacementMode = method,
											BaseItem = null
										};

						// figure out proper rotation for item
						int properRotation = client.Player.Heading / 10;
						properRotation = Math.Clamp(properRotation, 0, 360);

						if (method == 2 && IsSuitableForWall(orgitem))
						{
							properRotation = 360;
							if (objType != 50)
							{
								client.Out.SendInventorySlotsUpdate(new[] { slot });
							}
						}

						iitem.Rotation = properRotation;

						pos = GetFirstFreeSlot(house.IndoorItems.Keys);
						if (objType == 50 || objType == 51)
						{
							//its a housing item, so lets take it!
							client.Player.Inventory.RemoveItem(orgitem);
							InventoryLogging.LogInventoryAction(client.Player, "(HOUSE;" + housenumber + ")", EInventoryActionType.Other, orgitem.Template, orgitem.Count);

							//set right base item, so we can recreate it on take.
							if (orgitem.Id_nb.Contains("GuildBanner"))
							{
								iitem.BaseItem = orgitem.Template;
								iitem.Size = 50; // Banners have to be reduced in size
							}
							else
							{
								iitem.BaseItem = GameServer.Database.FindObjectByKey<DbItemTemplate>(orgitem.Id_nb);
							}
						}

						DbHouseIndoorItem idbitem = iitem.CreateDBIndoorItem(housenumber);
						iitem.DatabaseItem = idbitem;
						GameServer.Database.AddObject(idbitem);

						house.IndoorItems.Add(pos, iitem);

						// let player know the item has been placed
						ChatUtil.SendSystemMessage(client, "Scripts.Player.Housing.IndoorItemPlaced", (GetMaxIndoorItemsForHouse(house.Model) - house.IndoorItems.Count));

						switch (method)
						{
							case 2:
								ChatUtil.SendSystemMessage(client, "Scripts.Player.Housing.IndoorWallPlaced", orgitem.Name);
								break;
							case 3:
								ChatUtil.SendSystemMessage(client, "Scripts.Player.Housing.IndoorFloorPlaced", orgitem.Name);
								break;
						}

						// update furniture for all players in the house
						foreach (GamePlayer plr in house.GetAllPlayersInHouse())
						{
							plr.Out.SendFurniture(house, pos);
						}

						break;
					}
				case 4: // PORCH
					{
						// no permission to add to the garden, return
						if (!house.CanChangeGarden(client.Player, EDecorationPermissions.Add))
						{
							client.Out.SendInventorySlotsUpdate(new[] { slot });
							return;
						}

						switch (orgitem.Id_nb)
						{
							case "housing_porch_deed":
								// try and add the porch
								if (house.AddPorch())
								{
									// remove the original item from the player's inventory
									client.Player.Inventory.RemoveItem(orgitem);
									InventoryLogging.LogInventoryAction(client.Player, "(HOUSE;" + housenumber + ")", EInventoryActionType.Other, orgitem.Template, orgitem.Count);
								}
								else
								{
									ChatUtil.SendSystemMessage(client, "Scripts.Player.Housing.PorchAlready", null);
									client.Out.SendInventorySlotsUpdate(new[] { slot });
								}
								return;
							case "housing_porch_remove_deed":
								if (house.ConsignmentMerchant != null)
								{
									ChatUtil.SendSystemMessage(client, "You must first remove the consignment merchant.");
									client.Out.SendInventorySlotsUpdate(new[] { slot });
									return;
								}

								// try and remove the porch
								if (house.RemovePorch())
								{
									// remove the original item from the player's inventory
									client.Player.Inventory.RemoveItem(orgitem);
									InventoryLogging.LogInventoryAction(client.Player, "(HOUSE;" + housenumber + ")", EInventoryActionType.Other, orgitem.Template, orgitem.Count);
								}
								else
								{
									ChatUtil.SendSystemMessage(client, "Scripts.Player.Housing.PorchNone", null);
									client.Out.SendInventorySlotsUpdate(new[] { slot });
								}
								return;
							case "housing_consignment_deed":
								{
									// make sure there is a porch for this consignment merchant!
									if (!house.Porch)
									{
										ChatUtil.SendSystemMessage(client, "Your house needs a porch first.");
										client.Out.SendInventorySlotsUpdate(new[] { slot });
										return;
									}

									// try and add a new consignment merchant
									if (house.AddConsignment(0))
									{
										// remove the original item from the player's inventory
										client.Player.Inventory.RemoveItem(orgitem);
										InventoryLogging.LogInventoryAction(client.Player, "(HOUSE;" + housenumber + ")", EInventoryActionType.Other, orgitem.Template, orgitem.Count);
									}
									else
									{
										ChatUtil.SendSystemMessage(client, "You cannot add a consignment merchant here.");
										client.Out.SendInventorySlotsUpdate(new[] { slot });
									}
									return;
								}
							default:
								ChatUtil.SendSystemMessage(client, "Scripts.Player.Housing.PorchNotItem", null);
								client.Out.SendInventorySlotsUpdate(new[] { slot });
								return;
						}
					}
				case 5: // HOOKPOINT
					{
						if (client.Player.InHouse == false)
						{
							client.Out.SendInventorySlotsUpdate(new[] { slot });
							return;
						}

						// no permission to add to the interior, return
						if (!house.CanChangeInterior(client.Player, EDecorationPermissions.Add))
						{
							client.Out.SendInventorySlotsUpdate(new[] { slot });
							return;
						}

						// don't allow non-hookpoint items to be dropped on hookpoints
						if (IsSuitableForHookpoint(orgitem) == false)
						{
							client.Out.SendInventorySlotsUpdate(new[] { slot });
							return;
						}

						// if the hookpoint doesn't exist, prompt player to Log it in the database for us
						if (house.GetHookpointLocation((uint)_position) == null)
						{
							client.Out.SendInventorySlotsUpdate(new[] { slot });

							if (client.Account.PrivLevel == (int)EPrivLevel.Admin)
							{
								if (client.Player.TempProperties.GetProperty<bool>(HousingConstants.AllowAddHouseHookpoint, false))
								{

									ChatUtil.SendSystemMessage(client, "Scripts.Player.Housing.HookPointID", +_position);
									ChatUtil.SendSystemMessage(client, "Scripts.Player.Housing.HookPointCloser", null);

									client.Player.Out.SendCustomDialog(LanguageMgr.GetTranslation(client.Account.Language, "Scripts.Player.Housing.HookPointLogLoc"), LogLocation);
								}
								else
								{
									ChatUtil.SendDebugMessage(client, "use '/house addhookpoints' to allow addition of new housing hookpoints.");
								}
							}
						}
						else if (house.GetHookpointLocation((uint)_position) != null)
						{
							var point = new DbHouseHookPointItem
											{
												HouseNumber = house.HouseNumber,
												ItemTemplateID = orgitem.Id_nb,
												HookpointID = (uint)_position
											};

							// If we already have soemthing here, do not place more
							foreach (var hpitem in CoreDb<DbHouseHookPointItem>.SelectObjects(DB.Column("HouseNumber").IsEqualTo(house.HouseNumber)))
							{
								if (hpitem.HookpointID == point.HookpointID)
								{
									ChatUtil.SendSystemMessage(client, "Scripts.Player.Housing.HookPointAlready", null);
									client.Out.SendInventorySlotsUpdate(new[] { slot });
									return;
								}
							}

							if (house.HousepointItems.ContainsKey(point.HookpointID) == false)
							{
								house.HousepointItems.Add(point.HookpointID, point);
								house.FillHookpoint((uint)_position, orgitem.Id_nb, client.Player.Heading, 0);
							}
							else
							{
								string error = string.Format("Hookpoint already has item on attempt to attach {0} to hookpoint {1} for house {2}!", orgitem.Id_nb, _position, house.HouseNumber);
								log.ErrorFormat(error);
								client.Out.SendInventorySlotsUpdate(new[] { slot });
								throw new Exception(error);
							}

							// add the item to the database
							GameServer.Database.AddObject(point);

							// remove the original item from the player's inventory
							client.Player.Inventory.RemoveItem(orgitem);
							InventoryLogging.LogInventoryAction(client.Player, "(HOUSE;" + housenumber + ")", EInventoryActionType.Other, orgitem.Template, orgitem.Count);

							ChatUtil.SendSystemMessage(client, "Scripts.Player.Housing.HookPointAdded", null);

							// save the house
							house.SaveIntoDatabase();
						}
						else
						{
							ChatUtil.SendSystemMessage(client, "Scripts.Player.Housing.HookPointNot", null);
							client.Out.SendInventorySlotsUpdate(new[] { slot });
						}

						// broadcast updates
						house.SendUpdate();
						break;
					}
				case 6:
					{
						// no permission to change external appearance, return
						if (!house.CanChangeExternalAppearance(client.Player))
						{
							client.Out.SendInventorySlotsUpdate(new[] { slot });
							return;
						}

						if (objType == 57) // We have outdoor banner
						{
							house.OutdoorGuildBanner = true;
							ChatUtil.SendSystemMessage(client, "Scripts.Player.Housing.OutdoorBannersAdded", null);
							client.Player.Inventory.RemoveItem(orgitem);
							InventoryLogging.LogInventoryAction(client.Player, "(HOUSE;" + housenumber + ")", EInventoryActionType.Other, orgitem.Template, orgitem.Count);
						}
						else if (objType == 58) // We have outdoor shield
						{
							house.OutdoorGuildShield = true;
							ChatUtil.SendSystemMessage(client, "Scripts.Player.Housing.OutdoorShieldsAdded", null);
							client.Player.Inventory.RemoveItem(orgitem);
							InventoryLogging.LogInventoryAction(client.Player, "(HOUSE;" + housenumber + ")", EInventoryActionType.Other, orgitem.Template, orgitem.Count);
						}
						else if (objType == 66) // We have indoor banner
						{
							house.IndoorGuildBanner = true;
							ChatUtil.SendSystemMessage(client, "Scripts.Player.Housing.InteriorBannersAdded", null);
							client.Player.Inventory.RemoveItem(orgitem);
							InventoryLogging.LogInventoryAction(client.Player, "(HOUSE;" + housenumber + ")", EInventoryActionType.Other, orgitem.Template, orgitem.Count);
						}
						else if (objType == 67) // We have indoor shield
						{
							house.IndoorGuildShield = true;
							ChatUtil.SendSystemMessage(client, "Scripts.Player.Housing.InteriorShieldsAdded", null);
							client.Player.Inventory.RemoveItem(orgitem);
							InventoryLogging.LogInventoryAction(client.Player, "(HOUSE;" + housenumber + ")", EInventoryActionType.Other, orgitem.Template, orgitem.Count);
						}
						else
						{
							ChatUtil.SendSystemMessage(client, "Scripts.Player.Housing.BadShieldBanner", null);
							client.Out.SendInventorySlotsUpdate(new[] { slot });
						}

						// save the house and broadcast updates
						house.SaveIntoDatabase();
						house.SendUpdate();
						break;
					}
				case 7: // House vault.
					{
						if (client.Player.InHouse == false)
						{
							client.Out.SendInventorySlotsUpdate(new[] { slot });
							return;
						}

						// make sure the hookpoint position is valid
						if (_position > HousingConstants.MaxHookpointLocations)
						{
							ChatUtil.SendSystemMessage(client, "This hookpoint position is unknown, error logged.");
							log.Error("HOUSING: " + client.Player.Name + " working with invalid position " + _position + " in house " +
									  house.HouseNumber + " model " + house.Model);

							client.Out.SendInventorySlotsUpdate(new[] { slot });
							return;
						}

						// if hookpoint doesn't exist, prompt player to Log it in the database for us
						if (house.GetHookpointLocation((uint)_position) == null)
						{
							client.Out.SendInventorySlotsUpdate(new[] { slot });

							if (client.Account.PrivLevel == (int)EPrivLevel.Admin)
							{
								if (client.Player.TempProperties.GetProperty<bool>(HousingConstants.AllowAddHouseHookpoint, false))
								{

									ChatUtil.SendSystemMessage(client, "Scripts.Player.Housing.HookPointID", +_position);
									ChatUtil.SendSystemMessage(client, "Scripts.Player.Housing.HookPointCloser", null);

									client.Player.Out.SendCustomDialog(LanguageMgr.GetTranslation(client.Account.Language, "Scripts.Player.Housing.HookPointLogLoc"), LogLocation);
								}
								else
								{
									ChatUtil.SendDebugMessage(client, "use '/house addhookpoints' to allow addition of new housing hookpoints.");
								}
							}

							return;
						}

						// make sure we have space to add another vult
						int vaultIndex = house.GetFreeVaultNumber();
						if (vaultIndex < 0)
						{
							client.Player.Out.SendMessage("You can't add any more vaults to this house!", EChatType.CT_System,
														  EChatLoc.CL_SystemWindow);
							client.Out.SendInventorySlotsUpdate(new[] { slot });

							return;
						}

						// If we already have soemthing here, do not place more
						foreach (var hpitem in CoreDb<DbHouseHookPointItem>.SelectObjects(DB.Column("HouseNumber").IsEqualTo(house.HouseNumber)))
						{
							if (hpitem.HookpointID == _position)
							{
								ChatUtil.SendSystemMessage(client, "Scripts.Player.Housing.HookPointAlready", null);
								client.Out.SendInventorySlotsUpdate(new[] { slot });
								return;
							}
						}

						// create the new vault and attach it to the house
						var houseVault = new GameHouseVault(orgitem.Template, vaultIndex);
						houseVault.Attach(house, (uint)_position, (ushort)((client.Player.Heading + 2048) % 4096));

						// remove the original item from the player's inventory
						client.Player.Inventory.RemoveItem(orgitem);
						InventoryLogging.LogInventoryAction(client.Player, "(HOUSE;" + housenumber + ")", EInventoryActionType.Other, orgitem.Template, orgitem.Count);

						// save the house and broadcast uodates
						house.SaveIntoDatabase();
						house.SendUpdate();
						return;
					}
				default:
					{
						ChatUtil.SendDebugMessage(client, "Place Item: Unknown method, do nothing.");
						client.Out.SendInventorySlotsUpdate(null);
						break;
					}
			}
		}
		catch (Exception ex)
		{
			log.Error("HousingPlaceItemHandler", ex);
			client.Out.SendMessage("Error processing housing action; the error has been logged!", EChatType.CT_Staff, EChatLoc.CL_SystemWindow);
			client.Out.SendInventorySlotsUpdate(null);
		}
	}

	private static bool IsSuitableForWall(DbInventoryItem item)
	{
		switch (item.Object_Type)
		{
			case (int)EObjectType.HouseWallObject:
			case (int)EObjectType.Axe:
			case (int)EObjectType.Blades:
			case (int)EObjectType.Blunt:
			case (int)EObjectType.CelticSpear:
			case (int)EObjectType.CompositeBow:
			case (int)EObjectType.Crossbow:
			case (int)EObjectType.Flexible:
			case (int)EObjectType.Hammer:
			case (int)EObjectType.HandToHand:
			case (int)EObjectType.LargeWeapons:
			case (int)EObjectType.LeftAxe:
			case (int)EObjectType.Longbow:
			case (int)EObjectType.MaulerStaff:
			case (int)EObjectType.Piercing:
			case (int)EObjectType.PolearmWeapon:
			case (int)EObjectType.RecurvedBow:
			case (int)EObjectType.Scythe:
			case (int)EObjectType.Shield:
			case (int)EObjectType.SlashingWeapon:
			case (int)EObjectType.Spear:
			case (int)EObjectType.Staff:
			case (int)EObjectType.Sword:
			case (int)EObjectType.Thrown:
			case (int)EObjectType.ThrustWeapon:
			case (int)EObjectType.TwoHandedWeapon:
				return true;
			default:
				return false;
		}
	}


	private static bool IsSuitableForHookpoint(DbInventoryItem item)
	{
		switch (item.Object_Type)
		{
			case (int)EObjectType.HouseVault:
			case (int)EObjectType.HouseNPC:
			case (int)EObjectType.HouseBindstone:
			case (int)EObjectType.HouseInteriorObject:
				return true;
			default:
				return false;
		}
	}

	private static int GetMaxIndoorItemsForHouse(int model)
	{
		int maxitems = Properties.MAX_INDOOR_HOUSE_ITEMS;

		if (Properties.INDOOR_ITEMS_DEPEND_ON_SIZE)
		{
			switch (model)
			{
				case 1:
				case 5:
				case 9:
					maxitems = 40;
					break;

				case 2:
				case 6:
				case 10:
					maxitems = 60;
					break;

				case 3:
				case 7:
				case 11:
					maxitems = 80;
					break;

				case 4:
				case 8:
				case 12:
					maxitems = 100;
					break;
			}
		}
		return maxitems;
	}

	private static int GetFirstFreeSlot(ICollection<int> tbl)
	{
		int i = 0; //tbl.Count;

		while (tbl.Contains(i))
		{
			i++;
		}

		return i;
	}

	/// <summary>
	/// Does the player want to Log the offset location of the missing housepoint
	/// </summary>
	/// <param name="player">The player</param>
	/// <param name="response">1 = yes 0 = no</param>
	private void LogLocation(GamePlayer player, byte response)
	{
		if (response != 0x01)
			return;

		if (player.CurrentHouse == null)
			return;

		var a = new DbHouseHookPointOffset
		{
			HouseModel = player.CurrentHouse.Model,
			HookpointID = _position,
			X = player.X - player.CurrentHouse.X,
			Y = player.Y - player.CurrentHouse.Y,
			Z = player.Z - 25000,
			Heading = player.Heading - player.CurrentHouse.Heading
		};

		if (GameServer.Database.AddObject(a) && House.AddNewOffset(a))
		{
			ChatUtil.SendSystemMessage(player, "Scripts.Player.Housing.HookPointLogged", _position);

			string action = string.Format("HOUSING: {0} logged new HouseHookpointOffset for model {1}, position {2}, offset {3}, {4}, {5}",
							  player.Name, a.HouseModel, a.HookpointID, a.X, a.Y, a.Z);
			
			log.Debug(action);
			GameServer.Instance.LogGMAction(action);
		}
		else
		{
			log.Error(
				string.Format(
					"HOUSING: Player {0} error adding HouseHookpointOffset for model {1}, position {2}, offset {3}, {4}, {5}",
					player.Name, a.HouseModel, a.HookpointID, a.X, a.Y, a.Z));

			ChatUtil.SendSystemMessage(player, "Error adding position " + _position + ", error recorded in server error Log.");
		}
	}

	private static void HouseRemovalDialog(GamePlayer player, byte response)
	{
		if (response != 0x01)
			return;

		var itemWeak = player.TempProperties.GetProperty<WeakReference>(DeedWeak, new WeakRef(null));
		player.TempProperties.RemoveProperty(DeedWeak);

		var item = (DbInventoryItem)itemWeak.Target;
		var house = player.TempProperties.GetProperty<House>(TargetHouse, null);
		player.TempProperties.RemoveProperty(TargetHouse);

		if (house == null)
		{
			ChatUtil.SendSystemMessage(player, "No house selected!");
			return;
		}

		if (item == null || item.SlotPosition == (int)EInventorySlot.Ground
			|| item.OwnerID == null || item.OwnerID != player.InternalID)
		{
			ChatUtil.SendSystemMessage(player, "You need a House Removal Deed for this.");
			return;
		}

		// Demand any consignment merchant inventory is removed before allowing a removal
		// Again, note that sometimes checks are done here, sometimes in housemgr. In this case, at least, 
		// player will get remove item back if they answer no! - tolakram
		var consignmentMerchant = house.ConsignmentMerchant;
		if (consignmentMerchant != null && (consignmentMerchant.DBItems(player).Count > 0 || consignmentMerchant.TotalMoney > 0))
		{
			ChatUtil.SendSystemMessage(player, "All items and money must be removed from your consignment merchant in order to remove this house!");
			return;
		}

		player.Inventory.RemoveItem(item);
		log.WarnFormat("HOUSING: {0}:{1} is removing house from lot {2} owned by {3}", player.Name, player.Client.Account.Name, house.HouseNumber, house.OwnerID);
		InventoryLogging.LogInventoryAction(player, "(HOUSE;" + house.HouseNumber + ")", EInventoryActionType.Other, item.Template, item.Count);
		HouseMgr.RemoveHouse(house);

		ChatUtil.SendSystemMessage(player, "Your house has been removed!");
	}

	private static void HouseUpgradeDialog(GamePlayer player, byte response)
	{
		if (response != 0x01)
			return;

		var itemWeak = player.TempProperties.GetProperty<WeakReference>(DeedWeak, new WeakRef(null));
		player.TempProperties.RemoveProperty(DeedWeak);

		var item = (DbInventoryItem)itemWeak.Target;
		var house = player.TempProperties.GetProperty<House>(TargetHouse, null);
		player.TempProperties.RemoveProperty(TargetHouse);

		if (house == null)
		{
			ChatUtil.SendSystemMessage(player, "No House selected!");
			return;
		}

		if (item == null || item.SlotPosition == (int)EInventorySlot.Ground
			|| item.OwnerID == null || item.OwnerID != player.InternalID)
		{
			ChatUtil.SendSystemMessage(player, "This does not work without a House Deed.");
			return;
		}

		if (HouseMgr.UpgradeHouse(house, item))
		{
			player.Inventory.RemoveItem(item);
			InventoryLogging.LogInventoryAction(player, "(HOUSE;" + house.HouseNumber + ")", EInventoryActionType.Other, item.Template, item.Count);
		}
	}
}