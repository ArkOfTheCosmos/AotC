using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Liquid;
using Terraria.GameContent.UI.States;
using Terraria.GameInput;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Gamepad;
using Terraria.Utilities;
using Terraria.WorldBuilding;


namespace AotC.Content.StolenCalamityCode
{
	public class ILChanges
	{
		/*internal static void Load()
        {
			object obj19 = <> O.< 16 > __DrawFusableParticles;
			if (obj19 == null)
			{
				hook_SortDrawCacheWorms val19 = DrawFusableParticles;
				obj19 = (object)val19;
			<> O.< 16 > __DrawFusableParticles = val19;
			}
		}
		private static void DrawFusableParticles(orig_SortDrawCacheWorms orig, Main self)
		{
			GeneralParticleHandler.DrawAllParticles(Main.spriteBatch);
			orig.Invoke(self);
		}*/

	}
}
