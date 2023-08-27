using System;
using Terraria;
using Terraria.ID;
using AotC.Content;
using Terraria.Audio;
using ReLogic.Content;
using Terraria.ModLoader;
using AotC.Content.Sounds;
using AotC.Content.Particles;
using AotC.Content.Projectiles;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using AotC.Content.CustomHooks;
using Microsoft.Xna.Framework.Graphics;
using AotC.Content.Items.Weapons.Melee;
using AotC.Core.GlobalInstances.Systems;
using static AotC.Content.ModdedUtils;
using Terraria.DataStructures;

namespace AotC.Common.Players
{
    internal class PlotDevice : ModPlayer
    {
        public bool celesteTrail;
        public int flash = 0;
        public int celesteDashTimer;
        public int dashCount;
        public int killStars;
        public bool done = true;
        public float ArkDamage;
        public float moveSpeed = 100f;
        public float maxDistance = 50f;
        public float ArkThrowCooldown;
        public float celesteTrailDelay = 0f;
        public List<Particle> Afterimages = new();
        public List<Vector2> SlashPoints;
        public Texture2D playerTexture;
        public Projectile blade;
        public Direction dashDirection;
        public bool isDashing;
        public int maxDashes = 0;

        public override void Load()
        {
            On_Main.DrawProjectiles += On_Main_DrawProjectiles;
        }


        public void BootlegSpawnParticle(Particle particle)
        {
            if (!Main.dedServ)
            {
                Afterimages.Add(particle);
                particle.Type = GeneralParticleHandler.particleTypes[particle.GetType()];
            }
        }
        public void StartSlash(int damage)
        {
            if (Vector2.Distance(Player.position, ArkoftheCosmos.SlashPoints[0]) <= 600f)
            {
                done = false;
                ArkDamage = damage;
                blade = null;
                SlashPoints = ArkoftheCosmos.SlashPoints;
                Player.immune = true;
                Player.immuneTime = 300;
                SoundEngine.PlaySound(in AotCAudio.MeatySlash, Player.position);
            }
            else if (Vector2.Distance(Player.position, ArkoftheCosmos.SlashPoints[^1]) <= 600f)
            {
                done = false;
                ArkDamage = damage;
                blade = null;
                ArkoftheCosmos.SlashPoints.Reverse();
                SlashPoints = ArkoftheCosmos.SlashPoints;
                Player.immune = true;
                Player.immuneTime = 300;
                SoundEngine.PlaySound(in AotCAudio.MeatySlash, Player.position);
            }
            else
                SoundEngine.PlaySound(in SoundID.Run);
        }


