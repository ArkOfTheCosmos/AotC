using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using ReLogic.Content;
using System.IO;
using Terraria.Graphics.CameraModifiers;


namespace AotC.Content.Items.Weapons
{
    internal class ArkoftheCosmos : ModItem
    {
        Random rand = new();
        public int rnd;
        public float combo = 69; // dont change to "public float combo;" i think it broke the first swings or sth
        public float charge;
        public bool stab;
        public static float MaxThrowReach = 650f;
        public static float SwirlBoltAmount = 6f;
        public static float SwirlBoltDamageMultiplier = 1.5f;
        public static float SnapBoltsDamageMultiplier = 0.2f;
        public static float chainDamageMultiplier = 0.1f;

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 102;
            Item.height = 102;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.autoReuse = true;

            Item.DamageType = DamageClass.Melee;
            Item.damage = 2669;
            Item.knockBack = 9.5f;
            Item.crit = 15;
            Item.useTurn = true;

            Item.value = Item.buyPrice(2,6,6,9);
            Item.rare = ItemRarityID.Purple;

            Item.UseSound = null;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 28f;
            Item.noMelee = true;
            Item.channel = true;
            Item.noUseGraphic = true;
            
        }

        public override void AddRecipes()
        {
            Mod Calamity = AotC.Instance.Calamity;
            if (AotC.Instance.Calamity != null)
            {
                Main.NewText("a");
                Recipe recipe = CreateRecipe();
                recipe.AddIngredient(Calamity.Find<ModItem>("DormantBrimseeker"), 1);
                recipe.AddIngredient(Calamity.Find<ModItem>("SearedPan"), 1);
                recipe.AddIngredient(ItemID.LifeCrystal, 9);
                recipe.AddIngredient(ItemID.PiercingStarlight, 2);
                recipe.AddIngredient(ItemID.Bone, 66);
                recipe.AddIngredient(Calamity.Find<ModItem>("HyperiusBullet"), 92);
                recipe.AddIngredient(Calamity.Find<ModItem>("CounterScarf"), 1);
                recipe.AddIngredient(Calamity.Find<ModItem>("UndinesRetribution"), 1);
                recipe.AddIngredient(Calamity.Find<ModItem>("ExoPrism"), 1);
                recipe.AddIngredient(ItemID.InfernoFork, 1);
                recipe.AddIngredient(ItemID.Fireplace, 1);
                recipe.AddIngredient(Calamity.Find<ModItem>("ShadowspecBar"), 5);
                recipe.AddTile(Calamity.Find<ModTile>("DraedonsForge"));
                recipe.Register();
            }
        }

        //makes it so you can use weapon with rmb
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        //hash this out if you want speedy attacks, you'll see
        public override bool CanUseItem(Player player)
        {
            return !Main.projectile.Any((Projectile n) => n.active && n.owner == player.whoAmI && n.type == ModContent.ProjectileType<ArkoftheCosmosSwungBlade>());
        }


        //i think this is for multiplayer
        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(charge);
        }

        public override void NetReceive(BinaryReader reader)
        {
            charge = reader.ReadSingle();
        }



        //idk if this does anything
        public override ModItem Clone(Item item)
        {
            ModItem modItem = base.Clone(item);
            ArkoftheCosmos arkoftheCosmos = modItem as ArkoftheCosmos;
            if (arkoftheCosmos != null)
            {
                ArkoftheCosmos arkoftheCosmos2 = item.ModItem as ArkoftheCosmos;
                if (arkoftheCosmos2 != null)
                {
                    arkoftheCosmos.charge = arkoftheCosmos2.charge;
                }
            }
            return modItem;
        }


















        //this is where crud happens
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                Timers timers = player.GetModPlayer<Timers>();
                if (timers.ArkThrowCooldown < 0)
                {
                    Projectile.NewProjectile(source, player.Center, velocity, ModContent.ProjectileType<ArkoftheCosmosSwungBlade>(), damage * 4, knockback, player.whoAmI, 4f, charge);
                    timers.ArkThrowCooldown = 340; //split ark has 340 cooldown
                    PunchCameraModifier modifier = new(player.Center, (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2(), 50f, 6f, 20, 1000f, FullName);
                    Main.instance.CameraModifiers.Add(modifier);
                } 
                return false;
            }
            // resets combo
            if (combo > 4f)
            {
                combo = 0f;
                stab = true;
                rnd = rand.Next(1, 5);
            }
            combo += 1f;
            float num = (combo == 5f) ? 2f : (combo % 2f);
            if (combo == rnd && stab == true)
            {
                num = 3f;
                stab = false;
            }
            Projectile.NewProjectile(source, player.Center, velocity, ModContent.ProjectileType<ArkoftheCosmosSwungBlade>(), damage, knockback, player.whoAmI, num, charge);
            if (num == 3f)
            {
                float f = (player.Center - Main.screenPosition).AngleTo(Main.MouseScreen);
                if (charge == 0) 
                {
                    Projectile.NewProjectileDirect(player.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<Beam>(), Item.damage, 1f, player.whoAmI, f, 0f);
                }
                else
                {
                    Projectile.NewProjectileDirect(player.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<Beam>(), Item.damage, 1f, player.whoAmI, f, 1f);
                }
            }
            return false;

        }
    }
}
