using DOL.AI.Brain;
using DOL.GS.PlayerClass;

namespace DOL.GS.Keeps
{
	public class GuardCurrencyMerchant : GameAtlasGuardMerchant
	{
		public override bool AddToWorld()
		{
			TradeItems = new MerchantTradeItems("summonmerchant_merchant");
			GuildName = "Orb Merchant";
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
							Name = "Johanna";
						else Name = "Johann";
					}
					else
					{
						if (Gender == eGender.Female)
							Name = "Ulrike";
						else Name = "Ulrich";
					}
					GuildName = "Merchant";
					break;
				case eRealm.Midgard:
					if (IsPortalKeepGuard)
					{
						if (Gender == eGender.Female)
							Name = "Sarina";
						else Name = "Sander";
					}
					else
					{
						if (Gender == eGender.Female)
							Name = "Kaira";
						else Name = "Kaj";
					}
					break;
				case eRealm.Hibernia:
					if (IsPortalKeepGuard)
					{
						if (Gender == eGender.Female)
							Name = "Daireann";
						else Name = "Drystan";
					}
					else
					{
						if (Gender == eGender.Female)
							Name = "Moja";
						else Name = "Maeron";
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
