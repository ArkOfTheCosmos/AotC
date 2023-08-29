using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using Terraria.ID;
using AotC.Common.Players;
using AotC.Common.NPCs;
using AotC.Common.Projectiles;

namespace AotC.Content;
public static class ModdedUtils
{
    internal static readonly FieldInfo UImageFieldMisc = typeof(MiscShaderData).GetField("_uImage1", BindingFlags.Instance | BindingFlags.NonPublic);
    public static MiscShaderData SetShaderTexture(this MiscShaderData shader, Asset<Texture2D> texture)
    {
        UImageFieldMisc.SetValue(shader, texture);
        return shader;
    }
    public enum Direction
    {
        None,
        Up,
        Down,
        Left,
        Right,
        UpLeft,
        UpRight,
        DownLeft,
        DownRight
    }

    public enum EasingType
    {
        Linear,
        SineIn,
        SineOut,
        SineInOut,
        SineBump,
        PolyIn,
        PolyOut,
        PolyInOut,
        ExpIn,
        ExpOut,
        ExpInOut,
        CircIn,
        CircOut,
        CircInOut
    }
    public delegate float EasingFunction(float amount, int degree);
    public struct CurveSegment
    {
        public EasingFunction easing;
        public float originX;
        public float originY;
        public float displacement;
        public int degree;
        public CurveSegment(EasingType MODE, float ORGX, float ORGY, float DISP, int DEG = 1)
        {
            easing = EasingTypeToFunction[(int)MODE];
            originX = ORGX;
            originY = ORGY;
            displacement = DISP;
            degree = DEG;
        }
        public CurveSegment(EasingFunction MODE, float ORGX, float ORGY, float DISP, int DEG = 1)
        {
            easing = MODE;
            originX = ORGX;
            originY = ORGY;
            displacement = DISP;
            degree = DEG;
        }
    }
    private static readonly EasingFunction[] EasingTypeToFunction = new EasingFunction[14]
    {
        LinearEasing, SineInEasing, SineOutEasing, SineInOutEasing, SineBumpEasing, PolyInEasing, PolyOutEasing, PolyInOutEasing, ExpInEasing, ExpOutEasing,
        ExpInOutEasing, CircInEasing, CircOutEasing, CircInOutEasing
    };
    public static float LinearEasing(float amount, int degree)
    {
        return amount;
    }
    public static float SineInEasing(float amount, int degree)
    {
        return 1f - (float)Math.Cos((double)(amount * (float)Math.PI / 2f));
    }
    public static float SineOutEasing(float amount, int degree)
    {
        return (float)Math.Sin((double)(amount * (float)Math.PI / 2f));
    }
    public static float SineInOutEasing(float amount, int degree)
    {
        return (0f - ((float)Math.Cos((double)(amount * (float)Math.PI)) - 1f)) / 2f;
    }
    public static float SineBumpEasing(float amount, int degree)
    {
        return (float)Math.Sin((double)(amount * (float)Math.PI));
    }
    public static float PolyInEasing(float amount, int degree)
    {
        return (float)Math.Pow((double)amount, degree);
    }
    public static float PolyOutEasing(float amount, int degree)
    {
        return 1f - (float)Math.Pow((double)(1f - amount), degree);
    }
    public static float PolyInOutEasing(float amount, int degree)
    {
        if (!(amount < 0.5f))
        {
            return 1f - (float)Math.Pow((double)(-2f * amount + 2f), degree) / 2f;
        }
        return (float)Math.Pow(2.0, degree - 1) * (float)Math.Pow((double)amount, degree);
    }
    public static float ExpInEasing(float amount, int degree)
    {
        if (amount != 0f)
        {
            return (float)Math.Pow(2.0, (double)(10f * amount - 10f));
        }
        return 0f;
    }
    public static float ExpOutEasing(float amount, int degree)
    {
        if (amount != 1f)
        {
            return 1f - (float)Math.Pow(2.0, (double)(-10f * amount));
        }
        return 1f;
    }
    public static float ExpInOutEasing(float amount, int degree)
    {
        if (amount != 0f)
        {
            if (amount != 1f)
            {
                if (!(amount < 0.5f))
                {
                    return (2f - (float)Math.Pow(2.0, (double)(-20f * amount - 10f))) / 2f;
                }
                return (float)Math.Pow(2.0, (double)(20f * amount - 10f)) / 2f;
            }
            return 1f;
        }
        return 0f;
    }
    public static float CircInEasing(float amount, int degree)
    {
        return 1f - (float)Math.Sqrt(1.0 - Math.Pow((double)amount, 2.0));
    }
    public static float CircOutEasing(float amount, int degree)
    {
        return (float)Math.Sqrt(1.0 - Math.Pow((double)(amount - 1f), 2.0));
    }
    public static float CircInOutEasing(float amount, int degree)
    {
        if (!((double)amount < 0.5))
        {
            return ((float)Math.Sqrt(1.0 - Math.Pow((double)(-2f * amount - 2f), 2.0)) + 1f) / 2f;
        }
        return (1f - (float)Math.Sqrt(1.0 - Math.Pow((double)(2f * amount), 2.0))) / 2f;
    }
    public static float PiecewiseAnimation(float progress, CurveSegment[] segments)
    {
        if (segments.Length == 0)
        {
            return 0f;
        }
        if (segments[0].originX != 0f)
        {
            segments[0].originX = 0f;
        }
        progress = MathHelper.Clamp(progress, 0f, 1f);
        float result = 0f;
        for (int i = 0; i <= segments.Length - 1; i++)
        {
            CurveSegment curveSegment = segments[i];
            float originX = curveSegment.originX;
            float num = 1f;
            if (progress < curveSegment.originX)
            {
                continue;
            }
            if (i < segments.Length - 1)
            {
                if (segments[i + 1].originX <= progress)
                {
                    continue;
                }
                num = segments[i + 1].originX;
            }
            float num2 = num - originX;
            float amount = (progress - curveSegment.originX) / num2;
            result = curveSegment.originY;
            result = ((curveSegment.easing == null) ? (result + LinearEasing(amount, curveSegment.degree) * curveSegment.displacement) : (result + curveSegment.easing(amount, curveSegment.degree) * curveSegment.displacement));
            break;
        }
        return result;
    }
    public static NPC ClosestNPCAt(this Vector2 origin, float maxDistanceToCheck, bool ignoreTiles = true, bool bossPriority = false)
    {
        NPC result = null;
        float num = maxDistanceToCheck;
        if (bossPriority)
        {
            bool flag = false;
            for (int i = 0; i < Main.npc.Length; i++)
            {
                if ((flag && !Main.npc[i].boss && Main.npc[i].type != NPCID.WallofFleshEye) || !Main.npc[i].CanBeChasedBy())
                {
                    continue;
                }
                float num2 = Main.npc[i].width / 2 + Main.npc[i].height / 2;
                bool flag2 = true;
                if (num2 < num && !ignoreTiles)
                {
                    flag2 = Collision.CanHit(origin, 1, 1, Main.npc[i].Center, 1, 1);
                }
                if (Vector2.Distance(origin, Main.npc[i].Center) < num + num2 && flag2)
                {
                    if (Main.npc[i].boss || Main.npc[i].type == NPCID.WallofFleshEye)
                    {
                        flag = true;
                    }
                    num = Vector2.Distance(origin, Main.npc[i].Center);
                    result = Main.npc[i];
                }
            }
        }
        else
        {
            for (int j = 0; j < Main.npc.Length; j++)
            {
                if (Main.npc[j].CanBeChasedBy())
                {
                    float num3 = Main.npc[j].width / 2 + Main.npc[j].height / 2;
                    bool flag3 = true;
                    if (num3 < num && !ignoreTiles)
                    {
                        flag3 = Collision.CanHit(origin, 1, 1, Main.npc[j].Center, 1, 1);
                    }
                    if (Vector2.Distance(origin, Main.npc[j].Center) < num + num3 && flag3)
                    {
                        num = Vector2.Distance(origin, Main.npc[j].Center);
                        result = Main.npc[j];
                    }
                }
            }
        }
        return result;
    }
    public static float AngleBetween(this Vector2 v1, Vector2 v2)
    {
        return (float)Math.Acos((double)Vector2.Dot(v1.SafeNormalize(Vector2.Zero), v2.SafeNormalize(Vector2.Zero)));
    }
    public static void CalculatePerspectiveMatricies(out Matrix viewMatrix, out Matrix projectionMatrix)
    {
        Vector2 zoom = Main.GameViewMatrix.Zoom;
        Matrix val = Matrix.CreateScale(zoom.X, zoom.Y, 1f);
        Viewport viewport = Main.instance.GraphicsDevice.Viewport;
        int width = viewport.Width;
        viewport = Main.instance.GraphicsDevice.Viewport;
        int height = viewport.Height;
        viewMatrix = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up);
        viewMatrix *= Matrix.CreateTranslation(0f, -height, 0f);
        viewMatrix *= Matrix.CreateRotationZ((float)Math.PI);
        if (Main.LocalPlayer.gravDir == -1f)
        {
            viewMatrix *= Matrix.CreateScale(1f, -1f, 1f) * Matrix.CreateTranslation(0f, height, 0f);
        }
        viewMatrix *= val;
        projectionMatrix = Matrix.CreateOrthographicOffCenter(0f, width * zoom.X, 0f, height * zoom.Y, 0f, 1f) * val;
    }
    public static Color HsvToRgb(float h, float s, float v)
    {
        int hi = (int)Math.Floor(h * 6);
        float f = h * 6 - hi;
        float p = v * (1 - s);
        float q = v * (1 - f * s);
        float t = v * (1 - (1 - f) * s);

        return (hi % 6) switch
        {
            0 => new Color(v, t, p),
            1 => new Color(q, v, p),
            2 => new Color(p, v, t),
            3 => new Color(p, q, v),
            4 => new Color(t, p, v),
            _ => new Color(v, p, q),
        };
    }
    public static void ColorToHSV(Color color, out float h, out float s, out float v)
    {
        float r = color.R / 255.0f;
        float g = color.G / 255.0f;
        float b = color.B / 255.0f;

        float min = Math.Min(Math.Min(r, g), b);
        float max = Math.Max(Math.Max(r, g), b);
        float delta = max - min;

        // Calculate the value
        v = max;

        // Calculate the saturation
        if (max != 0)
        {
            s = delta / max;
        }
        else
        {
            s = 0;
            h = -1;
            return;
        }

        // Calculate the hue
        if (delta == 0)
        {
            h = 0;
        }
        else if (r == max)
        {
            h = (g - b) / delta;
        }
        else if (g == max)
        {
            h = 2 + (b - r) / delta;
        }
        else
        {
            h = 4 + (r - g) / delta;
        }

        h /= 6;
        if (h < 0)
        {
            h += 1;
        }
    }
    public static void EnterShaderRegion(this SpriteBatch spriteBatch, BlendState newBlendState = null)
    {
        spriteBatch.End();
        spriteBatch.Begin((SpriteSortMode)1, newBlendState ?? BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
    }
    public static void ExitShaderRegion(this SpriteBatch spriteBatch)
    {
        spriteBatch.End();
        spriteBatch.Begin((SpriteSortMode)1, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
    }
    public static Texture2D FlipHorizontal(this Texture2D texture)
    {
        Texture2D flippedTexture = new(texture.GraphicsDevice, texture.Width, texture.Height);
        Color[] data = new Color[texture.Width * texture.Height];
        texture.GetData(data);
        flippedTexture.SetData(data);
        for (int y = 0; y < texture.Height; y++)
        {
            for (int x = 0; x < texture.Width / 2; x++)
            {
                int leftIndex = y * texture.Width + x;
                int rightIndex = y * texture.Width + (texture.Width - x - 1);
                (data[rightIndex], data[leftIndex]) = (data[leftIndex], data[rightIndex]);
            }
        }
        flippedTexture.SetData(data);
        return flippedTexture;
    }
    public static void DrawLine(Vector2 start, Vector2 end, float width, Color color)
    {
        Vector2 direction = end - start;
        float angle = direction.ToRotation();
        float length = direction.Length();
        Texture2D pixelTexture = ModContent.Request<Texture2D>("AotC/Assets/Textures/Pixel").Value; // Replace with your desired texture
        Main.spriteBatch.Draw(pixelTexture, start, null, color, angle, Vector2.Zero, new Vector2(length, width), SpriteEffects.None, 0f);
    }
    public static float GetSpeed(Player player)
    {
        Vector2 vector = player.velocity + player.instantMovementAccumulatedThisFrame;
        int num15 = (int)(1f + vector.Length() * 6f);
        if (num15 > player.speedSlice.Length)
        {
            num15 = player.speedSlice.Length;
        }
        float num16 = 0f;
        for (int num17 = num15 - 1; num17 > 0; num17--)
        {
            player.speedSlice[num17] = player.speedSlice[num17 - 1];
        }
        player.speedSlice[0] = vector.Length();
        for (int m = 0; m < player.speedSlice.Length; m++)
        {
            if (m < num15)
            {
                num16 += player.speedSlice[m];
            }
            else
            {
                player.speedSlice[m] = num16 / num15;
            }
        }
        num16 /= num15;
        int num18 = 42240;
        int num19 = 216000;
        float num20 = num16 * num19 / num18;
        return num20;
    }

    internal static void MakeSilhouette(this Texture2D texture, int alpha, Color color)
    {
        Color[] pixelData = new Color[texture.Width * texture.Height];
        texture.GetData(pixelData);
        for (int i = 0; i < pixelData.Length; i++)
        {
            if (pixelData[i].A > alpha)
                pixelData[i] = color;
            else
                pixelData[i] = Color.Transparent;
        }        
        texture.SetData(pixelData);
    }
    internal static PlotDevice GetPlot(this Player player)
	{
		return player.GetModPlayer<PlotDevice>();
	}
    internal static CalamityPlayer Calamity(this Player player)
	{
		return player.GetModPlayer<CalamityPlayer>();
	}
    internal static CalamityGlobalNPC Calamity(this NPC npc)
    {
        return npc.GetGlobalNPC<CalamityGlobalNPC>();
    }
    internal static CalamityGlobalProjectile Calamity(this Projectile proj)
    {
        return proj.GetGlobalProjectile<CalamityGlobalProjectile>();
    }
    internal static Vector2 RandomVector2(float magnitude = 1)
    {
        return Main.rand.NextFloat((float)-Math.PI, (float)Math.PI).ToRotationVector2() * magnitude;
    }


    public static Direction GetDirection(Player player)
    {
        bool up = player.controlUp;
        bool down = player.controlDown;
        bool left = player.controlLeft;
        bool right = player.controlRight;
            
        if (up && left) return Direction.UpLeft;
        if (up && right) return Direction.UpRight;
        if (down && left) return Direction.DownLeft;
        if (down && right) return Direction.DownRight;

        if (up) return Direction.Up;
        if (down) return Direction.Down;
        if (left) return Direction.Left;
        if (right) return Direction.Right;

        return Direction.None;
    }
}
