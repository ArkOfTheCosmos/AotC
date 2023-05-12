using AotC.Content.Items.Weapons;
using AotC.Content.StolenCalamityCode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AotC.Content
{
    public class ArkoftheCosmosConstellation : ModProjectile
    {
        public List<Particle> Particles;

        public override string Texture => "AotC/Content/InvisibleProj";

        public Player Owner => Main.player[Projectile.owner];

        public float Timer => Projectile.ai[0] - Projectile.timeLeft;

        public float AnchorType => Projectile.ai[1];

        private Vector2 AnchorStart
        {
            get
            {
                if (AnchorType == 0f)
                {
                    return Owner.Center;
                }
                else if (AnchorType == 1f)
                {
                    return Vector2.Lerp(Projectile.Center.RotatedBy(-1, Owner.Center), Projectile.Center, 0.3f);
                }
                else
                {
                    return new();
                }
            }
        }
        private Vector2 AnchorEnd
        {
            get
            {
                if (AnchorType == 0f)
                {
                    return Main.MouseWorld;
                }
                else if (AnchorType == 1f)
                {
                    return Vector2.Lerp(Projectile.Center.RotatedBy(1, Owner.Center), Projectile.Center, 0.3f);
                }
                else
                {
                    return new();
                }
            }
        }

        public Vector2 SizeVector
        {
            get
            {
                if (AnchorType == 0f)
                {
                    Vector2 val = (AnchorEnd - AnchorStart).SafeNormalize(Vector2.Zero);
                    Vector2 val2 = AnchorEnd - AnchorStart;
                    return val * MathHelper.Clamp((val2).Length(), 0f, ArkoftheCosmos.MaxThrowReach);
                }
                else if (AnchorType == 1f)
                {
                    Vector2 val = (AnchorEnd - AnchorStart).SafeNormalize(Vector2.Zero);
                    Vector2 val2 = AnchorEnd - AnchorStart;
                    return val * val2.Length();
                }
                else
                {
                    return new();
                }
            }
        }
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 1;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = Projectile.height = 80;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + SizeVector, 30f, ref collisionPoint);
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
                foreach (Particle particle in Particles)
                {
                   //particle.Color = particle.Color + CalamityUtils.HsvToRgb(Main.GlobalTimeWrappedHourly * 255 % 255, 1f, 1f);
                }
            }
            /*if (!Owner.channel && Projectile.timeLeft > 20)
            {
                Projectile.timeLeft = 20;
            }*/
            if (!Owner.active)
            {
                Projectile.Kill();
                return;
            }
            if (AnchorType == 0f)
            {
                Projectile.Center = Owner.Center;
                if (Timer % 10f == 0f)
                {
                    Particles.Clear();
                    float num = Main.rand.NextFloat();
                    Color val = Main.hslToRgb(num, 1f, 0.5f);
                    Vector2 val2 = AnchorStart;
                    Particle particle2 = new GenericSparkle(val2, Vector2.Zero, Color.White, val, Main.rand.NextFloat(1f, 1.5f), 20, 0f, 3f);
                    BootlegSpawnParticle(particle2);
                    Particle particle3;
                    for (float num2 = 0f + Main.rand.NextFloat(0.2f, 0.5f); num2 < 1f; num2 += Main.rand.NextFloat(0.2f, 0.5f))
                    {
                        num = (num + 0.16f) % 1f;
                        val = Main.hslToRgb(num, 1f, 0.5f);
                        Vector2 val3 = Main.rand.NextFloat(-50f, 50f) * SizeVector.RotatedBy(1.5707963705062866).SafeNormalize(Vector2.Zero);
                        particle2 = new GenericSparkle(AnchorStart + SizeVector * num2 + val3, Vector2.Zero, Color.White, val, Main.rand.NextFloat(1f, 1.5f), 20, 0f, 3f);
                        BootlegSpawnParticle(particle2);
                        particle3 = new BloomLineVFX(val2, AnchorStart + SizeVector * num2 + val3 - val2, 0.8f, val, 20, capped: true, telegraph: true);
                        BootlegSpawnParticle(particle3);
                        if (Main.rand.NextBool())
                        {
                            num = (num + 0.16f) % 1f;
                            val = Main.hslToRgb(num, 1f, 0.5f);
                            val3 = Main.rand.NextFloat(-50f, 50f) * SizeVector.RotatedBy(1.5707963705062866).SafeNormalize(Vector2.Zero);
                            particle2 = new GenericSparkle(AnchorStart + SizeVector * num2 + val3, Vector2.Zero, Color.White, val, Main.rand.NextFloat(1f, 1.5f), 20, 0f, 3f);
                            BootlegSpawnParticle(particle2);
                            particle3 = new BloomLineVFX(val2, AnchorStart + SizeVector * num2 + val3 - val2, 0.8f, val, 20, capped: true, telegraph: true);
                            BootlegSpawnParticle(particle3);
                        }
                        val2 = AnchorStart + SizeVector * num2 + val3;
                    }
                    num = (num + 0.16f) % 1f;
                    val = Main.hslToRgb(num, 1f, 0.5f);
                    particle2 = new GenericSparkle(AnchorStart + SizeVector, Vector2.Zero, Color.White, val, Main.rand.NextFloat(1f, 1.5f), 20, 0f, 3f);
                    BootlegSpawnParticle(particle2);
                    particle3 = new BloomLineVFX(val2, AnchorStart + SizeVector - val2, 0.8f, val, 20, capped: true);
                    BootlegSpawnParticle(particle3);
                }
            }






            //swing
            else if (AnchorType == 1f && Timer == Projectile.ai[1])
            {
                Particles.Clear();
                float num = Main.rand.NextFloat();
                Color val = Main.hslToRgb(num, 1f, 0.5f);
                Vector2 val2 = AnchorStart;
                Particle particle2 = new GenericSparkle(val2, Vector2.Zero, Color.White, val, Main.rand.NextFloat(1f, 1.5f), 20, 0f, 3f);
                BootlegSpawnParticle(particle2);
                Particle particle3;
                for (float num2 = 0f + Main.rand.NextFloat(0.2f, 0.5f); num2 < 1f; num2 += Main.rand.NextFloat(0.2f, 0.5f))
                {
                    num = (num + 0.16f) % 1f;
                    val = Main.hslToRgb(num, 1f, 0.5f);
                    Vector2 val3 = Main.rand.NextFloat(-50f, 50f) * SizeVector.RotatedBy(1.5707963705062866).SafeNormalize(Vector2.Zero);
                    particle2 = new GenericSparkle(AnchorStart + SizeVector * num2 + val3, Vector2.Zero, Color.White, val, Main.rand.NextFloat(1f, 1.5f), 20, 0f, 3f);
                    BootlegSpawnParticle(particle2);
                    particle3 = new BloomLineVFX(val2, AnchorStart + SizeVector * num2 + val3 - val2, 0.8f, val, 20, capped: true, telegraph: true);
                    BootlegSpawnParticle(particle3);
                    if (Main.rand.NextBool())
                    {
                        num = (num + 0.16f) % 1f;
                        val = Main.hslToRgb(num, 1f, 0.5f);
                        val3 = Main.rand.NextFloat(-50f, 50f) * SizeVector.RotatedBy(1.5707963705062866).SafeNormalize(Vector2.Zero);
                        particle2 = new GenericSparkle(AnchorStart + SizeVector * num2 + val3, Vector2.Zero, Color.White, val, Main.rand.NextFloat(1f, 1.5f), 20, 0f, 3f);
                        BootlegSpawnParticle(particle2);
                        particle3 = new BloomLineVFX(val2, AnchorStart + SizeVector * num2 + val3 - val2, 0.8f, val, 20, capped: true, telegraph: true);
                        BootlegSpawnParticle(particle3);
                    }
                    val2 = AnchorStart + SizeVector * num2 + val3;
                }
                num = (num + 0.16f) % 1f;
                val = Main.hslToRgb(num, 1f, 0.5f);
                particle2 = new GenericSparkle(AnchorStart + SizeVector, Vector2.Zero, Color.White, val, Main.rand.NextFloat(1f, 1.5f), 20, 0f, 3f);
                BootlegSpawnParticle(particle2);
                particle3 = new BloomLineVFX(val2, AnchorStart + SizeVector - val2, 0.8f, val, 20, capped: true);
                BootlegSpawnParticle(particle3);
            } 







            Vector2 val4 = Vector2.Zero;
            if (Timer > Projectile.oldPos.Length)
            {
                val4 = Projectile.position - Projectile.oldPos[0];
            }
            foreach (Particle particle4 in Particles)
            {
                if (particle4 != null)
                {
                    particle4.Position += particle4.Velocity + val4;
                    particle4.Time++;
                    particle4.Update();
                    particle4.Color = Main.hslToRgb(Main.rgbToHsl(particle4.Color).X + 0.02f, Main.rgbToHsl(particle4.Color).Y, Main.rgbToHsl(particle4.Color).Z);
                }
            }
            Particles.RemoveAll((Particle particle) => particle.Time >= particle.Lifetime && particle.SetLifetime);
            Projectile.Center = Owner.Center;
        }

        public override bool PreDraw(ref Color lightColor)
        {

            if (Particles != null)
            {
                Main.spriteBatch.EnterShaderRegion(BlendState.Additive);
                foreach (Particle particle in Particles)
                {
                    particle.CustomDraw(Main.spriteBatch);
                }
                Main.spriteBatch.ExitShaderRegion();
            }
            return false;
        }
    }
}
