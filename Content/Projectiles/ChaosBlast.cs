using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace AotC.Content.Projectiles
{
    internal class ChaosBlast : ModProjectile
    {
        public bool Homing => !(Projectile.ai[0] == 0);
        public NPC target;
        internal PrimitiveTrail TrailDrawer;
        private float colorRand;
        public ref float HomingStrength => ref Projectile.ai[1];
        public Player Owner => Main.player[Projectile.owner];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults() 
        {
            Projectile.width = 26;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.timeLeft = 160;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = false;
            Projectile.light = 0.5f;
            colorRand = Main.rand.NextFloat() / 5f;
        }
        public override void AI()
        {
            if (Homing)
            {
                if (target == null)
                    target = Projectile.Center.ClosestNPCAt(300f);
                else
                {
                    if (Main.rand.NextBool(3))
                        Projectile.timeLeft++;
                    if (Projectile.velocity.AngleBetween(target.Center - Projectile.Center) < (float)Math.PI)
                    {
                        float targetAngle = Projectile.AngleTo(target.Center);
                        float f = Projectile.velocity.ToRotation().AngleTowards(targetAngle, HomingStrength);
                        Projectile.velocity = f.ToRotationVector2() * Projectile.velocity.Length() * 0.995f;
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.EnterShaderRegion(BlendState.Additive);
            TrailDrawer ??= new PrimitiveTrail(WidthFunction, ColorFunction, null, GameShaders.Misc["AotC:TrailStreak"]);
            GameShaders.Misc["AotC:TrailStreak"].SetShaderTexture(ModContent.Request<Texture2D>("AotC/Assets/Textures/ScarletDevilStreak", (AssetRequestMode)2));
            TrailDrawer.Draw(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 30);
            Main.spriteBatch.ExitShaderRegion();
            return true; 
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Owner.GetPlot().ChaosBusterCharge += 1;
        }

        private Color ColorFunction(float completionRatio)
        {
            return ModdedUtils.HsvToRgb(Main.GlobalTimeWrappedHourly + colorRand, 1f, 1f) * 0.5f;
        }
        private float WidthFunction(float completionRatio)
        {
            float num = (float)Math.Pow((double)(1f - completionRatio), 3.0);
            return MathHelper.Lerp(0f, 22f * Projectile.scale * Projectile.Opacity, num);
        }
    }
}
