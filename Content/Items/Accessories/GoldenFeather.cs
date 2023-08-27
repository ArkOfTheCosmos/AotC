using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Terraria.Graphics.Shaders;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using AotC.Core.GlobalInstances.Systems;

namespace AotC.Content.Items.Accessories
{
    public class GoldenFeather : ModItem
    {

        public override void SetDefaults()
        {
            Item.width = 31;
            Item.height = 28;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(gold: 6);
            Item.accessory = true;
            Item.rare = ItemRarityID.Purple;
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ConveyorBeltLeft)
                .AddIngredient(ItemID.Bubble)
                .AddIngredient(ItemID.BlackFairyDust)
                .AddIngredient(ItemID.CloudinaBottle)
                .AddIngredient(ItemID.MagicMirror)
                .AddIngredient(ItemID.CrystalShard)
                .AddTile(TileID.CrystalBall)
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
                tooltips[1] = new(AotC.Instance, "SocialDesc", "Close your eyes.\nPicture a feather floating infront of you.\nSee it? \nOkay.\nYour breathing keeps that feather floating.\nJust breathe slow and steady, in and out.");
            }
            else if (AotCSystem.CelesteDash.GetAssignedKeys().Count == 0)
                tooltips.Insert(3, new(AotC.Instance, "CelesteDashNotBound", "Bind a key to dash in controls"));
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (!hideVisual)
                player.GetPlot().celesteTrail = true;
            player.GetPlot().maxDashes = 1;
        }
        public override void UpdateVanity(Player player)
        {
            player.GetPlot().celesteTrail = true;
        }
    }
}