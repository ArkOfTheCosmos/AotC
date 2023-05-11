using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ModLoader;

namespace AotC.Content.StolenCalamityCode;

public class GenericSparkle : Particle
{
    public bool imporant;

    private float Spin;

    private float opacity;

    private Color Bloom;

    private float BloomScale;

    public override string Texture => "AotC/Content/Sparkle";

    public override bool UseAdditiveBlend => true;

    public override bool UseCustomDraw => true;

    public override bool SetLifetime => true;

    public override bool Important => imporant;

    private Color LightColor => Bloom * opacity;

    public GenericSparkle(Vector2 position, Vector2 velocity, Color color, Color bloom, float scale, int lifeTime, float rotationSpeed = 1f, float bloomScale = 1f, bool needed = false)
    {
        Position = position;
        Velocity = velocity;
        Color = color;
        Bloom = bloom;
        Scale = scale;
        Lifetime = lifeTime;
        Rotation = Main.rand.NextFloat((float)Math.PI * 2f);
        Spin = rotationSpeed;
        BloomScale = bloomScale;
        imporant = needed;
    }

    public override void Update()
    {
        opacity = (float)Math.Sin((double)(base.LifetimeCompletion * (float)Math.PI));
        Vector2 position = Position;
        Color lightColor = LightColor;
        float r = lightColor.R / 255f;
        lightColor = LightColor;
        float g = lightColor.G / 255f;
        lightColor = LightColor;
        Lighting.AddLight(position, r, g, lightColor.B / 255f);
        Velocity *= 0.95f;
        Rotation += Spin * ((Velocity.X > 0f) ? 1f : (-1f));
    }

    public override void CustomDraw(SpriteBatch spriteBatch)
    {
        Texture2D value = ModContent.Request<Texture2D>(Texture, (AssetRequestMode)2).Value;
        Texture2D value2 = ModContent.Request<Texture2D>("AotC/Content/BloomCircle", (AssetRequestMode)2).Value;
        float num = value.Height / (float)value2.Height;
        spriteBatch.Draw(value2, Position - Main.screenPosition, null, Bloom * opacity * 0.5f, 0f, value2.Size() / 2f, Scale * BloomScale * num, 0, 0f);
        spriteBatch.Draw(value, Position - Main.screenPosition, null, Color * opacity * 0.5f, Rotation + (float)Math.PI / 4f, value.Size() / 2f, Scale * 0.75f, 0, 0f);
        spriteBatch.Draw(value, Position - Main.screenPosition, null, Color * opacity, Rotation, value.Size() / 2f, Scale, 0, 0f);
    }
}
