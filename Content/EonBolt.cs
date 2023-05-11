using AotC.Content.StolenCalamityCode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace AotC.Content;

public class EonBolt : ModProjectile
{

    Random rand = new Random();

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

    public override void AI()
    {
        base.Projectile.rotation = base.Projectile.velocity.ToRotation() + (float)Math.PI / 2f;
        if (Head == null)
        {
            //Head = new GenericSparkle(base.Projectile.Center, Vector2.Zero, Color.White, Main.hslToRgb(Hue, 100f, 50f), 1.2f, 2, 0.06f, 3f, needed: true);
            //GeneralParticleHandler.SpawnParticle(Head);
        }
        else
        {
            Head.Position = base.Projectile.Center + base.Projectile.velocity * 0.5f;
            Head.Time = 0;
            Head.Scale += (float)Math.Sin((double)(Main.GlobalTimeWrappedHourly * 6f)) * 0.02f * base.Projectile.scale;
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
        Lighting.AddLight(base.Projectile.Center, 0.75f, 1f, 0.24f);  //fix this later
        /*if (Main.rand.Next(2) == 0)
		{
			GeneralParticleHandler.SpawnParticle(new HeavySmokeParticle(base.Projectile.Center, base.Projectile.velocity * 0.5f, Color.Lerp(Color.DodgerBlue, Color.MediumVioletRed, (float)Math.Sin((double)(Main.GlobalTimeWrappedHourly * 6f))), 20, Main.rand.NextFloat(0.6f, 1.2f) * base.Projectile.scale, 0.28f, 0f, glowing: false, 0f, required: true));
			if (Main.rand.Next(3) == 0)
			{
				GeneralParticleHandler.SpawnParticle(new HeavySmokeParticle(base.Projectile.Center, base.Projectile.velocity * 0.5f, Main.hslToRgb(Hue, 1f, 0.7f), 15, Main.rand.NextFloat(0.4f, 0.7f) * base.Projectile.scale, 0.8f, 0f, glowing: true, 0.05f, required: true));
			}
		}*/
    }

    internal Color ColorFunction(float completionRatio)
    {
        float num = MathHelper.Lerp(0.65f, 1f, (float)Math.Cos((double)((0f - Main.GlobalTimeWrappedHourly) * 3f)) * 0.5f + 0.5f);
        float num2 = Utils.GetLerpValue(1f, 0.64f, completionRatio, clamped: true) * base.Projectile.Opacity;
        Color gay = CalamityUtils.HsvToRgb(Main.GlobalTimeWrappedHourly * 255 % 255, 1f, 1f);

        //return Color.Lerp(Color.White, gay, num) * num2;
        return gay;
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
        Texture2D value = ModContent.Request<Texture2D>("AotC/Content/TestStar", (AssetRequestMode)2).Value;
        Main.EntitySpriteDraw(value, base.Projectile.Center - Main.screenPosition, null, Color.Lerp(lightColor, Color.White, 0.5f), base.Projectile.rotation + rotation, value.Size() / 2f, base.Projectile.scale, 0, 0);
        rotation += 0.2f;
        return false;
    }
}
