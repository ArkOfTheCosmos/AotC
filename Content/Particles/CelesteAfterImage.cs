using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Shaders;
using AotC.Core.GlobalInstances.Systems;
using System.Collections.Generic;
using AotC.Content.CustomHooks;

namespace AotC.Content.Particles;

public class CelesteAfterImage : Particle
{
    public float opacity = 100;

    public override string Texture => "AotC/Content/Projectiles/InvisibleProj";

    public Texture2D playerTexture;

    public Rectangle sourceRectangle;

    public override bool UseCustomDraw => true;

    public override bool SetLifetime => true;


    public CelesteAfterImage(Player Player)
    {
        Position = PlayerTarget.GetPlayerTargetPosition(Player.whoAmI) + Main.screenPosition;
        Lifetime = 60;
        sourceRectangle = PlayerTarget.GetPlayerTargetSourceRectangle(Player.whoAmI);
        playerTexture = SilhouettePool.Get();
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

    public override void OnKill()
    {
        SilhouettePool.Release(playerTexture);
    }
}