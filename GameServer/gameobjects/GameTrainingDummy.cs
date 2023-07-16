
using DOL.AI.Brain;
using DOL.Events;
using DOL.GS.PropertyCalc;

namespace DOL.GS
{
    /// <summary>
    /// Training Dummy: Can't move, fight back, or die
    /// </summary>
    public class GameTrainingDummy : GameNPC
    {
        public GameTrainingDummy() : base()
        {
            MaxSpeedBase = 0;
            SetOwnBrain(new BlankBrain());
        }

        
        public override void StartAttack(GameObject target) { }

        /// <summary>
        /// Training Dummies never loose health
        /// </summary>
        public override int Health
        {
            get => base.MaxHealth;
            set { }
        }

        /// <summary>
        /// Training Dummies are always alive
        /// </summary>
        public override bool IsAlive => true;

        /// <summary>
        /// Training Dummies never attack
        /// </summary>
        /// <param name="ad"></param>
        public override void OnAttackedByEnemy(AttackData ad)
        {
            if (ad.IsHit && ad.CausesCombat)
            {
                if (ad.Attacker.Realm == 0 || Realm == 0)
                {
                    LastAttackedByEnemyTickPvE = GameLoop.GameLoopTime;
                    ad.Attacker.LastAttackTickPvE = GameLoop.GameLoopTime;
                }
                else
                {
                    LastAttackedByEnemyTickPvP = GameLoop.GameLoopTime;
                    ad.Attacker.LastAttackTickPvP = GameLoop.GameLoopTime;
                }
            }
        }

        /// <summary>
        /// Interacting with a training dummy does nothing
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public override bool Interact(GamePlayer player)
        {
            Notify(GameObjectEvent.Interact, this, new InteractEventArgs(player));
            player.Notify(GameObjectEvent.InteractWith, player, new InteractWithEventArgs(this));
            return true;
        }

        protected static void ApplyBonus(GameLiving owner, eBuffBonusCategory BonusCat, eProperty Property, double Value, double Effectiveness, bool IsSubstracted)
        {
            int effectiveValue = (int)(Value * Effectiveness);

            IPropertyIndexer tblBonusCat;
            if (Property != eProperty.Undefined)
            {
                tblBonusCat = GetBonusCategory(owner, BonusCat);
                //Console.WriteLine($"Value before: {tblBonusCat[(int)Property]}");
                if (IsSubstracted)
                    tblBonusCat[(int)Property] -= effectiveValue;
                else
                    tblBonusCat[(int)Property] += effectiveValue;
                //Console.WriteLine($"Value after: {tblBonusCat[(int)Property]}");
            }
        }

        private static IPropertyIndexer GetBonusCategory(GameLiving target, eBuffBonusCategory categoryid)
        {
            IPropertyIndexer bonuscat = null;
            switch (categoryid)
            {
                case eBuffBonusCategory.BaseBuff:
                    bonuscat = target.BaseBuffBonusCategory;
                    break;
                case eBuffBonusCategory.SpecBuff:
                    bonuscat = target.SpecBuffBonusCategory;
                    break;
                case eBuffBonusCategory.Debuff:
                    bonuscat = target.DebuffCategory;
                    break;
                case eBuffBonusCategory.Other:
                    bonuscat = target.BuffBonusCategory4;
                    break;
                case eBuffBonusCategory.SpecDebuff:
                    bonuscat = target.SpecDebuffCategory;
                    break;
                case eBuffBonusCategory.AbilityBuff:
                    bonuscat = target.AbilityBonus;
                    break;
                default:
                    //if (log.IsErrorEnabled)
                    //    Console.WriteLine("BonusCategory not found " + categoryid + "!");
                    break;
            }
            return bonuscat;
        }
    }
}