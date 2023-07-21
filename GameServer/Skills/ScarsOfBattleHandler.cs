using DOL.GS;
using DOL.Database;

namespace DOL.GS.SkillHandler
{
	[SkillHandlerAttribute(Abilities.ScarsOfBattle)]
	public class ScarsOfBattleHandler : StatChangingAbility
	{
		public ScarsOfBattleHandler(DbAbilities dba, int level)
			: base(dba, 1, EProperty.MaxHealth)
		{
		}
		public override int GetAmountForLevel(int level)
		{
			return 10;
		}
	}
}
