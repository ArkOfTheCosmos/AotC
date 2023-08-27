using AotC.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SteelSeries.GameSense;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using static System.Net.Mime.MediaTypeNames;

namespace AotC.Common.UI
{
    internal class DashUI : UIState
    {

        private UIElement area;
        public override void OnInitialize()
        {
            // Create a UIElement for all the elements to sit on top of, this simplifies the numbers as nested elements can be positioned relative to the top left corner of this element. 
            // UIElement is invisible and has no padding.
            area = new UIElement();
            area.Left.Set(-area.Width.Pixels - 600, 1f); // Place the resource bar to the left of the hearts.
            area.Top.Set(30, 0f); // Placing it just a bit below the top of the screen.
            area.Width.Set(182, 0f); // We will be placing the following 2 UIElements within this 182x60 area.
            area.Height.Set(60, 0f);
            Append(area);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // no draw unless has dash :thumbsup:
            if (Main.LocalPlayer.GetPlot().maxDashes == 0)
                return;

            base.Draw(spriteBatch);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Main.NewText("asasd");
            Player player = Main.LocalPlayer;
            if (player.GetPlot().maxDashes == 1)
            {
                spriteBatch.Draw(player.GetPlot().dashCount == 1 ? ModContent.Request<Texture2D>("AotC/Assets/Textures/FeatherOneDash", AssetRequestMode.ImmediateLoad).Value : ModContent.Request<Texture2D>("AotC/Assets/Textures/FeatherNoDash", AssetRequestMode.ImmediateLoad).Value, new Vector2(500,500), Color.White);
                Main.NewText("asd");
            }
            else
            {

            }
        }
        public override void Update(GameTime gameTime)
        {
            if (Main.LocalPlayer.GetPlot().maxDashes == 0)
                return;

                base.Update(gameTime);
        }
    }
}
