
namespace DOL.GS
{
    /// <summary>
    /// The type of stat
    /// </summary>
    public enum eStat : byte
	{
		UNDEFINED = 0,
		_First = eProperty.Stat_First,
		STR = eProperty.Strength,
		DEX = eProperty.Dexterity,
		CON = eProperty.Constitution,
		QUI = eProperty.Quickness,
		INT = eProperty.Intelligence,
		PIE = eProperty.Piety,
		EMP = eProperty.Empathy,
		CHR = eProperty.Charisma,
		ACU = eProperty.Acuity,
		_Last = eProperty.Stat_Last,
	}
}
