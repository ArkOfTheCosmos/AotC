using AotC.Common.Configs;
using AotC.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.UI;

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
            if (Main.LocalPlayer.GetPlot().maxDashes == 0 || Main.playerInventory)
                return;

            base.Draw(spriteBatch);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            int x = ModContent.GetInstance<AotCConfig>().DashIndicatorX;
            int y = ModContent.GetInstance<AotCConfig>().DashIndicatorY;
            Player player = Main.LocalPlayer;
            if (player.GetPlot().maxDashes == 1)
            {
                spriteBatch.Draw(player.GetPlot().dashCount == 1 ? ModContent.Request<Texture2D>("AotC/Assets/Textures/FeatherOneDash", AssetRequestMode.ImmediateLoad).Value : ModContent.Request<Texture2D>("AotC/Assets/Textures/FeatherNoDash", AssetRequestMode.ImmediateLoad).Value, new Vector2(x, y), Color.White);
                spriteBatch.Draw(ModContent.Request<Texture2D>("AotC/Assets/Textures/FeatherFlash", AssetRequestMode.ImmediateLoad).Value, new Vector2(x, y), Color.White * (player.GetPlot().UIFlash / 127f));
            }
            else
            {
                spriteBatch.Draw(player.GetPlot().dashCount == 2 ? ModContent.Request<Texture2D>("AotC/Assets/Textures/HeartTwoDash", AssetRequestMode.ImmediateLoad).Value : player.GetPlot().dashCount == 1 ? ModContent.Request<Texture2D>("AotC/Assets/Textures/HeartOneDash", AssetRequestMode.ImmediateLoad).Value : ModContent.Request<Texture2D>("AotC/Assets/Textures/HeartNoDash", AssetRequestMode.ImmediateLoad).Value, new Vector2(x, y), Color.White);
                spriteBatch.Draw(ModContent.Request<Texture2D>("AotC/Assets/Textures/HeartFlash", AssetRequestMode.ImmediateLoad).Value, new Vector2(x, y), Color.White * (player.GetPlot().UIFlash / 127f));
            }
        }
        public override void Update(GameTime gameTime)
        {
            if (Main.LocalPlayer.GetPlot().maxDashes == 0)
                return;

                base.Update(gameTime);
        }



        [Autoload(Side = ModSide.Client)]
        internal class DashUISystem : ModSystem
        {
            private UserInterface DashUIUserInterface;

            internal DashUI DashUI;


            public override void Load()
            {
                DashUI = new();
                DashUIUserInterface = new();
                DashUIUserInterface.SetState(DashUI);

            }

            public override void UpdateUI(GameTime gameTime)
            {
                DashUIUserInterface?.Update(gameTime);
            }

            public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
            {
                int resourceBarIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
                if (resourceBarIndex != -1)
                {
                    layers.Insert(resourceBarIndex, new LegacyGameInterfaceLayer(
                        "ExampleMod: Example Resource Bar",
                        delegate {
                            DashUIUserInterface.Draw(Main.spriteBatch, new GameTime());
                            return true;
                        },
                        InterfaceScaleType.UI)
                    );
                }
            }
        }
    }
}
