using System;
using Terraria;
using System.IO;
using Terraria.ID;
using Terraria.Audio;
using ReLogic.Content;
using Terraria.ModLoader;
using AotC.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AotC.Content.Items.Weapons.Melee;

namespace AotC.Content.Projectiles
{
    internal class ArkoftheCosmosSwungBlade : ModProjectile
    {
        public float dir;

        public float[] rng = new float[5];

        private bool initialized;

        private Vector2 direction = Vector2.Zero;

        private Particle smear; //this is balls

        private readonly float SwingWidth = (float)Math.PI * 3f / 4f;

        public const float MaxThrowTime = 140f; //default is 140f

        public float ThrowReach;

        public const float SnapWindowStart = 0.2f;

        public const float SnapWindowEnd = 0.75f;

        public override string Texture => "AotC/Content/Items/Weapons/Melee/ArkoftheCosmos";

        public ref float SwingType => ref Projectile.ai[0];

        public ref float Charge => ref Projectile.ai[1];

        public Player Owner => Main.player[Projectile.owner];

        public float MaxSwingTime => SwingType == 5f ? 10 : SwirlSwing ? 55 : 35;

        public bool SwirlSwing => SwingType == 2f;

        public int SwingDirection

        {
            get
            {
                float i = SwingType;
                if (i == 5f)
                {
                    return 0;
                }
                if (i != 2f)
                {
                    if (i != 3f)
                    {
                        if (i == 1f)
                        {
                            return -1 * Math.Sign(direction.X);
                        }
                        return 1 * Math.Sign(direction.X);
                    }
                    return 0;
                }
                return -1 * Math.Sign(direction.X);
            }
        }
        public Vector2 DistanceFromPlayer => direction / 30f; //screw this variable

        public float SwingTimer => MaxSwingTime - Projectile.timeLeft;

        public float SwingCompletion => SwingTimer / MaxSwingTime;

        public ref float HasFired => ref Projectile.localAI[0];


        private bool OwnerCanShoot
        {
            get
            {
                if (Owner.channel && !Owner.noItems && !Owner.CCed)
                {
                    return Owner.HeldItem.type == ModContent.ItemType<ArkoftheCosmos>();
                }
                return false;
            }
        }

        public bool Thrown
        {
            get
            {
                if (SwingType == 4f)
                {
                    return true;
                }
                return false;
            }
        }

        public float ThrowTimer => MaxThrowTime - Projectile.timeLeft;

        public float ThrowCompletion => ThrowTimer / MaxThrowTime;

        public static float SnapEndTime => 35f;

        public float SnapEndCompletion => (SnapEndTime - Projectile.timeLeft) / SnapEndTime;

        public ref float ChanceMissed => ref Projectile.localAI[1];

        public bool LilliaCrit = false;



        // calamity utils stuff
        public ModdedUtils.CurveSegment anticipation = new ModdedUtils.CurveSegment(ModdedUtils.EasingType.ExpOut, 0f, 0f, 0.15f);

        public ModdedUtils.CurveSegment thrust = new ModdedUtils.CurveSegment(ModdedUtils.EasingType.PolyInOut, 0.1f, 0.15f, 0.85f, 3);

        public ModdedUtils.CurveSegment hold = new ModdedUtils.CurveSegment(ModdedUtils.EasingType.Linear, 0.5f, 1f, 0.2f);

        public ModdedUtils.CurveSegment startup = new ModdedUtils.CurveSegment(ModdedUtils.EasingType.SineIn, 0f, 0f, 0.25f);

        public ModdedUtils.CurveSegment swing = new ModdedUtils.CurveSegment(ModdedUtils.EasingType.SineOut, 0.1f, 0.25f, 0.75f);

        public ModdedUtils.CurveSegment shoot = new ModdedUtils.CurveSegment(ModdedUtils.EasingType.PolyIn, 0f, 1f, -0.2f, 3);

