using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using AotC.Content.Buffs;

namespace AotC.Content.Projectiles;

public class EonBeamLegacyV4 : ModProjectile
{
    public override void SetDefaults()
    {
        AIType = 173;
        Projectile.width = 20;
        Projectile.height = 20;
        Projectile.aiStyle = 27;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Melee;
        Projectile.tileCollide = false;
        Projectile.penetrate = 3;
        Projectile.timeLeft = 200;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 3;
    }

    public override void AI()
    {
        Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.3f / 255f, (255 - Projectile.alpha) * 0.4f / 255f, (255 - Projectile.alpha) * 1f / 255f);
        if (Projectile.localAI[1] > 7f)
        {
            int num308 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.RainbowTorch, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 150, new Color(255, Main.DiscoG, 53), 1.2f);
            Main.dust[num308].velocity *= 0.1f;
            Main.dust[num308].noGravity = true;
        }
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return new Color(255, Main.DiscoG, 53, Projectile.alpha);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        if (Projectile.timeLeft > 195)
        {
            return false;
        }
        Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
        Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0f);
        return false;
    }

    public override void Kill(int timeLeft)
    {
        for (int i = 0; i < 7; i++)
        {
            int num308 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.RainbowTorch, 0f, 0f, 150, new Color(255, Main.DiscoG, 53), 1.2f);
            Main.dust[num308].noGravity = true;
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120);
        target.AddBuff(ModContent.BuffType<GlacialState>(), 120);
        target.AddBuff(ModContent.BuffType<Plague>(), 120);
        target.AddBuff(ModContent.BuffType<HolyFlames>(), 120);
    }
}
