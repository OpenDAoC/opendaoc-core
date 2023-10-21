namespace DOL.GS;

public enum ESpellType : short
{
    SavageCombatSpeedBuff = 0,
    SavageEvadeBuff = 1,
    SavageCrushResistanceBuff = 2,
    SavageThrustResistanceBuff = 3,
    ArmorFactorBuff = 4,
    PaladinArmorFactorBuff = 5,
    HealthRegenBuff = 6,
    SummonTheurgistPet = 7,
    CombatHeal = 8,
    DamageAdd = 9,
    Resurrect = 10,
    BodyResistBuff = 11,
    SpiritResistBuff = 12,
    EnergyResistBuff = 13,
    HeatResistBuff = 14,
    ColdResistBuff = 15,
    MatterResistBuff = 16,
    Taunt = 17,
    EnduranceRegenBuff = 18,
    HeatColdMatterBuff = 19,
    BodySpiritEnergyBuff = 20,
    SpeedEnhancement = 21,
    Lifedrain = 22,
    DamageShield = 23,
    Bladeturn = 24,
    DirectDamage = 25,
    SpeedDecrease = 26,
    SummonAnimistPet = 27,
    Bomber = 28,
    SummonAnimistFnFCustom = 29,
    DamageOverTime = 30,
    PowerRegenBuff = 31,
    CombatSpeedBuff = 32,
    ArmorAbsorptionBuff = 33,
    OffensiveProc = 34,
    DefensiveProc = 35,
    ConstitutionDebuff = 36,
    DirectDamageWithDebuff = 37,
    MeleeDamageDebuff = 38,
    ArmorAbsorptionDebuff = 39,
    Charm = 40,
    Heal = 41,
    Stun = 42,
    AblativeArmor = 43,
    TurretPBAoE = 44,
    SummonAnimistFnF = 45,
    TurretsRelease = 46,
    Mesmerize = 47,
    Confusion = 48,
    CureMezz = 49,
    CombatSpeedDebuff = 50,
    DamageSpeedDecrease = 51,
    CurePoison = 52,
    CureDisease = 53,
    CureNearsightCustom = 54,
    SpreadHeal = 55,
    StrengthBuff = 56,
    DexterityBuff = 57,
    ConstitutionBuff = 58,
    StrengthConstitutionBuff = 59,
    DexterityQuicknessBuff = 60,
    AcuityBuff = 61,
    MeleeDamageBuff = 62,
    FatigueConsumptionBuff = 63,
    ArcheryDoT = 64,
    SavageDPSBuff = 65,
    StyleTaunt = 66,
    StyleCombatSpeedDebuff = 67,
    StrengthDebuff = 68,
    SlashResistDebuff = 69,
    StyleBleeding = 70,
    StyleSpeedDecrease = 71,
    StyleStun = 72,
    SavageSlashResistanceBuff = 73,
    SpeedOfTheRealm = 74,
    DexterityDebuff = 75,
    DexterityQuicknessDebuff = 76,
    BodyResistDebuff = 77,
    SpiritResistDebuff = 78,
    EnergyResistDebuff = 79,
    Nearsight = 80,
    StrengthConstitutionDebuff = 81,
    LifeTransfer = 82,
    SummonSpiritFighter = 83,
    MesmerizeDurationBuff = 84,
    Bolt = 85,
    HeatResistDebuff = 86,
    ColdResistDebuff = 87,
    MatterResistDebuff = 88,
    ArmorFactorDebuff = 89,
    HealOverTime = 90,
    Amnesia = 91,
    Disease = 92,
    SummonHunterPet = 93,
    SummonUnderhill = 94,
    HereticPiercingMagic = 95,
    SummonDruidPet = 96,
    NightshadeNuke = 97,
    SavageParryBuff = 98,
    SavageEnduranceHeal = 99,
    ArrowDamageTypes = 100,
    Archery = 101,
    SiegeArrow = 102,
    SummonSimulacrum = 103,
    PetConversion = 104,
    PetSpell = 105,
    PowerDrainPet = 106,
    PowerTransferPet = 107,
    FacilitatePainworking = 108,
    SummonNecroPet = 109,
    SummonNoveltyPet = 110,
    EternalPlant = 111,
    PowerHealthEnduranceRegenBuff = 112,
    GatewayPersonalBind = 113,
    UniPortal = 114,
    FrontalPulseConeDD = 115,

    //--------------------Added when refactoring--------------------
    MercHeal = 116,
    OmniHeal = 117,
    PBAoEHeal = 118,
    SummonHealingElemental = 119,
    Pet = 120, // May not be needed.
    AFHitsBuff = 121,
    AllMagicResistBuff = 122,
    Buff = 123,
    CelerityBuff = 124,
    CourageBuff = 125,
    CrushSlashTrustBuff = 126,
    EffectivenessBuff = 127,
    FlexibleSkillBuff = 128,
    HasteBuff = 129,
    HeroismBuff = 130,
    KeepDamageBuff = 131,
    MLABSBuff = 132,
    ParryBuff = 133,
    MagicResistBuff = 134,
    SuperiorCourageBuff = 135,
    ToHitBuff = 136,
    WeaponSkillBuff = 137,
    Summon = 138,
    SummonMinion = 139,
    SummonCommander = 140,
    AllStatsPercentDebuff = 141,
    CrushSlashThrustDebuff = 142,
    EffectivenessDebuff = 143,
    Mez = 144,
    StyleHandler = 145,
    MLStyleHandler = 146,
    BodyguardHandler = 147,
    Chamber = 148,
    Morph = 149,
    MagicalStrike = 150,
    UnrresistableNonImunityStun = 151,
    Disarm = 152,
    Uninterruptable = 153,
    Powerless = 154,
    Range = 155,
    Prescience = 156,
    PowerRend = 157,
    SpeedWrap = 158,
    FumbleChanceDebuff = 159,
    SiegeDirectDamage = 160,
    DirectDamageNoVariance = 161,
    PowerOverTime = 162,
    PoisonspikeDot = 163,
    UnresistableStun = 164,
    BloodRage = 165,
    HeightenedAwareness = 166,
    SubtleKills = 167,
    StormMissHit = 168,
    StormEnduDrain = 169,
    StormDexQuickDebuff = 170,
    PowerDrainStorm = 171,
    StormStrConstDebuff = 172,
    StormAcuityDebuff = 173,
    StormEnergyTempest = 174,
    DamageSpeedDecreaseNoVariance = 175,
    PetLifedrain = 176,
    Phaseshift = 177,
    Grapple = 178,
    BrittleGuard = 179,
    StyleRange = 180,
    MultiTarget = 181,
    PiercingMagic = 182,
    PveResurrectionIllness = 183,
    RvrResurrectionIllness = 184,
    UnbreakableSpeedDecrease = 185,
    WeaponSkillConstitutionDebuff = 186,
    EnduranceHeal = 187,
    PowerHeal = 188,
    FatigueConsumptionDebuff = 189,
    NaturesShield = 191,
    AllStatsBarrel = 192,
    DexterityConstitutionDebuff = 193,
    ComfortingFlames = 194,
    AllRegenBuff = 195,
    SummonMerchant = 196,
    BeadRegen = 197,
    SummonVaultkeeper = 198,
    OffhandDamage = 199,
    OffhandChance = 200,
    SummonSiegeRam = 201,
    SummonSiegeBallista = 202,
    SummonSiegeCatapult = 203,
    SummonSiegeTrebuchet = 204,
    SummonJuggernaut = 205,
    SummonAnimistAmbusher = 206,
    StrikingTheSoul = 207,
    Null = 208
}