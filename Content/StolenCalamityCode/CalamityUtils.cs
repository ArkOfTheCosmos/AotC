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
using scuffed = System.Drawing.Color;

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

	public static Color HsvToRgb(double h, double s, double v)
	{
		int hi = (int)Math.Floor(h / 60.0) % 6;
		double f = (h / 60.0) - Math.Floor(h / 60.0);

		double p = v * (1.0 - s);
		double q = v * (1.0 - (f * s));
		double t = v * (1.0 - ((1.0 - f) * s));

		Color ret;

		switch (hi)
		{
			case 0:
				ret = GetRgb(v, t, p);
				break;
			case 1:
				ret = GetRgb(q, v, p);
				break;
			case 2:
				ret = GetRgb(p, v, t);
				break;
			case 3:
				ret = GetRgb(p, q, v);
				break;
			case 4:
				ret = GetRgb(t, p, v);
				break;
			case 5:
				ret = GetRgb(v, p, q);
				break;
			default:
				ret = new Color(1f, 0f, 0f, 0f);
				break;
		}
		return ret;
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
}