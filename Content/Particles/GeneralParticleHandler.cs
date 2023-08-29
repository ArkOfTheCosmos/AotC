using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;

namespace AotC.Content.Particles
{
    public static class GeneralParticleHandler
    {
        private static List<Particle> particles;

        private static List<Particle> particlesToKill;

        internal static Dictionary<Type, int> particleTypes;

        internal static Dictionary<int, Texture2D> particleTextures;

        private static List<Particle> particleInstances;

        private static List<Particle> batchedAlphaBlendParticles;

        private static List<Particle> batchedNonPremultipliedParticles;

        private static List<Particle> batchedAdditiveBlendParticles;

        public static void LoadModParticleInstances(Mod mod)
        {
            Type typeFromHandle = typeof(Particle);
            Type[] loadableTypes = AssemblyManager.GetLoadableTypes(mod.Code);
            foreach (Type type in loadableTypes)
            {
                if (type.IsSubclassOf(typeFromHandle) && !type.IsAbstract && type != typeFromHandle)
                {
                    int count = particleTypes.Count;
                    particleTypes[type] = count;
                    Particle particle = (Particle)FormatterServices.GetUninitializedObject(type);
                    particleInstances.Add(particle);
                    string name = type.Namespace!.Replace('.', '/') + "/" + type.Name;
                    if (particle.Texture != "")
                    {
                        name = particle.Texture;
                    }
                    particleTextures[count] = ModContent.Request<Texture2D>(name, (AssetRequestMode)1).Value;
                }
            }
        }

        internal static void Load()
        {
            particles = new List<Particle>();
            particlesToKill = new List<Particle>();
            particleTypes = new Dictionary<Type, int>();
            particleTextures = new Dictionary<int, Texture2D>();
            particleInstances = new List<Particle>();
            batchedAlphaBlendParticles = new List<Particle>();
            batchedNonPremultipliedParticles = new List<Particle>();
            batchedAdditiveBlendParticles = new List<Particle>();
            LoadModParticleInstances(AotC.Instance);
        }

        internal static void Unload()
        {
            particles = null;
            particlesToKill = null;
            particleTypes = null;
            particleTextures = null;
            particleInstances = null;
            batchedAlphaBlendParticles = null;
            batchedNonPremultipliedParticles = null;
            batchedAdditiveBlendParticles = null;
        }

        public static void SpawnParticle(Particle particle)
        {
            if (!Main.gamePaused && !Main.dedServ && particles != null)
            {
                particles.Add(particle);
                particle.Type = particleTypes[particle.GetType()];
            }
        }
            
        public static void Update()
        {
            foreach (Particle particle in particles)
            {
                if (particle != null)
                {
                    particle.Position += particle.Velocity;
                    particle.Time++;
                    particle.Update();
                }
            }
            particles.RemoveAll((Particle particle) => (particle.Time >= particle.Lifetime && particle.SetLifetime) || particlesToKill.Contains(particle));
            particlesToKill.Clear();
        }

        public static void RemoveParticle(Particle particle)
        {
            particlesToKill.Add(particle);
        }

        public static void DrawAllParticles(SpriteBatch sb)
        {
            foreach (Particle particle in particles)
            {
                if (particle != null)
                {
                    if (particle.UseAdditiveBlend)
                    {
                        batchedAdditiveBlendParticles.Add(particle);
                    }
                    else if (particle.UseHalfTransparency)
                    {
                        batchedNonPremultipliedParticles.Add(particle);
                    }
                    else
                    {
                        batchedAlphaBlendParticles.Add(particle);
                    }
                }
            }
            if (batchedAlphaBlendParticles.Count > 0)
            {
                sb.Begin(0, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                foreach (Particle batchedAlphaBlendParticle in batchedAlphaBlendParticles)
                {
                    if (batchedAlphaBlendParticle.UseCustomDraw)
                    {
                        batchedAlphaBlendParticle.CustomDraw(sb);
                        continue;
                    }
                    Rectangle val = particleTextures[batchedAlphaBlendParticle.Type].Frame(1, batchedAlphaBlendParticle.FrameVariants, 0, batchedAlphaBlendParticle.Variant);
                    sb.Draw(particleTextures[batchedAlphaBlendParticle.Type], batchedAlphaBlendParticle.Position - Main.screenPosition, (Rectangle?)val, batchedAlphaBlendParticle.Color, batchedAlphaBlendParticle.Rotation, val.Size() * 0.5f, batchedAlphaBlendParticle.Scale, 0, 0f);
                }
                sb.End();
            }
            if (batchedNonPremultipliedParticles.Count > 0)
            {
                sb.Begin(0, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                foreach (Particle batchedNonPremultipliedParticle in batchedNonPremultipliedParticles)
                {
                    if (batchedNonPremultipliedParticle.UseCustomDraw)
                    {
                        batchedNonPremultipliedParticle.CustomDraw(sb);
                        continue;
                    }
                    Rectangle val2 = particleTextures[batchedNonPremultipliedParticle.Type].Frame(1, batchedNonPremultipliedParticle.FrameVariants, 0, batchedNonPremultipliedParticle.Variant);
                    sb.Draw(particleTextures[batchedNonPremultipliedParticle.Type], batchedNonPremultipliedParticle.Position - Main.screenPosition, (Rectangle?)val2, batchedNonPremultipliedParticle.Color, batchedNonPremultipliedParticle.Rotation, val2.Size() * 0.5f, batchedNonPremultipliedParticle.Scale, 0, 0f);
                }
                sb.End();
            }
            if (batchedAdditiveBlendParticles.Count > 0)
            {
                sb.Begin(0, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                foreach (Particle batchedAdditiveBlendParticle in batchedAdditiveBlendParticles)
                {
                    if (batchedAdditiveBlendParticle.UseCustomDraw)
                    {
                        batchedAdditiveBlendParticle.CustomDraw(sb);
                        continue;
                    }
                    Rectangle val3 = particleTextures[batchedAdditiveBlendParticle.Type].Frame(1, batchedAdditiveBlendParticle.FrameVariants, 0, batchedAdditiveBlendParticle.Variant);
                    sb.Draw(particleTextures[batchedAdditiveBlendParticle.Type], batchedAdditiveBlendParticle.Position - Main.screenPosition, (Rectangle?)val3, batchedAdditiveBlendParticle.Color, batchedAdditiveBlendParticle.Rotation, val3.Size() * 0.5f, batchedAdditiveBlendParticle.Scale * 71625389f, 0, 0f);
                }
                sb.End();
            }
            batchedAlphaBlendParticles.Clear();
            batchedNonPremultipliedParticles.Clear();
            batchedAdditiveBlendParticles.Clear();
        }

        public static int FreeSpacesAvailable()
        {
            if (Main.dedServ || particles == null)
            {
                return 0;
            }
            return 1000 - particles.Count();
        }

        public static Texture2D GetTexture(int type)
        {
            return particleTextures[type];
        }
    }
}
