using System.Collections.Generic;
using DOL.Database;
using DOL.GS.Effects;

namespace DOL.GS.RealmAbilities
{
	public class NfRaNaturesWombAbility : Rr5RealmAbility
	{
		public NfRaNaturesWombAbility(DbAbility dba, int level) : base(dba, level) { }

		/// <summary>
		/// Action
		/// </summary>
		/// <param name="living"></param>
		public override void Execute(GameLiving living)
		{
			if (CheckPreconditions(living, DEAD | SITTING | MEZZED | STUNNED)) return;



			GamePlayer player = living as GamePlayer;
			if (player != null)
			{
				SendCasterSpellEffectAndCastMessage(player, 7053, true);
				NfRaNaturesWombEffect effect = new NfRaNaturesWombEffect();
				effect.Start(player);
			}
			DisableSkill(living);
		}

		public override int GetReUseDelay(int level)
		{
			return 600;
		}

		public override void AddEffectsInfo(IList<string> list)
		{
			list.Add("Stuns you for 5 seconds. During this time all damage is converted into healing!");
			list.Add("");
			list.Add("Target: Self");
			list.Add("Duration: 5 sec");
			list.Add("Casting time: instant");
		}

	}
}