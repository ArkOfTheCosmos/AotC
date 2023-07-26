using AotC.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ID;
using AotC.Content.Buffs;
using AotC.Content;

namespace AotC.Common.Players
{
    internal class CalamityPlayer : ModPlayer
    {
        public bool tSad;
        public bool gState;
        public bool cDepth;
        public bool hFlames;
        public bool bFlames;
        public bool pFlames;
        public bool aCrunch;
        public double contactDamageReduction;

        public override void ResetEffects()
        {
            aCrunch = tSad = cDepth = gState = hFlames = bFlames = pFlames = false;
            contactDamageReduction = 0;
        }
        public override void UpdateDead()
        {
            tSad = cDepth = gState = hFlames = bFlames = pFlames = false;
        }
        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            if (npc.Calamity().tSad > 0)
            {
                contactDamageReduction += 0.5;
            }
            if (aCrunch && contactDamageReduction > 0)
            {
                contactDamageReduction *= 0.33;
            }
            contactDamageReduction = 1.0 / (1.0 + contactDamageReduction);
            modifiers.IncomingDamageMultiplier = new MultipliableFloat() * (float)contactDamageReduction;
        }
        public override void UpdateBadLifeRegen()
        {
            int lifeRegenLost = 0;
            if (bFlames)
            {
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;
                Player.lifeRegenTime = 0;
                lifeRegenLost += 16;
            }
            if (pFlames)
            {
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;
                Player.lifeRegenTime = 0;
                lifeRegenLost += 20;
            }
            if (hFlames)
            {
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;
                Player.lifeRegenTime = 0;
                lifeRegenLost += 16;
            }
            if (cDepth && Player.statDefense > 0)
            {
                int num3 = 18;
                int subtractDefense = (int)((double)Player.statDefense * 0.05);
                int calcDepthDamage = num3 - subtractDefense;
                if (calcDepthDamage < 0)
                {
                    calcDepthDamage = 0;
                }
                if (Player.lifeRegen > 0)
                {
                    Player.lifeRegen = 0;
                }
                Player.lifeRegenTime = 0;
                lifeRegenLost += calcDepthDamage;
            }
            Player.lifeRegen -= lifeRegenLost;
        }
        public override void PostUpdateMiscEffects()
        {
            if (pFlames)
            {
                Player.blind = true;
                Player.statDefense -= Plague.DefenseReduction;
                Player.moveSpeed -= 0.15f;
            }
            if (gState)
            {
                Player.statDefense -= GlacialState.DefenseReduction;
                Player.velocity.Y = 0f;
                Player.velocity.X = 0f;
            }
            if (aCrunch)
            {
                Player.statDefense -= ArmorCrunch.DefenseReduction;
                Player.endurance *= 0.33f;
            }
        }
        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if (bFlames && damage == 10.0 && hitDirection == 0 && damageSource.SourceOtherIndex == 8)
                damageSource = PlayerDeathReason.ByCustomReason(Player.name + " was consumed by the black flames.");
            if (hFlames && damage == 10.0 && hitDirection == 0 && damageSource.SourceOtherIndex == 8)
                damageSource = PlayerDeathReason.ByCustomReason(Player.name + " fell prey to their sins.");
            if (pFlames && damage == 10.0 && hitDirection == 0 && damageSource.SourceOtherIndex == 8)
            {
                if (Utils.NextBool(Main.rand, 2))
                    damageSource = PlayerDeathReason.ByCustomReason(Player.name + "'s flesh was melted by the plague.");
                else
                    damageSource = PlayerDeathReason.ByCustomReason(Player.name + " didn't vaccinate.");
            }
            if (this.cDepth && damage == 10.0 && hitDirection == 0 && damageSource.SourceOtherIndex == 8)
            {
                if (Utils.NextBool(Main.rand, 2))
                {
                    damageSource = PlayerDeathReason.ByCustomReason(Player.name + " was crushed by the pressure.");
                }
                else
                {
                    damageSource = PlayerDeathReason.ByCustomReason(Player.name + "'s lungs collapsed.");
                }
            }
            return true;
        }
        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (bFlames)
            {
                if (Utils.NextBool(Main.rand, 4) && drawInfo.shadow == 0f)
                {
                    int dust15 = Dust.NewDust(drawInfo.Position - new Vector2(2f, 2f), Player.width + 4, Player.height + 4, ModContent.DustType<BrimstoneFlame>(), Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 3f);
                    Main.dust[dust15].noGravity = true;
                    Main.dust[dust15].velocity *= 1.8f;
                    Main.dust[dust15].velocity.Y -= 0.5f;
                    drawInfo.DustCache.Add(dust15); 
                }
                r *= 0.25f;
                g *= 0.01f;
                b *= 0.01f;
                fullBright = true;
            }
            if (hFlames)
            {
                if (Utils.NextBool(Main.rand, 4) && drawInfo.shadow == 0f)
                {
                    int dust8 = Dust.NewDust(drawInfo.Position - new Vector2(2f, 2f), Player.width + 4, Player.height + 4, ModContent.DustType<HolyFireDust>(), Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default(Color), 3f);
                    Main.dust[dust8].noGravity = true;
                    Main.dust[dust8].velocity *= 1.8f;
                    Main.dust[dust8].velocity.Y -= 0.5f;
                    drawInfo.DustCache.Add(dust8);
                }
                r *= 0.25f;
                g *= 0.25f;
                b *= 0.1f;
                fullBright = true;
            }
            if (pFlames)
            {
                if (Utils.NextBool(Main.rand, 4) && drawInfo.shadow == 0f)
                {
                    int dust6 = Dust.NewDust(drawInfo.Position - new Vector2(2f, 2f), Player.width + 4, Player.height + 4, DustID.GemEmerald, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default(Color), 3f);
                    Main.dust[dust6].noGravity = true;
                    Main.dust[dust6].velocity *= 1.2f;
                    Main.dust[dust6].velocity.Y -= 0.15f;
                    drawInfo.DustCache.Add(dust6);
                }
                r *= 0.07f;
                g *= 0.15f;
                b *= 0.01f;
                fullBright = true;
            }
            if (gState || cDepth)
            {
                r *= 0f;
                g *= 0.05f;
                b *= 0.3f;
                fullBright = true;
            }
        }
    }
}