        public override void PreUpdate()
        {
            isDashing = celesteDashTimer > 0;
            if (AotCSystem.CelesteDash.JustPressed && celesteDashTimer == 0 && dashCount > 0)
            {
                flash = 200;
                dashCount--;
                Player.dashDelay = 30;
                celesteDashTimer = 15;
                dashDirection = GetDirection(Player);
                if (dashDirection == Direction.None)
                    dashDirection = Player.direction == 1 ? Direction.Right : Direction.Left;
            }
            if (Collision.SolidCollision(Player.BottomLeft, Player.width, 6, true))
            {
                if (Player.controlJump && celesteDashTimer > 0)
                {
                    if (celesteDashTimer < 8)
                        dashCount = maxDashes;
                    celesteDashTimer = -1;
                }
                else if (celesteDashTimer == 0)
                    dashCount = maxDashes;
            }
            if (Player.grapCount > 0)
                dashCount = maxDashes;
            if (celesteDashTimer == -2)
                celesteDashTimer = 0;
            if (SlashPoints != null && SlashPoints.Count > 0)
            {
                if (Player.itemAnimation == 0)
                    Player.itemAnimation++;
                Vector2 currentTargetPoint = SlashPoints[0];
                Vector2 direction = Vector2.Normalize(currentTargetPoint - Player.position);
                float distance = Vector2.Distance(Player.position, currentTargetPoint);
                if (distance <= maxDistance || Player.immuneTime <= 0)
                {
                    SoundEngine.PlaySound(in AotCAudio.MeatySlash, Player.position);
                    float rand = Main.rand.NextFloat() * (float)Math.PI / 2f;
                    Projectile.NewProjectileDirect(Player.GetSource_FromThis(), SlashPoints[0], rand.ToRotationVector2() * 20f, ModContent.ProjectileType<EonStar>(), (int)(ArkDamage * ArkoftheCosmos.DashStarMultiplier), 0f, Player.whoAmI, 0.65f, 0.15f).timeLeft = 100;
                    Projectile.NewProjectileDirect(Player.GetSource_FromThis(), SlashPoints[0], (rand + (float)Math.PI / 2f).ToRotationVector2() * 20f, ModContent.ProjectileType<EonStar>(), (int)(ArkDamage * ArkoftheCosmos.DashStarMultiplier), 0f, Player.whoAmI, 0.65f, 0.15f).timeLeft = 100;
                    Projectile.NewProjectileDirect(Player.GetSource_FromThis(), SlashPoints[0], (rand + (float)Math.PI).ToRotationVector2() * 20f, ModContent.ProjectileType<EonStar>(), (int)(ArkDamage * ArkoftheCosmos.DashStarMultiplier), 0f, Player.whoAmI, 0.65f, 0.15f).timeLeft = 100;
                    Projectile.NewProjectileDirect(Player.GetSource_FromThis(), SlashPoints[0], (rand + (float)Math.PI * 1.5f).ToRotationVector2() * 20f, ModContent.ProjectileType<EonStar>(), (int)(ArkDamage * ArkoftheCosmos.DashStarMultiplier), 0f, Player.whoAmI, 0.65f, 0.15f).timeLeft = 100;
                    SlashPoints.RemoveAt(0);
                    if (SlashPoints.Count == 0)
                    {
                        if (!done)
                        {
                            Player.immuneTime = 30;
                            done = true;
                        }
                        ArkoftheCosmos.SlashPoints.Clear();
                        SlashPoints = null;
                    }
                        if (SlashPoints != null)
                        {
                            if (ArkoftheCosmos.SlashLines.Count - 1 > SlashPoints.Count)
                            {
                                if (ArkoftheCosmos.SlashLines[0].ModProjectile is Constellation modProjectile)
                                    modProjectile.death = true;
                                ArkoftheCosmos.SlashLines.RemoveAt(0);
                            }
                        }
                        else
                        {
                            blade = null;
                            if (ArkoftheCosmos.SlashLines[0].ModProjectile is Constellation modProjectile)
                                modProjectile.death = true;
                            ArkoftheCosmos.SlashLines.RemoveAt(0);
                            if (ArkoftheCosmos.SlashLines[0].ModProjectile is Constellation modProjectile2)
                                modProjectile2.death = true;
                            ArkoftheCosmos.SlashLines.RemoveAt(0);
                        }
                }
                else
                {
                    Vector2 desiredMovement = direction * moveSpeed;
                    Player.position += desiredMovement;
                }
                if (blade == null && SlashPoints != null)
                {
                    blade = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.position, direction, ModContent.ProjectileType<ArkoftheCosmosSwungBlade>(), (int)(ArkDamage * ArkoftheCosmos.DashMultiplier), 10f, Player.whoAmI, 5f, 0f);
                    blade.timeLeft = 10;
                }
                else
                {
                    if (blade != null)
                    {
                        blade.velocity = direction;
                        if (blade.timeLeft < 5)
                            blade.timeLeft = 5;
                    }
                }
            }
        }

