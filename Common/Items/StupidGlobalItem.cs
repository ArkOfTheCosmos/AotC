using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.Physics;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Shaders;

namespace AotC.Common.Items
{
    internal class StupidGlobalItem : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            if (AotC.Instance.Calamity == null)
                return false;
            return entity.type == AotC.Instance.Calamity.Find<ModItem>("ArkoftheCosmos").Type;
        }

        public override void SetDefaults(Item entity)
        {
            entity.SetNameOverride("The Ark of the Cosmos (Split)");
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.Insert(1, new TooltipLine(AotC.Instance, "FrontiersReferenceTwo", "Now face it you're just an enemy"));
            tooltips[1].OverrideColor = Color.Red;
            if (item.favorited)
            {
                for (int i = 0; i < tooltips.Count; i++)
                {
                    if (tooltips[i].Name != "FrontiersReferenceTwo" && tooltips[i].Name != "ItemName")
                    {
                        tooltips.RemoveAt(i);
                        i--;
                    }
                }
            }
            else
                tooltips.Add(new(AotC.Instance, "AotC:FavouriteThingy", "Favourite this item to remove instructions"));
        }
        public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Name is "FrontiersReferenceTwo" or "ItemName")
            {
                Terraria.UI.Chat.ChatManager.DrawColorCodedStringShadow(Main.spriteBatch, line.Font, line.Text, new Vector2(line.X, line.Y), Color.Lerp(Color.HotPink * 0.5f, Color.Crimson * 0.5f, 0.5f + (float)Math.Sin(Main.GlobalTimeWrappedHourly) * 0.625f), line.Rotation, line.Origin, line.BaseScale * 1, line.MaxWidth, line.Spread);
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
    }
}
