using DOL.AI.Brain;
using DOL.GS.PlayerClass;
using DOL.GS.ServerProperties;
using DOL.Language;

namespace DOL.GS.Keeps
{
    public class GuardCaster : GameKeepGuard
    {
        public override double GetArmorAbsorb(eArmorSlot slot)
        {
            return base.GetArmorAbsorb(slot) - 0.05;
        }

        protected override ICharacterClass GetClass()
        {
            if (ModelRealm == eRealm.Albion)
                return new ClassWizard();
            else if (ModelRealm == eRealm.Midgard)
                return new ClassRunemaster();
            else if (ModelRealm == eRealm.Hibernia)
                return new ClassEldritch();

            return new DefaultCharacterClass();
        }

        protected override KeepGuardBrain GetBrain()
        {
            return new CasterBrain();
        }

        protected override void SetName()
        {
            switch (ModelRealm)
            {
                case eRealm.None:
                case eRealm.Albion:
                {
                    if (IsPortalKeepGuard)
                        Name = LanguageMgr.GetTranslation(Properties.SERV_LANGUAGE, "SetGuardName.MasterWizard");
                    else
                        Name = LanguageMgr.GetTranslation(Properties.SERV_LANGUAGE, "SetGuardName.Wizard");

                    break;
                }
                case eRealm.Midgard:
                {
                    if (IsPortalKeepGuard)
                        Name = LanguageMgr.GetTranslation(Properties.SERV_LANGUAGE, "SetGuardName.MasterRunes");
                    else
                        Name = LanguageMgr.GetTranslation(Properties.SERV_LANGUAGE, "SetGuardName.Runemaster");

                    break;
                }
                case eRealm.Hibernia:
                {
                    if (IsPortalKeepGuard)
                        Name = LanguageMgr.GetTranslation(Properties.SERV_LANGUAGE, "SetGuardName.MasterEldritch");
                    else
                        Name = LanguageMgr.GetTranslation(Properties.SERV_LANGUAGE, "SetGuardName.Eldritch");

                    break;
                }
            }

            if (Realm == eRealm.None)
                Name = LanguageMgr.GetTranslation(Properties.SERV_LANGUAGE, "SetGuardName.Renegade", Name);
        }
    }
}
