using DOL.GS;
using DOL.Database;

namespace DOL.GS.SkillHandler
{
	[SkillHandlerAttribute(Abilities.ScarsOfBattle)]
	public class ScarsOfBattle : StatChangingAbility
	{
		public ScarsOfBattle(DbAbilities dba, int level)
			: base(dba, 1, eProperty.MaxHealth)
		{
		}
		public override int GetAmountForLevel(int level)
		{
			return 10;
		}
	}
}
