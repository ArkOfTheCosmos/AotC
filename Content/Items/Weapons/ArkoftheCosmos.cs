using AotC.Content.StolenCalamityCode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;

namespace AotC.Content.Items.Weapons
{
    internal class ArkoftheCosmos : ModItem
    {
        Random rand = new();
        public int rnd;
        public float combo = 69; // dont change to "public float combo;" i think it broke the first swings or sth
        public float charge;
        public bool stab;
        public static float MaxThrowReach = 650f;
        public static float SwirlBoltAmount = 6f;
        public static float SwirlBoltDamageMultiplier = 1.5f;
        public static float SnapBoltsDamageMultiplier = 0.2f;
        public static float chainDamageMultiplier = 0.1f;
        public List<Vector2> SlashPoints = new();
        public List<Projectile> SlashLines = new();
        public RenderTarget2D rendertarget;
        public Texture2D nameTexture;

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 102;
            Item.height = 102;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 1;
            Item.useAnimation = 1;
            Item.autoReuse = true;

            Item.DamageType = DamageClass.Melee;
            Item.damage = 2669;
            Item.knockBack = 9.5f;
            Item.crit = 15;
            Item.useTurn = true;

            Item.value = Item.buyPrice(2, 6, 6, 9);
            Item.rare = ItemRarityID.Expert;

            Item.UseSound = null;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 28f;
            Item.noMelee = true;
            Item.channel = true;
            Item.noUseGraphic = true;

        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {


            if (tooltips != null && Main.player[Main.myPlayer] != null)
            {
                TooltipLine line = new(AotC.Instance, "AotCText", "\"I'm what you get when the stars collide\"")
                {
                    //OverrideColor = Color.Black
                };
                tooltips.Insert(1, line); // Insert the line at the desired position in the tooltip

                for (int i = 0; i < tooltips.Count; i++)
                {
                    if (Item.favorited && tooltips[i].Name != "AotCText" && tooltips[i].Name != "ItemName")
                    {
                        // Remove specific tooltips (e.g., speed and damage)
                        tooltips.RemoveAt(i);
                        i--;
                    }
                }




                if (!Item.favorited)
                {
                    TooltipLine tooltipLine0 = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
                    if (tooltipLine0 != null)
                    {
                        tooltipLine0.Text = "Use LMB to swing the sword out in front of you, firing out stars in the process";
                        tooltipLine0.OverrideColor = Color.Orange;
                    }
                    TooltipLine tooltipLine1 = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip1" && x.Mod == "Terraria");
                    if (tooltipLine1 != null)
                    {
                        tooltipLine1.Text = "Every fifth swing will swirl the sword around you with Blooming Blows, firing extra \nstars and dealing double damage";
                        tooltipLine1.OverrideColor = Color.DodgerBlue;
                    }
                    TooltipLine tooltipLine2 = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip2" && x.Mod == "Terraria");
                    if (tooltipLine2 != null)
                    {
                        tooltipLine2.Text = "One of the first four attacks will stab in front of you, dealing triple damage and \nfiring out a beam";
                        tooltipLine2.OverrideColor = Color.Yellow;
                    }
                    TooltipLine tooltipLine3 = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip3" && x.Mod == "Terraria");
                    if (tooltipLine3 != null)
                    {
                        tooltipLine3.Text = "Use RMB to throw the sword out, imploding with stars and following \nyour cursor with constellations";
                        tooltipLine3.OverrideColor = Color.Magenta;
                    }
                    TooltipLine tooltipLine4 = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip4" && x.Mod == "Terraria");
                    if (tooltipLine4 != null)
                    {
                        tooltipLine4.Text = "Hitting an enemy with either a thrown attack, Blooming Blows, or the \nstars they create will generate charge";
                        tooltipLine4.OverrideColor = Color.Cyan;
                    }
                    TooltipLine tooltipLine5 = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip5" && x.Mod == "Terraria");
                    if (tooltipLine5 != null)
                    {
                        tooltipLine5.Text = "Charge is consumed when attacking, causing swings to fire out three stars \ninstead of two, beams to be upgraded to bigger and \nlonger lasting Killer Wails, and Blooming Blows to fire twice as many stars";
                        tooltipLine5.OverrideColor = Color.SpringGreen;
                    }
                    TooltipLine tooltipLine6 = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip6" && x.Mod == "Terraria");
                    if (tooltipLine6 != null)
                    {
                        tooltipLine6.Text = "Pressing W + RMB will create stars that form constellations. Up \nto a maximum of ten stars can be placed. Pressing W + LMB while \nclose to the start will cause you to dash and slash across them";
                        tooltipLine6.OverrideColor = Color.Red;
                    }
                }
            }
        }
        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Name is "ItemName" or "AotCText")    
            {
                Main.spriteBatch.End();

                Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Main.UIScaleMatrix);