        public ModdedUtils.CurveSegment remain = new ModdedUtils.CurveSegment(ModdedUtils.EasingType.Linear, 0.2f, 0.8f, 0f);

        public ModdedUtils.CurveSegment retract = new ModdedUtils.CurveSegment(ModdedUtils.EasingType.SineIn, 0.75f, 1f, -1f);

        public ModdedUtils.CurveSegment sizeCurve = new ModdedUtils.CurveSegment(ModdedUtils.EasingType.SineBump, 0f, 0f, 1f);

        public ModdedUtils.CurveSegment fat = new ModdedUtils.CurveSegment(ModdedUtils.EasingType.PolyOut, 0f, 0.15f, 1f, 4);


        //more calamity utils stuff
        internal float SwingRatio()
        {
            return ModdedUtils.PiecewiseAnimation(SwingCompletion, new ModdedUtils.CurveSegment[3] { anticipation, thrust, hold });
        }

        internal float SwirlRatio()
        {
            return ModdedUtils.PiecewiseAnimation(SwingCompletion, new ModdedUtils.CurveSegment[2] { startup, swing });
        }

        internal float ThrowRatio()
        {
            return ModdedUtils.PiecewiseAnimation(ThrowCompletion, new ModdedUtils.CurveSegment[3] { shoot, remain, retract });
        }

        internal float ThrowScaleRatio()
        {
            return ModdedUtils.PiecewiseAnimation(ThrowCompletion, new ModdedUtils.CurveSegment[1] { sizeCurve });
        }

        //you made this
        internal float StabRatio()
        {
            return ModdedUtils.PiecewiseAnimation(SwingCompletion, new ModdedUtils.CurveSegment[2] { fat, retract });
        }













