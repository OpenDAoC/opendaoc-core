using System;
using DOL.GS;
using DOL.Events;
using DOL.GS.PacketHandler;
using log4net;
using System.Reflection;
using DOL.Database;
using log4net.Core;
using System.Collections.Generic;
using System.Linq;
using DOL.GS.API;

namespace DOL.GS.Scripts
{
    public class BattlegroundEventLoot : GameNPC
	{
		private static new readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static int freeLootLevelOffset = 2;
		private int playerRewardOffset = 6;
		
		private static string _currencyID = ServerProperties.Properties.ALT_CURRENCY_ID;
        public override bool AddToWorld()
        {
            Model = 2026;
            Name = "Quartermaster";
            GuildName = "Atlas";
            Level = 50;
            Size = 60;
            Flags |= GameNPC.eFlags.PEACE;
            return base.AddToWorld();
        }
		public override bool Interact(GamePlayer player)
		{
			if (!base.Interact(player)) return false;
			if(player.Level < 50){player.Out.SendMessage("Come back when you're older, kid.", EChatType.CT_Say, EChatLoc.CL_PopupWindow);}
			string realmName = player.Realm.ToString();
			if (realmName.Equals("_FirstPlayerRealm")) {
				realmName = "Albion";
			} else if (realmName.Equals("_LastPlayerRealm")){
				realmName = "Hibernia";
            }
			TurnTo(player.X, player.Y);

			/*
			if (!player.Boosted)
			{
				player.Out.SendMessage("I'm sorry " + player.Name + ", my services are not available to you.", eChatType.CT_Say,eChatLoc.CL_PopupWindow);
				return false;
			}*/
			
			player.Out.SendMessage("Hello " + player.Name + "! We're happy to see you here, supporting your realm.\n" +
				"For your efforts, " + realmName + " has procured a [full suit] of equipment and some [gems] to adorn them with. " +
				"Additionally, I can provide you with some [weapons], starting [coin], and an allotment of [Atlas Orbs].\n\n" +
                "This is the best gear we could provide on short notice. If you want something better, you'll have to take it from your enemies on the battlefield. " + 
				"Go forth, and do battle!", EChatType.CT_Say,EChatLoc.CL_PopupWindow);
			return true;
		}
		public override bool WhisperReceive(GameLiving source, string str) {
			if (!base.WhisperReceive(source, str)) return false;
			if (!(source is GamePlayer)) return false;
			GamePlayer player = (GamePlayer)source;
			if (player.Level < 50) return false;
			TurnTo(player.X, player.Y);
			ERealm realm = player.Realm;
			ECharacterClass charclass = (ECharacterClass)player.CharacterClass.ID;
			EObjectType armorType = GetArmorType(realm, charclass, (byte)(player.Level));
			EColor color = EColor.White;

			switch (realm) {
				case ERealm.Hibernia:
					color = EColor.Green_4;
					break;
				case ERealm.Albion:
					color = EColor.Red_4;
					break;
				case ERealm.Midgard:
					color = EColor.Blue_4;
					break;
			}
			if (str.Equals("full suit"))
			{
				const string customKey = "free_event_armor";
				var hasFreeArmor = CoreDb<DOLCharactersXCustomParam>.SelectObject(DB.Column("DOLCharactersObjectId").IsEqualTo(player.ObjectId).And(DB.Column("KeyName").IsEqualTo(customKey)));

				if (hasFreeArmor != null)
				{
					player.Out.SendMessage("Sorry " + player.Name + ", I don't have enough items left to give you another set.\n\n Go fight for your Realm to get more equipment!", EChatType.CT_Say,EChatLoc.CL_PopupWindow);
					return false;
				}

				GenerateArmor(player);

				DOLCharactersXCustomParam charFreeEventEquip = new DOLCharactersXCustomParam();
				charFreeEventEquip.DOLCharactersObjectId = player.ObjectId;
				charFreeEventEquip.KeyName = customKey;
				charFreeEventEquip.Value = "1";
				GameServer.Database.AddObject(charFreeEventEquip);
			} 
			else if (str.Equals("weapons")) {
				
				const string customKey = "free_event_weapons";
				var hasFreeWeapons = CoreDb<DOLCharactersXCustomParam>.SelectObject(DB.Column("DOLCharactersObjectId").IsEqualTo(player.ObjectId).And(DB.Column("KeyName").IsEqualTo(customKey)));

				if (hasFreeWeapons != null)
				{
					player.Out.SendMessage("Sorry " + player.Name + ", I don't have enough weapons left to give you another set.\n\n Go fight for your Realm to get more equipment!", EChatType.CT_Say,EChatLoc.CL_PopupWindow);
					return false;
				}
				
				GenerateWeaponsForClass(charclass, player);
				
				DOLCharactersXCustomParam charFreeEventEquip = new DOLCharactersXCustomParam();
				charFreeEventEquip.DOLCharactersObjectId = player.ObjectId;
				charFreeEventEquip.KeyName = customKey;
				charFreeEventEquip.Value = "1";
				GameServer.Database.AddObject(charFreeEventEquip);
			} else if (str.Equals("gems"))
            {
				const string customKey = "free_event_gems";
				var hasFreeArmor = CoreDb<DOLCharactersXCustomParam>.SelectObject(DB.Column("DOLCharactersObjectId").IsEqualTo(player.ObjectId).And(DB.Column("KeyName").IsEqualTo(customKey)));

				if (hasFreeArmor != null)
				{
					player.Out.SendMessage("Sorry " + player.Name + ", I don't have enough items left to give you another set.\n\n Go fight for your Realm to get more equipment!", EChatType.CT_Say, EChatLoc.CL_PopupWindow);
					return false;
				}

				if (player.Level < 50)
				{
					
					GenerateGems(player);
					// List<eInventorySlot> gemSlots = new List<eInventorySlot>();
					// gemSlots.Add(eInventorySlot.Cloak);
					// gemSlots.Add(eInventorySlot.Neck);
					// gemSlots.Add(eInventorySlot.Waist);
					// gemSlots.Add(eInventorySlot.Jewellery);
					// gemSlots.Add(eInventorySlot.LeftRing);
					// gemSlots.Add(eInventorySlot.RightRing);
					// gemSlots.Add(eInventorySlot.LeftBracer);
					// gemSlots.Add(eInventorySlot.RightBracer);
					//
					// foreach (eInventorySlot islot in gemSlots)
					// {
					// 	GeneratedUniqueItem item = null;
					// 	item = new GeneratedUniqueItem(realm, charclass, (byte)(player.Level + freeLootLevelOffset), eObjectType.Magical, islot);
					// 	item.AllowAdd = true;
					// 	item.Color = (int)color;
					// 	item.IsTradable = false;
					// 	item.Price = 1;
					// 	GameServer.Database.AddObject(item);
					// 	InventoryItem invitem = GameInventoryItem.Create<ItemUnique>(item);
					// 	player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, invitem);
					// 	//player.Out.SendMessage("Generated: " + item.Name, eChatType.CT_System, eChatLoc.CL_SystemWindow);
					// }
				}
				else
				{
					List<DbItemTemplates> atlasGem = new List<DbItemTemplates>(CoreDb<DbItemTemplates>.SelectObjects(DB.Column("Id_nb").IsEqualTo("atlas_gem")));
					InventoryItem invitem = GameInventoryItem.Create<DbItemUnique>(atlasGem.FirstOrDefault());
					player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, invitem);
				
					List<DbItemTemplates> atlasCloak = new List<DbItemTemplates>(CoreDb<DbItemTemplates>.SelectObjects(DB.Column("Id_nb").IsEqualTo("atlas_cloak")));
					InventoryItem invitem2 = GameInventoryItem.Create<DbItemUnique>(atlasCloak.FirstOrDefault());
					player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, invitem2);
					
					List<DbItemTemplates> atlasRing = new List<DbItemTemplates>(CoreDb<DbItemTemplates>.SelectObjects(DB.Column("Id_nb").IsEqualTo("atlas_ring")));
					InventoryItem invitem3 = GameInventoryItem.Create<DbItemUnique>(atlasRing.FirstOrDefault());
					player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, invitem3);
				}
				
				
				

