/*using AotC.Content.Items.Weapons.Ranged;
using AotC.Content.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using AotC.Content.Particles;

namespace AotC.Content.Projectiles
{
    internal class ChaosBusterHeld : ModProjectile
    {
        public override string Texture => "AotC/Content/Projectiles/InvisibleProj";
        public static Texture2D actualTexture;
        public Vector2 point;
        public int MaxTime = 40;
        public Color lineColor;
        public int Time => MaxTime - Projectile.timeLeft;
        public Player Owner => Main.player[Projectile.owner];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.timeLeft = MaxTime;
            actualTexture = ModContent.Request<Texture2D>("AotC/Content/Items/Weapons/Ranged/ChaosBuster", AssetRequestMode.ImmediateLoad).Value;
            lineColor = Main.rand.NextBool() ? Color.Red : Color.Yellow;
        }
        public override void AI()
        {
            Projectile.Center = Owner.RotatedRelativePoint(Owner.MountedCenter, true) - new Vector2(39, 14);
            point = Projectile.Center + new Vector2(37,24) + ChaosBuster.dir * 64f;
            Projectile.rotation = ChaosBuster.dir.ToRotation();
            if (Time >= 16 && Projectile.timeLeft % 4 == 0)
            {
                actualTexture = ModContent.Request<Texture2D>("AotC/Content/Items/Weapons/Ranged/ChaosBusterOpen", AssetRequestMode.ImmediateLoad).Value;
                SoundEngine.PlaySound(in AotCAudio.ChaosBusterShoot);
                for (int i = -1; i < 2; i++)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), point, (ChaosBuster.dir.ToRotation() + i * 0.436332f).ToRotationVector2() * 20f, ModContent.ProjectileType<ChaosBlast>(), (int)(Projectile.damage * (i == 0 ? 1f : 0.4f)), 0, Owner.whoAmI, i, 0.15f);
            }
            else if (Time < 16)
                actualTexture = ModContent.Request<Texture2D>("AotC/Content/Items/Weapons/Ranged/ChaosBuster", AssetRequestMode.ImmediateLoad).Value;
            else if (!((Projectile.timeLeft + 1) % 4 == 0))
                actualTexture = ModContent.Request<Texture2D>("AotC/Content/Items/Weapons/Ranged/ChaosBusterHalfOpen", AssetRequestMode.ImmediateLoad).Value;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Time - 1 < 16)
            {
                if ((Time - 1) % 4 == 0 && !Main.gamePaused)
                {
                    SoundEngine.PlaySound(in AotCAudio.ChaosBusterTarget);
                    lineColor = lineColor == Color.Red ? Color.Yellow : Color.Red;
                }
                for (int i = -1; i < 2; i++)
                    ModdedUtils.DrawLine(point - Main.screenPosition, point - Main.screenPosition + (ChaosBuster.dir.ToRotation() + i * 0.436332f).ToRotationVector2() * 1000f, 1, lineColor);
            }
            return false;
        }
    }
}
*/