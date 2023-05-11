//using AotC.Skies;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;

namespace AotC.Content.StolenCalamityCode;

public class CalamityShaders
{
	public static Effect AstralFogShader;

	public static Effect LightShader;

	public static Effect SCalMouseShader;

	public static Effect TentacleShader;

	public static Effect TeleportDisplacementShader;

	public static Effect LightDistortionShader;

	public static Effect PhaseslayerRipShader;

	public static Effect FadedUVMapStreakShader;

	public static Effect FlameStreakShader;

	public static Effect FadingSolidTrailShader;

	public static Effect ScarletDevilShader;

	public static Effect BordernadoFireShader;

	public static Effect PrismCrystalShader;

	public static Effect ImpFlameTrailShader;

	public static Effect SCalShieldShader;

	public static Effect RancorMagicCircleShader;

	public static Effect BasicTintShader;

	public static Effect CircularBarShader;

	public static Effect CircularBarSpriteShader;

	public static Effect DoGDisintegrationShader;

	public static Effect ArtAttackTrailShader;

	public static Effect CircularAoETelegraph;

	public static Effect IntersectionClipShader;

	public static Effect LocalLinearTransformationShader;

	public static Effect BasicPrimitiveShader;

	public static Effect ArtemisLaserShader;

	public static Effect BaseFusableParticleEdgeShader;

	public static Effect AdditiveFusableParticleEdgeShader;

	public static Effect DoGPortalShader;

	public static Effect FluidShaders;

