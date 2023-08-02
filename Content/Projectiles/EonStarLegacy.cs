using System;
using AotC.Content.Buffs;
using AotC.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using AotC.Common.Projectiles;
using Microsoft.Xna.Framework.Graphics;

namespace AotC.Content.Projectiles;

public class EonStarLegacy : ModProjectile
{
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
    }

    public override void SetDefaults()
    {
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.friendly = true;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.alpha = 50;
        Projectile.penetrate = 2;
        Projectile.DamageType = DamageClass.Melee;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 10;
    }

    public override void AI()
    {
        if (Projectile.soundDelay == 0)
        {
            Projectile.soundDelay = 20 + Main.rand.Next(40);
            if (Utils.NextBool(Main.rand, 5))
            {
                SoundEngine.PlaySound(SoundID.Item9, Projectile.position);
            }
        }
        Projectile.alpha -= 15;
        int num58 = 150;
        if (Projectile.Center.Y >= Projectile.ai[1])
        {
            num58 = 0;
        }
        if (Projectile.alpha < num58)
        {
            Projectile.alpha = num58;
        }
        Projectile.localAI[0] += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * Projectile.direction;
        Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * Projectile.direction;
        if (Utils.NextBool(Main.rand, 8))
        {
            Vector2 value3 = Vector2.UnitX.RotatedByRandom(1.5707963705062866).RotatedBy(Projectile.velocity.ToRotation());
            int num60 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.RainbowTorch, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 150, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1.2f);
            Main.dust[num60].velocity = value3 * 0.66f;
            Main.dust[num60].noGravity = true;
            Main.dust[num60].position = Projectile.Center + value3 * 12f;
        }
        if (Utils.NextBool(Main.rand, 24))
        {
            int num61 = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f), 16);
            Main.gore[num61].velocity *= 0.66f;
            Main.gore[num61].velocity += Projectile.velocity * 0.3f;
        }
        if (Projectile.ai[1] == 1f)
        {
            Projectile.light = 0.9f;
            if (Utils.NextBool(Main.rand, 5))
            {
                int num59 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.RainbowTorch, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 150, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1.2f);
                Main.dust[num59].noGravity = true;
            }
            if (Utils.NextBool(Main.rand, 10))
            {
                Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.position, new Vector2(Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f), Main.rand.Next(16, 18));
            }
        }
        CalamityGlobalProjectile.HomeInOnNPC(Projectile, ignoreTiles: true, 1600f, 35f, 20f);
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, Projectile.alpha);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        CalamityGlobalProjectile.DrawCenteredAndAfterimage(Projectile, lightColor, ProjectileID.Sets.TrailingMode[Projectile.type], 1, ModContent.Request<Texture2D>(Texture).Value);
        return false;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        Player player = Main.player[Projectile.owner];
        //bool astral = player.Calamity().ZoneAstral;
        bool jungle = player.ZoneJungle;
        bool snow = player.ZoneSnow;
        bool beach = player.ZoneBeach;
        bool corrupt = player.ZoneCorrupt;
        bool crimson = player.ZoneCrimson;
        bool dungeon = player.ZoneDungeon;
        bool desert = player.ZoneDesert;
        bool glow = player.ZoneGlowshroom;
        bool hell = player.ZoneUnderworldHeight;
        //bool holy = player.ZoneHoly;
        bool nebula = player.ZoneTowerNebula;
        bool stardust = player.ZoneTowerStardust;
        bool solar = player.ZoneTowerSolar;
        bool vortex = player.ZoneTowerVortex;
        bool bloodMoon = Main.bloodMoon;
        bool snowMoon = Main.snowMoon;
        bool pumpkinMoon = Main.pumpkinMoon;
        if (bloodMoon)
        {
            player.AddBuff(13, 600, true);
        }
        if (snowMoon)
        {
            player.AddBuff(58, 600, true);
        }
        if (pumpkinMoon)
        {
            player.AddBuff(26, 600, true);
        }
        //if (astral)
        //{
        //    target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 1200);
        //    player.AddBuff(ModContent.BuffType<GravityNormalizerBuff>(), 600, true);
        //    int proj9 = Projectile.NewProjectile(Projectile.Center.X, Projectile.Center.Y, Projectile.velocity.X, Projectile.velocity.Y, ModContent.ProjectileType<AstralStar>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
        //    Main.projectile[proj9].Calamity().forceMelee = true;
        //}
        else if (jungle)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 1200);
            player.AddBuff(14, 600, true);
            int proj8 = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, Projectile.velocity.X, Projectile.velocity.Y, 206, Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
            Main.projectile[proj8].Calamity().forceMelee = true;
        }
        else if (snow)
        {
            target.AddBuff(ModContent.BuffType<GlacialState>(), 1200);
            player.AddBuff(124, 600, true);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, Projectile.velocity.X, Projectile.velocity.Y, 118, Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
        }
        else if (beach)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 1200);
            player.AddBuff(103, 600, true);
            int proj7 = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 405, Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
            Main.projectile[proj7].Calamity().forceMelee = true;
        }
        else if (corrupt)
        {
            player.AddBuff(117, 600, true);
            int ball5 = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, Projectile.velocity.X, Projectile.velocity.Y, 95, Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
            Main.projectile[ball5].Calamity().forceMelee = true;
            Main.projectile[ball5].penetrate = 1;
        }
        else if (crimson)
        {
            player.AddBuff(115, 600, true);
            int ball4 = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, Projectile.velocity.X, Projectile.velocity.Y, 280, Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
            Main.projectile[ball4].Calamity().forceMelee = true;
            Main.projectile[ball4].penetrate = 1;
        }
        else if (dungeon)
        {
            target.AddBuff(44, 1200);
            player.AddBuff(111, 600, true);
            int ball3 = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, Projectile.velocity.X, Projectile.velocity.Y, 27, Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
            Main.projectile[ball3].Calamity().forceMelee = true;
            Main.projectile[ball3].penetrate = 1;
        }
        else if (desert)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 1200);
            player.AddBuff(114, 600, true);
            int proj6 = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, Projectile.velocity.X, Projectile.velocity.Y, 661, Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
            Main.projectile[proj6].Calamity().forceMelee = true;
        }
        else if (glow)
        {
            target.AddBuff(ModContent.BuffType<TemporalSadness>(), 1200);
            player.AddBuff(9, 600, true);
            int proj5 = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, Projectile.velocity.X, Projectile.velocity.Y, 131, Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
            Main.projectile[proj5].Calamity().forceMelee = true;
        }
        else if (hell)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 1200);
            player.AddBuff(116, 600, true);
            int proj4 = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, Projectile.velocity.X, Projectile.velocity.Y, 15, Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
            Main.projectile[proj4].Calamity().forceMelee = true;
        }
        //else if (holy)
        //{
        //    target.AddBuff(ModContent.BuffType<HolyFlames>(), 1200);
        //    player.AddBuff(105, 600, true);
        //    int proj3 = Projectile.NewProjectile(Projectile.Center.X, Projectile.Center.Y, Projectile.velocity.X, Projectile.velocity.Y, 644, Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
        //    Main.projectile[proj3].Calamity().forceMelee = true;
        //    Main.projectile[proj3].usesLocalNPCImmunity = true;
        //    Main.projectile[proj3].localNPCHitCooldown = -1;
        //}
        else if (nebula)
        {
            player.AddBuff(7, 600, true);
            int proj2 = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, Projectile.velocity.X, Projectile.velocity.Y, 634, Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
            Main.projectile[proj2].Calamity().forceMelee = true;
        }
        else if (stardust)
        {
            player.AddBuff(110, 600, true);
            int ball2 = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, Projectile.velocity.X, Projectile.velocity.Y, 614, Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
            Main.projectile[ball2].Calamity().forceMelee = true;
            Main.projectile[ball2].penetrate = 1;
        }
        else if (solar)
        {
            player.AddBuff(108, 600, true);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, Projectile.velocity.X, Projectile.velocity.Y, 612, Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
        }
        else if (vortex)
        {
            player.AddBuff(112, 600, true);
            int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, Projectile.velocity.X, Projectile.velocity.Y, 616, Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
            Main.projectile[proj].Calamity().forceMelee = true;
            Main.projectile[proj].usesLocalNPCImmunity = true;
            Main.projectile[proj].localNPCHitCooldown = -1;
        }
        else
        {
            target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 1200);
            player.AddBuff(165, 600, true);
            int ball = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, Projectile.velocity.X, Projectile.velocity.Y, 604, Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
            Main.projectile[ball].penetrate = 1;
        }
    }
}
