﻿using System;
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

namespace AotC.Common.Players
{
    internal class PlotDevice : ModPlayer
    {
        public bool done;
        public bool celesteTrail;
        public float moveSpeed = 100f;
        public float maxDistance = 50f;
        public float ArkThrowCooldown;
        public float celesteTrailDelay = 0f;
        public Projectile blade;
        public Texture2D playerTexture;
        public List<Vector2> SlashPoints;
        public List<Particle> Afterimages = new();

        public override void Load()
        {
            On_Main.DrawProjectiles += On_Main_DrawProjectiles;
        }

        private void On_Main_DrawProjectiles(On_Main.orig_DrawProjectiles orig, Main self)
        { 
            orig(self);
            Texture2D value = ModContent.Request<Texture2D>("AotC/Assets/Textures/GenericBarBack", AssetRequestMode.ImmediateLoad).Value;
            Texture2D value2 = ModContent.Request<Texture2D>("AotC/Assets/Textures/GenericBarFront", AssetRequestMode.ImmediateLoad).Value;
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);    
            Color val2 = ModdedUtils.HsvToRgb(Main.GlobalTimeWrappedHourly, 1f, 1f);
            float ParryTime = 0;
            foreach (Player player in Main.player)
            {
                if (!player.active)
                {
                    continue;
                }
                var p = player.Glitchtale();
                //this is to draw the cooldown
                if (p.ArkThrowCooldown >= 0)
                {
                    float Timer = 340 - p.ArkThrowCooldown;
                    Vector2 val = p.Player.Center - Main.screenPosition + new Vector2(0f, 36f) - value.Size() / 2f;
                    Rectangle value3 = new(0, 0, (int)((Timer - ParryTime) / (340f - ParryTime) * value2.Width), value2.Height);
                    float num = (Timer <= ParryTime + 25f) ? ((Timer - ParryTime) / 25f) : ((340f - Timer <= 8f) ? (p.ArkThrowCooldown / 8f) : 1f);
                    Main.spriteBatch.Draw(value, val, val2 * num);
                    Main.spriteBatch.Draw(value2, val, (Rectangle?)value3, val2 * num * 0.8f);
                }
                foreach (Particle particle in p.Afterimages)
                    particle?.CustomDraw(Main.spriteBatch);
                p.Afterimages.RemoveAll((Particle particle) => particle.Time >= particle.Lifetime || !p.celesteTrail);
            }
            Main.spriteBatch.End();
        }

        public void BootlegSpawnParticle(Particle particle)
        {
            if (!Main.dedServ)
            {
                Afterimages.Add(particle);
                particle.Type = GeneralParticleHandler.particleTypes[particle.GetType()];
            }
        }
        public void StartSlash()
        {
            done = false;
            ArkoftheCosmos arkoftheCosmos = Player.HeldItem.ModItem as ArkoftheCosmos;
            if (Vector2.Distance(Player.position, arkoftheCosmos.SlashPoints[0]) <= 500f)
            {
                blade = null;
                if (arkoftheCosmos != null)
                {
                    SlashPoints = arkoftheCosmos.SlashPoints;
                    Player.immune = true;
                    Player.immuneTime = 3600;
                }
                SoundEngine.PlaySound(in AotCAudio.MeatySlash, Player.position);
            }
            else
            {
                SoundEngine.PlaySound(in SoundID.Run);
            }
        }