	public static void LoadShaders()
	{
		if (!Main.dedServ)
		{
			AstralFogShader = AotC.Instance.Assets.Request<Effect>("Effects/CustomShader", (AssetRequestMode)1).Value;
			LightShader = AotC.Instance.Assets.Request<Effect>("Effects/LightBurstShader", (AssetRequestMode)1).Value;
			TentacleShader = AotC.Instance.Assets.Request<Effect>("Effects/TentacleShader", (AssetRequestMode)1).Value;
			TeleportDisplacementShader = AotC.Instance.Assets.Request<Effect>("Effects/TeleportDisplacementShader", (AssetRequestMode)1).Value;
			SCalMouseShader = AotC.Instance.Assets.Request<Effect>("Effects/SCalMouseShader", (AssetRequestMode)1).Value;
			LightDistortionShader = AotC.Instance.Assets.Request<Effect>("Effects/DistortionShader", (AssetRequestMode)1).Value;
			PhaseslayerRipShader = AotC.Instance.Assets.Request<Effect>("Effects/PhaseslayerRipShader", (AssetRequestMode)1).Value;
			ScarletDevilShader = AotC.Instance.Assets.Request<Effect>("Effects/ScarletDevilStreak", (AssetRequestMode)1).Value;
			BordernadoFireShader = AotC.Instance.Assets.Request<Effect>("Effects/BordernadoFire", (AssetRequestMode)1).Value;
			PrismCrystalShader = AotC.Instance.Assets.Request<Effect>("Effects/PrismCrystalStreak", (AssetRequestMode)1).Value;
			FadedUVMapStreakShader = AotC.Instance.Assets.Request<Effect>("Effects/FadedUVMapStreak", (AssetRequestMode)1).Value;
			FlameStreakShader = AotC.Instance.Assets.Request<Effect>("Effects/Flame", (AssetRequestMode)1).Value;
			FadingSolidTrailShader = AotC.Instance.Assets.Request<Effect>("Effects/FadingSolidTrail", (AssetRequestMode)1).Value;
			ImpFlameTrailShader = AotC.Instance.Assets.Request<Effect>("Effects/ImpFlameTrail", (AssetRequestMode)1).Value;
			SCalShieldShader = AotC.Instance.Assets.Request<Effect>("Effects/SupremeShieldShader", (AssetRequestMode)1).Value;
			RancorMagicCircleShader = AotC.Instance.Assets.Request<Effect>("Effects/RancorMagicCircle", (AssetRequestMode)1).Value;
			BasicTintShader = AotC.Instance.Assets.Request<Effect>("Effects/BasicTint", (AssetRequestMode)1).Value;
			CircularBarShader = AotC.Instance.Assets.Request<Effect>("Effects/CircularBarShader", (AssetRequestMode)1).Value;
			CircularBarSpriteShader = AotC.Instance.Assets.Request<Effect>("Effects/CircularBarSpriteShader", (AssetRequestMode)1).Value;
			DoGDisintegrationShader = AotC.Instance.Assets.Request<Effect>("Effects/DoGDisintegration", (AssetRequestMode)1).Value;
			ArtAttackTrailShader = AotC.Instance.Assets.Request<Effect>("Effects/ArtAttackTrail", (AssetRequestMode)1).Value;
			CircularAoETelegraph = AotC.Instance.Assets.Request<Effect>("Effects/CircularAoETelegraph", (AssetRequestMode)1).Value;
			IntersectionClipShader = AotC.Instance.Assets.Request<Effect>("Effects/IntersectionClipShader", (AssetRequestMode)1).Value;
			LocalLinearTransformationShader = AotC.Instance.Assets.Request<Effect>("Effects/LocalLinearTransformationShader", (AssetRequestMode)1).Value;
			BasicPrimitiveShader = AotC.Instance.Assets.Request<Effect>("Effects/BasicPrimitiveShader", (AssetRequestMode)1).Value;
			ArtemisLaserShader = AotC.Instance.Assets.Request<Effect>("Effects/ArtemisLaserShader", (AssetRequestMode)1).Value;
			BaseFusableParticleEdgeShader = AotC.Instance.Assets.Request<Effect>("Effects/ParticleFusion/BaseFusableParticleEdgeShader", (AssetRequestMode)1).Value;
			AdditiveFusableParticleEdgeShader = AotC.Instance.Assets.Request<Effect>("Effects/ParticleFusion/AdditiveFusableParticleEdgeShader", (AssetRequestMode)1).Value;
			DoGPortalShader = AotC.Instance.Assets.Request<Effect>("Effects/ScreenShaders/DoGPortalShader", (AssetRequestMode)1).Value;
			FluidShaders = AotC.Instance.Assets.Request<Effect>("Effects/FluidShaders", (AssetRequestMode)1).Value;
			//Filters.Scene["AotC:Astral"] = new Filter(new AstralScreenShaderData(new Ref<Effect>(AstralFogShader), "AstralPass").UseColor(0.18f, 0.08f, 0.24f), EffectPriority.VeryHigh);
			Filters.Scene["CalamityMod:LightBurst"] = new Filter(new ScreenShaderData(new Ref<Effect>(LightShader), "BurstPass"), EffectPriority.VeryHigh);
			Filters.Scene["CalamityMod:LightBurst"].Load();
			GameShaders.Misc["CalamityMod:FireMouse"] = new MiscShaderData(new Ref<Effect>(SCalMouseShader), "DyePass");
			GameShaders.Misc["CalamityMod:SubsumingTentacle"] = new MiscShaderData(new Ref<Effect>(TentacleShader), "BurstPass");
			GameShaders.Misc["CalamityMod:TeleportDisplacement"] = new MiscShaderData(new Ref<Effect>(TeleportDisplacementShader), "GlitchPass");
			GameShaders.Misc["CalamityMod:PhaseslayerRipEffect"] = new MiscShaderData(new Ref<Effect>(PhaseslayerRipShader), "TrailPass");
			GameShaders.Misc["CalamityMod:TrailStreak"] = new MiscShaderData(new Ref<Effect>(FadedUVMapStreakShader), "TrailPass");
			GameShaders.Misc["CalamityMod:Flame"] = new MiscShaderData(new Ref<Effect>(FlameStreakShader), "TrailPass");
			GameShaders.Misc["CalamityMod:FadingSolidTrail"] = new MiscShaderData(new Ref<Effect>(FadingSolidTrailShader), "TrailPass");
			GameShaders.Misc["CalamityMod:OverpoweredTouhouSpearShader"] = new MiscShaderData(new Ref<Effect>(ScarletDevilShader), "TrailPass");
			GameShaders.Misc["CalamityMod:Bordernado"] = new MiscShaderData(new Ref<Effect>(BordernadoFireShader), "TrailPass");
			GameShaders.Misc["CalamityMod:PrismaticStreak"] = new MiscShaderData(new Ref<Effect>(PrismCrystalShader), "TrailPass");
			GameShaders.Misc["CalamityMod:ImpFlameTrail"] = new MiscShaderData(new Ref<Effect>(ImpFlameTrailShader), "TrailPass");
			GameShaders.Misc["CalamityMod:SupremeShield"] = new MiscShaderData(new Ref<Effect>(SCalShieldShader), "ShieldPass");
			GameShaders.Misc["CalamityMod:RancorMagicCircle"] = new MiscShaderData(new Ref<Effect>(RancorMagicCircleShader), "ShieldPass");
			GameShaders.Misc["CalamityMod:BasicTint"] = new MiscShaderData(new Ref<Effect>(BasicTintShader), "TintPass");
			GameShaders.Misc["CalamityMod:CircularBarShader"] = new MiscShaderData(new Ref<Effect>(CircularBarShader), "Pass0");
			GameShaders.Misc["CalamityMod:CircularBarSpriteShader"] = new MiscShaderData(new Ref<Effect>(CircularBarSpriteShader), "Pass0");
			GameShaders.Misc["CalamityMod:DoGDisintegration"] = new MiscShaderData(new Ref<Effect>(DoGDisintegrationShader), "DisintegrationPass");
			GameShaders.Misc["CalamityMod:ArtAttack"] = new MiscShaderData(new Ref<Effect>(ArtAttackTrailShader), "TrailPass");
			GameShaders.Misc["CalamityMod:CircularAoETelegraph"] = new MiscShaderData(new Ref<Effect>(CircularAoETelegraph), "TelegraphPass");
			GameShaders.Misc["CalamityMod:IntersectionClip"] = new MiscShaderData(new Ref<Effect>(IntersectionClipShader), "ClipPass");
			GameShaders.Misc["CalamityMod:LinearTransformation"] = new MiscShaderData(new Ref<Effect>(LocalLinearTransformationShader), "TransformationPass");
			GameShaders.Misc["CalamityMod:PrimitiveDrawer"] = new MiscShaderData(new Ref<Effect>(BasicPrimitiveShader), "TrailPass");
			GameShaders.Misc["CalamityMod:ArtemisLaser"] = new MiscShaderData(new Ref<Effect>(ArtemisLaserShader), "TrailPass");
			GameShaders.Misc["CalamityMod:BaseFusableParticleEdge"] = new MiscShaderData(new Ref<Effect>(BaseFusableParticleEdgeShader), "ParticlePass");
			GameShaders.Misc["CalamityMod:AdditiveFusableParticleEdge"] = new MiscShaderData(new Ref<Effect>(AdditiveFusableParticleEdgeShader), "ParticlePass");
			GameShaders.Misc["CalamityMod:DoGPortal"] = new MiscShaderData(new Ref<Effect>(DoGPortalShader), "ScreenPass");
			Ref<Effect> shader = new Ref<Effect>(AotC.Instance.Assets.Request<Effect>("Effects/SpreadTelegraph", (AssetRequestMode)1).Value);
			Filters.Scene["SpreadTelegraph"] = new Filter(new ScreenShaderData(shader, "TelegraphPass"), EffectPriority.High);
			Filters.Scene["SpreadTelegraph"].Load();
			shader = new Ref<Effect>(AotC.Instance.Assets.Request<Effect>("Effects/PixelatedSightLine", (AssetRequestMode)1).Value);
			Filters.Scene["PixelatedSightLine"] = new Filter(new ScreenShaderData(shader, "SightLinePass"), EffectPriority.High);
			Filters.Scene["PixelatedSightLine"].Load();
		}
	}
}
