using AotC.Content.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using AotC.Content.Particles;

namespace AotC.Content.Projectiles;

public class EonStar : ModProjectile
{

    public List<Particle> Particles;

    internal PrimitiveTrail TrailDrawer;

    public NPC target;

    private Particle Head;

    private bool initialized;

    private float colorRand;

    public override string Texture => "AotC/Content/Projectiles/EonStar";

    public Player Owner => Main.player[Projectile.owner];

    public ref float Hue => ref Projectile.ai[0];

    public ref float HomingStrenght => ref Projectile.ai[1];

    public ref float SourceType => ref Projectile.ai[2];

    public float rotation = 0;

    public override void SetStaticDefaults()

    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.width = (Projectile.height = 30);
        Projectile.friendly = true;
        Projectile.penetrate = 1;
        Projectile.timeLeft = 160;
        Projectile.DamageType = DamageClass.Melee;
        Projectile.tileCollide = false;
        Projectile.scale = 1.1f;
    }

    public void BootlegSpawnParticle(Particle particle)
    {
        if (!Main.dedServ)
        {
            Particles.Add(particle);
            particle.Type = GeneralParticleHandler.particleTypes[particle.GetType()];
        }
    }

    public override void AI()
    {
        if (!initialized)
        {
            colorRand = Main.rand.NextFloat() / 5f;
            initialized = true;
        }
        Particles ??= new List<Particle>();
        Projectile.rotation = Projectile.velocity.ToRotation() + (float)Math.PI / 2f;
        if (Head == null)
        {
            Particle particle2 = new GenericSparkle(Projectile.Center, Vector2.Zero, Color.White, Color.White, 2f, 2, 0.1f, 3f, needed: true);
            BootlegSpawnParticle(particle2);
            Head = new GenericSparkle(Projectile.Center, Vector2.Zero, Color.White, Main.hslToRgb(Hue, 100f, 50f), 1.2f, 2, 0.06f, 3f, needed: true);
        }
        else
        {
            Head.Update();
        }
        if (target == null)
        {
            target = Projectile.Center.ClosestNPCAt(1000f);
        }
        else
        {
            if (Main.rand.NextBool(3))
            {
                Projectile.timeLeft++;
            }
            if (Projectile.velocity.AngleBetween(target.Center - Projectile.Center) < (float)Math.PI)
            {
                float targetAngle = Projectile.AngleTo(target.Center);
                float f = Projectile.velocity.ToRotation().AngleTowards(targetAngle, HomingStrenght);
                Projectile.velocity = f.ToRotationVector2() * Projectile.velocity.Length() * 0.995f;
            }
        }
        Color val = new(0.75f, 1f, 0.24f);
        ModdedUtils.ColorToHSV(val, out float h, out float s, out float v);
        val = ModdedUtils.HsvToRgb(h + Main.GlobalTimeWrappedHourly % 1 + colorRand, s, v);
        Lighting.AddLight(Projectile.Center, val.R / 255f, val.G / 255f, val.B / 255f);
        foreach (Particle particle4 in Particles)
        {
            if (particle4 != null)
            {
                particle4.Position = Projectile.Center + Projectile.velocity;
                particle4.Lifetime = Projectile.timeLeft;
                particle4.Time = particle4.Lifetime / 2;
                particle4.Update();
            }
        }
        Particles.RemoveAll((Particle particle) => particle.Time >= particle.Lifetime && particle.SetLifetime);

            Vector2 random = Vector2.UnitX.RotatedByRandom(Math.PI);
            int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.FireworksRGB, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 150, ModdedUtils.HsvToRgb(Main.GlobalTimeWrappedHourly % 1, 0.7f, 1), 0.6f);
            Main.dust[dustIndex].velocity = random;
            Main.dust[dustIndex].noGravity = true;
            Main.dust[dustIndex].position = Projectile.Center + random * 12f;
    }

    internal Color ColorFunction(float completionRatio)
    {
        return ModdedUtils.HsvToRgb(Main.GlobalTimeWrappedHourly + colorRand, 1f, 1f);
    }

    internal float WidthFunction(float completionRatio)
    {
        float num = (float)Math.Pow((double)(1f - completionRatio), 3.0);
        return MathHelper.Lerp(0f, 22f * Projectile.scale * Projectile.Opacity, num);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Main.spriteBatch.EnterShaderRegion(BlendState.Additive);
        TrailDrawer ??= new PrimitiveTrail(WidthFunction, ColorFunction, null, GameShaders.Misc["CalamityMod:TrailStreak"]);
        GameShaders.Misc["CalamityMod:TrailStreak"].SetShaderTexture(ModContent.Request<Texture2D>("AotC/Assets/Textures/ScarletDevilStreak", (AssetRequestMode)2));
        TrailDrawer.Draw(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 30);
        if (Particles != null)
        {
            foreach (GenericSparkle particle in Particles.Cast<GenericSparkle>())
            {
                particle.Bloom = ModdedUtils.HsvToRgb(Main.GlobalTimeWrappedHourly + colorRand, 1f, 1f);
                particle.Color = ModdedUtils.HsvToRgb(Main.GlobalTimeWrappedHourly + colorRand, 1f, 1f);
            }
            foreach (Particle particle in Particles)
            {
                particle.CustomDraw(Main.spriteBatch);
            }
        }
        Main.spriteBatch.ExitShaderRegion();
        rotation += 0.2f;
        return false;
    }

    public override void PostDraw(Color lightColor)
    {
        Texture2D value = ModContent.Request<Texture2D>("AotC/Content/Projectiles/EonStar", (AssetRequestMode)2).Value;
        Main.EntitySpriteDraw(value, Projectile.Center - Main.screenPosition, null, Color.Lerp(lightColor, Color.White, 0.5f), Projectile.rotation + rotation, value.Size() / 2f, Projectile.scale, 0, 0);

    }



    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (Owner.HeldItem.ModItem is ArkoftheCosmos arkoftheCosmos && SourceType == 1f)
        {
            arkoftheCosmos.charge += 1f;
            if (arkoftheCosmos.charge > 100f)
            {
                arkoftheCosmos.charge = 100f;
            }
        }
    }
}
