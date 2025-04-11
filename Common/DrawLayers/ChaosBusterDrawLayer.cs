/* using AotC.Content.Items.Weapons.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.ID;
using AotC.Content.Projectiles;
using Terraria.GameContent.Drawing;

namespace AotC.Common
{
    public class ChaosBusterDrawLayer : PlayerDrawLayer
    {
        // Returning true in this property makes this layer appear on the minimap player head icon.
        public override bool IsHeadLayer => false;

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            // The layer will be visible only if the player is holding an ExampleItem in their hands. Or if another modder forces this layer to be visible.
            return drawInfo.drawPlayer.HeldItem?.type == ModContent.ItemType<ChaosBuster>();

            // If you'd like to reference another PlayerDrawLayer's visibility,
            // you can do so by getting its instance via ModContent.GetInstance<OtherDrawLayer>(), and calling GetDefaultVisiblity on it
        }

        // This layer will be a 'child' of the head layer, and draw before (beneath) it.
        // If the Head layer is hidden, this layer will also be hidden.
        // If the Head layer is moved, this layer will move with it.
        public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.ArmOverItem);
        // If you want to make a layer which isn't a child of another layer, use `new Between(Layer1, Layer2)` to specify the position.
        // If you want to make a 'mobile' layer which can render in different locations depending on the drawInfo, use a `Multiple` position.

        protected override void Draw(ref PlayerDrawSet drawinfo)
        {
            //// The following code draws ExampleItem's texture behind the player's head.

            //ChaosBusterTexture ??= ModContent.Request<Texture2D>("AotC/Content/Items/Weapons/Ranged/ChaosBuster");

            //var position = drawInfo.Center + new Vector2(0f, -20f) - Main.screenPosition;
            //position = new Vector2((int)position.X, (int)position.Y); // You'll sometimes want to do this, to avoid quivering.

            //// Queues a drawing of a sprite. Do not use SpriteBatch in drawlayers!
            //drawInfo.DrawDataCache.Add(new DrawData(
            //    ChaosBusterTexture.Value, // The texture to render.
            //    position, // Position to render at.
            //    null, // Source rectangle.
            //    Color.White, // Color.
            //    0f, // Rotation.
            //    ChaosBusterTexture.Size() * 0.5f, // Origin. Uses the texture's center.
            //    1f, // Scale.
            //    SpriteEffects.None, // SpriteEffects.
            //    0 // 'Layer'. This is always 0 in Terraria.
            //));
            Texture2D value = ChaosBusterHeld.actualTexture;
            if (drawinfo.drawPlayer.JustDroppedAnItem)
            {
                return;
            }
            if (drawinfo.drawPlayer.heldProj >= 0 && drawinfo.shadow == 0f && !drawinfo.heldProjOverHand)
            {
                drawinfo.projectileDrawPosition = drawinfo.DrawDataCache.Count;
            }
            Item heldItem = drawinfo.heldItem;
            int num = heldItem.type;
            float adjustedItemScale = drawinfo.drawPlayer.GetAdjustedItemScale(heldItem);
            Main.instance.LoadItem(num);
            Rectangle itemDrawFrame = drawinfo.drawPlayer.GetItemDrawFrame(num);
            drawinfo.itemColor = Lighting.GetColor((int)(drawinfo.Position.X + drawinfo.drawPlayer.width * 0.5) / 16, (int)((drawinfo.Position.Y + drawinfo.drawPlayer.height * 0.5) / 16.0));
            if (drawinfo.drawPlayer.shroomiteStealth)
            {
                float num2 = drawinfo.drawPlayer.stealth;
                if ((double)num2 < 0.03)
                {
                    num2 = 0.03f;
                }
                float num3 = (1f + num2 * 10f) / 11f;
                drawinfo.itemColor = new Color((byte)(drawinfo.itemColor.R * num2), (byte)(drawinfo.itemColor.G * num2), (byte)(drawinfo.itemColor.B * num3), (byte)(drawinfo.itemColor.A * num2));
            }
            if (drawinfo.drawPlayer.setVortex)
            {
                float num4 = drawinfo.drawPlayer.stealth;
                if ((double)num4 < 0.03)
                {
                    num4 = 0.03f;
                }
                _ = (1f + num4 * 10f) / 11f;
                drawinfo.itemColor = drawinfo.itemColor.MultiplyRGBA(new Color(Vector4.Lerp(Vector4.One, new Vector4(0f, 0.12f, 0.16f, 0f), 1f - num4)));
            }
            bool flag = drawinfo.drawPlayer.itemAnimation > 0 && heldItem.useStyle != ItemUseStyleID.None;
            bool flag2 = heldItem.holdStyle != 0 && !drawinfo.drawPlayer.pulley;
            if (!drawinfo.drawPlayer.CanVisuallyHoldItem(heldItem))
            {
                flag2 = false;
            }
            if (drawinfo.shadow != 0f || drawinfo.drawPlayer.frozen || !(flag || flag2) || num <= 0 || drawinfo.drawPlayer.dead || (drawinfo.drawPlayer.wet && heldItem.noWet) || (drawinfo.drawPlayer.happyFunTorchTime && drawinfo.drawPlayer.inventory[drawinfo.drawPlayer.selectedItem].createTile == TileID.Torches && drawinfo.drawPlayer.itemAnimation == 0))
            {
                return;
            }
            _ = drawinfo.drawPlayer.name;
            Vector2 origin = new(itemDrawFrame.Width * 0.5f - itemDrawFrame.Width * 0.5f * drawinfo.drawPlayer.direction, itemDrawFrame.Height);
            if (heldItem.useStyle == ItemUseStyleID.DrinkLiquid && drawinfo.drawPlayer.itemAnimation > 0)
            {
                Vector2 vector2 = new(0.5f, 0.4f);
                if (heldItem.type == ItemID.Teacup || heldItem.type == ItemID.CoffeeCup)
                {
                    vector2 = new Vector2(0.26f, 0.5f);
                    if (drawinfo.drawPlayer.direction == -1)
                    {
                        vector2.X = 1f - vector2.X;
                    }
                }
                origin = itemDrawFrame.Size() * vector2;
            }
            if (drawinfo.drawPlayer.gravDir == -1f)
            {
                origin.Y = itemDrawFrame.Height - origin.Y;
            }
            ItemSlot.GetItemLight(ref drawinfo.itemColor, heldItem);
            DrawData item;
            if (heldItem.useStyle == ItemUseStyleID.Shoot)
            {
                if (Item.staff[num])
                {
                    float num9 = drawinfo.drawPlayer.itemRotation + 0.785f * drawinfo.drawPlayer.direction;
                    float num10 = 0f;
                    float num11 = 0f;
                    Vector2 origin5 = new(0f, itemDrawFrame.Height);
                    if (drawinfo.drawPlayer.gravDir == -1f)
                    {
                        if (drawinfo.drawPlayer.direction == -1)
                        {
                            num9 += 1.57f;
                            origin5 = new Vector2(itemDrawFrame.Width, 0f);
                            num10 -= itemDrawFrame.Width;
                        }
                        else
                        {
                            num9 -= 1.57f;
                            origin5 = Vector2.Zero;
                        }
                    }
                    else if (drawinfo.drawPlayer.direction == -1)
                    {
                        origin5 = new Vector2(itemDrawFrame.Width, itemDrawFrame.Height);
                        num10 -= itemDrawFrame.Width;
                    }
                    item = new DrawData(value, new Vector2((int)(drawinfo.ItemLocation.X - Main.screenPosition.X + origin5.X + num10), (int)(drawinfo.ItemLocation.Y - Main.screenPosition.Y + num11)), itemDrawFrame, heldItem.GetAlpha(drawinfo.itemColor), num9, origin5, adjustedItemScale, drawinfo.itemEffect);
                    drawinfo.DrawDataCache.Add(item);
                    return;
                }
                Vector2 vector9 = new(itemDrawFrame.Width / 2, itemDrawFrame.Height / 2);
                Vector2 vector10 = Main.DrawPlayerItemPos(drawinfo.drawPlayer.gravDir, num);
                int num12 = (int)vector10.X;
                vector9.Y = vector10.Y;
                Vector2 origin7 = new(-num12, itemDrawFrame.Height / 2);
                if (drawinfo.drawPlayer.direction == -1)
                {
                    origin7 = new Vector2(itemDrawFrame.Width + num12, itemDrawFrame.Height / 2);
                }
                item = new DrawData(value, new Vector2((int)(drawinfo.ItemLocation.X - Main.screenPosition.X + vector9.X), (int)(drawinfo.ItemLocation.Y - Main.screenPosition.Y + vector9.Y)), itemDrawFrame, heldItem.GetAlpha(drawinfo.itemColor), drawinfo.drawPlayer.itemRotation, origin7, adjustedItemScale, drawinfo.itemEffect);
                drawinfo.DrawDataCache.Add(item);
                if (heldItem.color != default)
                {
                    item = new DrawData(value, new Vector2((int)(drawinfo.ItemLocation.X - Main.screenPosition.X + vector9.X), (int)(drawinfo.ItemLocation.Y - Main.screenPosition.Y + vector9.Y)), itemDrawFrame, heldItem.GetColor(drawinfo.itemColor), drawinfo.drawPlayer.itemRotation, origin7, adjustedItemScale, drawinfo.itemEffect);
                    drawinfo.DrawDataCache.Add(item);
                }
                if (heldItem.glowMask != -1)
                {
                    item = new DrawData(TextureAssets.GlowMask[heldItem.glowMask].Value, new Vector2((int)(drawinfo.ItemLocation.X - Main.screenPosition.X + vector9.X), (int)(drawinfo.ItemLocation.Y - Main.screenPosition.Y + vector9.Y)), itemDrawFrame, new Color(250, 250, 250, heldItem.alpha), drawinfo.drawPlayer.itemRotation, origin7, adjustedItemScale, drawinfo.itemEffect);
                    drawinfo.DrawDataCache.Add(item);
                }
                Main.NewText(drawinfo.Position);
                return;
            }
        }
    }
}*/