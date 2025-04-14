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
using AotC.Content.Dusts;
using Terraria.Graphics.Shaders;

namespace AotC.Content.Projectiles;

public class KillerWail : BaseLaserbeamProjectile
{

    public List<Particle> Particles;
    public override string Texture => "AotC/Content/Projectiles/KillerWail";
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

    public override Texture2D LaserBeginTexture => null;
    public override Texture2D LaserMiddleTexture => ModContent.Request<Texture2D>("Aotc/Content/Projectiles/KillerWail", (AssetRequestMode)1).Value;
    public override Texture2D LaserEndTexture => null;


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
                Projectile.direction = (Main.MouseWorld.X > Owner.Center.X) ? 1 : (-1);
                Projectile.netUpdate = true;
                Projectile.direction = (Main.MouseWorld.X > Owner.Center.X) ? 1 : (-1);
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
                SoundEngine.PlaySound(in Sounds.AotCAudio.KillerWail, Projectile.position);
            else
                SoundEngine.PlaySound(in SoundID.Item68, Projectile.position);
            PlayedSound = true;
        }
        return true;
    }

    //dont remove this it gets rid of the base code which causes funny stuff to happen
    public override void UpdateLaserMotion()
    {

    }

    public override void DetermineScale()
    {
        Projectile.scale = (Wail ? 6f : 2f) * ModdedUtils.PiecewiseAnimation(Time / Lifetime, new ModdedUtils.CurveSegment[2] {balls, retract});
        if (Projectile.scale > MaxScale)
        {
            Projectile.scale = MaxScale;
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        lightColor.A = 255;
        float scale = Projectile.scale;
        Rectangle val2 = LaserMiddleTexture.Frame(1, Main.projFrames[Projectile.type], 0, 0);
        Vector2 center = Projectile.Center;
        Particle particle3 = new BloomLineVFX(Projectile.Center + Projectile.rotation.ToRotationVector2().RotatedBy(Math.PI / 2f) * 30f, (Projectile.rotation + (float)Math.PI / 2f).ToRotationVector2() * 10000, Projectile.scale, ModdedUtils.HsvToRgb(Main.GlobalTimeWrappedHourly, 1f, 1f), 20, capped: true);
        if (!Main.dedServ)
            particle3.Type = GeneralParticleHandler.particleTypes[particle3.GetType()];
        Main.spriteBatch.EnterShaderRegion(BlendState.Additive);
        particle3.CustomDraw(Main.spriteBatch);
        Main.spriteBatch.ExitShaderRegion();
        Main.spriteBatch.EnterShaderRegion();
        GameShaders.Misc["HueShiftShader"].UseOpacity(Wail ? 0.5f : 1f);
        GameShaders.Misc["HueShiftShader"].Shader.Parameters["uShift"].SetValue(-Main.GlobalTimeWrappedHourly * (Wail ? 1.6f : 5f));
        GameShaders.Misc["HueShiftShader"].Apply();
        Main.spriteBatch.Draw(LaserMiddleTexture, center - Main.screenPosition, val2, Color.White * (Wail ? 0.5f : 1f), Projectile.rotation, LaserMiddleTexture.Width * 0.5f * Vector2.UnitX, scale, 0, 0);
        Main.spriteBatch.ExitShaderRegion();
        Texture2D value = ModContent.Request<Texture2D>("AotC/Content/Projectiles/KillerWailWave").Value;
        Main.spriteBatch.Draw(value, center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(8, 0 - ear % 14), Projectile.scale, 0, 0);
        Vector2 vector = ModdedUtils.RandomVector2(Main.rand.NextFloat(0,30));
        int dust;
        for (int i = 0; i  < 7;  i++)
        {
            dust = Dust.NewDust(center + Dir.ToRotationVector2() * Main.rand.NextFloat(i * 100, (i+1) * 100), 1, 1, ModContent.DustType<TeleporterDustRGB>(), vector.X, vector.Y, newColor: ModdedUtils.HsvToRgb(Main.rand.NextFloat(0, 255f), 1f, 1f), Scale: 2f);
            Main.dust[dust].noGravity = true;
            Main.dust[dust].fadeIn = 1f;
        }   
        ear += Wail ? 1f : 3;
        return false;
    }
}
