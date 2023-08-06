using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.ID;
using AotC.Content.StolenCalamityCode;
using Terraria.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using AotC.Content.Items.Weapons;
using System.IO;
using Terraria.DataStructures;

namespace AotC.Content
{
    internal class Timers : ModPlayer
    {
        public float ArkThrowCooldown;
        public bool done;
        public float maxDistance = 50f; // Maximum distance to consider the player "close enough"
        public float moveSpeed = 100f; // Movement speed // this is normally 100f
        public List<Vector2> SlashPoints;
        public Projectile blade;
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
                SoundEngine.PlaySound(in Sounds.AotCAudio.MeatySlash, Player.position);
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

                // Get the current target point
                Vector2 currentTargetPoint = SlashPoints[0];

                // Get the direction the player is traveling
                Vector2 direction = Vector2.Normalize(currentTargetPoint - Player.position);

                // Calculate the distance to the current target point
                float distance = Vector2.Distance(Player.position, currentTargetPoint);

                if (distance <= maxDistance)
                {
                    ArkoftheCosmos arkoftheCosmos = Player.HeldItem.ModItem as ArkoftheCosmos;
                    // Player has reached the current target point, move to the next point
                    SoundEngine.PlaySound(in Sounds.AotCAudio.MeatySlash, Player.position);
                    float rand = Main.rand.NextFloat() * (float)Math.PI / 2f;
                    Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.Center, rand.ToRotationVector2() * 20f, ModContent.ProjectileType<EonBolt>(), 5555, 0f, Player.whoAmI, 0.65f, (float)Math.PI / 2f).timeLeft = 100;
                    Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.Center, (rand + (float)Math.PI / 2f).ToRotationVector2() * 20f, ModContent.ProjectileType<EonBolt>(), 5555, 0f, Player.whoAmI, 0.65f, (float)Math.PI / 2f).timeLeft = 100;
                    Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.Center, (rand + (float)Math.PI).ToRotationVector2() * 20f, ModContent.ProjectileType<EonBolt>(), 5555, 0f, Player.whoAmI, 0.65f, (float)Math.PI / 2f).timeLeft = 100;
                    Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.Center, (rand + (float)Math.PI * 1.5f).ToRotationVector2() * 20f, ModContent.ProjectileType<EonBolt>(), 5555, 0f, Player.whoAmI, 0.65f, (float)Math.PI / 2f).timeLeft = 100;
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
                            {
                                modProjectile.death = true;
                            }
                            arkoftheCosmos.SlashLines.RemoveAt(0);
                            if (arkoftheCosmos.SlashLines[0].ModProjectile is ArkoftheCosmosConstellation modProjectile2)
                            {
                                modProjectile2.death = true;
                            }
                            arkoftheCosmos.SlashLines.RemoveAt(0);
                        }
                    }
                }
                else
                {
                    // Calculate the desired movement vector
                    Vector2 desiredMovement = direction * moveSpeed;

                    // Move the player towards the desired movement vector
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
                        {
                            blade.timeLeft = 5;
                        }
                    }
                }
            }
        }


       






        public override void UpdateBadLifeRegen()
        {
            ArkThrowCooldown--;
            if (ArkThrowCooldown == 0)
            {
                SoundStyle style = SoundID.Item35;
                style.Volume = SoundID.Item35.Volume * 2f;
                SoundEngine.PlaySound(in Sounds.AotCAudio.Bell);
            }
        }
        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            //this is to draw the cooldown

            Player owner = Main.player[Player.whoAmI];
            if (ArkThrowCooldown >= 0)
            {
                float Timer = 340 - ArkThrowCooldown;
                float ParryTime = 0;

                Texture2D value = ModContent.Request<Texture2D>("AotC/Assets/Textures/GenericBarBack", (AssetRequestMode)2).Value;
                Texture2D value2 = ModContent.Request<Texture2D>("AotC/Assets/Textures/GenericBarFront", (AssetRequestMode)2).Value;
                Vector2 val = owner.Center - Main.screenPosition + new Vector2(0f, 36f) - value.Size() / 2f;
                Rectangle value3 = new(0, 0, (int)((Timer - ParryTime) / (340f - ParryTime) * (float)value2.Width), value2.Height);
                float num = ((Timer <= ParryTime + 25f) ? ((Timer - ParryTime) / 25f) : ((340f - Timer <= 8f) ? (ArkThrowCooldown / 8f) : 1f));
                Color val2 = CalamityUtils.HsvToRgb(Main.GlobalTimeWrappedHourly, 1f, 1f);
                Main.spriteBatch.Draw(value, val, val2 * num);
                Main.spriteBatch.Draw(value2, val, (Rectangle?)value3, val2 * num * 0.8f);
            }
        }
    }
}