                GameShaders.Misc["ImageShader"].Apply();


                Utils.DrawBorderString(Main.spriteBatch, line.Text, new Vector2(line.X, line.Y), Color.White);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.UIScaleMatrix);
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            Mod Calamity = AotC.Instance.Calamity;
            if (AotC.Instance.Calamity != null)
            {
                Recipe recipe = CreateRecipe();
                recipe.AddIngredient(Calamity.Find<ModItem>("DormantBrimseeker"), 1);
                recipe.AddIngredient(Calamity.Find<ModItem>("SearedPan"), 1);
                recipe.AddIngredient(ItemID.LifeCrystal, 9);
                recipe.AddIngredient(ItemID.PiercingStarlight, 2);
                recipe.AddIngredient(ItemID.Bone, 66);
                recipe.AddIngredient(Calamity.Find<ModItem>("HyperiusBullet"), 92);
                recipe.AddIngredient(Calamity.Find<ModItem>("CounterScarf"), 1);
                recipe.AddIngredient(Calamity.Find<ModItem>("UndinesRetribution"), 1);
                recipe.AddIngredient(Calamity.Find<ModItem>("ExoPrism"), 1);
                recipe.AddIngredient(ItemID.InfernoFork, 1);
                recipe.AddIngredient(ItemID.Fireplace, 1);
                recipe.AddIngredient(Calamity.Find<ModItem>("ShadowspecBar"), 5);
                recipe.AddTile(Calamity.Find<ModTile>("DraedonsForge"));
                recipe.Register();
            }
        }

        //makes it so you can use weapon with rmb
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        //hash this out if you want speedy attacks, you'll see
        public override bool CanUseItem(Player player)
        {
            return !Main.projectile.Any((Projectile n) => n.active && n.owner == player.whoAmI && n.type == ModContent.ProjectileType<ArkoftheCosmosSwungBlade>());
        }


        //i think this is for multiplayer
        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(charge);
        }

        public override void NetReceive(BinaryReader reader)
        {
            charge = reader.ReadSingle();
        }



        //idk if this does anything
        //i think it syncs charge if you have multiple of the item
        public override ModItem Clone(Item item)
        {
            ModItem modItem = base.Clone(item);
            ArkoftheCosmos arkoftheCosmos = modItem as ArkoftheCosmos;
            if (arkoftheCosmos != null)
            {
                ArkoftheCosmos arkoftheCosmos2 = item.ModItem as ArkoftheCosmos;
                if (arkoftheCosmos2 != null)
                {
                    arkoftheCosmos.charge = arkoftheCosmos2.charge;
                }
            }
            return modItem;
        }


















        //this is where crud happens
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Item.useTime = 1;
            Item.useAnimation = 1;
            if (charge < 0f)
            {
                charge = 0f;
            }
            if (player.altFunctionUse == 2)
            {
                if (player.controlUp)
                {
                    if (charge >= 20f && SlashPoints.Count < 10)
                    {
                        charge -= 20f;
                        SlashPoints.Add(Main.MouseWorld);
                        Projectile projectile = Projectile.NewProjectileDirect(source, player.position, Vector2.Zero, ModContent.ProjectileType<ArkoftheCosmosConstellation>(), 0, 0f, player.whoAmI, 0f, 5f, SlashPoints.Count);
                        projectile.timeLeft = -1;
                        SlashLines.Add(projectile);
                        if (SlashLines.Count > 1)
                        {
                            Projectile projectile2 = SlashLines[SlashLines.Count - 2];
                            if (projectile2.ModProjectile is ArkoftheCosmosConstellation modProjectile)
                            {
                                modProjectile.balls = false;
                            }
                        }
                    }
                    return false;
                }
                else
                {
                    Timers timers = player.GetModPlayer<Timers>();
                    if (timers.ArkThrowCooldown < 0)
                    {
                        Projectile.NewProjectile(source, player.Center, velocity, ModContent.ProjectileType<ArkoftheCosmosSwungBlade>(), damage * 4, knockback, player.whoAmI, 4f, charge);
                        timers.ArkThrowCooldown = 340; //split ark has 340 cooldown
                        PunchCameraModifier modifier = new(player.Center, player.DirectionTo(Main.MouseScreen), 100f, 4f, 20, 2669f, FullName);
                        Main.instance.CameraModifiers.Add(modifier);
                    }
                    return false;
                }
            }





            else if (player.controlUp)
            {
                //slash code
                if (SlashPoints.Count > 1)
                {
                    Item.autoReuse = false;
                    player.GetModPlayer<Timers>().StartSlash();
                }

                return false;
            }
            Item.autoReuse = true;
            Item.useTime = 15;
            Item.useAnimation = 15;





            // resets combo
            if (combo > 4f)
            {
                combo = 0f;
                stab = true;
                rnd = rand.Next(1, 5);
            }
            combo += 1f;
            float num = (combo == 5f) ? 2f : (combo % 2f);
            if (combo == rnd && stab == true)
            {
                num = 3f;
                stab = false;
            }
            Projectile.NewProjectile(source, player.Center, velocity, ModContent.ProjectileType<ArkoftheCosmosSwungBlade>(), damage, knockback, player.whoAmI, num, charge);
            if (num == 3f)
            {
                float f = (player.Center - Main.screenPosition).AngleTo(Main.MouseScreen);
                if (charge < 10)
                {
                    Projectile.NewProjectileDirect(player.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<Beam>(), Item.damage, 1f, player.whoAmI, f, 0f);
                }
                else
                {
                    Projectile.NewProjectileDirect(player.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<Beam>(), Item.damage, 1f, player.whoAmI, f, 1f);
                }
            }
            if (charge > 10f)
            {
                charge -= 10f;
            }
            return false;
        }

        public override bool AllowPrefix(int pre)
        {
            return false;
        }


        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (!(charge <= 10f))
            {
                Texture2D value = ModContent.Request<Texture2D>("AotC/Assets/Textures/GenericBarBack", (AssetRequestMode)2).Value;
                Texture2D value2 = ModContent.Request<Texture2D>("AotC/Assets/Textures/GenericBarFront", (AssetRequestMode)2).Value;
                float num = 3f;
                Vector2 origin2 = new(value.Width / 2f, value.Height / 2f);
                Vector2 val = position + Vector2.UnitY * (frame.Height - 50f) * scale + Vector2.UnitX * (frame.Width - value.Width * num) * scale * 0.5f;
                Rectangle value3 = new(0, 0, (int)(charge / 100f * value2.Width), value2.Height);
                Color val2 = CalamityUtils.HsvToRgb(Main.GlobalTimeWrappedHourly, 1f, 1f);
                spriteBatch.Draw(value, val, null, val2, 0f, origin2, scale * num, 0, 0f);
                spriteBatch.Draw(value2, val, (Rectangle?)value3, val2 * 0.8f, 0f, origin2, scale * num, 0, 0f);
            }
        }
    }
}
