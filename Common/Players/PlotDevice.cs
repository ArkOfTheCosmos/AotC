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
using static AotC.Content.ModdedUtils;
using Microsoft.Xna.Framework.Graphics;
using AotC.Content.Items.Weapons.Melee;
using AotC.Core.GlobalInstances.Systems;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;

namespace AotC.Common.Players
{
    internal class PlotDevice : ModPlayer
    {
        public bool celesteTrail;
        public int flash;
        public int UIFlash;
        public int celesteDashTimer;
        public int dashCount;
        public int killStars;
        public bool done = true;
        public float ArkDamage;
        public const float maxDistance = 50f;
        public float ArkThrowCooldown;
        public float celesteTrailDelay;
        public List<Vector2> SlashPoints;
        public Projectile blade;
        public Direction dashDirection;
        public bool isDashing;
        public int maxDashes;
        public bool Plimp;
        public bool PlimpFunny;
        public int PlimpShield;
        public int TimeSinceLastHit;

        public override void Load()
        {
            if (Main.dedServ)
                return;
            On_Main.DrawProjectiles += On_Main_DrawProjectiles;
            On_Main.DrawDust += On_Main_DrawDust;
        }

        public override void Unload()
        {
            if (Main.dedServ)
                return;
            On_Main.DrawProjectiles -= On_Main_DrawProjectiles;
            On_Main.DrawDust -= On_Main_DrawDust;   
        }
        public void StartSlash(int damage)
        {
            if (Vector2.Distance(Player.position, ArkoftheCosmos.SlashPoints[0]) <= 600f && Vector2.Distance(Player.position, ArkoftheCosmos.SlashPoints[0]) < Vector2.Distance(Player.position, ArkoftheCosmos.SlashPoints[1]))
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
                SoundEngine.PlaySound(AotCAudio.Dash);
                flash = 200;
                UIFlash = 200;
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
                    if (celesteDashTimer < 8 && dashCount < maxDashes)
                    {
                        UIFlash = 200;
                        dashCount = maxDashes;
                    }
                    celesteDashTimer = -1;
                }
                else if (celesteDashTimer == 0 && dashCount < maxDashes)
                {
                    UIFlash = 200;
                    dashCount = maxDashes;
                }
            }
            if (Player.grapCount > 0 && dashCount < maxDashes)
            {
                UIFlash = 200;
                dashCount = maxDashes;
            }
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
                    for (int i = 0; i < 4; i++)
                        Projectile.NewProjectileDirect(Player.GetSource_FromThis(), SlashPoints[0], (rand + (float)Math.PI * 0.5f * i).ToRotationVector2() * 20f, ModContent.ProjectileType<EonStar>(), (int)(ArkDamage * ArkoftheCosmos.DashStarMultiplier), 0f, Player.whoAmI, 0.65f, 0.15f).timeLeft = 100;
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
                    Vector2 desiredMovement = direction * 100f;
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
                    if (AotCSystem.CelesteTrailShader != null)
                        Main.dust[dustIndex].shader = AotCSystem.CelesteTrailShader;
                }
                //make afterimages seperate to the normal afterimage system
                if (celesteDashTimer % 3 == 0)
                {
                    GeneralParticleHandler.SpawnParticle(new CelesteAfterImage(Player));
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

            Plimp = false;
            PlimpFunny = true;
        }

        public override void ModifyHurt (ref Player.HurtModifiers modifiers)
        {
            if (Plimp && PlimpShield > 0)
                modifiers.ModifyHurtInfo += ModifyDamage;
        }
        public void ModifyDamage(ref Player.HurtInfo info)
        {
            if (PlimpShield > info.Damage)
            {
                CombatText.NewText(Player.Hitbox, Color.BlueViolet, info.Damage);
                PlimpShield -= info.Damage;
                info.Damage = 0;
            }
            else
            {
                CombatText.NewText(Player.Hitbox, Color.Red, PlimpShield);
                info.Damage -= PlimpShield;
                PlimpShield = 0;
            }
        }
        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            TimeSinceLastHit = 0;
        }
        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            TimeSinceLastHit = 0;
        }
        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Plimp && PlimpShield > 0)
            {
                Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.Center, new Vector2(), ModContent.ProjectileType<PlasmaShrimpMissile>(), (int)Math.Ceiling(hit.Damage / 2.5f), 0, -1, target.whoAmI, Main.rand.NextFloat(-20, 20));
                if (PlimpFunny)
                    SoundEngine.PlaySound(in AotCAudio.PlasmaShrimp, Player.position);
            }
        }
        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Plimp && proj.ModProjectile is not PlasmaShrimpMissile && PlimpShield > 0)
            {
                Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.Center, new Vector2(), ModContent.ProjectileType<PlasmaShrimpMissile>(), (int)Math.Ceiling(hit.Damage / 2.5f), 0, -1, target.whoAmI, Main.rand.NextFloat(-20,20));
                if (PlimpFunny)
                    SoundEngine.PlaySound(in AotCAudio.PlasmaShrimp, Player.position);
            }
        }
        public override void PostUpdateRunSpeeds()
        {
            if (!done)
                Player.gravity = 0f;
        }
        public override void UpdateBadLifeRegen()
        {
            TimeSinceLastHit++;
            ArkThrowCooldown--;

            if (Plimp && PlimpShield < Player.statLifeMax2 / 10f && TimeSinceLastHit > 480)
            {
                PlimpShield++;
                if (TimeSinceLastHit == 481)
                    SoundEngine.PlaySound(in AotCAudio.PlimpRecharge, Player.position);
            }

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
            if (celesteTrail && (GetSpeed(Player) >= 30f || Player.dashDelay == -1) && celesteTrailDelay <= 0f && celesteDashTimer == 0)
            {
                celesteTrailDelay = 8f;
                GeneralParticleHandler.SpawnParticle(new CelesteAfterImage(Player));
            }
        }
        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (flash > 0)
            {
                flash -= 10;
            }
            if (UIFlash > 0)
                UIFlash -= 10;
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
            }
            Main.spriteBatch.End();
        }
        private void On_Main_DrawDust(On_Main.orig_DrawDust orig, Main self)
        {
            orig(self);
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            foreach (Player player in Main.player)
            {
                if (!player.active)
                    continue;
                var p = player.GetPlot();
                if (p.flash > 0)
                {
                    Texture2D texture = SilhouettePool.Get();
                    if (AotCSystem.CelesteTrailShader != null)
                        AotCSystem.CelesteTrailShader.Apply(null, new(texture, Vector2.Zero, Color.White));
                    else
                    {
                        GameShaders.Misc["CelesteTrailShader"].UseOpacity(p.flash / 170f);
                        GameShaders.Misc["CelesteTrailShader"].Apply();
                    }
                    Main.spriteBatch.Draw(texture, PlayerTarget.GetPlayerTargetPosition(player.whoAmI), PlayerTarget.GetPlayerTargetSourceRectangle(player.whoAmI), Color.White * (p.flash / 255f), 0, new(), 1f, 0, 0f);
                    SilhouettePool.Release(texture);
                }
            }
            Main.spriteBatch.End();
        }
    }
}