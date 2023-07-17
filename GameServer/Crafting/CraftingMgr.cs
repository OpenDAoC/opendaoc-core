using System;
using System.Collections.Generic;
using System.Reflection;
using DOL.Database;
using DOL.GS.PacketHandler;
using log4net;

namespace DOL.GS
{
	/// <summary>
	/// Enum of all crafting skill (related to client file)
	/// </summary>
	public enum eCraftingSkill : int
	{
		NoCrafting = 0,
		WeaponCrafting = 1,
		ArmorCrafting = 2,
		SiegeCrafting = 3,
		Alchemy = 4,
		MetalWorking = 6,
		LeatherCrafting = 7,
		ClothWorking = 8,
		GemCutting = 9,
		HerbalCrafting = 10,
		Tailoring = 11,
		Fletching = 12,
		SpellCrafting = 13,
		WoodWorking = 14,
		BasicCrafting = 15,
		_Last = 15,
	}

	/// <summary>
	/// Description r�sum�e de CraftingMgr.
	/// </summary>
	public class CraftingMgr
	{
		/// <summary>
		/// Defines a logger for this class.
		/// </summary>
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		/// Hold all crafting skill
		/// </summary>
		protected static AbstractCraftingSkill[] m_craftingskills = new AbstractCraftingSkill[(int)eCraftingSkill._Last];

		/// <summary>
		/// get a crafting skill by the enum index
		/// </summary>
		/// <param name="skill"></param>
		/// <returns></returns>
		public static AbstractCraftingSkill getSkillbyEnum(eCraftingSkill skill)
		{
			if (skill == eCraftingSkill.NoCrafting) return null;
			return m_craftingskills[(int)skill - 1] as AbstractCraftingSkill;
		}

		/// <summary>
		/// Initialize the crafting system
		/// </summary>
		/// <returns></returns>
		public static bool Init()
		{
			// skill
			m_craftingskills[(int)eCraftingSkill.ArmorCrafting - 1] = new ArmorCraftingSkill();
			m_craftingskills[(int)eCraftingSkill.Fletching - 1] = new FletchingSkill();
			m_craftingskills[(int)eCraftingSkill.SiegeCrafting - 1] = new SiegeCraftingSkill();
			m_craftingskills[(int)eCraftingSkill.Tailoring - 1] = new TailoringSkill();
			m_craftingskills[(int)eCraftingSkill.WeaponCrafting - 1] = new WeaponCraftingSkill();

			m_craftingskills[(int)eCraftingSkill.ClothWorking - 1] = new ClothWorkingSkill();
			m_craftingskills[(int)eCraftingSkill.GemCutting - 1] = new GemCuttingSkill();
			m_craftingskills[(int)eCraftingSkill.HerbalCrafting - 1] = new HerbCraftingSkill();
			m_craftingskills[(int)eCraftingSkill.LeatherCrafting - 1] = new LeatherWorkingSkill();
			m_craftingskills[(int)eCraftingSkill.MetalWorking - 1] = new MetalWorkingSkill();
			m_craftingskills[(int)eCraftingSkill.WoodWorking - 1] = new WoodWorkingSkill();
			m_craftingskills[(int)eCraftingSkill.BasicCrafting - 1] = new BasicCraftingSkill();

			//Advanced skill
			m_craftingskills[(int)eCraftingSkill.Alchemy - 1] = new AlchemySkill();
			m_craftingskills[(int)eCraftingSkill.SpellCrafting - 1] = new SpellCraftingSkill();

			return true;
		}

		#region Global craft functions

		/// <summary>
		/// Return the crafting skill which created the item
		/// </summary>
		public static eCraftingSkill GetCraftingSkill(InventoryItem item)
		{
			if (!item.IsCrafted)
				return eCraftingSkill.NoCrafting;

			switch (item.Object_Type)
			{
				case (int)EObjectType.Cloth:
				case (int)EObjectType.Leather:
					return eCraftingSkill.Tailoring;

				case (int)EObjectType.Studded:
				case (int)EObjectType.Reinforced:
				case (int)EObjectType.Chain:
				case (int)EObjectType.Scale:
				case (int)EObjectType.Plate:
					return eCraftingSkill.ArmorCrafting;

				// all weapon
				case (int)EObjectType.Axe:
				case (int)EObjectType.Blades:
				case (int)EObjectType.Blunt:
				case (int)EObjectType.CelticSpear:
				case (int)EObjectType.CrushingWeapon:
				case (int)EObjectType.Flexible:
				case (int)EObjectType.Hammer:
				case (int)EObjectType.HandToHand:
				case (int)EObjectType.LargeWeapons:
				case (int)EObjectType.LeftAxe:
				case (int)EObjectType.Piercing:
				case (int)EObjectType.PolearmWeapon:
				case (int)EObjectType.Scythe:
				case (int)EObjectType.Shield:
				case (int)EObjectType.SlashingWeapon:
				case (int)EObjectType.Spear:
				case (int)EObjectType.Sword:
				case (int)EObjectType.ThrustWeapon:
				case (int)EObjectType.TwoHandedWeapon:
					return eCraftingSkill.WeaponCrafting;

				case (int)EObjectType.CompositeBow:
				case (int)EObjectType.Crossbow:
				case (int)EObjectType.Fired:
				case (int)EObjectType.Instrument:
				case (int)EObjectType.Longbow:
				case (int)EObjectType.RecurvedBow:
				case (int)EObjectType.Staff:
					return eCraftingSkill.Fletching;

				case (int)EObjectType.AlchemyTincture:
				case (int)EObjectType.Poison:
					return eCraftingSkill.Alchemy;

				case (int)EObjectType.SpellcraftGem:
					return eCraftingSkill.SpellCrafting;

				case (int)EObjectType.SiegeBalista:
				case (int)EObjectType.SiegeCatapult:
				case (int)EObjectType.SiegeCauldron:
				case (int)EObjectType.SiegeRam:
				case (int)EObjectType.SiegeTrebuchet:
					return eCraftingSkill.SiegeCrafting;

				default:
					return eCraftingSkill.NoCrafting;
			}
		}