				DOLCharactersXCustomParam charFreeEventEquip = new DOLCharactersXCustomParam();
				charFreeEventEquip.DOLCharactersObjectId = player.ObjectId;
				charFreeEventEquip.KeyName = customKey;
				charFreeEventEquip.Value = "1";
				GameServer.Database.AddObject(charFreeEventEquip);
			}
			else if (str.Equals("coin"))
			{
				const string moneyKey = "free_money";
				//var hasFreeOrbs = DOLDB<DOLCharactersXCustomParam>.SelectObject(DB.Column("DOLCharactersObjectId").IsEqualTo(player.ObjectId).And(DB.Column("KeyName").IsEqualTo(customKey)));
				string customKey = moneyKey + player.Realm;
				var hasAccountMoney = CoreDb<AccountXCustomParam>.SelectObject(DB.Column("Name").IsEqualTo(player.Client.Account.Name).And(DB.Column("KeyName").IsEqualTo(customKey)));
			
				if (hasAccountMoney != null)
				{
					player.Out.SendMessage("Sorry " + player.Name + ", I don't have enough money left to give you more.\n\n Go fight for your Realm to get some!", EChatType.CT_Say, EChatLoc.CL_PopupWindow);
					return false;
				}
			
				AccountXCustomParam charFreeEventMoney = new AccountXCustomParam();
				charFreeEventMoney.Name = player.Client.Account.Name;
				charFreeEventMoney.KeyName = customKey;
				charFreeEventMoney.Value = "1";
				GameServer.Database.AddObject(charFreeEventMoney);

				player.AddMoney(5000000);
			}
			else if (str.Equals("Atlas Orbs"))
			{

				const string orbKey = "free_orbs";
				string customKey = orbKey + player.Realm;
				var hasFreeOrbs = CoreDb<AccountXCustomParam>.SelectObject(DB.Column("Name").IsEqualTo(player.Client.Account.Name).And(DB.Column("KeyName").IsEqualTo(customKey)));

				if (hasFreeOrbs != null)
				{
					player.Out.SendMessage("Sorry " + player.Name + ", I don't have enough Atlas Orbs left to give you more.\n\n Go fight for your Realm to get some!", EChatType.CT_Say, EChatLoc.CL_PopupWindow);
					return false;
				}

				AccountXCustomParam charFreeEventMoney = new AccountXCustomParam();
				charFreeEventMoney.Name = player.Client.Account.Name;
				charFreeEventMoney.KeyName = customKey;
				charFreeEventMoney.Value = "15000";
				GameServer.Database.AddObject(charFreeEventMoney);

				DbItemTemplates orbs = GameServer.Database.FindObjectByKey<DbItemTemplates>(_currencyID);
				
				if (orbs == null)
				{
					player.Out.SendMessage("Error: Currency ID not found!", EChatType.CT_System, EChatLoc.CL_SystemWindow);
					return false;
				}

				InventoryItem item = GameInventoryItem.Create(orbs);

				player.Inventory.AddTemplate(item, 15000, eInventorySlot.FirstBackpack, eInventorySlot.LastBackpack);

				//GeneratedUniqueItem(eRealm realm, eCharacterClass charClass, byte level, eObjectType type, eInventorySlot slot);
			}
			return true;
		}

        private static EObjectType GetArmorType(ERealm realm, ECharacterClass charClass, byte level) {
            switch (realm) {
				case ERealm.Albion:
					return GeneratedUniqueItem.GetAlbionArmorType(charClass, level);
				case ERealm.Hibernia:
					return GeneratedUniqueItem.GetHiberniaArmorType(charClass, level);
				case ERealm.Midgard:
					return GeneratedUniqueItem.GetMidgardArmorType(charClass, level);
			}
			return EObjectType.Cloth;
        }

        private void SendReply(GamePlayer target, string msg)
			{
				target.Client.Out.SendMessage(
					msg,
					EChatType.CT_Say,EChatLoc.CL_PopupWindow);
			}
		[ScriptLoadedEvent]
        public static void OnScriptCompiled(CoreEvent e, object sender, EventArgs args)
        {
            log.Info("\t BG Loot NPC initialized: true");
        }


        public static void GenerateGems(GamePlayer player)
        {
	        var realm = player.Realm;
	        var charclass = (ECharacterClass)player.CharacterClass.ID;
	        
	        List<eInventorySlot> gemSlots = new List<eInventorySlot>();
	        gemSlots.Add(eInventorySlot.Cloak);
	        gemSlots.Add(eInventorySlot.Neck);
	        gemSlots.Add(eInventorySlot.Waist);
	        gemSlots.Add(eInventorySlot.Jewellery);
	        gemSlots.Add(eInventorySlot.LeftRing);
	        gemSlots.Add(eInventorySlot.RightRing);
	        gemSlots.Add(eInventorySlot.LeftBracer);
	        gemSlots.Add(eInventorySlot.RightBracer);

	        foreach (eInventorySlot islot in gemSlots)
	        {
		        GeneratedUniqueItem item = null;
		        item = new GeneratedUniqueItem(realm, charclass, (byte)(player.Level + freeLootLevelOffset), EObjectType.Magical, islot);
		        item.AllowAdd = true;
		        item.Color = 0;
		        item.IsTradable = false;
		        item.Price = 1;
		        GameServer.Database.AddObject(item);
		        InventoryItem invitem = GameInventoryItem.Create<DbItemUnique>(item);
		        player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, invitem);
		        //player.Out.SendMessage("Generated: " + item.Name, eChatType.CT_System, eChatLoc.CL_SystemWindow);
	        }
        }

		public static void GenerateWeapon(GameLiving player, ECharacterClass charClass, EObjectType type, eInventorySlot invSlot)
        {
			//need to figure out shield size
			EColor color = EColor.White;
			ERealm realm = player.Realm;
			switch (realm)
			{
				case ERealm.Hibernia:
					color = EColor.Green_4;
					break;
				case ERealm.Albion:
					color = EColor.Red_4;
					break;
				case ERealm.Midgard:
					color = EColor.Blue_4;
					break;
			}
			if(type == EObjectType.Shield)
            {
				int shieldSize = GetShieldSizeFromClass(charClass);
                for (int i = 0; i < shieldSize; i++)
                {
					GeneratedUniqueItem item = null;
					item = new GeneratedUniqueItem(realm, charClass, (byte)(player.Level + freeLootLevelOffset), type, invSlot, (EDamageType)i+1);
					item.AllowAdd = true;
					item.Color = (int)color;
					item.IsTradable = false;
					item.Price = 1;
					GameServer.Database.AddObject(item);
					InventoryItem invitem = GameInventoryItem.Create<DbItemUnique>(item);
					player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, invitem);
				}
				
			}
			else if (type == EObjectType.Flexible)
            {
				//slash flex
				GeneratedUniqueItem dmgTypeItem = new GeneratedUniqueItem(realm, charClass, (byte)(player.Level + freeLootLevelOffset), type, invSlot, EDamageType.Slash);
				dmgTypeItem.AllowAdd = true;
				dmgTypeItem.Color = (int)color;
				dmgTypeItem.IsTradable = false;
				dmgTypeItem.Price = 1;
				GameServer.Database.AddObject(dmgTypeItem);
				InventoryItem tempItem = GameInventoryItem.Create<DbItemUnique>(dmgTypeItem);
				player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, tempItem);

				//crush flex
				GeneratedUniqueItem dmgTypeItem2 = new GeneratedUniqueItem(realm, charClass, (byte)(player.Level + freeLootLevelOffset), type, invSlot, EDamageType.Crush);
				dmgTypeItem2.AllowAdd = true;
				dmgTypeItem2.Color = (int)color;
				dmgTypeItem2.IsTradable = false;
				dmgTypeItem2.Price = 1;
				GameServer.Database.AddObject(dmgTypeItem2);
				InventoryItem tempItem2 = GameInventoryItem.Create<DbItemUnique>(dmgTypeItem2);
				player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, tempItem2);
			}
			else if(type == EObjectType.TwoHandedWeapon || type == EObjectType.PolearmWeapon || type == EObjectType.LargeWeapons)
            {
				int endDmgType = 4; //default for all 3, slash/crush/thrust
				if(type == EObjectType.LargeWeapons || realm == ERealm.Midgard)
                {
					endDmgType = 3; //only slash/crush
                }

				//one for each damage type
                for (int i = 1; i < endDmgType; i++)
                {
					GeneratedUniqueItem dmgTypeItem = new GeneratedUniqueItem(realm, charClass, (byte)(player.Level + freeLootLevelOffset), type, invSlot, (EDamageType) i);
					dmgTypeItem.AllowAdd = true;
					dmgTypeItem.Color = (int)color;
					dmgTypeItem.IsTradable = false;
					dmgTypeItem.Price = 1;
					GameServer.Database.AddObject(dmgTypeItem);
					InventoryItem tempItem = GameInventoryItem.Create<DbItemUnique>(dmgTypeItem);
					player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, tempItem);
				}	
			} else
            {
				GeneratedUniqueItem item = null;
				item = new GeneratedUniqueItem(realm, charClass, (byte)(player.Level + freeLootLevelOffset), type, invSlot);
				item.AllowAdd = true;
				item.Color = (int)color;
				item.IsTradable = false;
				item.Price = 1;
				GameServer.Database.AddObject(item);
				InventoryItem invitem = GameInventoryItem.Create<DbItemUnique>(item);
				player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, invitem);
			}	
		}

        public static int GetShieldSizeFromClass(ECharacterClass charClass)
        {
			//shield size is based off of damage type
			//1 = small shield
			//2 = medium
			//3 = large
            switch (charClass)
            {
				case ECharacterClass.Berserker:
				case ECharacterClass.Skald:
				case ECharacterClass.Savage:
				case ECharacterClass.Healer:
				case ECharacterClass.Shaman:
				case ECharacterClass.Shadowblade:
				case ECharacterClass.Bard:
				case ECharacterClass.Druid:
				case ECharacterClass.Nightshade:
				case ECharacterClass.Ranger:
				case ECharacterClass.Infiltrator:
				case ECharacterClass.Minstrel:
				case ECharacterClass.Scout:
					return 1;

				case ECharacterClass.Thane:
				case ECharacterClass.Warden:
				case ECharacterClass.Blademaster:
				case ECharacterClass.Champion:
				case ECharacterClass.Mercenary:
				case ECharacterClass.Cleric:
					return 2;

				case ECharacterClass.Warrior:
				case ECharacterClass.Hero:
				case ECharacterClass.Armsman:
				case ECharacterClass.Paladin:
				case ECharacterClass.Reaver:
					return 3;
				default: return 1;
            }
        }

        public static List<EObjectType> GenerateWeaponsForClass(ECharacterClass charClass, GameLiving player) {
			List<EObjectType> weapons = new List<EObjectType>();

            switch (charClass) {
				case ECharacterClass.Friar:
				case ECharacterClass.Cabalist:
				case ECharacterClass.Sorcerer:
				case ECharacterClass.Theurgist:
				case ECharacterClass.Wizard:
				case ECharacterClass.Necromancer:
				case ECharacterClass.Animist:
				case ECharacterClass.Eldritch:
				case ECharacterClass.Enchanter:
				case ECharacterClass.Mentalist:
				case ECharacterClass.Runemaster:
				case ECharacterClass.Spiritmaster:
				case ECharacterClass.Bonedancer:
					GenerateWeapon(player, charClass, EObjectType.Staff, eInventorySlot.TwoHandWeapon);
					break;

				case ECharacterClass.Valewalker:
					GenerateWeapon(player, charClass, EObjectType.Scythe, eInventorySlot.TwoHandWeapon); ;
					break;

				case ECharacterClass.Reaver:
					GenerateWeapon(player, charClass, EObjectType.Flexible, eInventorySlot.RightHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.CrushingWeapon, eInventorySlot.RightHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.ThrustWeapon, eInventorySlot.RightHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.SlashingWeapon, eInventorySlot.RightHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Shield, eInventorySlot.LeftHandWeapon);
					break;

				case ECharacterClass.Savage:
					GenerateWeapon(player, charClass, EObjectType.HandToHand, eInventorySlot.LeftHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.HandToHand, eInventorySlot.RightHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Axe, eInventorySlot.TwoHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Hammer, eInventorySlot.TwoHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Sword, eInventorySlot.TwoHandWeapon);
					break;

				case ECharacterClass.Berserker:
					GenerateWeapon(player, charClass, EObjectType.Axe, eInventorySlot.RightHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Hammer, eInventorySlot.RightHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Sword, eInventorySlot.RightHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.LeftAxe, eInventorySlot.LeftHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Axe, eInventorySlot.TwoHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Hammer, eInventorySlot.TwoHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Sword, eInventorySlot.TwoHandWeapon);
					break;

				case ECharacterClass.Shadowblade:
					GenerateWeapon(player, charClass, EObjectType.Axe, eInventorySlot.RightHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Sword, eInventorySlot.RightHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.LeftAxe, eInventorySlot.LeftHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Axe, eInventorySlot.TwoHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Sword, eInventorySlot.TwoHandWeapon);
					break;

				case ECharacterClass.Warrior:
				case ECharacterClass.Thane:
					GenerateWeapon(player, charClass, EObjectType.Axe, eInventorySlot.RightHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Hammer, eInventorySlot.RightHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Sword, eInventorySlot.RightHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Shield, eInventorySlot.LeftHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Axe, eInventorySlot.TwoHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Hammer, eInventorySlot.TwoHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Sword, eInventorySlot.TwoHandWeapon);
					break;

				case ECharacterClass.Skald:
					GenerateWeapon(player, charClass, EObjectType.Axe, eInventorySlot.RightHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Hammer, eInventorySlot.RightHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Sword, eInventorySlot.RightHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Shield, eInventorySlot.LeftHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Axe, eInventorySlot.TwoHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Hammer, eInventorySlot.TwoHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Sword, eInventorySlot.TwoHandWeapon);
					break;

				case ECharacterClass.Hunter:
					GenerateWeapon(player, charClass, EObjectType.Sword, eInventorySlot.RightHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Sword, eInventorySlot.TwoHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.CompositeBow, eInventorySlot.DistanceWeapon);
					GenerateWeapon(player, charClass, EObjectType.Spear, eInventorySlot.TwoHandWeapon);
					break;

				case ECharacterClass.Healer:
				case ECharacterClass.Shaman:
					GenerateWeapon(player, charClass, EObjectType.Staff, eInventorySlot.TwoHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Shield, eInventorySlot.LeftHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Hammer, eInventorySlot.RightHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Hammer, eInventorySlot.TwoHandWeapon);
					break;

				case ECharacterClass.Bard:
					GenerateWeapon(player, charClass, EObjectType.Instrument, eInventorySlot.DistanceWeapon);
					GenerateWeapon(player, charClass, EObjectType.Shield, eInventorySlot.LeftHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Blades, eInventorySlot.RightHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Blunt, eInventorySlot.RightHandWeapon);
					break;

				case ECharacterClass.Warden:
					GenerateWeapon(player, charClass, EObjectType.Shield, eInventorySlot.LeftHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Blades, eInventorySlot.RightHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Blunt, eInventorySlot.RightHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Fired, eInventorySlot.DistanceWeapon);
					break;

				case ECharacterClass.Druid:
					GenerateWeapon(player, charClass, EObjectType.Shield, eInventorySlot.LeftHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Blades, eInventorySlot.RightHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Blunt, eInventorySlot.RightHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Staff, eInventorySlot.TwoHandWeapon);
					break;

				case ECharacterClass.Blademaster:
					GenerateWeapon(player, charClass, EObjectType.Shield, eInventorySlot.LeftHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Blades, eInventorySlot.RightHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Blunt, eInventorySlot.RightHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Piercing, eInventorySlot.RightHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Blades, eInventorySlot.LeftHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Blunt, eInventorySlot.LeftHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Piercing, eInventorySlot.LeftHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Fired, eInventorySlot.DistanceWeapon);
					
					break;

				case ECharacterClass.Hero:
					GenerateWeapon(player, charClass, EObjectType.Blades, eInventorySlot.RightHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Blunt, eInventorySlot.RightHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Piercing, eInventorySlot.RightHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Shield, eInventorySlot.LeftHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.LargeWeapons, eInventorySlot.TwoHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.CelticSpear, eInventorySlot.TwoHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Fired, eInventorySlot.DistanceWeapon);
					break;

				case ECharacterClass.Champion:
					GenerateWeapon(player, charClass, EObjectType.Blades, eInventorySlot.RightHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Blunt, eInventorySlot.RightHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Piercing, eInventorySlot.RightHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Shield, eInventorySlot.LeftHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.LargeWeapons, eInventorySlot.TwoHandWeapon);
					break;

				case ECharacterClass.Ranger:
					GenerateWeapon(player, charClass, EObjectType.RecurvedBow, eInventorySlot.DistanceWeapon);
					goto case ECharacterClass.Nightshade;

				case ECharacterClass.Nightshade:
					GenerateWeapon(player, charClass, EObjectType.Blades, eInventorySlot.RightHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Piercing, eInventorySlot.RightHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Blades, eInventorySlot.LeftHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Piercing, eInventorySlot.LeftHandWeapon);
					break;

				case ECharacterClass.Scout:
					GenerateWeapon(player, charClass, EObjectType.Longbow, eInventorySlot.DistanceWeapon);
					GenerateWeapon(player, charClass, EObjectType.Shield, eInventorySlot.LeftHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.SlashingWeapon, eInventorySlot.RightHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.CrushingWeapon, eInventorySlot.RightHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.ThrustWeapon, eInventorySlot.RightHandWeapon);
					break;

				case ECharacterClass.Minstrel:
					GenerateWeapon(player, charClass, EObjectType.Instrument, eInventorySlot.DistanceWeapon);
					GenerateWeapon(player, charClass, EObjectType.SlashingWeapon, eInventorySlot.RightHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Shield, eInventorySlot.LeftHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.ThrustWeapon, eInventorySlot.RightHandWeapon);
					break;

				case ECharacterClass.Infiltrator:
					GenerateWeapon(player, charClass, EObjectType.SlashingWeapon, eInventorySlot.RightHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.ThrustWeapon, eInventorySlot.RightHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.SlashingWeapon, eInventorySlot.LeftHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.ThrustWeapon, eInventorySlot.LeftHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Crossbow, eInventorySlot.DistanceWeapon);
					break;

				case ECharacterClass.Cleric:
					GenerateWeapon(player, charClass, EObjectType.CrushingWeapon, eInventorySlot.RightHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Shield, eInventorySlot.LeftHandWeapon);
					break;

				case ECharacterClass.Armsman:
					GenerateWeapon(player, charClass, EObjectType.PolearmWeapon, eInventorySlot.TwoHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Crossbow, eInventorySlot.DistanceWeapon);
					goto case ECharacterClass.Paladin;

				case ECharacterClass.Paladin: //hey one guy might get these :')
					GenerateWeapon(player, charClass, EObjectType.TwoHandedWeapon, eInventorySlot.TwoHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Shield, eInventorySlot.LeftHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.SlashingWeapon, eInventorySlot.RightHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.CrushingWeapon, eInventorySlot.RightHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.ThrustWeapon, eInventorySlot.RightHandWeapon);
					break;

				case ECharacterClass.Mercenary:
					GenerateWeapon(player, charClass, EObjectType.SlashingWeapon, eInventorySlot.RightHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.CrushingWeapon, eInventorySlot.RightHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.ThrustWeapon, eInventorySlot.RightHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.SlashingWeapon, eInventorySlot.LeftHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.CrushingWeapon, eInventorySlot.LeftHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.ThrustWeapon, eInventorySlot.LeftHandWeapon);
					GenerateWeapon(player, charClass, EObjectType.Fired, eInventorySlot.DistanceWeapon);
					GenerateWeapon(player, charClass, EObjectType.Shield, eInventorySlot.LeftHandWeapon);
					break;


				default:
					weapons.Add(EObjectType.GenericWeapon);
					break;
					
            }

			return weapons;
		}

        public static void GenerateArmor(GamePlayer player)
        {
	        var color = EColor.White;
	        var realm = player.Realm;
	        var charclass = (ECharacterClass)player.CharacterClass.ID;
	        var armorType = GetArmorType(realm, charclass, player.Level);

	        switch (realm) {
		        case ERealm.Hibernia:
			        color = EColor.Green_4;
			        break;
		        case ERealm.Albion:
			        color = EColor.Red_4;
			        break;
		        case ERealm.Midgard:
			        color = EColor.Blue_4;
			        break;
	        }
	        
	        List<eInventorySlot> bodySlots = new List<eInventorySlot>();
	        bodySlots.Add(eInventorySlot.ArmsArmor);
	        bodySlots.Add(eInventorySlot.FeetArmor);
	        bodySlots.Add(eInventorySlot.HandsArmor);
	        bodySlots.Add(eInventorySlot.HeadArmor);
	        bodySlots.Add(eInventorySlot.LegsArmor);
	        bodySlots.Add(eInventorySlot.TorsoArmor);

	        foreach (eInventorySlot islot in bodySlots) {
		        GeneratedUniqueItem item = null;
		        item = new GeneratedUniqueItem(realm, charclass, (byte)(player.Level + freeLootLevelOffset), armorType, islot);
		        item.AllowAdd = true;
		        item.Color = (int)color;
		        item.IsTradable = false;
		        item.Price = 1;
		        GameServer.Database.AddObject(item);
		        InventoryItem invitem = GameInventoryItem.Create<DbItemUnique>(item);
		        player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, invitem);
		        //player.Out.SendMessage("Generated: " + item.Name, eChatType.CT_System, eChatLoc.CL_SystemWindow);
	        }
        }
    }
}