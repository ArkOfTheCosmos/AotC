using AotC.Content.Buffs;
using AotC.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.WorldBuilding;

namespace AotC.Common.NPCs
{
    internal class CalamityGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public int bFlames;
        public int gState;
        public int hFlames;
        public int pFlames;
        public int cDepth;
        public int tSad;
        public int aCrunch;

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (this.tSad > 0)
            {
                npc.velocity /= 2f;
            }
            if (npc.oiled)
            {
                int oiledDoT = ((bFlames > 0) ? 10 : 0) + ((hFlames > 0) ? 13 : 0);
                if (oiledDoT > 0)
                {
                    int lifeRegenValue = oiledDoT * 4 + 12;
                    npc.lifeRegen -= lifeRegenValue;
                    int damageValue = lifeRegenValue / 6;
                    if (damage < damageValue)
                    {
                        damage = damageValue;
                    }
                }
            }
            if (cDepth > 0)
            {
                if (npc.defense < 0)
                {
                    npc.defense = 0;
                }
                int depthDamage = Main.hardMode ? 80 : 12;
                int calcDepthDamage = depthDamage - npc.defense;
                if (calcDepthDamage < 0)
                {
                    calcDepthDamage = 0;
                }
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }
                npc.lifeRegen -= calcDepthDamage * 5;
                if (damage < calcDepthDamage)
                {
                    damage = calcDepthDamage;
                }
            }
            ApplyDPSDebuff(bFlames, 40, 8, ref npc.lifeRegen, ref damage);
            ApplyDPSDebuff(hFlames, 50, 10, ref npc.lifeRegen, ref damage);
            ApplyDPSDebuff(pFlames, 100, 20, ref npc.lifeRegen, ref damage);
        }
        public override void PostAI(NPC npc)
        {
            if (gState > 0)
                gState--;
            if (hFlames > 0)
                hFlames--;
            if (bFlames > 0)
                bFlames--;
            if (pFlames > 0)
                pFlames--;
            if (cDepth > 0)
                cDepth--;
            if (tSad > 0)
                tSad--;         
            if (aCrunch > 0)
                aCrunch--;
        }
        public static void ApplyDPSDebuff(int debuff, int lifeRegenValue, int damageValue, ref int lifeRegen, ref int damage)
        {
            if (debuff > 0)
            {
                if (lifeRegen > 0)
                {
                    lifeRegen = 0;
                }
                lifeRegen -= lifeRegenValue;
                if (damage < damageValue)
                {
                    damage = damageValue;
                }
            }
        }
        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (bFlames > 0)
            {
                if (Main.rand.Next(5) < 4)
                {
                    int dust9 = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, ModContent.DustType<BrimstoneFlame>(), npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default, 3.5f);
                    Main.dust[dust9].noGravity = true;
                    Main.dust[dust9].velocity *= 1.8f;
                    Main.dust[dust9].velocity.Y -= 0.5f;
                    if (Utils.NextBool(Main.rand, 4))
                    {
                        Main.dust[dust9].noGravity = false;
                        Main.dust[dust9].scale *= 0.5f;
                    }
                }
                Lighting.AddLight(npc.position, 0.05f, 0.01f, 0.01f);
            }
            if (hFlames > 0)
            {
                if (Main.rand.Next(5) < 4)
                {
                    int dust14 = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, ModContent.DustType<HolyFireDust>(), npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default, 3.5f);
                    Main.dust[dust14].noGravity = true;
                    Main.dust[dust14].velocity *= 1.8f;
                    Main.dust[dust14].velocity.Y -= 0.5f;
                    if (Utils.NextBool(Main.rand, 4))
                    {
                        Main.dust[dust14].noGravity = false;
                        Main.dust[dust14].scale *= 0.5f;
                    }
                }
                Lighting.AddLight(npc.position, 0.25f, 0.25f, 0.1f);
            }
            if (pFlames > 0)
            {
                if (Main.rand.Next(5) < 4)
                {
                    int dust15 = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, DustID.GemEmerald, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default, 3.5f);
                    Main.dust[dust15].noGravity = true;
                    Main.dust[dust15].velocity *= 1.2f;
                    Main.dust[dust15].velocity.Y -= 0.15f;
                    if (Utils.NextBool(Main.rand, 4))
                    {
                        Main.dust[dust15].noGravity = false;
                        Main.dust[dust15].scale *= 0.5f;
                    }
                }
                Lighting.AddLight(npc.position, 0.07f, 0.15f, 0.01f);
            }
            if ((cDepth > 0 || tSad > 0) && Main.rand.Next(6) < 3)
            {
                int dust8 = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, DustID.Water, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default, 3.5f);
                Main.dust[dust8].noGravity = false;
                Main.dust[dust8].velocity *= 1.2f;
                Main.dust[dust8].velocity.Y += 0.15f;
                if (Utils.NextBool(Main.rand, 4))
                {
                    Main.dust[dust8].noGravity = false;
                    Main.dust[dust8].scale *= 0.5f;
                }
            }
            if (gState > 0)
            {
                drawColor = Color.Cyan;
            }
        }
        public override void OnHitNPC(NPC npc, NPC target, NPC.HitInfo hit)
        {
            float defenseLost = ((pFlames > 0) ? Plague.DefenseReduction : 0) - ((gState > 0) ? GlacialState.DefenseReduction : 0 - ((aCrunch > 0) ? ArmorCrunch.DefenseReduction : 0));
            if (defenseLost > npc.defense)
                defenseLost = npc.defense;
            hit.Damage += (int)(defenseLost * (Main.GameModeInfo.IsMasterMode ? 1 : (Main.GameModeInfo.IsExpertMode ? 0.75f : 0.5f)));
        }
    }
}
