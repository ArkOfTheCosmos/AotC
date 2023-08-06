using AotC.Content.Particles;
using AotC.Content.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;

namespace AotC.Content.Projectiles
{
    public class Constellation : ModProjectile
    {
        public List<Particle> Particles;

        public int frick = 0;

        public Vector2? cachedStart = null;

        public bool balls = false;

        public bool death = false;

        public Vector2 idk = new(69,69);

        public Vector2 origPos;

        public double randRot = Main.rand.NextFloat(0f, MathHelper.Pi / 2.5f);

        public override string Texture => "AotC/Content/Projectiles/InvisibleProj";

        public Player Owner => Main.player[Projectile.owner];

        public float Timer => AnchorType == 5f ? -2 : Projectile.ai[0] - Projectile.timeLeft;

        public float AnchorType => Projectile.ai[1];

        public float SlashLineNum => Projectile.ai[2];

    private Vector2 AnchorStart
        {
            get
            {
                if (AnchorType is 0f or 4f)
                {
                    return Owner.Center;
                }
                else if (AnchorType == 1f)
                {
                    return Vector2.Lerp(Projectile.Center.RotatedBy(-1, Owner.Center), Projectile.Center, 0.3f);
                }
                else if (AnchorType == 2f)
                {
                    return Vector2.Lerp(Projectile.Center.RotatedBy(1, Owner.Center), Projectile.Center, 0.3f);
                }
                else if (AnchorType == 3f)
                {
                    return Projectile.Center;
                }
                else if (AnchorType == 5f)
                {
                    cachedStart??= ArkoftheCosmos.SlashPoints[(int)SlashLineNum - 1];
                    return (Vector2)cachedStart;
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
                else if (AnchorType == 2f)
                {
                    return Vector2.Lerp(Projectile.Center.RotatedBy(-1, Owner.Center), Projectile.Center, 0.3f);
                }
                else if (AnchorType == 3f)
                {
                    return Projectile.Center + Projectile.velocity * 1000f;
                }
                else if (AnchorType == 4f)      
                {
                    return Owner.Center + idk.RotatedBy(randRot) * 100f;
                }
                else if (AnchorType == 5f)
                {
                    if (ArkoftheCosmos.SlashPoints.Count > SlashLineNum)
                    {
                        return ArkoftheCosmos.SlashPoints[(int)SlashLineNum];
                    }
                    return new();
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
                    return val * MathHelper.Clamp(val2.Length(), 0f, ArkoftheCosmos.MaxThrowReach);
                }
                else if (AnchorType is 1f or 2f or 3f)
                {
                    Vector2 val = (AnchorEnd - AnchorStart).SafeNormalize(Vector2.Zero);
                    Vector2 val2 = AnchorEnd - AnchorStart;
                    return val * MathHelper.Clamp(val2.Length(), 0f, ArkoftheCosmos.MaxThrowReach * 3f);
                }
                if (AnchorType == 4f)
                {
                    Vector2 val = (AnchorEnd - AnchorStart).SafeNormalize(Vector2.Zero);
                    Vector2 val2 = AnchorEnd - AnchorStart;
                    return val * MathHelper.Clamp(val2.Length(), 0f, ArkoftheCosmos.MaxThrowReach);
                }
                else if (AnchorType == 5f)
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
            if (!balls && AnchorType != 5f)
            {
                origPos = Projectile.Center;
                balls = true;
            }
            if (idk == new Vector2 (69,69) && AnchorType == 4f)
            {
                idk = Projectile.velocity;
                Projectile.velocity = Vector2.Zero;
            }
            Particles ??= new List<Particle>();
            /*if (!Owner.channel && Projectile.timeLeft > 20)
            {
                Projectile.timeLeft = 20;
            }*/
            if (!Owner.active)
            {
                Projectile.Kill();
                return;
            }
            if (AnchorType is 0f or 3f or 4f)
            {
                if (AnchorType != 3f)
                {
                    Projectile.Center = Owner.Center;
                }
                if (Timer % 10f == 0f && frick < 3 || AnchorType == 3f && Timer % 5f == 0f)
                {
                    if (AnchorType == 4f)
                    {
                        frick++;
                    }
                    randRot = Main.rand.NextFloat(0f, MathHelper.Pi / 2.5f);
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
                        Vector2 val3 = Main.rand.NextFloat(-50f, 50f) * SizeVector.RotatedBy(Math.PI / 2f).SafeNormalize(Vector2.Zero);
                        particle2 = new GenericSparkle(AnchorStart + SizeVector * num2 + val3, Vector2.Zero, Color.White, val, Main.rand.NextFloat(1f, 1.5f), 20, 0f, 3f);
                        BootlegSpawnParticle(particle2);
                        particle3 = new BloomLineVFX(val2, AnchorStart + SizeVector * num2 + val3 - val2, 0.8f, val, 20, capped: true, telegraph: true);
                        BootlegSpawnParticle(particle3);
                        if (Main.rand.NextBool())
                        {
                            num = (num + 0.16f) % 1f;
                            val = Main.hslToRgb(num, 1f, 0.5f);
                            val3 = Main.rand.NextFloat(-50f, 50f) * SizeVector.RotatedBy(Math.PI / 2f).SafeNormalize(Vector2.Zero);
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
            else if (AnchorType is 1f or 2f && Timer == Projectile.ai[1])
            {
                Particles.Clear();
                float num = Main.rand.NextFloat();
                Color val = Main.hslToRgb(num, 1f, 0.5f);
                Vector2 val2 = Vector2.Lerp((AnchorStart).RotatedBy(AnchorType == 1f ? -0.5f : 0.5f, Owner.Center), Owner.Center, Main.rand.NextFloat(0f, 0.5f));
                Particle particle2 = new GenericSparkle(val2, Vector2.Zero, Color.White, val, Main.rand.NextFloat(1f, 1.5f), 20, 0f, 3f);
                BootlegSpawnParticle(particle2);
                Particle particle3;
                for (float num2 = 0f + Main.rand.NextFloat(0.2f, 0.5f); num2 < 1f; num2 += Main.rand.NextFloat(0.2f, 0.5f))
                {
                    num = (num + 0.16f) % 1f;
                    val = Main.hslToRgb(num, 1f, 0.5f);
                    Vector2 val3 = Main.rand.NextFloat(-100f, 100f) * SizeVector.RotatedBy(Math.PI / 2f).SafeNormalize(Vector2.Zero);
                    particle2 = new GenericSparkle(AnchorStart + SizeVector * num2 + val3, Vector2.Zero, Color.White, val, Main.rand.NextFloat(1f, 1.5f), 20, 0f, 3f);
                    BootlegSpawnParticle(particle2);
                    particle3 = new BloomLineVFX(val2, AnchorStart + SizeVector * num2 + val3 - val2, 0.8f, val, 20, capped: true, telegraph: true);
                    BootlegSpawnParticle(particle3);
                    if (Main.rand.NextBool())
                    {
                        num = (num + 0.16f) % 1f;
                        val = Main.hslToRgb(num, 1f, 0.5f);
                        val3 = Main.rand.NextFloat(-100f, 100f) * SizeVector.RotatedBy(Math.PI / 2f).SafeNormalize(Vector2.Zero);
                        particle2 = new GenericSparkle(AnchorStart + SizeVector * num2 + val3, Vector2.Zero, Color.White, val, Main.rand.NextFloat(1f, 1.5f), 20, 0f, 3f);
                        BootlegSpawnParticle(particle2);
                        particle3 = new BloomLineVFX(val2, AnchorStart + SizeVector * num2 + val3 - val2, 0.8f, val, 20, capped: true, telegraph: true);
                        BootlegSpawnParticle(particle3);
                    }
                    val2 = AnchorStart + SizeVector * num2 + val3;
                }
                num = (num + 0.16f) % 1f;
                val = Main.hslToRgb(num, 1f, 0.5f);
                float val5 = Main.rand.NextFloat(0f, 0.5f);
                particle2 = new GenericSparkle(Vector2.Lerp((AnchorStart + SizeVector).RotatedBy(AnchorType == 1f ? 0.5f : -0.5f, Owner.Center), Owner.Center, val5), Vector2.Zero, Color.White, val, Main.rand.NextFloat(1f, 1.5f), 20, 0f, 3f);
                BootlegSpawnParticle(particle2);
                particle3 = new BloomLineVFX(val2, Vector2.Lerp((AnchorStart + SizeVector).RotatedBy(AnchorType == 1f ? 0.5f : -0.5f, Owner.Center), Owner.Center, val5) - val2, 0.8f, val, 20, capped: true);
                BootlegSpawnParticle(particle3);
            }

            else if (AnchorType == 5f)
            {
                if (!death)
                {
                    Projectile.timeLeft = 20;
                }
                if (!balls)
                {
                    balls = true;
                    if (SlashLineNum == ArkoftheCosmos.SlashPoints.Count)
                    {
                        float num = Main.rand.NextFloat();
                        Color val = Main.hslToRgb(num, 1f, 0.5f);
                        Particle particle2 = new GenericSparkle(AnchorStart, Vector2.Zero, Color.White, val, SlashLineNum == 1 ? 2.01f : 2f, 20, 0f, 3f);
                        BootlegSpawnParticle(particle2);
                    }
                    else
                    {
                        randRot = Main.rand.NextFloat(0f, MathHelper.Pi / 2.5f);
                        Particles.Clear();
                        float num = Main.rand.NextFloat();
                        Color val = Main.hslToRgb(num, 1f, 0.5f);
                        Vector2 val2 = AnchorStart;
                        Particle particle2 = new GenericSparkle(val2, Vector2.Zero, Color.White, val, SlashLineNum == 1 ? 2.01f : Main.rand.NextFloat(1f, 1.5f), 20, 0f, 3f);
                        BootlegSpawnParticle(particle2);
                        Particle particle3;
                        for (float num2 = 0f + Main.rand.NextFloat(0.2f, 0.5f); num2 < 1f; num2 += Main.rand.NextFloat(0.2f, 0.5f))
                        {
                            num = (num + 0.16f) % 1f;
                            val = Main.hslToRgb(num, 1f, 0.5f);
                            Vector2 val3 = Main.rand.NextFloat(-50f, 50f) * SizeVector.RotatedBy(Math.PI / 2f).SafeNormalize(Vector2.Zero);
                            particle2 = new GenericSparkle(AnchorStart + SizeVector * num2 + val3, Vector2.Zero, Color.White, val, Main.rand.NextFloat(1f, 1.5f), 20, 0f, 3f);
                            BootlegSpawnParticle(particle2);
                            particle3 = new BloomLineVFX(val2, AnchorStart + SizeVector * num2 + val3 - val2, 0.8f, val, 20, capped: true, telegraph: true);
                            BootlegSpawnParticle(particle3);
                            if (Main.rand.NextBool())
                            {
                                num = (num + 0.16f) % 1f;
                                val = Main.hslToRgb(num, 1f, 0.5f);
                                val3 = Main.rand.NextFloat(-50f, 50f) * SizeVector.RotatedBy(Math.PI / 2f).SafeNormalize(Vector2.Zero);
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
                        particle3 = new BloomLineVFX(val2, AnchorStart + SizeVector - val2, 0.8f, val, 20, capped: true);
                        BootlegSpawnParticle(particle3);
                    }
                }
            }







            Vector2 val4 = Vector2.Zero;
            if (Timer > Projectile.oldPos.Length || AnchorType == 4f)
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
                    if (AnchorType == 5f && particle4.LifetimeCompletion > 0.5f && !death)
                    {
                        particle4.Time--;
                    }
                    if (death)
                    {
                        particle4.Color *= 0.9f;
                    }
                    if (AnchorType == 5f && particle4 is GenericSparkle && particle4.Scale is 2f or 2.01f)
                    {
                        particle4.Rotation += particle4.Scale == 2f ? 0.05f : -0.05f;
                    }
                }
            }
            Particles.RemoveAll((Particle particle) => particle.Time >= particle.Lifetime && particle.SetLifetime);
            if (!(AnchorType == 3f))
            {
                Projectile.Center = Owner.Center;
            }
            else
            {
                Projectile.Center = origPos;
            }
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
