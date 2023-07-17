using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using DOL.Database;
using DOL.Events;
using DOL.GS;
using ECS.Debug;

namespace DOL.GS.Commands
{
    [Command(
        "&charstats",
        EPrivLevel.GM,
        "Shows normally hidden character stats.")]
    public class CharStatsCommand : AbstractCommandHandler, ICommandHandler
    {
        public void OnCommand(GameClient client, string[] args)
        {
            List<string> messages = new();
            string header = "Hidden Character Stats";
            GamePlayer player = client.Player;
            InventoryItem lefthand = player.Inventory.GetItem(eInventorySlot.LeftHandWeapon);

            // Block chance.
            if (player.HasAbility(Abilities.Shield))
            {
                if (lefthand == null)
                    messages.Add($"Block Chance: No Shield Equipped!");
                else
                {
                    double blockChance = player.GetBlockChance();
                    messages.Add($"Block Chance: {blockChance}%");
                }
            }

            // Parry chance.
            if (player.HasSpecialization(Specs.Parry))
            {
                double parryChance = player.GetParryChance();
                messages.Add($"Parry Chance: {parryChance}%");
            }

            // Evade chance.
            if (player.HasAbility(Abilities.Evade))
            {
                double evadeChance = player.GetEvadeChance();
                messages.Add($"Evade Chance: {evadeChance}%");
            }

            // Melee crit chance.
            int meleeCritChance = player.GetModified(EProperty.CriticalMeleeHitChance);
            messages.Add($"Melee Crit Chance: {meleeCritChance}%");

            // Spell crit chance
            int spellCritChance = player.GetModified(EProperty.CriticalSpellHitChance);
            messages.Add($"Spell Crit Chance: {spellCritChance}");

            // Spell casting speed bonus.
            int spellCastSpeed = player.GetModified(EProperty.CastingSpeed);
            messages.Add($"Spell Casting Speed Bonus: {spellCastSpeed}%");

            // Heal crit chance.
            int healCritChance = player.GetModified(EProperty.CriticalHealHitChance);
            messages.Add($"Heal Crit Chance: {healCritChance}%");

            // Archery crit chance.
            if (player.HasSpecialization(Specs.Archery)
                || player.HasSpecialization(Specs.CompositeBow)
                || player.HasSpecialization(Specs.RecurveBow)
                || player.HasSpecialization(Specs.ShortBow)
                || player.HasSpecialization(Specs.Crossbow)
                || player.HasSpecialization(Specs.Longbow))
            {
                int archeryCritChance = player.GetModified(EProperty.CriticalArcheryHitChance);
                messages.Add($"Archery Crit Chance: {archeryCritChance}%");
            }

            // Finalize.
            player.Out.SendCustomTextWindow(header, messages);
        }
    }
}
