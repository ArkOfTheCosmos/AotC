using AotC.Content.StolenCalamityCode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Renderers;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace AotC.Content;

public class EonBolt : ModProjectile
{

    Random rand = new Random();

    public List<Particle> Particles;

    internal PrimitiveTrail TrailDrawer;

    public NPC target;

    private Particle Head;

    public override string Texture => "AotC/Content/TestStar";

    public Player Owner => Main.player[base.Projectile.owner];

    public ref float Hue => ref base.Projectile.ai[0];

    public ref float HomingStrenght => ref base.Projectile.ai[1];

    public float rotation = 0;

    public override void SetStaticDefaults()

    {
        // base.DisplayName.SetDefault("Eon Bolt");
        ProjectileID.Sets.TrailCacheLength[base.Projectile.type] = 20;
        ProjectileID.Sets.TrailingMode[base.Projectile.type] = 2;
    }

    public override void SetDefaults()
    {
        base.Projectile.width = (base.Projectile.height = 30);
        base.Projectile.friendly = true;
        base.Projectile.penetrate = 1;
        base.Projectile.timeLeft = 160;
        base.Projectile.DamageType = DamageClass.Melee;
        base.Projectile.tileCollide = false;
        Projectile.scale = 1.3f;
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
        if (Particles == null)
        {
            Particles = new List<Particle>();
        }
        base.Projectile.rotation = base.Projectile.velocity.ToRotation() + (float)Math.PI / 2f;
        if (Head == null)
        {
            Particle particle2 = new GenericSparkle(Projectile.Center, Vector2.Zero, Color.White, Color.Plum, 2f, 2, 0.06f, 3f, needed: true);
            BootlegSpawnParticle(particle2);
            Head = new GenericSparkle(base.Projectile.Center, Vector2.Zero, Color.White, Main.hslToRgb(Hue, 100f, 50f), 1.2f, 2, 0.06f, 3f, needed : true);     
        }
        else
        {
            //Head.Position = base.Projectile.Center + base.Projectile.velocity * 0.5f;
            //Head.Time = 0;
            //Head.Scale += (float)Math.Sin((double)(Main.GlobalTimeWrappedHourly * 6f)) * base.Projectile.scale;
            Head.Update();        
        }
        if (target == null)
        {
            target = base.Projectile.Center.ClosestNPCAt(1000f);
        }
        else
        {
            if (rand.NextInt64(2) == 0)
            {
                Projectile.timeLeft++;
            }
            if (base.Projectile.velocity.AngleBetween(target.Center - base.Projectile.Center) < (float)Math.PI)
            {
                float targetAngle = base.Projectile.AngleTo(target.Center);
                float f = base.Projectile.velocity.ToRotation().AngleTowards(targetAngle, HomingStrenght);
                base.Projectile.velocity = f.ToRotationVector2() * Projectile.velocity.Length() * 0.995f;
            }
        }
        Color val = new(0.75f, 1f, 0.24f);
        CalamityUtils.ColorToHSV(val, out float h, out float s, out float v);
        val = CalamityUtils.HsvToRgb(h + Main.GlobalTimeWrappedHourly % 1, s, v);
        Lighting.AddLight(base.Projectile.Center, val.R / 255f, val.G / 255f, val.B / 255f);
        /*if (Main.rand.Next(2) == 0)
		{
			GeneralParticleHandler.SpawnParticle(new HeavySmokeParticle(base.Projectile.Center, base.Projectile.velocity * 0.5f, Color.Lerp(Color.DodgerBlue, Color.MediumVioletRed, (float)Math.Sin((double)(Main.GlobalTimeWrappedHourly * 6f))), 20, Main.rand.NextFloat(0.6f, 1.2f) * base.Projectile.scale, 0.28f, 0f, glowing: false, 0f, required: true));
			if (Main.rand.Next(3) == 0)
			{
				GeneralParticleHandler.SpawnParticle(new HeavySmokeParticle(base.Projectile.Center, base.Projectile.velocity * 0.5f, Main.hslToRgb(Hue, 1f, 0.7f), 15, Main.rand.NextFloat(0.4f, 0.7f) * base.Projectile.scale, 0.8f, 0f, glowing: true, 0.05f, required: true));
			}
		}*/
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

    }

    internal Color ColorFunction(float completionRatio)
    {
        return CalamityUtils.HsvToRgb(Main.GlobalTimeWrappedHourly, 1f, 1f); ;
    }

    internal float WidthFunction(float completionRatio)
    {
        float num = (float)Math.Pow((double)(1f - completionRatio), 3.0);
        return MathHelper.Lerp(0f, 22f * base.Projectile.scale * base.Projectile.Opacity, num);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        
        if (TrailDrawer == null)
        {
            TrailDrawer = new PrimitiveTrail(WidthFunction, ColorFunction, null, GameShaders.Misc["CalamityMod:TrailStreak"]);
        }
        GameShaders.Misc["CalamityMod:TrailStreak"].SetShaderTexture(ModContent.Request<Texture2D>("AotC/Content/ScarletDevilStreak", (AssetRequestMode)2));
        TrailDrawer.Draw(base.Projectile.oldPos, base.Projectile.Size * 0.5f - Main.screenPosition, 30);
        if (Particles != null)
        {
            Main.spriteBatch.EnterShaderRegion(BlendState.Additive);
            foreach (Particle particle in Particles)
            {
                particle.CustomDraw(Main.spriteBatch);
            }
            Main.spriteBatch.ExitShaderRegion();
        }
        rotation += 0.2f;
        return false;
    }

    public override void PostDraw(Color lightColor)
    {
        Texture2D value = ModContent.Request<Texture2D>("AotC/Content/TestStar", (AssetRequestMode)2).Value;
        Main.EntitySpriteDraw(value, base.Projectile.Center - Main.screenPosition, null, Color.Lerp(lightColor, Color.White, 0.5f), base.Projectile.rotation + rotation, value.Size() / 2f, base.Projectile.scale, 0, 0);

    }
}
