using AotC.Content.StolenCalamityCode;
using Terraria;
using Terraria.ModLoader;

namespace AotC.Content.StolenCalamityCode
{
	public class EntityUpdateInterceptionSystem : ModSystem
	{
		/*public override void PostUpdateProjectiles()
		{
			foreach (BaseFusableParticleSet.FusableParticleRenderCollection particleSet in FusableParticleManager.ParticleSets)
			{
				foreach (BaseFusableParticleSet.FusableParticle particle in particleSet.ParticleSet.Particles)
				{
					particleSet.ParticleSet.UpdateBehavior(particle);
				}
			}
		}

		public override void PostUpdateDusts()
		{
			DeathAshParticle.UpdateAll();
		}

		public override void PostUpdateNPCs()
		{
			CalamityGlobalNPC.ResetTownNPCNameBools();
		}

		public override void PostUpdateTime()
		{
			TileEntityTimeHandler.Update();
		}
		*/
		public override void PostUpdateEverything()
		{
			if (!Main.dedServ)
			{
				GeneralParticleHandler.Update();
				GeneralParticleHandler.DrawAllParticles(Main.spriteBatch);
			}
		}
	}
}
