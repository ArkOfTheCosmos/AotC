﻿using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Shaders;
using AotC.Content.CustomHooks;

namespace AotC.Content.Particles;

public class CelesteAfterImage : Particle
{
    public float opacity = 100;

    public override string Texture => "AotC/Content/Projectiles/InvisibleProj";

    public Texture2D playerTexture;

    public Rectangle sourceRectangle;


    public CelesteAfterImage(Vector2 position, Texture2D PlayerTexture, Rectangle SourceRectangle)
    {
        Position = position;
        Lifetime = 60;
        Color = Color.White;
        playerTexture = PlayerTexture;
        sourceRectangle = SourceRectangle;
    }

    public override void Update()
    {
        opacity =  1f - LifetimeCompletion;
        Time++;
    }
    public override void CustomDraw(SpriteBatch spriteBatch)
    {
        spriteBatch.EnterShaderRegion(BlendState.Additive);
        if (PlayerTarget.CelesteTrailShader != null)
            PlayerTarget.CelesteTrailShader.Apply(null, new(playerTexture, Vector2.Zero, Color.White));
        else
        {
            GameShaders.Misc["CelesteTrailShader"].UseOpacity(opacity);
            GameShaders.Misc["CelesteTrailShader"].Apply();
        }
        spriteBatch.Draw(playerTexture, Position - Main.screenPosition, sourceRectangle, Color * opacity, Rotation, new(), 1f, 0, 0f);
        spriteBatch.ExitShaderRegion();
    }
}