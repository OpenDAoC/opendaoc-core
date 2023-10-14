using DOL.Database;

namespace DOL.GS.Spells
{
    //http://www.camelotherald.com/masterlevels/ma.php?ml=Convoker
    //no shared timer

    [SpellHandler("SummonWood")]
    public class SummonWoodSpell : SummonItemSpellHandler
    {
        public SummonWoodSpell(GameLiving caster, Spell spell, SpellLine line)
            : base(caster, spell, line)
        {
            DbItemTemplate template = GameServer.Database.FindObjectByKey<DbItemTemplate>("mysticwood_wooden_boards");
            if (template != null)
            {
                items.Add(GameInventoryItem.Create(template));
                foreach (DbInventoryItem item in items)
                {
                    if (item.IsStackable)
                    {
                        item.Count = 1;
                        item.Weight = item.Count * item.Weight;
                    }
                }
            }
        }
    }
}