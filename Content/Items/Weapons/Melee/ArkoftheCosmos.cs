using System;
using Terraria;
using System.IO;
using Terraria.ID;
using System.Linq;
using ReLogic.Content;
using Terraria.ModLoader;
using AotC.Common.Players;
using Terraria.DataStructures;
using AotC.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria.Graphics.Shaders;
using System.Collections.Generic;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.CameraModifiers;
using rail;

namespace AotC.Content.Items.Weapons.Melee
{
    internal class ArkoftheCosmos : ModItem
    {

        // damage values yay
        public static readonly float SwirlBoltAmount = 6f;
        public static readonly float MaxThrowReach = 650f;

        public static readonly float SwirlMultiplier = 2f;
        public static readonly float StabMultiplier = 3f;
        public static readonly float DashMultiplier = 5f;
        public static readonly float BeamMultiplier = 0.2f;
        public static readonly float EonStarMultiplier = 0.25f;
        public static readonly float SwirlStarMultiplier = 0.2f;
        public static readonly float ThrownStarMultiplier = 0.15f;
        public static readonly float DashStarMultiplier = 1f;
        public static readonly float ConstellationMultiplier = 0.1f;

        public static float BalanceMultiplier
        {
            get
            {
                if (AotC.Instance.Calamity != null)
                    return 4f;
                return 1f;
            }
        }
        public int rnd;
        public bool stab;
        public float combo = 69;
        public static float charge;
        public static List<Vector2> SlashPoints = new();
        public static List<Projectile> SlashLines = new();
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults()
        {
            Item.crit = 15;
            Item.width = 102;
            Item.height = 102;
            Item.useTime = 1;
            Item.damage = 333;
            Item.knockBack = 9.5f;
            Item.useAnimation = 1;
            Item.shootSpeed = 28f;
            Item.ResearchUnlockCount = 1;
            Item.UseSound = null;
            Item.channel = true;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.rare = ItemRarityID.Expert;
            Item.value = Item.buyPrice(2, 6, 6, 9);
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.DamageType = DamageClass.Melee;
            Item.shoot = ProjectileID.PurificationPowder;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (tooltips != null && Main.player[Main.myPlayer] != null)
            {
                TooltipLine line = new(AotC.Instance, "FrontiersReferenceLmao", "\"I'm what you get when the stars collide\"");
                tooltips.Insert(1, line);
                for (int i = 0; i < tooltips.Count; i++)
                {
                    if (Item.favorited && tooltips[i].Name != "FrontiersReferenceLmao" && tooltips[i].Name != "ItemName")
                    {
                        tooltips.RemoveAt(i);
                        i--;
                    }
                }
                if (!Item.favorited)
                {
                    TooltipLine damageLine = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Damage" && x.Mod == "Terraria");
                    if (damageLine != null)
                        damageLine.Text = "2669 damage";
                    TooltipLine speedLine = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Speed" && x.Mod == "Terraria");
                    if (speedLine != null)
                        speedLine.Text = "Supersonic speed";
                    TooltipLine tooltipLine0 = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
                    if (tooltipLine0 != null)
                    {
                        tooltipLine0.Text = "Use LMB to swing the sword out in front of you, firing out stars in the process";
                        tooltipLine0.OverrideColor = Color.Orange;
                    }
                    TooltipLine tooltipLine1 = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip1" && x.Mod == "Terraria");
                    if (tooltipLine1 != null)
                    {
                        tooltipLine1.Text = "Every fifth swing will swirl the sword around you with Blooming Blows, firing\nextra stars and dealing double damage";
                        tooltipLine1.OverrideColor = new(65, 100, 255);
                    }
                    TooltipLine tooltipLine2 = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip2" && x.Mod == "Terraria");
                    if (tooltipLine2 != null)
                    {
                        tooltipLine2.Text = "One of the first four attacks will stab in front of you, dealing triple\ndamage and firing out a beam";
                        tooltipLine2.OverrideColor = Color.Yellow;
                    }
                    TooltipLine tooltipLine3 = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip3" && x.Mod == "Terraria");
                    if (tooltipLine3 != null)
                    {
                        tooltipLine3.Text = "Use RMB to throw the sword, causing it to implode with stars and follow\nyour cursor with constellations guiding it";
                        tooltipLine3.OverrideColor = new(130, 70, 255);
                    }
                    TooltipLine tooltipLine4 = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip4" && x.Mod == "Terraria");
                    if (tooltipLine4 != null)
                    {
                        tooltipLine4.Text = "Hitting an enemy with either a thrown attack, Blooming Blows, or the\nstars they create will generate charge";
                        tooltipLine4.OverrideColor = Color.Cyan;
                    }
                    TooltipLine tooltipLine5 = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip5" && x.Mod == "Terraria");
                    if (tooltipLine5 != null)
                    {
                        tooltipLine5.Text = "Charge is consumed when attacking, causing swings to fire out three stars\ninstead of two. Beams will be upgraded to bigger and longer lasting\nKiller Wails, and Blooming Blows will fire twice as many stars";
                        tooltipLine5.OverrideColor = Color.SpringGreen;
                    }
                    TooltipLine tooltipLine6 = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip6" && x.Mod == "Terraria");
                    if (tooltipLine6 != null)
                    {
                        tooltipLine6.Text = "Pressing W + RMB will consume charge to create stars that form constellations.\nUp to a maximum of ten stars can be placed. Pressing W + LMB while close\nto the first star placed will cause you to dash and slash across them. Hold \ndown S and W to remove stars";
                        tooltipLine6.OverrideColor = Color.Red;
                    }
                }
            }
        }
        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Name is "ItemName" or "FrontiersReferenceLmao")
            {
                Terraria.UI.Chat.ChatManager.DrawColorCodedStringShadow(Main.spriteBatch, line.Font, line.Text, new Vector2(line.X, line.Y), Color.Lerp(Color.Lime * 0.5f, Color.Yellow * 0.5f, (float)Math.Sin(Main.GlobalTimeWrappedHourly) / 2f + 0.7f), line.Rotation, line.Origin, line.BaseScale * 1, line.MaxWidth, line.Spread);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Main.UIScaleMatrix);
                GameShaders.Misc["ImageShader"].UseImage1(ModContent.Request<Texture2D>("AotC/Assets/Textures/StarTexture"));
                GameShaders.Misc["ImageShader"].Apply();
                Terraria.UI.Chat.ChatManager.DrawColorCodedString(Main.spriteBatch, line.Font, line.Text, new Vector2(line.X, line.Y), line.OverrideColor ?? line.Color, line.Rotation, line.Origin, line.BaseScale, line.MaxWidth); // no spread, spread is for shadow // ok boomer
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.UIScaleMatrix);
                return false;
            }
            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.StylistKilLaKillScissorsIWish)
                .AddIngredient(ItemID.GenderChangePotion)
                .AddIngredient(ItemID.IntenseGreenFlameDye)
                .AddIngredient(ItemID.LunarBar, 5)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
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
        //this is where crud happens
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            //make sure not slashing because buggy af code lmao i suck
            if (player.GetPlot().done == false)
                return false;
            //makes sure charge isnt negative
            if (charge < 0f)
                charge = 0f;
            //temporarily set usetime and animation incase player drawing stars
            Item.useTime = 1;
            Item.useAnimation = 1;
            //if the player is drawing stars
            if (player.altFunctionUse == 2)
            {
                if (player.controlDown)
                    return false;
                if (player.controlUp)
                {
                    if (charge >= 20f && SlashPoints.Count < 10)
                    {
                        charge -= 20f;
                        if (SlashPoints.Count > 0)
                        {
                           if (Vector2.Distance(Main.MouseWorld, SlashPoints[^1]) < 666)
                                SlashPoints.Add(SlashPoints[^1].DirectionTo(Main.MouseWorld + (Main.MouseWorld == SlashPoints[^1] ? new(Main.rand.NextFloat(-1,1), Main.rand.NextFloat(-1, 1)) : new())) * 666 + SlashPoints[^1]);
                           else
                                SlashPoints.Add(Main.MouseWorld);
                        }
                        else
                            SlashPoints.Add(Main.MouseWorld);
                        Projectile projectile = Projectile.NewProjectileDirect(source, player.position, Vector2.Zero, ModContent.ProjectileType<Constellation>(), 0, 0f, player.whoAmI, 0f, 5f, SlashPoints.Count);
                        projectile.timeLeft = -1;
                        SlashLines.Add(projectile);
                        //redraws star if its no longer the end
                        if (SlashLines.Count > 1)
                        {
                            Projectile projectile2 = SlashLines[^2];
                            if (projectile2.ModProjectile is Constellation modProjectile)
                                modProjectile.balls = false;
                        }
                    }
                    return false;
                }
                //if sword is being thrown
                else
                {
                    PlotDevice p = player.GetPlot();
                    if (p.ArkThrowCooldown < 0)
                    {
                        Projectile.NewProjectile(source, player.Center, velocity, ModContent.ProjectileType<ArkoftheCosmosSwungBlade>(), (int)(damage * BalanceMultiplier), knockback, player.whoAmI, 4f, charge);
                        p.ArkThrowCooldown = 340; // split ark has 340 cooldown
                        PunchCameraModifier modifier = new(player.Center, player.DirectionTo(Main.MouseScreen), 100f, 4f, 20, 2669f, FullName);
                        Main.instance.CameraModifiers.Add(modifier);
                    }
                    return false;
                }
            }
            //if player goes SHING SHING SHING
            else if (player.controlUp)
            {
                if (SlashPoints.Count > 1)
                {
                    Item.autoReuse = false; // makes it so your ears dont break from trying to slash 20 times a second while out of range
                    player.GetPlot().StartSlash((int)(damage * BalanceMultiplier));
                }
                return false;
            }
            //player isnt drawing stars so fix usetime and stuff so players arm doesnt have a seizure
            Item.autoReuse = true;
            Item.useTime = 15;
            Item.useAnimation = 15;
            //resets swing cycle
            if (combo > 4f)
            {
                combo = 0f;
                stab = true;
                rnd = Main.rand.Next(1, 5);
            }
            combo += 1f; //progresses swing cycle
            float num = (combo == 5f) ? 2f : (combo % 2f); //decide what type of swing your on
                                                           //decides whether to stab or not
            if (combo == rnd && stab == true)
            {
                num = 3f;
                stab = false;
            }
            //summons the sword
            Projectile.NewProjectile(source, player.Center, velocity, ModContent.ProjectileType<ArkoftheCosmosSwungBlade>(), (int)(damage * BalanceMultiplier), knockback, player.whoAmI, num, charge);
            //summons beam/wail if its a stab
            if (num == 3f)
            {
                float f = (player.Center - Main.screenPosition).AngleTo(Main.MouseScreen);
                if (charge < 20)
                    Projectile.NewProjectileDirect(player.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<KillerWail>(), (int)(damage * BeamMultiplier * BalanceMultiplier), 1f, player.whoAmI, f, 0f);
                else
                    Projectile.NewProjectileDirect(player.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<KillerWail>(), (int)(damage * BeamMultiplier * BalanceMultiplier), 1f, player.whoAmI, f, 1f);
            }
            //decrease charge
            if (charge >= (num == 3f ? 20f :  10f))
                charge -= 10f;
            return false;
        }
        //no reforges for you bucko
        public override bool AllowPrefix(int pre)
        {
            return false;
        }
        //draw charge meter
        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (charge >= 10f)
            {
                Texture2D value = ModContent.Request<Texture2D>("AotC/Assets/Textures/GenericBarBack", (AssetRequestMode)2).Value;
                Texture2D value2 = ModContent.Request<Texture2D>("AotC/Assets/Textures/GenericBarFront", (AssetRequestMode)2).Value;
                float num = 3f;
                Vector2 origin2 = new(value.Width / 2f, value.Height / 2f);
                Vector2 val = position + Vector2.UnitY * (frame.Height - 50f) * scale + Vector2.UnitX * (frame.Width - value.Width * num) * scale * 0.5f;
                Rectangle value3 = new(0, 0, (int)(charge / 100f * value2.Width), value2.Height);
                Color val2 = ModdedUtils.HsvToRgb(Main.GlobalTimeWrappedHourly, 1f, 1f);
                spriteBatch.Draw(value, val, null, val2, 0f, origin2, scale * num, 0, 0f);
                spriteBatch.Draw(value2, val, (Rectangle?)value3, val2 * 0.8f, 0f, origin2, scale * num, 0, 0f);
            }
        }
    }
}