		/// <summary>
		/// Return the crafting skill needed to work on the item
		/// </summary>
		public static eCraftingSkill GetSecondaryCraftingSkillToWorkOnItem(InventoryItem item)
		{
			switch (item.Object_Type)
			{
				case (int)EObjectType.Cloth:
					return eCraftingSkill.ClothWorking;

				case (int)EObjectType.Leather:
				case (int)EObjectType.Studded:
					return eCraftingSkill.LeatherCrafting;

				// all weapon
				case (int)EObjectType.Axe:
				case (int)EObjectType.Blades:
				case (int)EObjectType.Blunt:
				case (int)EObjectType.CelticSpear:
				case (int)EObjectType.CrushingWeapon:
				case (int)EObjectType.Flexible:
				case (int)EObjectType.Hammer:
				case (int)EObjectType.HandToHand:
				case (int)EObjectType.LargeWeapons:
				case (int)EObjectType.LeftAxe:
				case (int)EObjectType.Piercing:
				case (int)EObjectType.PolearmWeapon:
				case (int)EObjectType.Scythe:
				case (int)EObjectType.Shield:
				case (int)EObjectType.SlashingWeapon:
				case (int)EObjectType.Spear:
				case (int)EObjectType.Sword:
				case (int)EObjectType.ThrustWeapon:
				case (int)EObjectType.TwoHandedWeapon:
				// all other armor
				case (int)EObjectType.Chain:
				case (int)EObjectType.Plate:
				case (int)EObjectType.Reinforced:
				case (int)EObjectType.Scale:
					return eCraftingSkill.MetalWorking;

				case (int)EObjectType.CompositeBow:
				case (int)EObjectType.Crossbow:
				case (int)EObjectType.Fired:
				case (int)EObjectType.Instrument:
				case (int)EObjectType.Longbow:
				case (int)EObjectType.RecurvedBow:
				case (int)EObjectType.Staff:
					return eCraftingSkill.WoodWorking;
				
				case (int)EObjectType.Magical:
					return eCraftingSkill.GemCutting;

				default:
					return eCraftingSkill.NoCrafting;
			}
		}

		/// <summary>
		/// Return the approximative craft level of the item
		/// </summary>
		public static int GetItemCraftLevel(InventoryItem item)
		{
			switch (item.Object_Type)
			{
				case (int)EObjectType.Cloth:
				case (int)EObjectType.Leather:
				case (int)EObjectType.Studded:
				case (int)EObjectType.Chain:
				case (int)EObjectType.Plate:
				case (int)EObjectType.Reinforced:
				case (int)EObjectType.Scale:
					{
						int baseLevel = 15 + item.Level * 20; // gloves
						switch (item.Item_Type)
						{
							case (int)eInventorySlot.HeadArmor: // head
								return baseLevel + 15;

							case (int)eInventorySlot.FeetArmor: // feet
								return baseLevel + 30;

							case (int)eInventorySlot.LegsArmor: // legs
								return baseLevel + 50;

							case (int)eInventorySlot.ArmsArmor: // arms
								return baseLevel + 65;

							case (int)eInventorySlot.TorsoArmor: // torso
								return baseLevel + 80;

							default:
								return baseLevel;
						}
					}

				case (int)EObjectType.Axe:
				case (int)EObjectType.Blades:
				case (int)EObjectType.Blunt:
				case (int)EObjectType.CelticSpear:
				case (int)EObjectType.CrushingWeapon:
				case (int)EObjectType.Flexible:
				case (int)EObjectType.Hammer:
				case (int)EObjectType.HandToHand:
				case (int)EObjectType.LargeWeapons:
				case (int)EObjectType.LeftAxe:
				case (int)EObjectType.Piercing:
				case (int)EObjectType.PolearmWeapon:
				case (int)EObjectType.Scythe:
				case (int)EObjectType.Shield:
				case (int)EObjectType.SlashingWeapon:
				case (int)EObjectType.Spear:
				case (int)EObjectType.Sword:
				case (int)EObjectType.ThrustWeapon:
				case (int)EObjectType.TwoHandedWeapon:

				case (int)EObjectType.CompositeBow:
				case (int)EObjectType.Crossbow:
				case (int)EObjectType.Fired:
				case (int)EObjectType.Instrument:
				case (int)EObjectType.Longbow:
				case (int)EObjectType.RecurvedBow:
				case (int)EObjectType.Staff:
				case (int)EObjectType.Magical:
					return 15 + (item.Level - 1) * 20;

				default:
					return 0;
			}
		}

		#endregion

	}
}
