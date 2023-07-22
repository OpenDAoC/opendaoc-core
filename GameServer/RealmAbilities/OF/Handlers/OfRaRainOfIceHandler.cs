using DOL.Database;

namespace DOL.GS.RealmAbilities
{
    public class OfRaRainOfIceHandler : OfRaRainOfBaseHandler
    {
        public OfRaRainOfIceHandler(DbAbilities ability, int level) : base(ability, level) { }

        public override void Execute(GameLiving living)
        {
            Execute("Rain Of Ice", 7126, 7126, 13, living);
        }
    }
}
