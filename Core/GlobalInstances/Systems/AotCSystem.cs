using AotC.Content.Items.Weapons.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AotC.Core.GlobalInstances.Systems
{
    internal class AotCSystem : ModSystem
    {
        private static RecipeGroup JellyfishRecipeGroup;
        public static ModKeybind CelesteDash { get; private set; }
        public override void AddRecipeGroups()
        {
            JellyfishRecipeGroup = new(() => "Any Jellyfish", ItemID.BlueJellyfish, ItemID.PinkJellyfish, ItemID.GreenJellyfish);
            RecipeGroup.RegisterGroup("AotC:Jellyfish", JellyfishRecipeGroup);
        }
        public override void Load()
        {
            CelesteDash = KeybindLoader.RegisterKeybind(Mod, "Heart of the Mountain Dash", "LeftShift");
        }
        public override void OnWorldUnload()
        {
            ArkoftheCosmos.SlashLines.Clear();
            ArkoftheCosmos.SlashPoints.Clear();
        }
    }
}