        //setting defaults
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }


        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = (Projectile.height = 60);
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = Thrown ? 0 : ((int)MaxSwingTime);
        }


        //hitbox crap i think
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float num = 172f * Projectile.scale;
            if (Thrown)
            {
                bool flag = Collision.CheckAABBvAABBCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - Vector2.One * num / 2f, Vector2.One * num); // original is bool flag = Collision.CheckAABBvAABBCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - Vector2.get_One() * num / 2f, Vector2.get_One() * num);
                if (SwingType == 4f)
                {
                    return flag;
                }
                Vector2 val = Vector2.SmoothStep(Owner.Center, Projectile.Center, MathHelper.Clamp(SnapEndCompletion + 0.25f, 0f, 1f));
                bool flag2 = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), val, val + direction * num);
                return flag || flag2;
            }
            float collisionPoint = 0f;
            Vector2 distanceFromPlayer = DistanceFromPlayer;
            Vector2 val2 = distanceFromPlayer.Length() * Projectile.rotation.ToRotationVector2();
            if (!SwirlSwing)
            {
                return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Owner.Center + val2, Owner.Center + val2 + Projectile.rotation.ToRotationVector2() * num, 24f, ref collisionPoint);
            }
            else
            {
                num = 290f;
                return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Owner.Center + val2, Owner.Center + val2 + Projectile.rotation.ToRotationVector2() * num, 24f, ref collisionPoint);
            }
        }


























        // this is where crap happens

        public override void AI()
        {
            if (!initialized)
            {
                Projectile.timeLeft = Thrown ? (int)MaxThrowTime : ((int)MaxSwingTime);
                rng[0] = Main.rand.Next(-2, 2);
                rng[1] = Main.rand.Next(-2, 2);
                rng[2] = Main.rand.Next(-1, 1);
                rng[3] = Main.rand.Next(-1, 1);
                rng[4] = Main.rand.Next(-1, 1);
                direction = Projectile.velocity;
                direction.Normalize();
                Projectile.velocity = direction;
                Projectile.rotation = direction.ToRotation();
                initialized = true;
                if (Owner.DirectionTo(Main.MouseWorld).ToRotation() > -(Math.PI / 2f) && Owner.DirectionTo(Main.MouseWorld).ToRotation() < (Math.PI / 2f))
                {
                    dir = 1f;
                }
                else
                {
                    dir = -1f;
                }

                if (SwirlSwing)
                {
                    Projectile.damage *= 2;
                    Projectile.localNPCHitCooldown = (int)(Projectile.localNPCHitCooldown / 4f);
                    SoundEngine.PlaySound(in Sounds.AotCAudio.BloomingBlows, Projectile.position);
                }
                else if (SwingType == 3f)
                {
                    Projectile.damage *= 3;
                    Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Owner.Center, Projectile.velocity, ModContent.ProjectileType<ArkoftheCosmosConstellation>(), (int)(Projectile.damage * ArkoftheCosmos.chainDamageMultiplier), 0f, Owner.whoAmI, (int)(Projectile.timeLeft / 2f), 3f).timeLeft = Charge >= 10f ? 100 : 35;
                }

                else if (SwingType is 0f or 1f)
                {
                    SoundEngine.PlaySound(in SoundID.DD2_MonkStaffSwing, Projectile.position);
                    Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Owner.Center, Projectile.velocity * 300, ModContent.ProjectileType<ArkoftheCosmosConstellation>(), (int)(Projectile.damage * ArkoftheCosmos.chainDamageMultiplier), 0f, Owner.whoAmI, (int)(Projectile.timeLeft / 2f), SwingType == 0f ? 1f : 2f).timeLeft = (int)(Projectile.timeLeft / 2f);
                }
                else if (SwingType != 5f)
                {
                    SoundEngine.PlaySound(in SoundID.Item84, Projectile.position);
                }
            }




            if (!Thrown)
            {
                if (SwingType != 3f)
                {
                    Projectile.Center = Owner.Center + DistanceFromPlayer;
                }
                if (!SwirlSwing)
                {
                    if (SwingType != 5f)
                    {
                        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Lerp(SwingWidth / 2f * SwingDirection, (0f - SwingWidth) / 2f * SwingDirection, SwingRatio());
                        if (SwingType != 3f)
                        {
                            if (Charge >= 10f)
                            {
                                if (Owner.whoAmI == Main.myPlayer && (Projectile.timeLeft == 23 + (int)rng[2] || Projectile.timeLeft == 21 + (int)rng[3] || Projectile.timeLeft == 19 + (int)rng[4]))
                                {
                                    float f = Projectile.rotation - (float)Math.PI * 23f / 80f * dir;
                                    if (SwingType == 1f)
                                    {
                                        Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Owner.Center + f.ToRotationVector2() * 150f, f.ToRotationVector2() * 20f, ModContent.ProjectileType<EonStar>(), (int)(ArkoftheCosmos.SwirlBoltDamageMultiplier / ArkoftheCosmos.SwirlBoltAmount * Projectile.damage), 0f, Owner.whoAmI, 0.65f, (float)Math.PI / 20f).timeLeft = 100;
                                    }
                                    else
                                    {
                                        f = Projectile.rotation - (float)Math.PI * 23f / -80f * dir;
                                        Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Owner.Center + f.ToRotationVector2() * 150f, f.ToRotationVector2() * 20f, ModContent.ProjectileType<EonStar>(), (int)(ArkoftheCosmos.SwirlBoltDamageMultiplier / ArkoftheCosmos.SwirlBoltAmount * Projectile.damage), 0f, Owner.whoAmI, 0.65f, (float)Math.PI / 20f).timeLeft = 100;
                                    }
                                }
                            }
                            else
                            {
                                if (Owner.whoAmI == Main.myPlayer && (Projectile.timeLeft == 23 + (int)rng[0] || Projectile.timeLeft == 19 + (int)rng[1]))
                                {
                                    float f = Projectile.rotation - (float)Math.PI * 23f / 80f * dir;
                                    if (SwingType == 1f)
                                    {
                                        Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Owner.Center + f.ToRotationVector2() * 150f, f.ToRotationVector2() * 20f, ModContent.ProjectileType<EonStar>(), (int)(ArkoftheCosmos.SwirlBoltDamageMultiplier / ArkoftheCosmos.SwirlBoltAmount * Projectile.damage), 0f, Owner.whoAmI, 0.65f, (float)Math.PI / 20f).timeLeft = 100;
                                    }
                                    else
                                    {
                                        f = Projectile.rotation - (float)Math.PI * 23f / -80f * dir;
                                        Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Owner.Center + f.ToRotationVector2() * 150f, f.ToRotationVector2() * 20f, ModContent.ProjectileType<EonStar>(), (int)(ArkoftheCosmos.SwirlBoltDamageMultiplier / ArkoftheCosmos.SwirlBoltAmount * Projectile.damage), 0f, Owner.whoAmI, 0.65f, (float)Math.PI / 20f).timeLeft = 100;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Projectile.rotation = Projectile.velocity.ToRotation();
                    }
                }
                else
                {
                    float num = (float)Math.PI * 3f / 4f * SwingDirection;
                    float num2 = -7.4612827f * SwingDirection;
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Lerp(num, num2, SwirlRatio());
                    if (Charge >= 10)
                    {
                        if (Owner.whoAmI == Main.myPlayer && (Projectile.timeLeft - 1) % Math.Ceiling((double)(MaxSwingTime / ArkoftheCosmos.SwirlBoltAmount * 0.5f)) == 0.0)
                        {
                            float f = Projectile.rotation - (float)Math.PI * 23f / 80f * dir;
                            Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Owner.Center + f.ToRotationVector2() * 10f, f.ToRotationVector2() * 20f, ModContent.ProjectileType<EonStar>(), (int)(ArkoftheCosmos.SwirlBoltDamageMultiplier / ArkoftheCosmos.SwirlBoltAmount * Projectile.damage), 0f, Owner.whoAmI, 0.55f, (float)Math.PI / 20f, 1f).timeLeft = 100;
                        }
                    }
                    else
                    {
                        if (Owner.whoAmI == Main.myPlayer && (Projectile.timeLeft - 1) % Math.Ceiling((double)(MaxSwingTime / ArkoftheCosmos.SwirlBoltAmount)) == 0.0)
                        {
                            float f = Projectile.rotation - (float)Math.PI * 23f / 80f * dir;
                            Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Owner.Center + f.ToRotationVector2() * 10f, f.ToRotationVector2() * 20f, ModContent.ProjectileType<EonStar>(), (int)(ArkoftheCosmos.SwirlBoltDamageMultiplier / ArkoftheCosmos.SwirlBoltAmount * Projectile.damage), 0f, Owner.whoAmI, 0.55f, (float)Math.PI / 20f, 1f).timeLeft = 100;
                        }
                    }
                }
                Projectile.scale = ((SwingType != 3f) ? 1.2f + (float)Math.Sin((double)(SwingRatio() * (float)Math.PI)) * 0.6f : 1.2f + (float)Math.Sin((double)(ThrowScaleRatio() * (float)Math.PI)) * 0.6f) * (SwingType == 5f ? 1.5f : 1f);
            }










            else
            {
                if (Owner.whoAmI == Main.myPlayer && (Projectile.timeLeft - 30) % Math.Ceiling((double)(MaxThrowTime / 3)) == 0.0)
                {
                    float f = Projectile.rotation - (float)Math.PI * 23f / 80f * Owner.direction;
                    float val2 = Main.rand.NextFloat(-(float)Math.PI / 2, (float)Math.PI / 2);
                    for (int i = 0; i < 8; i++)
                    {
                        Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center + f.ToRotationVector2() * 10f, (val2 + i * (float)Math.PI / 4f).ToRotationVector2() * 20f, ModContent.ProjectileType<EonStar>(), (int)(ArkoftheCosmos.SwirlBoltDamageMultiplier / ArkoftheCosmos.SwirlBoltAmount * Projectile.damage), 0f, Owner.whoAmI, 0.55f, (float)Math.PI / 20f, 1f).timeLeft = 100;
                        SoundEngine.PlaySound(in SoundID.Item9, Projectile.position);
                    }
                }
                if (Math.Abs(ThrowCompletion - 0.2f + 0.1f) <= 0.005f && ChanceMissed == 0f && Main.myPlayer == Owner.whoAmI)
                {
                    Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Owner.Center, Vector2.Zero, ModContent.ProjectileType<ArkoftheCosmosConstellation>(), (int)(Projectile.damage * ArkoftheCosmos.chainDamageMultiplier), 0f, Owner.whoAmI, (int)(Projectile.timeLeft / 2f), 0f).timeLeft = (int)(Projectile.timeLeft / 2f);
                }
                Projectile.Center = Projectile.Center.MoveTowards(Main.MouseWorld, 40f * ThrowRatio());
                Vector2 val = Projectile.Center - Owner.Center;
                if (val.Length() > ArkoftheCosmos.MaxThrowReach)
                {
                    Projectile.Center = Owner.Center + Owner.DirectionTo(Projectile.Center) * ArkoftheCosmos.MaxThrowReach;
                }
                Projectile.rotation -= 213f / 904f;
                Projectile.scale = 1f + ThrowScaleRatio();
                if (Math.Abs(ThrowCompletion - 0.75f) <= 0.005f)
                {
                    direction = Projectile.Center - Owner.Center;
                }
                if (ThrowCompletion > 0.75f)
                {
                    Projectile.Center = Owner.Center + direction * ThrowRatio();
                }
                else if (!OwnerCanShoot && SwingType == 2f && ChanceMissed == 0f)
                {
                    ChanceMissed = 1f;
                }
                DoParticleEffects(swirlSwing: false);
            }





















            Owner.direction = Math.Sign(Projectile.velocity.X);
            Owner.itemRotation = Projectile.rotation;
            if (Owner.direction != 1)
            {
                Owner.itemRotation -= (float)Math.PI;
            }
            Owner.itemRotation = MathHelper.WrapAngle(Owner.itemRotation);
        }












        public override bool PreDraw(ref Color lightColor)
        {
            if (Thrown)
            {
                DrawSingleThrownScissorBlade(lightColor);
                return false;
            }
            DrawSingleSwungScissorBlade(lightColor);
            return false;
        }









        public void DrawSingleSwungScissorBlade(Color lightColor)
        {

            Texture2D value = ModContent.Request<Texture2D>("AotC/Content/Items/Weapons/Melee/ArkoftheCosmos").Value;
            Texture2D value2 = ModContent.Request<Texture2D>("AotC/Content/Items/Weapons/Melee/ArkoftheCosmosGlow").Value;
            bool flag = Owner.direction < 0;
            SpriteEffects val = (SpriteEffects)(flag ? 1 : 0);
            float num = ((Owner.direction < 0) ? ((float)Math.PI / 2f) : 0f);
            float rotation = Projectile.rotation;
            float num2 = (float)Math.PI / 4f;
            float rotation2 = Projectile.rotation + num2 + num;
            Vector2 val2 = new (flag ? value.Width : 0f, value.Height);
            Vector2 val3 = (SwingType != 3f) ? Owner.Center + rotation.ToRotationVector2() * 10f - Main.screenPosition : Owner.Center + (rotation.ToRotationVector2() / 2f) * StabRatio() * 100f - Main.screenPosition;
            if (SwingTimer > ProjectileID.Sets.TrailCacheLength[Projectile.type] && (SwingType == 1f || SwingType == 0f))
            {
                for (int i = 1; i < Projectile.oldRot.Length; i++)
                {
                    Color val4 = Main.hslToRgb(i / (float)Projectile.oldRot.Length * 0.1f, 1f, 0.6f);
                    float num3 = Projectile.oldRot[i] + num2 + num;
                    Main.spriteBatch.Draw(value2, val3, null, val4 * 0.05f, num3, val2, Projectile.scale - 0.2f * (i / (float)Projectile.oldRot.Length), val, 0f);
                }
            }
            Main.EntitySpriteDraw(value, val3, null, lightColor, rotation2, val2, Projectile.scale * 1.2f, val, 0);
            Main.EntitySpriteDraw(value2, val3, null, Color.Lerp(lightColor, Color.White, 0.75f), rotation2, val2, Projectile.scale * 1.2f, val, 0);
            if (SwingCompletion > 0.5f && (SwingType == 1f || SwingType == 0f))
            {
                Texture2D value3 = ModContent.Request<Texture2D>("AotC/Assets/Textures/TrientCircularSmear", (AssetRequestMode)2).Value;
                Main.spriteBatch.End();
                Main.spriteBatch.Begin((SpriteSortMode)1, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                float num4 = (float)Math.Sin((double)(SwingCompletion * (float)Math.PI));
                float num5 = (-(float)Math.PI / 8f + (float)Math.PI / 8f * SwingCompletion + ((SwingType == 2f) ? ((float)Math.PI / 4f) : 0f)) * SwingDirection;
                Color val5 = Main.hslToRgb((SwingType == 0f) ? 0.15f : 0.3f, 1f, 0.6f);
                Main.EntitySpriteDraw(value3, Owner.Center - Main.screenPosition, null, val5 * 0.5f * num4, Projectile.velocity.ToRotation() + (float)Math.PI + num5, value3.Size() / 2f, Projectile.scale * 3.4f, 0, 0);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(0, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            }
            if (SwirlSwing)
            {
                Texture2D value3 = ModContent.Request<Texture2D>("AotC/Assets/Textures/OpaqueLilliaQ", (AssetRequestMode)2).Value;
                Texture2D value4 = ModContent.Request<Texture2D>("AotC/Assets/Textures/lillia_base_q_indicator", (AssetRequestMode)2).Value;
                Main.spriteBatch.End();
                Main.spriteBatch.Begin((SpriteSortMode)1, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                Color val5 = lightColor * (MathHelper.Clamp((float)Math.Sin((double)((SwirlRatio() - 0.1f) * (float)Math.PI)), 0f, 1f));
                float rotationVal = Projectile.rotation + (float)Math.PI / 4f + ((Owner.direction < 0) ? ((float)Math.PI / 2f) : 0f);
                if (Owner.direction < 0)
                {
                    value3 = ModdedUtils.FlipHorizontal(value3);
                }


                Main.EntitySpriteDraw(value3, Owner.Center - Main.screenPosition, null, val5, rotationVal, value3.Size() / 2f, 2.4f, 0, 0);
                Main.EntitySpriteDraw(value4, Owner.Center - Main.screenPosition, null, val5, direction.ToRotation(), value4.Size() / 2f, 1.3f, 0, 0); ;
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(0, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            }
        }

        public void DrawSingleThrownScissorBlade(Color lightColor)
        {
            Texture2D value = ModContent.Request<Texture2D>("AotC/Content/Items/Weapons/Melee/ArkoftheCosmos", (AssetRequestMode)2).Value;
            Texture2D value2 = ModContent.Request<Texture2D>("AotC/Content/Items/Weapons/Melee/ArkoftheCosmosGlow", (AssetRequestMode)2).Value;
            Vector2 center = Projectile.Center;
            float rotation2 = Projectile.rotation + (float)Math.PI / 4f;
            Vector2 origin2 = new(32f, 86f);
            Main.EntitySpriteDraw(value, center - Main.screenPosition, null, lightColor, rotation2, origin2, base.Projectile.scale, 0, 0);
            Main.EntitySpriteDraw(value2, center - Main.screenPosition, null, Color.Lerp(lightColor, Color.White, 0.75f), rotation2, origin2, base.Projectile.scale, 0, 0);







            Texture2D value3 = ModContent.Request<Texture2D>("AotC/Assets/Textures/CircularSmearSmokey", (AssetRequestMode)2).Value;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin((SpriteSortMode)1, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            Color val5 = ModdedUtils.HsvToRgb(Main.GlobalTimeWrappedHourly, 1f, 1f) * (MathHelper.Clamp((float)Math.Sin((double)((ThrowRatio() - 0.2f) * (float)Math.PI)), 0f, 1f) * 0.7f);

            Main.EntitySpriteDraw(value3, Projectile.Center - Main.screenPosition, null, val5, Projectile.rotation - (float)Math.PI * 7f / 8f, value3.Size() / 2f, Projectile.scale * 1.5f, 0, 0);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(0, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

        }














        //holy balls this code gives me hemorrhoids
        public void DoParticleEffects(bool swirlSwing)
        {

            if (swirlSwing)
            {
                Projectile.scale = 1.6f + (float)Math.Sin((double)(SwirlRatio() * (float)Math.PI)) * 1f;
                Color val = Color.Purple * (MathHelper.Clamp((float)Math.Sin((double)((SwirlRatio() - 0.2f) * (float)Math.PI)), 0f, 1f) * 0.8f);
                if (smear == null)
                {
                    smear = new CircularSmearSmokeyVFX(Owner.Center, val, Projectile.rotation, Projectile.scale * 3.4f);
                    GeneralParticleHandler.SpawnParticle(smear);
                }
                else
                {
                    smear.Rotation = Projectile.rotation + (float)Math.PI / 4f + ((Owner.direction < 0) ? ((float)Math.PI) : 0f);
                    smear.Time = 0;
                    smear.Position = Owner.Center;
                    smear.Scale = MathHelper.Lerp(2.6f, 3.5f, (Projectile.scale - 1.6f) / 1f);
                    smear.Color = val;
                }
                return;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (LilliaCrit == false && SwirlSwing)
            {
                SoundEngine.PlaySound(in Sounds.AotCAudio.BloomingBlowsCrit, Projectile.position);
                LilliaCrit = true;
            }
            if (Thrown || SwirlSwing)
            {
                if (Owner.HeldItem.ModItem is ArkoftheCosmos arkoftheCosmos)
                {
                    arkoftheCosmos.charge += 10f;
                    if (arkoftheCosmos.charge > 100f)
                    {
                        arkoftheCosmos.charge = 100f;
                    }
                }
            }
        }








        public override void SendExtraAI(BinaryWriter writer)
        {
            //IL_000e: Unknown result type (might be due to invalid IL or missing references)
            writer.Write(initialized);
            writer.WriteVector2(direction);
            writer.Write(ChanceMissed);
            writer.Write(ThrowReach);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //IL_000e: Unknown result type (might be due to invalid IL or missing references)
            //IL_0013: Unknown result type (might be due to invalid IL or missing references)
            initialized = reader.ReadBoolean();
            direction = reader.ReadVector2();
            ChanceMissed = reader.ReadSingle();
            ThrowReach = reader.ReadSingle();
        }
        /* public override void PostDraw(Color lightColor)
         {
             float num = 290f;
             Vector2 distanceFromPlayer = DistanceFromPlayer;
             Vector2 val2 = distanceFromPlayer.Length() * Projectile.rotation.ToRotationVector2();
             CalamityUtils.DrawLine(Owner.Center + val2 - Main.screenPosition, (Owner.Center + val2 + Projectile.rotation.ToRotationVector2() * num) - Main.screenPosition, 24f, Color.Red);
         }*/
    }
}