        public override void PreUpdate()
        {
            if (SlashPoints != null && SlashPoints.Count > 0)
            {
                Vector2 currentTargetPoint = SlashPoints[0];

                Vector2 direction = Vector2.Normalize(currentTargetPoint - Player.position);

                float distance = Vector2.Distance(Player.position, currentTargetPoint);

                if (distance <= maxDistance)
                {
                    ArkoftheCosmos arkoftheCosmos = Player.HeldItem.ModItem as ArkoftheCosmos;
                    SoundEngine.PlaySound(in AotCAudio.MeatySlash, Player.position);
                    float rand = Main.rand.NextFloat() * (float)Math.PI / 2f;
                    Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.Center, rand.ToRotationVector2() * 20f, ModContent.ProjectileType<EonStar>(), 5555, 0f, Player.whoAmI, 0.65f, (float)Math.PI / 2f).timeLeft = 100;
                    Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.Center, (rand + (float)Math.PI / 2f).ToRotationVector2() * 20f, ModContent.ProjectileType<EonStar>(), 5555, 0f, Player.whoAmI, 0.65f, (float)Math.PI / 2f).timeLeft = 100;
                    Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.Center, (rand + (float)Math.PI).ToRotationVector2() * 20f, ModContent.ProjectileType<EonStar>(), 5555, 0f, Player.whoAmI, 0.65f, (float)Math.PI / 2f).timeLeft = 100;
                    Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.Center, (rand + (float)Math.PI * 1.5f).ToRotationVector2() * 20f, ModContent.ProjectileType<EonStar>(), 5555, 0f, Player.whoAmI, 0.65f, (float)Math.PI / 2f).timeLeft = 100;
                    SlashPoints.RemoveAt(0);
                    if (SlashPoints.Count == 0)
                    {
                        if (!done)
                        {
                            Player.immuneTime = 30;
                            done = true;
                        }
                        arkoftheCosmos.SlashPoints.Clear();
                        SlashPoints = null;
                    }
                    if (arkoftheCosmos != null)
                    {
                        if (SlashPoints != null)
                        {
                            if (arkoftheCosmos.SlashLines.Count - 1 > SlashPoints.Count)
                            {
                                if (arkoftheCosmos.SlashLines[0].ModProjectile is ArkoftheCosmosConstellation modProjectile)
                                {
                                    modProjectile.death = true;
                                }
                                arkoftheCosmos.SlashLines.RemoveAt(0);
                            }
                        }
                        else
                        {
                            blade = null;
                            if (arkoftheCosmos.SlashLines[0].ModProjectile is ArkoftheCosmosConstellation modProjectile)
                                modProjectile.death = true;
                            arkoftheCosmos.SlashLines.RemoveAt(0);
                            if (arkoftheCosmos.SlashLines[0].ModProjectile is ArkoftheCosmosConstellation modProjectile2)
                                modProjectile2.death = true;
                            arkoftheCosmos.SlashLines.RemoveAt(0);
                        }
                    }
                }
                else
                {
                    Vector2 desiredMovement = direction * moveSpeed;
                    Player.position += desiredMovement;
                }
                if (blade == null && SlashPoints != null)
                {
                    blade = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.position, direction, ModContent.ProjectileType<ArkoftheCosmosSwungBlade>(), 26690, 10f, Player.whoAmI, 5f, 0f);
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

        public override void ResetEffects()
        {
            celesteTrail = false;
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

            // trans rights
            celesteTrailDelay--;
            if (celesteTrail)
            {
                if (ModdedUtils.GetSpeed(Player) >= 30f && celesteTrailDelay <= 0f)
                {
                    RenderTarget2D renderTarget = PlayerTarget.Target;
                    celesteTrailDelay = 10;
                    if (PlayerTarget.canUseTarget)
                    {
                        Color[] data = new Color[renderTarget.Width * renderTarget.Height];
                        renderTarget.GetData(data);

                        playerTexture = new Texture2D(Main.graphics.GraphicsDevice, renderTarget.Width, renderTarget.Height);
                        playerTexture.SetData(data);
                        playerTexture = ModdedUtils.MakeSilhouette(playerTexture, 50, Color.White);
                    }
                    Particle particle1 = new CelesteAfterImage(PlayerTarget.GetPlayerTargetPosition(Player.whoAmI) + Main.screenPosition, playerTexture, PlayerTarget.GetPlayerTargetSourceRectangle(Player.whoAmI));
                    BootlegSpawnParticle(particle1);
                }
                foreach (Particle particle in Afterimages)
                    particle?.Update();
            }
        }
    }
}