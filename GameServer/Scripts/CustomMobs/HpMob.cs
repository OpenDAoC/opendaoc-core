namespace DOL.GS.Scripts
{
    /// <summary>
    /// Npc with a more easy to control HP, set its Max hp with /mob cha (HP Amount), HP is not affected by
    /// CON Please note CON is still needed for a Mobs Defense.
    /// </summary>
    public class HpMob : GameNPC
    {
        
        public override bool AddToWorld()
        {
           
            Flags = 0;
            return base.AddToWorld();
        }

        public override int MaxHealth
        {
            get
            {
                return base.Charisma;
            }
        }
    }
}