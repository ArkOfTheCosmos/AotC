using AotC.Content;
using AotC.Content.Items.Accessories;
using AotC.Content.Items.Weapons.Melee;
using AotC.Content.Particles;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace AotC.Core.GlobalInstances.Systems
{
    internal class AotCSystem : ModSystem
    {
        public static ArmorShaderData CelesteTrailShader;

        private static RecipeGroup JellyfishRecipeGroup;
        private static RecipeGroup GemRecipeGroup;
        public static ModKeybind CelesteDash { get; private set; }

        public override void Load()
        {
            CelesteDash = KeybindLoader.RegisterKeybind(Mod, "Heart of the Mountain Dash", "LeftShift");
            On_Main.SortDrawCacheWorms += DrawParticles;
            DyeFindingSystem.FindDyeEvent += FindCelesteTrailShader;
        }

        public override void Unload()
        {
            On_Main.SortDrawCacheWorms -= DrawParticles;
            DyeFindingSystem.FindDyeEvent -= FindCelesteTrailShader;
        }
        public override void OnWorldUnload()
        {
            Main.LocalPlayer.GetPlot().SlashLines.Clear();
            Main.LocalPlayer.GetPlot().SlashPoints.Clear();
        }
        public override void AddRecipeGroups()
        {
            JellyfishRecipeGroup = new(() => "Any Jellyfish", ItemID.BlueJellyfish, ItemID.PinkJellyfish, ItemID.GreenJellyfish);
            RecipeGroup.RegisterGroup("AotC:Jellyfish", JellyfishRecipeGroup);
            GemRecipeGroup = new(() => "Any Gem", ItemID.Diamond, ItemID.Amber, ItemID.Ruby, ItemID.Emerald, ItemID.Sapphire, ItemID.Topaz, ItemID.Amethyst);
            RecipeGroup.RegisterGroup("AotC:Gem", GemRecipeGroup);
        }

        private void DrawParticles(On_Main.orig_SortDrawCacheWorms orig, Main self)
        {
            GeneralParticleHandler.DrawAllParticles(Main.spriteBatch);
            orig.Invoke(self);
        }

        private void FindCelesteTrailShader(Item armorItem, Item dyeItem)
        {
            if (armorItem.type == ModContent.ItemType<HeartoftheMountain>())
            {
                CelesteTrailShader = GameShaders.Armor.GetShaderFromItemId(dyeItem.type);
            }
        }
        public override void PostUpdateEverything()
        {
            GeneralParticleHandler.Update();
        }
    }
}
