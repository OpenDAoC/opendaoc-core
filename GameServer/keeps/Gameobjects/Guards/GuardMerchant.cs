using DOL.AI.Brain;
using DOL.GS.PlayerClass;

namespace DOL.GS.Keeps
{
	public class GuardMerchant : GameGuardMerchant
	{
		public override bool AddToWorld()
		{
			switch (Realm)
			{
				case eRealm.Albion:
					TradeItems = new MerchantTradeItems("AlbRvRCraftingList");
					break;
				case eRealm.Midgard:
					TradeItems = new MerchantTradeItems("MidRvRCraftingList");
					break;
				case eRealm.Hibernia:
					TradeItems = new MerchantTradeItems("HibRvRCraftingList");
					break;
			}

			GuildName = "Merchant";
			return base.AddToWorld();
		}

		public override double GetArmorAbsorb(eArmorSlot slot)
		{
			return base.GetArmorAbsorb(slot) - 0.05;
		}

		protected override KeepGuardBrain GetBrain() => new KeepGuardBrain();
		
		protected override ICharacterClass GetClass()
		{
			if (ModelRealm == eRealm.Albion) return new ClassArmsman();
			else if (ModelRealm == eRealm.Midgard) return new ClassWarrior();
			else if (ModelRealm == eRealm.Hibernia) return new ClassHero();
			return new DefaultCharacterClass();
		}
		protected override void SetName()
		{
			switch (ModelRealm)
			{
				case eRealm.None:
				case eRealm.Albion:
					if (IsPortalKeepGuard)
					{
						if (Gender == eGender.Female)
							Name = "Frida";
						else Name = "Frederic";
					}
					else
					{
						if (Gender == eGender.Female)
							Name = "Fabienne";
						else Name = "Francis";
					}
					GuildName = "Merchant";
					break;
				case eRealm.Midgard:
					if (IsPortalKeepGuard)
					{
						if (Gender == eGender.Female)
							Name = "Olga";
						else Name = "Odun";
					}
					else
					{
						if (Gender == eGender.Female)
							Name = "Rikke";
						else Name = "Rollo";
					}
					break;
				case eRealm.Hibernia:
					if (IsPortalKeepGuard)
					{
						if (Gender == eGender.Female)
							Name = "Alenja";
						else Name = "Airell";
					}
					else
					{
						if (Gender == eGender.Female)
							Name = "Arwen";
						else Name = "Aidan";
					}
					break;
			}

			if (Realm == eRealm.None)
			{
				if (Gender == eGender.Female)
					Name = "Finnja";
				else Name = "Fynn";
			}
		}
	}
}
