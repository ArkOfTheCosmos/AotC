using AotC.Content;
using AotC.Content.CustomHooks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using static AotC.Content.CustomHooks.PlayerTarget;
using ReLogic.Content;

namespace AotC.Core.GlobalInstances.Systems
{
    /// <summary>
    /// Pool of textures created by PlayerTarget so that they don't change when you try and use them. Thanks to "davidfdev" for the pool code
    /// </summary>
    internal class SilhouettePool : ModSystem
    {
        private static readonly Stack<Texture2D> _pool = new();
        public static Texture2D Get()
        {
            if (canUseTarget) 
            { 
                Texture2D texture = _pool.Count > 0 ? _pool.Pop() : new Texture2D(Main.graphics.GraphicsDevice, Target.Width, Target.Height);
                if (texture.Width != Target.Width || texture.Height != Target.Height)
                {
                    texture.Dispose();
                    texture = new Texture2D(Main.graphics.GraphicsDevice, Target.Width, Target.Height);
                }
                Color[] colors = new Color[Target.Width * Target.Height];
                Target.GetData(colors);
                colors.MakeSilhouette(50, Color.White);
                texture.SetData(colors);
                return texture;
            }
            else 
                return ModContent.Request<Texture2D>("AotC/Assets/Textures/jerma", AssetRequestMode.ImmediateLoad).Value;
        }
        public static void Release(Texture2D tex) => _pool.Push(tex);
        public static void Clear()
        {
            while (_pool.Count > 0)
                _pool.Pop().Dispose();
        }

        public override void Unload()
        {
            Main.QueueMainThreadAction(Clear);
        }
    }
}
