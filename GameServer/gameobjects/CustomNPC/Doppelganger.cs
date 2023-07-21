
using System.Reflection;
using DOL.AI;
using DOL.Database;
using DOL.GS.Keeps;
using DOL.GS.PlayerClass;
using DOL.GS.ServerProperties;
using DOL.Language;
using log4net;

namespace DOL.GS
{
    public class Doppelganger : GameSummoner
    {
        /// <summary>
        /// Defines a logger for this class.
        /// </summary>
        private static new readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        override public int PetSummonThreshold { get { return 50; } }

        override public NpcTemplate PetTemplate { get { return m_petTemplate; } }
        static private NpcTemplate m_petTemplate = null;

        override public byte PetLevel { get { return 50; } }
        override public byte PetSize { get { return 50; } }

        public Doppelganger() : base() { }
        public Doppelganger(ABrain defaultBrain) : base(defaultBrain) { }
        public Doppelganger(INpcTemplate template) : base(template) { }

        static Doppelganger()
        {
           DBNpcTemplate chthonian = CoreDb<DBNpcTemplate>.SelectObject(DB.Column("Name").IsEqualTo("chthonian crawler"));
            if (chthonian != null)
                m_petTemplate = new NpcTemplate(chthonian);
        }

        /// <summary>
        /// Realm point value of this living
        /// </summary>
        public override int RealmPointsValue
        {
            get { return ServerProperties.ServerProperties.DOPPELGANGER_REALM_POINTS; }
        }

        /// <summary>
        /// Bounty point value of this living
        /// </summary>
        public override int BountyPointsValue
        {
            get { return ServerProperties.ServerProperties.DOPPELGANGER_BOUNTY_POINTS; }
        }

        protected const ushort doppelModel = 2248;

        /// <summary>
        /// Gets/sets the object health
        /// </summary>
        public override int Health
        {
            get { return base.Health; }
            set
            {
                base.Health = value;

                if (value >= MaxHealth)
                {
                    if (Model == doppelModel)
                        Disguise();
                }
                else if (value <= MaxHealth >> 1 && Model != doppelModel)
                {
                    Model = doppelModel;
                    Name = "doppelganger";
                    Inventory = new GameNpcInventory(GameNpcInventoryTemplate.EmptyTemplate);
                    BroadcastLivingEquipmentUpdate();
                }
            }
        }

        /// <summary>
        /// Load a npc from the npc template
        /// </summary>
        /// <param name="obj">template to load from</param>
        public override void LoadFromDatabase(DataObject obj)
        {
            base.LoadFromDatabase(obj);

            Disguise();
        }

        /// <summary>
        /// Starts a melee or ranged attack on a given target.
        /// </summary>
        /// <param name="target">The object to attack.</param>
        public override void StartAttack(GameObject target)
        {
            // Don't allow ranged attacks
            if (ActiveWeaponSlot == EActiveWeaponSlot.Distance)
            {
                bool standard = Inventory.GetItem(eInventorySlot.RightHandWeapon) != null;
                bool twoHanded = Inventory.GetItem(eInventorySlot.TwoHandWeapon) != null;

                if (standard && twoHanded)
                {
                    if (UtilCollection.Random(1) < 1)
                        SwitchWeapon(EActiveWeaponSlot.Standard);
                    else
                        SwitchWeapon(EActiveWeaponSlot.TwoHanded);
                }
                else if (twoHanded)
                    SwitchWeapon(EActiveWeaponSlot.TwoHanded);
                else
                    SwitchWeapon(EActiveWeaponSlot.Standard);
            }

            base.StartAttack(target);
        }

