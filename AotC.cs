using Terraria.ModLoader;
using AotC.Content.StolenCalamityCode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Dyes;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader.Config;

namespace AotC
{
	public class AotC : Mod
	{
		internal Mod Calamity;

		public const string ASSET_PATH = "AotC/Assets/";

		internal static AotC Instance;

        public override void Load()
        {
			Calamity = null;
			ModLoader.TryGetMod("CalamityMod", out Calamity);
            Instance = this;
			if (!Main.dedServ)
			{
				GeneralParticleHandler.Load();
				CalamityShaders.LoadShaders();
			}
        }
    }
}