        public override void PreUpdateMovement()
        {
            if (celesteDashTimer > 0)
            {
                //add a bit of dust effects
                Vector2 random;
                int dustIndex;
                for (int i = 0; i < 2; i++)
                {
                    random = Vector2.UnitX.RotatedByRandom(2 * Math.PI);
                    dustIndex = Dust.NewDust(Player.position, 10, 10, DustID.FireworksRGB, 1, 1, 150, HsvToRgb(0.5f, 0.7f, 1), 1f);
                    Main.dust[dustIndex].velocity = random * 3;
                    Main.dust[dustIndex].noGravity = true;
                    Main.dust[dustIndex].position = Player.Center + random * 12f;
                    if (PlayerTarget.CelesteTrailShader != null)
                        Main.dust[dustIndex].shader = PlayerTarget.CelesteTrailShader;
                }
                //make afterimages seperate to the normal afterimage system
                if (celesteDashTimer % 3 == 0)
                {
                    RenderTarget2D renderTarget = PlayerTarget.Target;
                    if (PlayerTarget.canUseTarget)
                    {
                        Color[] data = new Color[renderTarget.Width * renderTarget.Height];
                        renderTarget.GetData(data);

                        playerTexture = new Texture2D(Main.graphics.GraphicsDevice, renderTarget.Width, renderTarget.Height);
                        playerTexture.SetData(data);
                        playerTexture = MakeSilhouette(playerTexture, 50, Color.White);
                    }
                    Particle particle1 = new CelesteAfterImage(PlayerTarget.GetPlayerTargetPosition(Player.whoAmI) + Main.screenPosition, playerTexture, PlayerTarget.GetPlayerTargetSourceRectangle(Player.whoAmI));
                    BootlegSpawnParticle(particle1);
                }
                switch (dashDirection)
                {
                    case Direction.Left:
                        if (Player.velocity.X > -18)
                            Player.velocity = new(-18, 0.01f);
                        else
                            Player.velocity = new(Player.velocity.X, 0.01f);
                        break;
                    case Direction.Right:
                        if (Player.velocity.X < 18)
                            Player.velocity = new(18, 0.01f);
                        else
                            Player.velocity = new(Player.velocity.X, 0.01f);
                        break;
                    case Direction.Up:
                        Player.velocity = new(0, -18);
                        break;                    
                    case Direction.Down:
                        Player.velocity = new(0, 18);
                        break;
                    case Direction.UpRight:
                        if (Player.velocity.X < 12)
                            Player.velocity = new(12, -12);
                        else
                            Player.velocity = new(Player.velocity.X, -12);
                        break;
                    case Direction.DownRight:
                        if (Player.velocity.X < 12)
                            Player.velocity = new(12, 12);
                        else
                            Player.velocity = new(Player.velocity.X, 12);
                        break;
                    case Direction.DownLeft:
                        if (Player.velocity.X > -12)
                            Player.velocity = new(-12, 12);
                        else
                            Player.velocity = new(Player.velocity.X, 12);
                        break;
                    case Direction.UpLeft:
                        if (Player.velocity.X > -12)
                            Player.velocity = new(-12, -12);
                        else
                            Player.velocity = new(Player.velocity.X, -12);
                        break;
                }
                celesteDashTimer--;
                if (celesteDashTimer == 0)
                {
                    celesteTrailDelay = 3;
                    switch (dashDirection)
                    {
                        case Direction.Left:
                            Player.velocity = new(-12, 0.01f);
                            break;
                        case Direction.Right:
                            Player.velocity = new(12, 0.01f);
                            break;
                        case Direction.Up:
                            Player.velocity = new(0, -12);
                            break;
                        case Direction.UpRight:
                            Player.velocity = new(12, -12);
                            break;
                        case Direction.UpLeft:
                            Player.velocity = new(-12, -12);
                            break;
                    }
                }
            }
            //if super/hyper/wave
            else if (celesteDashTimer == -1)
            {
                int direction;
                if (dashDirection is Direction.Left or Direction.DownLeft)
                    direction = Player.controlRight ? 1 : -1;
                else if (dashDirection is Direction.Right or Direction.DownRight)
                    direction = Player.controlLeft ? -1 : 1;
                else
                    direction = Player.direction;
                if (dashDirection is Direction.Left or Direction.Right)
                    Player.velocity = new Vector2(direction * 15, -10);

                else if (dashDirection is Direction.DownLeft or Direction.Down or Direction.DownRight)
                    Player.velocity = new Vector2(direction * 20, -5);
                
                celesteDashTimer = -2;
            }
        }

