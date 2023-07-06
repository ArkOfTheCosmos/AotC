using AotC.Content.StolenCalamityCode;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Drawing;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace AotC
{
    public class AotC : Mod
    {
        internal Mod Calamity;

        public const string ASSET_PATH = "AotC/Assets/";

        internal static AotC Instance;

        public override void Load()
        {
            Ref<Effect> shader4 = new(Assets.Request<Effect>("Effects/TextShader", (AssetRequestMode)1).Value);
            GameShaders.Misc["PulseUpwards"] = new MiscShaderData(shader4, "PulseUpwards");
            GameShaders.Misc["PulseDiagonal"] = new MiscShaderData(shader4, "PulseDiagonal");
            GameShaders.Misc["PulseCircle"] = new MiscShaderData(shader4, "PulseCircle");

            Ref<Effect> FadedUVMapStreakShader = new(Assets.Request<Effect>("Effects/FadedUVMapStreak", (AssetRequestMode)1).Value);
            GameShaders.Misc["CalamityMod:TrailStreak"] = new MiscShaderData(FadedUVMapStreakShader, "TrailPass");

            var ExampleMiscEffect = new Ref<Effect>(Assets.Request<Effect>("Effects/ConstellationShader", AssetRequestMode.ImmediateLoad).Value);
            GameShaders.Misc["TooltipShader"] = new MiscShaderData(ExampleMiscEffect, "ExampleCyclePass");

            Calamity = null;
            ModLoader.TryGetMod("CalamityMod", out Calamity);
            Instance = this;
            if (!Main.dedServ)
            {
                GeneralParticleHandler.Load();
            }
        }
    }
}