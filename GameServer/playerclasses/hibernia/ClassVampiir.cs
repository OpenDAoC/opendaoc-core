using System.Collections.Generic;
using DOL.GS.Realm;

namespace DOL.GS.PlayerClass
{
    [CharacterClass((int)ECharacterClass.Vampiir, "Vampiir", "Stalker")]
    public class ClassVampiir : ClassStalker
    {
        public ClassVampiir()
            : base()
        {
            m_profession = "PlayerClass.Profession.PathofAffinity";
            m_specializationMultiplier = 15;
            m_primaryStat = EStat.CON;
            m_secondaryStat = EStat.STR;
            m_tertiaryStat = EStat.DEX;
            //Vampiirs do not have a mana stat
            //Special handling is need in the power pool calculator
            //m_manaStat = eStat.STR;
            m_wsbase = 440;
            m_baseHP = 878;
        }

        public override eClassType ClassType
        {
            get { return eClassType.ListCaster; }
        }

        public override bool HasAdvancedFromBaseClass()
        {
            return true;
        }

        public override List<PlayerRace> EligibleRaces => new List<PlayerRace>()
        {
            // PlayerRace.Celt, PlayerRace.Lurikeen, PlayerRace.Shar,
        };
    }
}
