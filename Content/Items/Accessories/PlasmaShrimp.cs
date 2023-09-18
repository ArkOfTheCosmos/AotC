using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace AotC.Content.Items.Accessories
{
    internal class PlasmaShrimp : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 62;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(gold: 9);
            Item.accessory = true;
            Item.rare = ItemRarityID.Pink;
            Item.ResearchUnlockCount = 1;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.RocketIII, 50)
                .AddIngredient(ItemID.CorruptSeeds, 5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetPlot().Plimp = true;
            if (hideVisual)
                player.GetPlot().PlimpFunny = false;
        }
    }
}
