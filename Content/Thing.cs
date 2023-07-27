

using System;
using System.Linq;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace AotC.Content
{
    public class ExampleNPCLoot : GlobalNPC
    {

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type != NPCID.Mothron)
            {
                return;
            }

            var rrBefore = npcLoot.Get();
            int before = rrBefore.Count;
            npcLoot.RemoveWhere(r => r is LeadingConditionRule l && l.ChainedRules.Any(c => c is Chains.TryIfSucceeded t && t.RuleToChain is LeadingConditionRule l2 && l2.ChainedRules.Any(c2 => c2 is Chains.TryIfSucceeded t2 && t2.RuleToChain is DropBasedOnExpertMode d && d.ruleForNormalMode is CommonDropWithRerolls cd && cd.itemId == ItemID.TheEyeOfCthulhu)));
            var rrAfter = npcLoot.Get();
            int after = rrAfter.Count;
            Mod.Logger.Debug($"Removing Cthulhu Yoyo from Mothron: Before: {before}, After: {after}");

        
        }

    }
}

