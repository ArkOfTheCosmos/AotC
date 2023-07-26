using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AotC.Common.Projectiles;

public class CalamityGlobalProjectile : GlobalProjectile
{
    public bool forceMelee;
    public override bool InstancePerEntity => true;
    public static void HomeInOnNPC(Projectile projectile, bool ignoreTiles, float distanceRequired, float homingVelocity, float N)
    {
        Vector2 center = projectile.Center;
        bool homeIn = false;
        for (int i = 0; i < 200; i++)
        {
            if (Main.npc[i].CanBeChasedBy(projectile))
            {
                float extraDistance = Main.npc[i].width / 2 + Main.npc[i].height / 2;
                bool canHit = true;
                if (extraDistance < distanceRequired && !ignoreTiles)
                {
                    canHit = Collision.CanHit(projectile.Center, 1, 1, Main.npc[i].Center, 1, 1);
                }
                if (Vector2.Distance(Main.npc[i].Center, projectile.Center) < distanceRequired + extraDistance && canHit)
                {
                    center = Main.npc[i].Center;
                    homeIn = true;
                    break;
                }
            }
        }
        if (!projectile.friendly)
        {
            homeIn = false;
        }
        if (homeIn)
        {
            Vector2 homeInVector = projectile.DirectionTo(center);
            if (homeInVector.HasNaNs())
            {
                homeInVector = Vector2.UnitY;
            }
            projectile.velocity = (projectile.velocity * N + homeInVector * homingVelocity) / (N + 1f);
        }
    }
    public static void DrawCenteredAndAfterimage(Projectile projectile, Color lightColor, int trailingMode, int afterimageCounter, Texture2D texture = null, bool drawCentered = true)
    {
        texture ??= ModContent.Request<Texture2D>("AotC/Assets/Textures/Pixel").Value;
        int frameHeight = texture.Height / Main.projFrames[projectile.type];
        int frameY = frameHeight * projectile.frame;
        float scale = projectile.scale;
        float rotation = projectile.rotation;
        Rectangle rectangle = new(0, frameY, texture.Width, frameHeight);
        Vector2 origin = rectangle.Size() / 2f;
        SpriteEffects spriteEffects = SpriteEffects.None;
        if (projectile.spriteDirection == -1)
        {
            spriteEffects = SpriteEffects.FlipHorizontally;
        }
        Vector2 centerOffset = (drawCentered ? (projectile.Size / 2f) : Vector2.Zero);
        switch (trailingMode)
        {
            case 0:
                for (int i = 0; i < projectile.oldPos.Length; i++)
                {
                    Vector2 drawPos = projectile.oldPos[i] + centerOffset - Main.screenPosition + new Vector2(0f, projectile.gfxOffY);
                    Color color = projectile.GetAlpha(lightColor) * ((projectile.oldPos.Length - i) / projectile.oldPos.Length);
                    Main.spriteBatch.Draw(texture, drawPos, rectangle, color, rotation, origin, scale, spriteEffects, 0f);
                }
                break;
            case 1:
                Color color2 = Lighting.GetColor((int)(projectile.Center.X / 16f), (int)(projectile.Center.Y / 16f));
                int whyIsThisAlwaysEight = 8;
                for (int j = 1; j < whyIsThisAlwaysEight; j += afterimageCounter)
                {
                    Color color3 = color2;
                    color3 = projectile.GetAlpha(color3);
                    float num164 = whyIsThisAlwaysEight - j;
                    color3 *= num164 / ((float)ProjectileID.Sets.TrailCacheLength[projectile.type] * 1.5f);
                    Main.spriteBatch.Draw(texture, projectile.oldPos[j] + centerOffset - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), rectangle, color3, rotation, origin, scale, spriteEffects, 0f);
                }
                break;
        }
        Vector2 startPos = (drawCentered ? projectile.Center : projectile.position);
        Main.spriteBatch.Draw(texture, startPos - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), rectangle, projectile.GetAlpha(lightColor), rotation, origin, scale, spriteEffects, 0f);
    }
    public override void AI(Projectile projectile)
    {
        if (forceMelee)
        {
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.DamageType = DamageClass.Melee;
        }
    }
}
