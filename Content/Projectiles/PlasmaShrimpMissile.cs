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
    internal class PlasmaShrimpMissile : ModProjectile
    {
        internal PrimitiveTrail TrailDrawer;
        public Vector2 homingVelocity = new();
        public Vector2 initVelocity;
        public NPC Target => Main.npc[(int)Projectile.ai[0]];
        private bool init = false;
        public float fricker;
        public override void SetStaticDefaults() 
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.width = Projectile.height = 8;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 34;
            Projectile.extraUpdates = 1;
        }
        public override void AI()
        {
            if (!init)
            {
                init = true;
                initVelocity.Y = Projectile.ai[1];
                fricker = Main.rand.NextFloat(-0.1f,0.1f);
            }
            Lighting.AddLight(Projectile.Center, 138 / 255f, 43 / 255f, 226 / 255f);
            Projectile.velocity = initVelocity + homingVelocity;
            homingVelocity = (Target.Center - Projectile.Center) * ((float)Math.Pow(3600 - Projectile.timeLeft, 2)  / 2900f);
            initVelocity.Y *= 0.9f;
            if (!Target.active && Projectile.timeLeft < 3570)
                Projectile.Kill();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            TrailDrawer ??= new PrimitiveTrail(WidthFunction, ColorFunction, null, GameShaders.Misc["AotC:TrailStreak"]);
            GameShaders.Misc["AotC:TrailStreak"].SetShaderTexture(ModContent.Request<Texture2D>("AotC/Assets/Textures/ScarletDevilStreak", (AssetRequestMode)2));
            TrailDrawer.Draw(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 30);
            Main.spriteBatch.ExitShaderRegion();
            return false;
        }
        public override void PostDraw(Color lightColor)
        {
            Texture2D value = ModContent.Request<Texture2D>("AotC/Content/Projectiles/PlasmaShrimpMissile", (AssetRequestMode)2).Value;
            Main.EntitySpriteDraw(value, Projectile.Center - Main.screenPosition, null, Color.Lerp(lightColor, Color.White, 0.5f), Projectile.rotation, value.Size() / 2f, Projectile.scale, 0, 0);
        }
        private Color ColorFunction(float completionRatio)
        {
            Color color = Color.BlueViolet;
            ModdedUtils.ColorToHSV(color, out float h, out float s, out float v);
            s += fricker;
            h += fricker / 3;
            color = ModdedUtils.HsvToRgb(h, s, v);
            return color;
        }
        private float WidthFunction(float completionRatio)
        {
            float num = (float)Math.Pow((double)(1f - completionRatio), 3.0);
            return MathHelper.Lerp(0f, 10f * Projectile.scale * Projectile.Opacity, num);
        }
    }
}
