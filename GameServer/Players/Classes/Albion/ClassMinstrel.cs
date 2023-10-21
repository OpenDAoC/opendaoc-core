using System.Collections.Generic;
using Core.GS.Enums;
using Core.GS.Players.Races;
using Core.GS.Skills;

namespace Core.GS.Players.Classes;

[PlayerClass((int)EPlayerClass.Minstrel, "Minstrel", "Rogue")]
public class ClassMinstrel : ClassAlbionRogue
{
	private static readonly string[] AutotrainableSkills = new[] { SpecConstants.Instruments };

	public ClassMinstrel()
		: base()
	{
		m_profession = "PlayerClass.Profession.Academy";
		m_specializationMultiplier = 15;
		m_primaryStat = EStat.CHR;
		m_secondaryStat = EStat.DEX;
		m_tertiaryStat = EStat.STR;
		m_manaStat = EStat.CHR;
		m_wsbase = 360;
		m_baseHP = 720;
	}

	public override EPlayerClassType ClassType
	{
		get { return EPlayerClassType.Hybrid; }
	}

	public override IList<string> GetAutotrainableSkills()
	{
		return AutotrainableSkills;
	}

	public override bool HasAdvancedFromBaseClass()
	{
		return true;
	}

	public override ushort MaxPulsingSpells
	{
		get { return 2; }
	}

	public override List<PlayerRace> EligibleRaces => new List<PlayerRace>()
	{
		PlayerRace.Briton, PlayerRace.Highlander, PlayerRace.Saracen,
	};
}