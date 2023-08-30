using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Shaders;
using AotC.Core.GlobalInstances.Systems;

namespace AotC.Content.Particles;

public class CelesteAfterImage : Particle
{
    public float opacity = 100;

    public override string Texture => "AotC/Content/Projectiles/InvisibleProj";

    public Texture2D playerTexture;

    public Rectangle sourceRectangle;

    public override bool UseCustomDraw => true;

    public override bool SetLifetime => true;

    public CelesteAfterImage(Vector2 position, Texture2D PlayerTexture, Rectangle SourceRectangle)
    {
        Position = position;
        Lifetime = 60;
        playerTexture = PlayerTexture;
        sourceRectangle = SourceRectangle;
    }

    public override void Update()
    {
        opacity = 1f - LifetimeCompletion;
    }
    public override void CustomDraw(SpriteBatch spriteBatch)
    {
        spriteBatch.EnterShaderRegion(BlendState.Additive);
        if (AotCSystem.CelesteTrailShader != null)
            AotCSystem.CelesteTrailShader.Apply(null, new(playerTexture, Vector2.Zero, Color.White));
        else
        {
            GameShaders.Misc["CelesteTrailShader"].UseOpacity(opacity);
            GameShaders.Misc["CelesteTrailShader"].Apply();
        }
        spriteBatch.Draw(playerTexture, Position - Main.screenPosition, sourceRectangle, Color.White * opacity, Rotation, new(), 1f, 0, 0f);
        spriteBatch.ExitShaderRegion();
    }
}