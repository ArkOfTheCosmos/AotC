using Terraria.ModLoader.Config;
using System.ComponentModel;
using Terraria;

namespace AotC.Common.Configs
{
    public class AotCConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("HUD")]

        [DefaultValue(480)]
        [Slider]
        [Range(0, 1920)]
        public int DashIndicatorX;

        [DefaultValue(20)]
        [Slider]
        [Range(0, 1080)]
        public int DashIndicatorY;
    }
}