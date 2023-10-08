﻿/*
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

		public override double GetArmorAbsorb(EArmorSlot slot)
		{
			return base.GetArmorAbsorb(slot) - 0.05;
		}

		protected override KeepGuardBrain GetBrain() => new KeepGuardBrain();
		
		protected override IPlayerClass GetClass()
		{
			if (ModelRealm == ERealm.Albion) return new ClassArmsman();
			else if (ModelRealm == ERealm.Midgard) return new ClassWarrior();
			else if (ModelRealm == ERealm.Hibernia) return new ClassHero();
			return new DefaultPlayerClass();
		}
		protected override void SetName()
		{
			switch (ModelRealm)
			{
				case ERealm.None:
				case ERealm.Albion:
					if (IsPortalKeepGuard)
					{
						if (Gender == EGender.Female)
							Name = "Johanna";
						else Name = "Johann";
					}
					else
					{
						if (Gender == EGender.Female)
							Name = "Ulrike";
						else Name = "Ulrich";
					}
					GuildName = "Merchant";
					break;
				case ERealm.Midgard:
					if (IsPortalKeepGuard)
					{
						if (Gender == EGender.Female)
							Name = "Sarina";
						else Name = "Sander";
					}
					else
					{
						if (Gender == EGender.Female)
							Name = "Kaira";
						else Name = "Kaj";
					}
					break;
				case ERealm.Hibernia:
					if (IsPortalKeepGuard)
					{
						if (Gender == EGender.Female)
							Name = "Daireann";
						else Name = "Drystan";
					}
					else
					{
						if (Gender == EGender.Female)
							Name = "Moja";
						else Name = "Maeron";
					}
					break;
			}

			if (Realm == ERealm.None)
			{
				if (Gender == EGender.Female)
					Name = "Finnja";
				else Name = "Fynn";
			}
		}
	}
}
