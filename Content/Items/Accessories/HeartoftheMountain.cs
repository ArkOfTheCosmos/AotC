using AotC.Common.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace AotC.Content.Items.Accessories
{
    public class HeartoftheMountain : ModItem
    {

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(gold: 9);
            Item.accessory = true;
            Item.rare = ItemRarityID.Purple;
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 14));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ConveyorBeltRight, 1)
                .AddIngredient(ItemID.Bubble, 1)
                .AddIngredient(ItemID.BlackFairyDust, 1)
                .AddIngredient(ItemID.CloudinaBottle, 1)
                .AddIngredient(ItemID.MagicMirror, 1)
                .AddIngredient(ItemID.CrystalShard, 1)
                .AddIngredient(ItemID.GolfCupFlagRed, 1)
                .AddIngredient(ItemID.LifeCrystal, 1)
                .Register();
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Name == "ItemName")
            {
                Terraria.UI.Chat.ChatManager.DrawColorCodedStringShadow(Main.spriteBatch, line.Font, line.Text, new Vector2(line.X, line.Y), Color.Black, line.Rotation, line.Origin, line.BaseScale * 1, line.MaxWidth, line.Spread);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Main.UIScaleMatrix);
                GameShaders.Misc["StaticImageShader"].UseImage1(ModContent.Request<Texture2D>("AotC/Assets/Textures/BreakingBadFlag"));
                GameShaders.Misc["StaticImageShader"].Shader.Parameters["uUIPosition"].SetValue(new Vector2(line.X, line.Y + 3));
                GameShaders.Misc["StaticImageShader"].Shader.Parameters["uScale"].SetValue(Main.UIScale);
                GameShaders.Misc["StaticImageShader"].Apply();
                Terraria.UI.Chat.ChatManager.DrawColorCodedString(Main.spriteBatch, line.Font, line.Text, new Vector2(line.X, line.Y), line.OverrideColor ?? line.Color, line.Rotation, line.Origin, line.BaseScale, line.MaxWidth); // no spread, spread is for shadow // ok boomer
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.UIScaleMatrix);
                return false;
            }
            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Item.social) 
            {
                tooltips.RemoveAt(1);
                tooltips[1] = new(AotC.Instance, "SocialDesc", Tooltip.ToString());
            }
            else
                tooltips.Insert(2, new TooltipLine(AotC.Instance, "HeartoftheMountainInfo", "Gain the trail of a certain mountain climber\nWorks with most non-base dyes"));
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetPlot().celesteTrail = true;
        }
        public override void UpdateVanity(Player player)
        {
            player.GetPlot().celesteTrail = true;
        }
    }
}