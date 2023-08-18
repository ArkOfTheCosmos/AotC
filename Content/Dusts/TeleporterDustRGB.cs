using Microsoft.Xna.Framework;
using Terraria; 
using Terraria.ModLoader;

namespace AotC.Content.Dusts
{
    internal class TeleporterDustRGB : ModDust
    {
        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return new(255,255,255,100);
        }
        public override bool Update(Dust dust)
        {
            Lighting.AddLight((int)(dust.position.X / 16f), (int)(dust.position.Y / 16f), dust.color.R / 255f, dust.color.G / 255f, dust.color.B / 255f);
            if (dust.noGravity)
            {
                if (dust.scale < 0.7f)
                {
                    dust.velocity *= 1.075f;
                }
                else if (Main.rand.NextBool(2))
                {
                    dust.velocity *= -0.95f;
                }
                else
                {
                    dust.velocity *= 1.05f;
                }
                dust.scale -= 0.03f;
            }
            else
            {
                dust.scale += 0.005f;
                dust.velocity *= 0.9f;
                dust.velocity.X += Main.rand.Next(-10, 11) * 0.02f;
                dust.velocity.Y += Main.rand.Next(-10, 11) * 0.02f;
                if (Main.rand.NextBool(5))
                {
                    int num59 = Dust.NewDust(dust.position, 4, 4, dust.type);
                    Main.dust[num59].noGravity = true;
                    Main.dust[num59].scale = dust.scale * 2.5f;
                }
            }
            return true;
        }
    }
}
