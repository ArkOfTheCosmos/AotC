/*using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using System.Linq;
using AotC.Content.Projectiles;

namespace AotC.Content.Items.Weapons.Ranged
{
    internal class ChaosBuster : ModItem
    {
        public ChaosBusterHeld Gun;
        public static Vector2 dir;
        public static bool open;
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults()
        {
            Item.crit = 15;
            Item.width = 99;
            Item.height = 45;
            Item.useTime = 1;
            Item.damage = 160;
            Item.knockBack = 4f;
            Item.useAnimation = 40;
            Item.shootSpeed = 1f;
            Item.ResearchUnlockCount = 1;
            Item.rare = ItemRarityID.Purple;
            Item.value = Item.buyPrice(0, 9, 99, 99);
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.DamageType = DamageClass.Ranged;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.channel = true;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
        }
        public override Vector2? HoldoutOffset() => new Vector2(-35, 9);
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (!Main.projectile.Any((Projectile n) => n.active && n.owner == player.whoAmI && n.type == ModContent.ProjectileType<ChaosBusterHeld>()))
                Projectile.NewProjectile(source, player.Center, new(), ModContent.ProjectileType<ChaosBusterHeld>(), damage, 0);
            
            dir = velocity;

            return false;
        }
    }
}*/