        public override void ResetEffects()
        {
            celesteTrail = false;
            maxDashes = 0;
        }

        public override void PostUpdateRunSpeeds()
        {
            if (!done)
                Player.gravity = 0f;
        }
        public override void UpdateBadLifeRegen()
        {
            ArkThrowCooldown--;
            if (ArkThrowCooldown == 0)
            {
                SoundStyle style = SoundID.Item35;
                style.Volume = SoundID.Item35.Volume * 2f;
                SoundEngine.PlaySound(in AotCAudio.Bell);
            }
            if (Player.controlDown && Player.controlUp)
                killStars++;
            else
                killStars = 0;
            if (killStars > 60)
            {
                foreach (Projectile projectile in ArkoftheCosmos.SlashLines)
                    if (projectile.ModProjectile is Constellation asd)
                        asd.death = true;
                ArkoftheCosmos.SlashLines.Clear();
                ArkoftheCosmos.SlashPoints.Clear();
            }
            // trans rights
            celesteTrailDelay--;
            if (celesteTrail)
            {
                if ((GetSpeed(Player) >= 30f || Player.dashDelay == -1) && celesteTrailDelay <= 0f && celesteDashTimer == 0)
                {
                    RenderTarget2D renderTarget = PlayerTarget.Target;
                    celesteTrailDelay = 8f;
                    if (PlayerTarget.canUseTarget)
                    {
                        Color[] data = new Color[renderTarget.Width * renderTarget.Height];
                        renderTarget.GetData(data);
                        playerTexture = new Texture2D(Main.graphics.GraphicsDevice, renderTarget.Width, renderTarget.Height);
                        playerTexture.SetData(data);
                        playerTexture = MakeSilhouette(playerTexture, 50, Color.White);
                    }
                    Particle particle1 = new CelesteAfterImage(PlayerTarget.GetPlayerTargetPosition(Player.whoAmI) + Main.screenPosition, playerTexture, PlayerTarget.GetPlayerTargetSourceRectangle(Player.whoAmI));
                    BootlegSpawnParticle(particle1);
                }
            }
            foreach (Particle particle in Afterimages)
                particle?.Update();
        }

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (flash > 0)
            {
                a = flash;
                flash -= 10;
            }
        }
        private void On_Main_DrawProjectiles(On_Main.orig_DrawProjectiles orig, Main self)
        { 
            orig(self);
            Texture2D value = ModContent.Request<Texture2D>("AotC/Assets/Textures/GenericBarBack", AssetRequestMode.ImmediateLoad).Value;
            Texture2D value2 = ModContent.Request<Texture2D>("AotC/Assets/Textures/GenericBarFront", AssetRequestMode.ImmediateLoad).Value;
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);    
            Color val2 = HsvToRgb(Main.GlobalTimeWrappedHourly, 1f, 1f);
            float ParryTime = 0;
            foreach (Player player in Main.player)
            {
                if (!player.active)
                    continue;
                var p = player.GetPlot();
                //this is to draw the cooldown
                if (p.ArkThrowCooldown >= 0)
                {
                    float Timer = 340 - p.ArkThrowCooldown; //normally 340
                    Vector2 val = p.Player.Center - Main.screenPosition + new Vector2(0f, 36f) - value.Size() / 2f;
                    Rectangle value3 = new(0, 0, (int)((Timer - ParryTime) / (340f - ParryTime) * value2.Width), value2.Height);
                    float num = (Timer <= ParryTime + 25f) ? ((Timer - ParryTime) / 25f) : ((340f - Timer <= 8f) ? (p.ArkThrowCooldown / 8f) : 1f);
                    Main.spriteBatch.Draw(value, val, val2 * num);
                    Main.spriteBatch.Draw(value2, val, (Rectangle?)value3, val2 * num * 0.8f);
                }
                foreach (Particle particle in p.Afterimages)
                    particle?.CustomDraw(Main.spriteBatch);
                p.Afterimages.RemoveAll((Particle particle) => particle.Time >= particle.Lifetime);
            }
            Main.spriteBatch.End();
        }
    }
}
