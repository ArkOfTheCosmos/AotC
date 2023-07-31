using System;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using ReLogic.Content;
using Terraria.ModLoader;
using AotC.Content.Particles;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.CameraModifiers;

namespace AotC.Content.Projectiles;

public class KillerWail : BaseLaserbeamProjectile
{

    public List<Particle> Particles;
    public override string Texture => "AotC/Content/Projectiles/Beam";
    public ref float Dir => ref Projectile.ai[0];
    public Player Owner => Main.player[Projectile.owner];
    public bool initialized = false;
    public float ear;
    public bool PlayedSound;
    public bool Wail => Projectile.ai[1] == 1f;

    public ModdedUtils.CurveSegment balls = new(ModdedUtils.EasingType.PolyOut, 0f, 0.15f, 1f, 4);

    public ModdedUtils.CurveSegment retract = new(ModdedUtils.EasingType.SineIn, 0.75f, 1f, -1f);

    public override float Lifetime => Wail ? 100f : 35f;

    public override float MaxScale => Wail ? 9f : 3f;

    public override float MaxLaserLength => 80f;

    public override Texture2D LaserBeginTexture => ModContent.Request<Texture2D>("Aotc/Content/Projectiles/BeamBegin", (AssetRequestMode)1).Value;
    public override Texture2D LaserMiddleTexture => ModContent.Request<Texture2D>("Aotc/Content/Projectiles/BeamMiddle", (AssetRequestMode)1).Value;
    public override Texture2D LaserEndTexture => ModContent.Request<Texture2D>("Aotc/Content/Projectiles/BeamEnd", (AssetRequestMode)1).Value;


    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
    }
    public override void SetDefaults()
    {
        Projectile.width = 1;
        Projectile.height = 1;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.DamageType = DamageClass.Melee;
        Projectile.tileCollide = false;
    }

    public override void OnSpawn(IEntitySource source)
    {
        Projectile.CritChance = Owner.GetWeaponCrit(Owner.HeldItem);
        base.OnSpawn(source);
    }
    public override bool PreAI()
    {

        if (!initialized)
        {
            initialized = true;
            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.direction = ((Main.MouseWorld.X > Owner.Center.X) ? 1 : (-1));
                Projectile.netUpdate = true;
                Projectile.direction = ((Main.MouseWorld.X > Owner.Center.X) ? 1 : (-1));
            }
            Projectile.rotation = Dir - (float)Math.PI / 2;
            Projectile.Center = Dir.ToRotationVector2() * 370f + Owner.Center;
            Projectile.timeLeft = Wail ? 100 : 35;
            if (Wail)
            {
                PunchCameraModifier modifier = new(Owner.Center, (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2(), 5f, 6f, 20, 1000f, FullName);
                Main.instance.CameraModifiers.Add(modifier);
            }
        }
        Projectile.netUpdate = true;
        if (!PlayedSound)
        {
            if (Wail)
            {
                SoundEngine.PlaySound(in Sounds.AotCAudio.KillerWail, Projectile.position);
                SoundEngine.PlaySound(in Sounds.AotCAudio.KillerWail, Projectile.position);
            }
            else
            {
                SoundEngine.PlaySound(in Sounds.AotCAudio.ChaosBlasterFire, Projectile.position);
            }

            PlayedSound = true;
        }
        return true;
    }

    public override void UpdateLaserMotion()
    {

    }

    public override void DetermineScale()
    {
        try
        {
            Projectile.scale = (Wail ? 6f : 2f) * ModdedUtils.PiecewiseAnimation(Time / Lifetime, new ModdedUtils.CurveSegment[2] {balls, retract});
            if (Projectile.scale > MaxScale)
            {
                Projectile.scale = MaxScale;
            }
        }
        catch
        {
            Main.NewText("if you see this it means my code sucks - ArkoftheCosmos");
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        lightColor.A = 255;
        float scale = Projectile.scale;
        Rectangle val = LaserBeginTexture.Frame(1, Main.projFrames[Projectile.type], 0, 0);
        Rectangle val2 = LaserMiddleTexture.Frame(1, Main.projFrames[Projectile.type], 0, 0);
        Rectangle val3 = LaserEndTexture.Frame(1, Main.projFrames[Projectile.type], 0, 0);
        Main.EntitySpriteDraw(LaserBeginTexture, Projectile.Center - Main.screenPosition, val, lightColor, Projectile.rotation, LaserBeginTexture.Size() / 2f, scale, 0, 0);
        float laserLength = LaserLength;
        laserLength -= (val.Height / 2 + val3.Height) * scale;
        Vector2 center = Projectile.Center;



        Particle particle3 = new BloomLineVFX(Projectile.Center + Projectile.rotation.ToRotationVector2().RotatedBy(Math.PI / 2f) * 30f, (Projectile.rotation + (float)Math.PI / 2f).ToRotationVector2() * 10000, Projectile.scale, ModdedUtils.HsvToRgb(Main.GlobalTimeWrappedHourly, 1f, 1f), 20, capped: true);
        if (!Main.dedServ)
        {
            particle3.Type = GeneralParticleHandler.particleTypes[particle3.GetType()];
        }
        Main.spriteBatch.EnterShaderRegion(BlendState.Additive);
        particle3.CustomDraw(Main.spriteBatch);
        Main.spriteBatch.ExitShaderRegion();



        if (laserLength > 0f)
        {
            float num = val2.Height * scale;
            float num2 = 0f;
            while (num2 + 1f < laserLength)
            {

                Texture2D FinalTexture = ModdedUtils.ShiftHue(LaserMiddleTexture, -Main.GlobalTimeWrappedHourly * (Wail ? 1.6f : 5f) % 1 + 1);                Main.spriteBatch.Draw(FinalTexture, center - Main.screenPosition, val2, Color.White * (Wail ? 0.5f : 1f), Projectile.rotation, (float)LaserMiddleTexture.Width * 0.5f * Vector2.UnitX, scale, (SpriteEffects)0, 0);
                num2 += num;
            }
        }
        if (Math.Abs(LaserLength - DetermineLaserLength()) < 30f)
        {
            Vector2 position = center - Main.screenPosition;
            Main.EntitySpriteDraw(LaserEndTexture, position, val3, lightColor, Projectile.rotation, LaserEndTexture.Frame().Top(), scale, (SpriteEffects)0, 0);
        }
        Texture2D value = ModContent.Request<Texture2D>("AotC/Content/Projectiles/BeamWave").Value;
        Main.spriteBatch.Draw(value, center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(8, 0 - ear % 14), Projectile.scale, 0, 0);
        ear += Wail ? 1f : 3;




        return false;
    }
}