        /// <summary>
        /// Disguise the doppelganger as an invader
        /// </summary>
        protected void Disguise()
        {
            if (UtilCollection.Chance(50))
                Gender = EGender.Male;
            else
                Gender = EGender.Female;

            ICharacterClass characterClass = new DefaultCharacterClass();

            switch (UtilCollection.Random(2))
            {
                case 0: // Albion
                    Name = $"Albion {LanguageMgr.GetTranslation(LanguageMgr.DefaultLanguage, "GamePlayer.RealmTitle.Invader")}";

                    switch (UtilCollection.Random(4))
                    {
                        case 0: // Archer
                            Inventory = KeepGuardInventoryMgr.Albion_Archer.CloneTemplate();
                            SwitchWeapon(EActiveWeaponSlot.Distance);
                            characterClass = new ClassScout();
                            break;
                        case 1: // Caster
                            Inventory = KeepGuardInventoryMgr.Albion_Caster.CloneTemplate();
                            characterClass = new ClassTheurgist();
                            break;
                        case 2: // Fighter
                            Inventory = KeepGuardInventoryMgr.Albion_Fighter.CloneTemplate();
                            characterClass = new ClassArmsman();
                            break;
                        case 3: // GuardHealer
                            Inventory = KeepGuardInventoryMgr.Albion_Healer.CloneTemplate();
                            characterClass = new ClassCleric();
                            break;
                        case 4: // Stealther
                            Inventory = KeepGuardInventoryMgr.Albion_Stealther.CloneTemplate();
                            characterClass = new ClassInfiltrator();
                            break;
                    }
                    break;
                case 1: // Hibernia
                    Name = $"Hibernia {LanguageMgr.GetTranslation(LanguageMgr.DefaultLanguage, "GamePlayer.RealmTitle.Invader")}";

                    switch (UtilCollection.Random(4))
                    {
                        case 0: // Archer
                            Inventory = KeepGuardInventoryMgr.Hibernia_Archer.CloneTemplate();
                            SwitchWeapon(EActiveWeaponSlot.Distance);
                            characterClass = new ClassRanger();
                            break;
                        case 1: // Caster
                            Inventory = KeepGuardInventoryMgr.Hibernia_Caster.CloneTemplate();
                            characterClass = new ClassEldritch();
                            break;
                        case 2: // Fighter
                            Inventory = KeepGuardInventoryMgr.Hibernia_Fighter.CloneTemplate();
                            characterClass = new ClassArmsman();
                            break;
                        case 3: // GuardHealer
                            Inventory = KeepGuardInventoryMgr.Hibernia_Healer.CloneTemplate();
                            characterClass = new ClassDruid();
                            break;
                        case 4: // Stealther
                            Inventory = KeepGuardInventoryMgr.Hibernia_Stealther.CloneTemplate();
                            characterClass = new ClassNightshade();
                            break;
                    }
                    break;
                case 2: // Midgard
                    Name = $"Midgard {LanguageMgr.GetTranslation(LanguageMgr.DefaultLanguage, "GamePlayer.RealmTitle.Invader")}";

                    switch (UtilCollection.Random(4))
                    {
                        case 0: // Archer
                            Inventory = KeepGuardInventoryMgr.Midgard_Archer.CloneTemplate();
                            SwitchWeapon(EActiveWeaponSlot.Distance);
                            characterClass = new ClassHunter();
                            break;
                        case 1: // Caster
                            Inventory = KeepGuardInventoryMgr.Midgard_Caster.CloneTemplate();
                            characterClass = new ClassRunemaster();
                            break;
                        case 2: // Fighter
                            Inventory = KeepGuardInventoryMgr.Midgard_Fighter.CloneTemplate();
                            characterClass = new ClassWarrior();
                            break;
                        case 3: // GuardHealer
                            Inventory = KeepGuardInventoryMgr.Midgard_Healer.CloneTemplate();
                            characterClass = new ClassHealer();
                            break;
                        case 4: // Stealther
                            Inventory = KeepGuardInventoryMgr.Midgard_Stealther.CloneTemplate();
                            characterClass = new ClassShadowblade();
                            break;
                    }
                    break;
            }

            var possibleRaces = characterClass.EligibleRaces;
            var indexPick = UtilCollection.Random(0, possibleRaces.Count - 1);
            Model = (ushort)possibleRaces[indexPick].GetModel(Gender);

            bool distance = Inventory.GetItem(eInventorySlot.DistanceWeapon) != null;
            bool standard = Inventory.GetItem(eInventorySlot.RightHandWeapon) != null;
            bool twoHanded = Inventory.GetItem(eInventorySlot.TwoHandWeapon) != null;

            if (distance)
                SwitchWeapon(EActiveWeaponSlot.Distance);
            else if (standard && twoHanded)
            {
                if (UtilCollection.Random(1) < 1)
                    SwitchWeapon(EActiveWeaponSlot.Standard);
                else
                    SwitchWeapon(EActiveWeaponSlot.TwoHanded);
            }
            else if (twoHanded)
                SwitchWeapon(EActiveWeaponSlot.TwoHanded);
            else
                SwitchWeapon(EActiveWeaponSlot.Standard);
            
        }
    }
}
