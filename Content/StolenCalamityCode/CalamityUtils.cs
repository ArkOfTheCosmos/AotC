using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
/*using CalamityMod.Balancing;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using CalamityMod.Cooldowns;
using CalamityMod.DataStructures;
using CalamityMod.Events;
using CalamityMod.Items;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Tools.ClimateChange;
using CalamityMod.NPCs;
using CalamityMod.NPCs.Abyss;
using CalamityMod.NPCs.AcidRain;
using CalamityMod.NPCs.AquaticScourge;
using CalamityMod.NPCs.Astral;
using CalamityMod.NPCs.Crags;
using CalamityMod.NPCs.GreatSandShark;
using CalamityMod.NPCs.Leviathan;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.OldDuke;
using CalamityMod.NPCs.PlagueEnemies;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.SlimeGod;
using CalamityMod.NPCs.SulphurousSea;
using CalamityMod.NPCs.SunkenSea;
using CalamityMod.Projectiles;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Tiles;
using CalamityMod.Tiles.Abyss;
using CalamityMod.Tiles.Astral;
using CalamityMod.Tiles.AstralDesert;
using CalamityMod.Tiles.AstralSnow;
using CalamityMod.Tiles.Crags;
using CalamityMod.Tiles.FurnitureAbyss;
using CalamityMod.Tiles.FurnitureAshen;
using CalamityMod.Tiles.FurnitureEutrophic;
using CalamityMod.Tiles.FurnitureOtherworldly;
using CalamityMod.Tiles.FurnitureProfaned;
using CalamityMod.Tiles.FurnitureVoid;
using CalamityMod.Tiles.Ores;
using CalamityMod.Tiles.SunkenSea;
using CalamityMod.UI.CalamitasEnchants;
using CalamityMod.World; */
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using ReLogic.Content;
using ReLogic.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Events;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.UI.Chat;

namespace AotC.Content.StolenCalamityCode;

public static class CalamityUtils
{
	internal static readonly FieldInfo UImageFieldMisc = typeof(MiscShaderData).GetField("_uImage1", BindingFlags.Instance | BindingFlags.NonPublic);

	public static MiscShaderData SetShaderTexture(this MiscShaderData shader, Asset<Texture2D> texture)
	{
		UImageFieldMisc.SetValue(shader, texture);
		return shader;
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
		return (float)Math.Pow((double)amount, (double)degree);
	}

	public static float PolyOutEasing(float amount, int degree)
	{
		return 1f - (float)Math.Pow((double)(1f - amount), (double)degree);
	}

	public static float PolyInOutEasing(float amount, int degree)
	{
		if (!(amount < 0.5f))
		{
			return 1f - (float)Math.Pow((double)(-2f * amount + 2f), (double)degree) / 2f;
		}
		return (float)Math.Pow(2.0, (double)(degree - 1)) * (float)Math.Pow((double)amount, (double)degree);
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
				if ((flag && !Main.npc[i].boss && Main.npc[i].type != 114) || !Main.npc[i].CanBeChasedBy())
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
					if (Main.npc[i].boss || Main.npc[i].type == 114)
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
		Viewport viewport = ((Game)Main.instance).GraphicsDevice.Viewport;
		int width = ((Viewport)(viewport)).Width;
		viewport = ((Game)Main.instance).GraphicsDevice.Viewport;
		int height = ((Viewport)(viewport)).Height;
		viewMatrix = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up);
		viewMatrix *= Matrix.CreateTranslation(0f, (float)(-height), 0f);
		viewMatrix *= Matrix.CreateRotationZ((float)Math.PI);
		if (Main.LocalPlayer.gravDir == -1f)
		{
			viewMatrix *= Matrix.CreateScale(1f, -1f, 1f) * Matrix.CreateTranslation(0f, (float)height, 0f);
		}
		viewMatrix *= val;
		projectionMatrix = Matrix.CreateOrthographicOffCenter(0f, (float)width * zoom.X, 0f, (float)height * zoom.Y, 0f, 1f) * val;
	}








    public static Color HsvToRgb(float h, float s, float v)
    {
        int hi = (int)Math.Floor(h * 6);
        float f = h * 6 - hi;
        float p = v * (1 - s);
        float q = v * (1 - f * s);
        float t = v * (1 - (1 - f) * s);

        switch (hi % 6)
        {
            case 0:
                return new Color(v, t, p);
            case 1:
                return new Color(q, v, p);
            case 2:
                return new Color(p, v, t);
            case 3:
                return new Color(p, q, v);
            case 4:
                return new Color(t, p, v);
            default:
                return new Color(v, p, q);
        }
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


    public static Texture2D ShiftHue(Texture2D originalTexture, float hueShift)
    {
        int width = originalTexture.Width;
        int height = originalTexture.Height;

        Color[] originalColors = new Color[width * height];
        originalTexture.GetData(originalColors);

        Color[] shiftedColors = new Color[width * height];

        for (int i = 0; i < originalColors.Length; i++)
        {
            Color originalColor = originalColors[i];

            float h, s, v;
            ColorToHSV(originalColor, out h, out s, out v);

            // Shift the hue
            h = (h + hueShift) % 1.0f;

            Color shiftedColor = HsvToRgb(h, s, v);
			shiftedColor.A = originalColor.A;
            shiftedColors[i] = shiftedColor;
        }

        Texture2D shiftedTexture = new Texture2D(originalTexture.GraphicsDevice, width, height);
        shiftedTexture.SetData(shiftedColors);

        return shiftedTexture;
    }









    public static Color GetRgb(double r, double g, double b)
	{
		return new Color((byte)(r * 255.0), (byte)(g * 255.0), (byte)(b * 255.0), 255);
	}

	public static void EnterShaderRegion(this SpriteBatch spriteBatch, BlendState newBlendState = null)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		spriteBatch.End();
		spriteBatch.Begin((SpriteSortMode)1, newBlendState ?? BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, (Effect)null, Main.GameViewMatrix.TransformationMatrix);
	}

	public static void ExitShaderRegion(this SpriteBatch spriteBatch)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		spriteBatch.End();
		spriteBatch.Begin((SpriteSortMode)0, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, (Effect)null, Main.GameViewMatrix.TransformationMatrix);
	}

	public static Texture2D FlipHorizontal(this Texture2D texture)
	{
		Texture2D flippedTexture = new Texture2D(texture.GraphicsDevice, texture.Width, texture.Height);
		Color[] data = new Color[texture.Width * texture.Height];
		texture.GetData(data);
		flippedTexture.SetData(data);

		for (int y = 0; y < texture.Height; y++)
		{
			for (int x = 0; x < texture.Width / 2; x++)
			{
				int leftIndex = y * texture.Width + x;
				int rightIndex = y * texture.Width + (texture.Width - x - 1);
				Color temp = data[leftIndex];
				data[leftIndex] = data[rightIndex];
				data[rightIndex] = temp;
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
}
