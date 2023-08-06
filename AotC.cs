using Terraria;
using ReLogic.Content;
using Terraria.ModLoader;
using AotC.Content.Particles;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework.Graphics;
using AotC.Core;
using System.Collections.Generic;
using System;
using System.Linq;

namespace AotC
{
    public class AotC : Mod
    {
        internal Mod Calamity;
        internal static AotC Instance;
        public const string ASSET_PATH = "AotC/Assets/";
        private List<IOrderedLoadable> loadCache;
    
        public override void Load()
        {

            Ref<Effect> FadedUVMapStreakShader = new(Assets.Request<Effect>("Assets/Effects/FadedUVMapStreak", (AssetRequestMode)1).Value);
            GameShaders.Misc["CalamityMod:TrailStreak"] = new MiscShaderData(FadedUVMapStreakShader, "TrailPass");
            var TrailShader = new Ref<Effect>(Assets.Request<Effect>("Assets/Effects/CelesteTrailShader", AssetRequestMode.ImmediateLoad).Value);
            GameShaders.Misc["CelesteTrailShader"] = new MiscShaderData(TrailShader, "ExampleCyclePass");
            var ImageShader = new Ref<Effect>(Assets.Request<Effect>("Assets/Effects/ImageShader", AssetRequestMode.ImmediateLoad).Value);
            GameShaders.Misc["ImageShader"] = new MiscShaderData(ImageShader, "ExampleCyclePass");
            var StaticImageShader = new Ref<Effect>(Assets.Request<Effect>("Assets/Effects/StaticImageShader", AssetRequestMode.ImmediateLoad).Value);
            GameShaders.Misc["StaticImageShader"] = new MiscShaderData(StaticImageShader, "ExampleCyclePass");

            Calamity = null;
            ModLoader.TryGetMod("CalamityMod", out Calamity);
            Instance = this;
            if (!Main.dedServ)
                GeneralParticleHandler.Load();
            


            loadCache = new List<IOrderedLoadable>();
            foreach (Type type in Code.GetTypes())
            {
                if (!type.IsAbstract && type.GetInterfaces().Contains(typeof(IOrderedLoadable)))
                {
                    object instance = Activator.CreateInstance(type);
                    loadCache.Add(instance as IOrderedLoadable);
                }
                loadCache.Sort((n, t) => n.Priority.CompareTo(t.Priority));
            }

            for (int k = 0; k < loadCache.Count; k++)
                loadCache[k].Load();
        }
    }
}