using System;
using Terraria;
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
        private PlotDevice pd => Main.LocalPlayer.GetPlot();

        public static float BalanceMultiplier => AotC.Instance.Calamity != null ? 4f : 1f;
        public int rnd;
        public int rnd2;
        public float combo = 69;
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults()
        {
            Item.crit = 15;
            Item.width = 102;
            Item.height = 102;
            Item.useTime = 20;
            Item.damage = 333;
            Item.knockBack = 9.5f;
            Item.useAnimation = 20;
            Item.shootSpeed = 28f;
            Item.channel = true;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.ResearchUnlockCount = 1;
            Item.rare = ItemRarityID.Purple;
            Item.value = Item.buyPrice(0, 2, 66, 9);
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.DamageType = DamageClass.Melee;
            Item.shoot = ProjectileID.PurificationPowder;
            //Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 30));
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (tooltips != null && Main.player[Main.myPlayer] != null)
            {
                TooltipLine line = new(AotC.Instance, "FrontiersReferenceLmao", "\"I'm what you get when the stars collide\"");
                tooltips.Insert(1, line);
                if (AotC.Instance.Calamity != null)
                {
                    line = new(AotC.Instance, "AotC:CalamityScaleNotifier", "You currently have Calamity enabled, this weapon will\nbe harder to craft, but become 4 times stronger");
                    tooltips.Add(line);
                }
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
                        damageLine.Text = "2669 melee damage";
                    TooltipLine speedLine = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Speed" && x.Mod == "Terraria");
                    if (speedLine != null)
                        speedLine.Text = "Supersonic speed";
                    TooltipLine tooltipLine0 = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
                    if (tooltipLine0 != null)
                    {
                        tooltipLine0.Text = "Use LMB to swing the sword out in front of you, firing out Eon stars in the process";
                        tooltipLine0.OverrideColor = Color.SpringGreen;
                    }
                    TooltipLine tooltipLine1 = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip1" && x.Mod == "Terraria");
                    if (tooltipLine1 != null)
                    {
                        tooltipLine1.Text = "For every eight swings, one will be a stab, and one will be a swirl";
                        tooltipLine1.OverrideColor = Color.Cyan;
                    }
                    TooltipLine tooltipLine2 = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip2" && x.Mod == "Terraria");
                    if (tooltipLine2 != null)
                    {
                        tooltipLine2.Text = "Stabs deal three times the melee damage, and fire out a beam";
                        tooltipLine2.OverrideColor = Color.DeepSkyBlue;
                    }
                    TooltipLine tooltipLine3 = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip3" && x.Mod == "Terraria");
                    if (tooltipLine3 != null)
                    {
                        tooltipLine3.Text = "Swirls deal double damage, and fire out extra stars";
                        tooltipLine3.OverrideColor = Color.MediumPurple;
                    }
                    TooltipLine tooltipLine4 = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip4" && x.Mod == "Terraria");
                    if (tooltipLine4 != null)
                    {
                        tooltipLine4.Text = "Use RMB to throw the sword, causing it to implode with stars and follow\nyour cursor with constellations guiding it";
                        tooltipLine4.OverrideColor = Color.HotPink;
                    }
                    TooltipLine tooltipLine5 = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip5" && x.Mod == "Terraria");
                    if (tooltipLine5 != null)
                    {
                        tooltipLine5.Text = "Hitting an enemy with the blade itself will generate charge";
                        tooltipLine5.OverrideColor = Color.Red;
                    }
                    TooltipLine tooltipLine6 = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip6" && x.Mod == "Terraria");
                    if (tooltipLine6 != null)
                    {
                        tooltipLine6.Text = "Charge is consumed when attacking, causing swings to fire out three stars\ninstead of two. Blooming Blows will fire twice as many stars, and Beams will be\nupgraded to bigger and longer lasting Killer Wails with enough charge";
                        tooltipLine6.OverrideColor = Color.Orange;
                    }
                    TooltipLine tooltipLine7 = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "Tooltip7" && x.Mod == "Terraria");
                    if (tooltipLine7 != null)
                    {
                        tooltipLine7.Text = "Pressing UP + RMB will consume charge to create stars that form constellations.\nUp to a maximum of ten stars can be placed. Pressing UP + LMB while close\nto one of the ends of the constellation will cause you to Cosmic Cut across them.\nHold down UP and DOWN to remove the stars";
                        tooltipLine7.OverrideColor = Color.Yellow;
                    }
                }
            }
        }
        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Name is "ItemName" or "FrontiersReferenceLmao")
            {
                Terraria.UI.Chat.ChatManager.DrawColorCodedStringShadow(Main.spriteBatch, line.Font, line.Text, new Vector2(line.X, line.Y), new Color(77, 136, 236), line.Rotation, line.Origin, line.BaseScale * 1, line.MaxWidth, line.Spread);
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
                .AddIngredient(ItemID.MusicBox)
                .AddRecipeGroup(RecipeGroup.recipeGroupIDs["AotC:Gem"], 8)
                .AddIngredient(AotC.Instance.Calamity == null ? ItemID.LunarBar : AotC.Instance.Calamity.Find<ModItem>("AuricBar").Type, 5)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
        //makes it so you can use weapon with rmb
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
        //idk anymore
        public override bool CanUseItem(Player player)
        {
            //return !Main.projectile.Any((Projectile n) => n.active && n.owner == player.whoAmI && n.type == ModContent.ProjectileType<ArkoftheCosmosSwungBlade>());
            return true;
        }
        //this is where crud happens
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //make sure not slashing because buggy af code lmao i suck
            if (pd.SlashDone == false)
                return false;
            //makes sure charge isnt negative
            if (pd.ArkCharge < 0f)
                pd.ArkCharge = 0f;
            if (player.altFunctionUse == 2)
            {
                if (player.controlDown || player.controlUp)
                    return false;
                //if sword is being thrown
                else
                {
                    PlotDevice p = player.GetPlot();
                    if (p.ArkThrowCooldown < 0)
                    {
                        player.itemTime = 65;
                        Projectile.NewProjectile(source, player.Center, velocity, ModContent.ProjectileType<ArkoftheCosmosSwungBlade>(), (int)(damage * BalanceMultiplier), knockback, player.whoAmI, 4f, pd.ArkCharge);
                        p.ArkThrowCooldown = 340; // split ark has 340 cooldown
                        PunchCameraModifier modifier = new(player.Center, player.DirectionTo(Main.MouseScreen), 100f, 4f, 20, 2669f, FullName);
                        Main.instance.CameraModifiers.Add(modifier);
                    }
                    return false;
                }
            }
            //if player goes SHING SHING SHING
            else if (player.controlUp && pd.justPressedLeftClick)
            {
                if (pd.SlashPoints.Count > 1)
                {
                    Item.autoReuse = false; // makes it so your ears dont break from trying to slash 20 times a second while out of range
                    player.GetPlot().StartSlash((int)(damage * BalanceMultiplier));
                    return false;
                }
            }
            //resets swing cycle
            if (combo > 7f)
            {
                combo = 0f;
                rnd = Main.rand.Next(1, 8);
                rnd2 = Main.rand.Next(1, 8);
                while (rnd2 == rnd)
                {
                    rnd2 = Main.rand.Next(1, 8);
                }
            }
            combo += 1f; //progresses swing cycle
            float num  = combo % 2f; //decide what type of swing your on
            //decides whether to stab or not
            if (combo == rnd)
            {
                num = 3f;
            }
            else if (combo == rnd2)
            {
                num = 2f;
            }
            //summons the sword
            Projectile.NewProjectile(source, player.Center, velocity, ModContent.ProjectileType<ArkoftheCosmosSwungBlade>(), (int)(damage * BalanceMultiplier), knockback, player.whoAmI, num, pd.ArkCharge);
            //summons beam/wail if its a stab
            if (num == 3f)
            {
                float f = (player.Center - Main.screenPosition).AngleTo(Main.MouseScreen);
                if (pd.ArkCharge < 50)
                    Projectile.NewProjectileDirect(player.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<KillerWail>(), (int)(damage * BeamMultiplier * BalanceMultiplier), 1f, player.whoAmI, f, 0f);
                else
                    Projectile.NewProjectileDirect(player.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<KillerWail>(), (int)(damage * BeamMultiplier * BalanceMultiplier), 1f, player.whoAmI, f, 1f);
            }
            //decrease charge
            if (pd.ArkCharge >= (num == 3f ? 50f :  10f))
                pd.ArkCharge -= 10f;
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
            if (pd.ArkCharge >= 10f)
            {
                Texture2D value = ModContent.Request<Texture2D>("AotC/Assets/Textures/GenericBarBack", (AssetRequestMode)2).Value;
                Texture2D value2 = ModContent.Request<Texture2D>("AotC/Assets/Textures/GenericBarFront", (AssetRequestMode)2).Value;
                float num = 3f;
                Vector2 origin2 = new(value.Width / 2f, value.Height / 2f);
                Vector2 val = position + Vector2.UnitY * (frame.Height - 50f) * scale + Vector2.UnitX * (frame.Width - value.Width * num) * scale * 0.5f;
                Rectangle value3 = new(0, 0, (int)(pd.ArkCharge / 100f * value2.Width), value2.Height);
                Color val2 = ModdedUtils.HsvToRgb(Main.GlobalTimeWrappedHourly, 1f, 1f);
                spriteBatch.Draw(value, val, null, val2, 0f, origin2, scale * num, 0, 0f);
                spriteBatch.Draw(value2, val, (Rectangle?)value3, val2 * 0.8f, 0f, origin2, scale * num, 0, 0f);
            }
        }
    }
}