using AotC.Common.Players;
using AotC.Content;
using AotC.Core.Loaders.UILoading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace AotC.Common.UI
{
    internal class PlimpShieldOverlay : SmartUIState
    {
        readonly List<string> resourceSetsWithText = new() { "Default", "HorizontalBarsWithText", "HorizontalBarsWithFullText", "NewWithText" };
        public override bool Visible => true;

        public override int InsertionIndex(List<GameInterfaceLayer> layers)
        {
            return layers.FindIndex(n => n.Name == "Vanilla: Resource Bars") + 1;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Player player = Main.LocalPlayer;
            PlotDevice plot = player.GetPlot();
            if (plot.PlimpShield > 0 && resourceSetsWithText.Contains(Main.ResourceSetsManager.ActiveSetKeyName)) //Text
            {
                int num4 = (int)((float)Main.player[Main.myPlayer].statLifeMax2 / 20);
                if (num4 >= 10)
                    num4 = 10;

                string text = string.Concat(new string[]
                {
                Lang.inter[0].Value,
                " ",
                Main.player[Main.myPlayer].statLifeMax2.ToString(),
                "/",
                Main.player[Main.myPlayer].statLifeMax2.ToString()
                });

                Vector2 vector = Terraria.GameContent.FontAssets.MouseText.Value.MeasureString(text);

                string shieldText = $"  {plot.PlimpShield}/{player.statLifeMax2 / 10f}";
                float textWidth = Terraria.GameContent.FontAssets.MouseText.Value.MeasureString(shieldText).X / 2;

                Vector2 textPos;
                if (Main.ResourceSetsManager.ActiveSetKeyName == "Default")
                    textPos = new Vector2(Main.screenWidth - 300 + 13 * num4 + vector.X * 0.5f - textWidth - 6, 6f);
                else if (Main.ResourceSetsManager.ActiveSetKeyName == "HorizontalBarsWithText")
                    textPos = new Vector2(Main.screenWidth - 215 + 8 * num4 + vector.X * 0.5f - textWidth - 6, 1f);
                else if (Main.ResourceSetsManager.ActiveSetKeyName == "HorizontalBarsWithFullText")
                    textPos = new Vector2(Main.screenWidth - 215 + 8 * num4 + vector.X * 0.5f - textWidth - 6, 0f);
                else // NewWithText
                    textPos = new Vector2(Main.screenWidth - 205 + 8 * num4 + vector.X * 0.5f - textWidth - 6, 0f);
                spriteBatch.DrawString(Terraria.GameContent.FontAssets.MouseText.Value, shieldText, textPos, Main.MouseTextColorReal.MultiplyRGB(Color.Magenta));
            }

            if (plot.PlimpShield > 0)
            {
                int vanillaHearts = Math.Min(20, player.statLifeMax / 20);
                int fullHeartsToDraw = Math.Min(vanillaHearts, plot.PlimpShield / 20);
                float shieldPerHeart = plot.PlimpShield > vanillaHearts * 20 ? player.statLifeMax2 / 10f / vanillaHearts : 20;

                Texture2D tex = ModContent.Request<Texture2D>("AotC/Assets/Textures/ShieldHeart").Value;
                Texture2D texOver = ModContent.Request<Texture2D>("AotC/Assets/Textures/ShieldHeartOver").Value;
                Texture2D texLine = ModContent.Request<Texture2D>("AotC/Assets/Textures/ShieldHeartLine").Value;

                for (int k = 0; k <= fullHeartsToDraw; k++)
                {
                    Vector2 pos = Vector2.Zero;

                    if (Main.ResourceSetsManager.ActiveSetKeyName == "HorizontalBars" || Main.ResourceSetsManager.ActiveSetKeyName == "HorizontalBarsWithText" || Main.ResourceSetsManager.ActiveSetKeyName == "HorizontalBarsWithFullText")
                    {
                        float yOffset = Main.ResourceSetsManager.ActiveSetKeyName switch
                        {
                            "HorizontalBarsWithText" => 28f,
                            "HorizontalBarsWithFullText" => 26f,
                            _ => 24f,
                        };

                        pos = new Vector2(Main.screenWidth - 72 - k * 12, yOffset);

                        Texture2D texBar = ModContent.Request<Texture2D>("AotC/Assets/Textures/PlayerShieldBar").Value;
                        Texture2D texOverBar = ModContent.Request<Texture2D>("AotC/Assets/Textures/PlayerShieldBarOver").Value;
                        Texture2D texLineBar = Terraria.GameContent.TextureAssets.MagicPixel.Value;

                        int width2 = 0;

                        if (plot.PlimpShield >= (k + 1) * shieldPerHeart)
                            width2 = texBar.Width;
                        else if (plot.PlimpShield > k * shieldPerHeart)
                            width2 = (int)(plot.PlimpShield % shieldPerHeart / shieldPerHeart * texBar.Width);

                        if (width2 > 0 && k < 20)
                        {
                            var source = new Rectangle(texBar.Width - width2, 0, width2, texBar.Height);
                            var target = new Rectangle((int)pos.X + texBar.Width - width2, (int)pos.Y, width2, texBar.Height);
                            var lineTarget = new Rectangle((int)pos.X + texBar.Width - width2 - 2, (int)pos.Y, 2, texBar.Height);
                            var lineSource = new Rectangle(0, 0, 2, texBar.Height);

                            spriteBatch.Draw(texBar, target, source, Color.White * 0.25f);
                            spriteBatch.Draw(texOverBar, target, source, Color.White);
                            spriteBatch.Draw(texLineBar, lineTarget, lineSource, Color.White);
                        }

                        continue;
                    }

                    if (Main.ResourceSetsManager.ActiveSetKeyName == "Default")
                    {
                        pos = new Vector2(Main.screenWidth - 300 + k * 26, 32f);

                        if (k >= 10)
                            pos += new Vector2(-260, 26);
                    }
                    else if (Main.ResourceSetsManager.ActiveSetKeyName == "New" || Main.ResourceSetsManager.ActiveSetKeyName == "NewWithText")
                    {
                        float yOffset = 19f;

                        if (Main.ResourceSetsManager.ActiveSetKeyName == "NewWithText")
                            yOffset = 25f;

                        pos = new Vector2(Main.screenWidth - 292 + k * 24, yOffset);

                        if (k >= 10)
                            pos += new Vector2(-240, 28);
                    }

                    int width = 0;

                    if (plot.PlimpShield >= (k + 1) * shieldPerHeart)
                        width = tex.Width;
                    else if (plot.PlimpShield > k * shieldPerHeart)
                        width = (int)(plot.PlimpShield % shieldPerHeart / shieldPerHeart * tex.Width);

                    if (width > 0 && k < 20)
                    {
                        var source = new Rectangle(0, 0, width, tex.Height);
                        var target = new Rectangle((int)pos.X, (int)pos.Y, width, tex.Height);
                        var lineTarget = new Rectangle((int)pos.X + width - 2, (int)pos.Y, 2, tex.Height);
                        var lineSource = new Rectangle(width - 2, 0, 2, tex.Height);

                        spriteBatch.Draw(tex, target, source, Color.White * 0.25f);
                        spriteBatch.Draw(texOver, target, source, Color.White);
                        spriteBatch.Draw(texLine, lineTarget, lineSource, Color.White);
                    }
                }
            }
        }
    }
}
