using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using AotC.Content.Projectiles;
using System.Collections.Generic;

namespace AotC.Content.Items.Weapons.Melee;

public class ArkoftheCosmosLegacy : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 102;
        Item.damage = 140;
        Item.DamageType = DamageClass.Melee;
        Item.useAnimation = 15;
        Item.useTime = 15;
        Item.useTurn = true;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.crit += 15;
        Item.knockBack = 9.5f;
        Item.UseSound = SoundID.Item60;
        Item.autoReuse = true;
        Item.height = 102;
        Item.value = Item.buyPrice(2, 50);
        Item.rare = ItemRarityID.Red;
        Item.shoot = ModContent.ProjectileType<EonBeamLegacy>();
        Item.shootSpeed = 28f;
        Mod calamity = AotC.Instance.Calamity;
        if (calamity != null)
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = calamity.Find<ModItem>("ArkoftheCosmos").Type;
            ItemID.Sets.ShimmerTransformToItem[calamity.Find<ModItem>("ArkoftheCosmos").Type] = Type;
        }
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        if (tooltips != null && Main.player[Main.myPlayer] != null)
        {
            tooltips.Insert(1, new TooltipLine(AotC.Instance, "CopyrightInfringement", "You're as beautiful as the day I lost you"));
            tooltips[1].OverrideColor = Color.Yellow;
            tooltips[0].OverrideColor = new Color(108, 45, 199);
        }
    }


    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        switch (Main.rand.Next(4))
        {
            case 0:
                type = ModContent.ProjectileType<EonBeamLegacy>();
                break;
            case 1:
                type = ModContent.ProjectileType<EonBeamLegacyV2>();
                break;
            case 2:
                type = ModContent.ProjectileType<EonBeamLegacyV3>();
                break;
            case 3:
                type = ModContent.ProjectileType<EonBeamLegacyV4>();
                break;
        }
        int projectile = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, Main.myPlayer, 0f, 0f);
        Main.projectile[projectile].timeLeft = 160;
        Main.projectile[projectile].tileCollide = false;
        float num109 = Main.rand.Next(22, 30);
        Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
        float num110 = Main.mouseX + Main.screenPosition.X + vector2.X;
        float num111 = Main.mouseY + Main.screenPosition.Y + vector2.Y;
        if (player.gravDir == -1f)
        {
            num111 = Main.screenPosition.Y + Main.screenHeight + Main.mouseY + vector2.Y;
        }
        // DONT TRUST VS HERE OK
        float num112 = (float)Math.Sqrt(num110 * num110 + num111 * num111);
        if ((float.IsNaN(num110) && float.IsNaN(num111)) || (num110 == 0f && num111 == 0f))
        {
            num110 = player.direction;
            num111 = 0f;
            num112 = num109;
        }
        else
        {
            num112 = num109 / num112;
        }
        int num107 = 4;
        for (int num108 = 0; num108 < num107; num108++)
        {
            vector2 = new Vector2(player.position.X + player.width * 0.5f + (0f - player.direction) + (Main.mouseX + Main.screenPosition.X - player.position.X), player.MountedCenter.Y);
            vector2.X = (vector2.X + player.Center.X) / 2f;
            vector2.Y -= 100 * num108;
            num110 = Main.mouseX + Main.screenPosition.X - vector2.X;
            num111 = Main.mouseY + Main.screenPosition.Y - vector2.Y;
            num112 = (float)Math.Sqrt(num110 * num110 + num111 * num111);
            num112 = num109 / num112;
            num110 *= num112;
            num111 *= num112;
            float speedX2 = num110 + Main.rand.Next(-360, 361) * 0.02f;
            float speedY2 = num111 + Main.rand.Next(-360, 361) * 0.02f;
            int projectileFire = Projectile.NewProjectile(source, vector2, new(speedX2,speedY2), ModContent.ProjectileType<EonStarLegacy>(), damage, knockback, player.whoAmI, 0f, (float)Main.rand.Next(3));
            Main.projectile[projectileFire].timeLeft = 80;
        }
        return false;
    }

    public override void MeleeEffects(Player player, Rectangle hitbox)
    {
        if (Utils.NextBool(Main.rand, 5))
        {
            int num250 = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.RainbowTorch, player.direction * 2, 0f, 150, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1.3f);
            Main.dust[num250].velocity *= 0.2f;
            Main.dust[num250].noGravity = true;
        }
    }

    public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
    {
        //bool zoneAstral = player.Calamity().ZoneAstral;
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
        //if (zoneAstral)
        //{
        //    player.AddBuff(ModContent.BuffType<GravityNormalizerBuff>(), 600, true);
        //}
        else if (jungle)
        {
            player.AddBuff(14, 600, true);
        }
        else if (snow)
        {
            player.AddBuff(124, 600, true);
        }
        else if (beach)
        {
            player.AddBuff(103, 600, true);
        }
        else if (corrupt)
        {
            player.AddBuff(117, 600, true);
        }
        else if (crimson)
        {
            player.AddBuff(115, 600, true);
        }
        else if (dungeon)
        {
            player.AddBuff(111, 600, true);
        }
        else if (desert)
        {
            player.AddBuff(114, 600, true);
        }
        else if (glow)
        {
            player.AddBuff(9, 600, true);
        }
        else if (hell)
        {
            player.AddBuff(116, 600, true);
        }
        //else if (holy)
        //{
        //   player.AddBuff(105, 600, true);
        //}
        else if (nebula)
        {
            player.AddBuff(7, 600, true);
        }
        else if (stardust)
        {
            player.AddBuff(110, 600, true);
        }
        else if (solar)
        {
            player.AddBuff(108, 600, true);
        }
        else if (vortex)
        {
            player.AddBuff(112, 600, true);
        }
        else
        {
            player.AddBuff(165, 600, true);
        }
    }

    public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
    {
        bool zoneJungle = player.ZoneJungle;
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
        if (zoneJungle)
        {
            player.AddBuff(14, 600, true);
        }
        else if (snow)
        {
            player.AddBuff(124, 600, true);
        }
        else if (beach)
        {
            player.AddBuff(103, 600, true);
        }
        else if (corrupt)
        {
            player.AddBuff(117, 600, true);
        }
        else if (crimson)
        {
            player.AddBuff(115, 600, true);
        }
        else if (dungeon)
        {
            player.AddBuff(111, 600, true);
        }
        else if (desert)
        {
            player.AddBuff(114, 600, true);
        }
        else if (glow)
        {
            player.AddBuff(9, 600, true);
        }
        else if (hell)
        {
            player.AddBuff(116, 600, true);
        }
        //else if (holy)
        //{
        //    player.AddBuff(105, 600, true);
        //}
        else if (nebula)
        {
            player.AddBuff(7, 600, true);
        }
        else if (stardust)
        {
            player.AddBuff(110, 600, true);
        }
        else if (solar)
        {
            player.AddBuff(108, 600, true);
        }
        else if (vortex)
        {
            player.AddBuff(112, 600, true);
        }
        else
        {
            player.AddBuff(165, 600, true);
        }
    }
}
