using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace AotC.Content.Projectiles;

public abstract class BaseLaserbeamProjectile : ModProjectile
{
    public float RotationalSpeed
    {
        get
        {
            return Projectile.ai[0];
        }
        set
        {
            Projectile.ai[0] = value;
        }
    }

    public float Time
    {
        get
        {
            return Projectile.localAI[0];
        }
        set
        {
            Projectile.localAI[0] = value;
        }
    }

    public float LaserLength
    {
        get
        {
            return Projectile.localAI[1];
        }
        set
        {
            Projectile.localAI[1] = value;
        }
    }

    public abstract float Lifetime { get; }

    public abstract float MaxScale { get; }

    public abstract float MaxLaserLength { get; }

    public abstract Texture2D LaserBeginTexture { get; }

    public abstract Texture2D LaserMiddleTexture { get; }

    public abstract Texture2D LaserEndTexture { get; }

    public virtual float ScaleExpandRate => 4f;

    public virtual Color LightCastColor => Color.White;

    public virtual Color LaserOverlayColor => Color.White * 0.9f;

    public virtual void Behavior()
    {
        //IL_0012: Unknown result type (might be due to invalid IL or missing references)
        //IL_0017: Unknown result type (might be due to invalid IL or missing references)
        //IL_001c: Unknown result type (might be due to invalid IL or missing references)
        //IL_0021: Unknown result type (might be due to invalid IL or missing references)
        //IL_0026: Unknown result type (might be due to invalid IL or missing references)
        //IL_0084: Unknown result type (might be due to invalid IL or missing references)
        //IL_0089: Unknown result type (might be due to invalid IL or missing references)
        //IL_0096: Unknown result type (might be due to invalid IL or missing references)
        //IL_009b: Unknown result type (might be due to invalid IL or missing references)
        //IL_009e: Unknown result type (might be due to invalid IL or missing references)
        //IL_00a3: Unknown result type (might be due to invalid IL or missing references)
        //IL_00ae: Unknown result type (might be due to invalid IL or missing references)
        //IL_00b9: Unknown result type (might be due to invalid IL or missing references)
        //IL_00c4: Unknown result type (might be due to invalid IL or missing references)
        //IL_00cf: Unknown result type (might be due to invalid IL or missing references)
        //IL_00d4: Unknown result type (might be due to invalid IL or missing references)
        AttachToSomething();
        Projectile.velocity = Projectile.velocity.SafeNormalize(-Vector2.UnitY);
        Time++;
        if (Time >= Lifetime)
        {
            Projectile.Kill();
            return;
        }
        DetermineScale();
        UpdateLaserMotion();
        float num = DetermineLaserLength();
        LaserLength = MathHelper.Lerp(LaserLength, num, 0.9f);
        if (LightCastColor != Color.Transparent)
        {
            Color lightCastColor = LightCastColor;
            DelegateMethods.v3_1 = ((Color)(lightCastColor)).ToVector3();
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * LaserLength, (float)Projectile.width * Projectile.scale, DelegateMethods.CastLight);
        }
    }

    public virtual void UpdateLaserMotion()
    {
        //IL_0006: Unknown result type (might be due to invalid IL or missing references)
        //IL_0031: Unknown result type (might be due to invalid IL or missing references)
        //IL_0036: Unknown result type (might be due to invalid IL or missing references)
        float num = Projectile.velocity.ToRotation() + RotationalSpeed;
        Projectile.rotation = num - (float)Math.PI / 2f;
        Projectile.velocity = num.ToRotationVector2();
    }

    public virtual void DetermineScale()
    {
        Projectile.scale = (float)Math.Sin((double)(Time / Lifetime * (float)Math.PI)) * ScaleExpandRate * MaxScale;
        if (Projectile.scale > MaxScale)
        {
            Projectile.scale = MaxScale;
        }
    }

    public virtual void AttachToSomething()
    {
    }

    public virtual float DetermineLaserLength()
    {
        return MaxLaserLength;
    }

    public virtual void ExtraBehavior()
    {
    }

    public float DetermineLaserLength_CollideWithTiles(int samplePointCount)
    {
        //IL_000d: Unknown result type (might be due to invalid IL or missing references)
        //IL_0018: Unknown result type (might be due to invalid IL or missing references)
        float[] array = new float[samplePointCount];
        Collision.LaserScan(Projectile.Center, Projectile.velocity, Projectile.scale, MaxLaserLength, array);
        return array.Average();
    }

    protected internal void DrawBeamWithColor(Color beamColor, float scale, int startFrame = 0, int middleFrame = 0, int endFrame = 0)
    {
        Rectangle val = LaserBeginTexture.Frame(1, Main.projFrames[Projectile.type], 0, startFrame);
        Rectangle val2 = LaserMiddleTexture.Frame(1, Main.projFrames[Projectile.type], 0, middleFrame);
        Rectangle val3 = LaserEndTexture.Frame(1, Main.projFrames[Projectile.type], 0, endFrame);
        Main.EntitySpriteDraw(LaserBeginTexture, Projectile.Center - Main.screenPosition, val, beamColor, Projectile.rotation, LaserBeginTexture.Size() / 2f, scale, (SpriteEffects)0, 0);
        float laserLength = LaserLength;
        laserLength -= (float)(val.Height / 2 + val3.Height) * scale;
        Vector2 center = Projectile.Center;
        center += Projectile.velocity * scale * (float)val.Height / 2f;
        if (laserLength > 0f)
        {
            float num = (float)val2.Height * scale;
            float num2 = 0f;
            while (num2 + 1f < laserLength)
            {
                Main.EntitySpriteDraw(LaserMiddleTexture, center - Main.screenPosition, val2, beamColor, Projectile.rotation, (float)LaserMiddleTexture.Width * 0.5f * Vector2.UnitX, scale, (SpriteEffects)0, 0);
                num2 += num;
                center += Projectile.velocity * num;
            }
        }
        if (Math.Abs(LaserLength - DetermineLaserLength()) < 30f)
        {
            Vector2 position = center - Main.screenPosition;
            Main.EntitySpriteDraw(LaserEndTexture, position, val3, beamColor, Projectile.rotation, LaserEndTexture.Frame().Top(), scale, (SpriteEffects)0, 0);
        }
    }

    public override void AI()
    {
        ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 10000;
        Behavior();
        ExtraBehavior();
    }

    public override void CutTiles()
    {
        DelegateMethods.tilecut_0 = TileCuttingContext.AttackMelee;
        Vector2 center = Projectile.Center;
        Vector2 end = Projectile.Center + Projectile.velocity * LaserLength;
        Vector2 size = Projectile.Size;
        Utils.PlotTileLine(center, end, ((Vector2)(size)).Length() * Projectile.scale, DelegateMethods.CutTiles);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        //IL_0006: Unknown result type (might be due to invalid IL or missing references)
        //IL_000b: Unknown result type (might be due to invalid IL or missing references)
        //IL_001b: Unknown result type (might be due to invalid IL or missing references)
        if (Projectile.velocity == Vector2.Zero)
        {
            return false;
        }
        DrawBeamWithColor(LaserOverlayColor, Projectile.scale);
        return false;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        //IL_0002: Unknown result type (might be due to invalid IL or missing references)
        //IL_0017: Unknown result type (might be due to invalid IL or missing references)
        //IL_0018: Unknown result type (might be due to invalid IL or missing references)
        //IL_001d: Unknown result type (might be due to invalid IL or missing references)
        //IL_001e: Unknown result type (might be due to invalid IL or missing references)
        //IL_0029: Unknown result type (might be due to invalid IL or missing references)
        //IL_0034: Unknown result type (might be due to invalid IL or missing references)
        //IL_003f: Unknown result type (might be due to invalid IL or missing references)
        //IL_004a: Unknown result type (might be due to invalid IL or missing references)
        //IL_004f: Unknown result type (might be due to invalid IL or missing references)
        //IL_005a: Unknown result type (might be due to invalid IL or missing references)
        //IL_005f: Unknown result type (might be due to invalid IL or missing references)
        if (((Rectangle)(projHitbox)).Intersects(targetHitbox))
        {
            return true;
        }
        float collisionPoint = 0f;
        Vector2 objectPosition = targetHitbox.TopLeft();
        Vector2 objectDimensions = targetHitbox.Size();
        Vector2 center = Projectile.Center;
        Vector2 lineEnd = Projectile.Center + Projectile.ai[0].ToRotationVector2() * 908;
        Vector2 size = Projectile.Size;
        return Collision.CheckAABBvLineCollision(objectPosition, objectDimensions * Projectile.scale / 3f, center, lineEnd, Projectile.scale, ref collisionPoint);
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }
}
