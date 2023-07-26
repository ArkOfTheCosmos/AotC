using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ModLoader;

namespace AotC.Content.Particles;

public class BloomLineVFX : Particle
{
    public bool Capped;

    public float opacity;

    public Vector2 LineVector;

    public bool Telegraph;

    public override string Texture => "AotC/Assets/Textures/BloomLine";

    public override bool UseAdditiveBlend => true;

    public override bool UseCustomDraw => true;

    public override bool SetLifetime => true;

    public override bool Important => Telegraph;

    public BloomLineVFX(Vector2 startPoint, Vector2 lineVector, float thickness, Color color, int lifetime, bool capped = false, bool telegraph = false)
    {
        Position = startPoint;
        LineVector = lineVector;
        Scale = thickness;
        Color = color;
        Lifetime = lifetime;
        Capped = capped;
        Telegraph = telegraph;
        Velocity = Vector2.Zero;
        Rotation = 0f;
    }

    public override void CustomDraw(SpriteBatch spriteBatch)
    {
        Texture2D value = ModContent.Request<Texture2D>(Texture, (AssetRequestMode)2).Value;
        float num = LineVector.ToRotation() + (float)Math.PI / 2f;
        Vector2 val = new(value.Width / 2f, value.Height);
        Vector2 val2 = new(Scale, LineVector.Length() / value.Height);
        spriteBatch.Draw(value, Position - Main.screenPosition, null, Color, num, val, val2, 0, 0f);
        if (Capped)
        {
            Texture2D value2 = ModContent.Request<Texture2D>("AotC/Assets/Textures/BloomLineCap", (AssetRequestMode)2).Value;
            val2 = new(Scale, Scale);
            val = new(value2.Width / 2f, (float)value2.Height);
            spriteBatch.Draw(value2, Position - Main.screenPosition, null, Color, num + (float)Math.PI, val, val2, 0, 0f);
            spriteBatch.Draw(value2, Position + LineVector - Main.screenPosition, null, Color, num, val, val2, 0, 0f);
        }
    }
}
