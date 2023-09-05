using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace AotC.Content.Particles
{
	public class Particle
	{
		public int ID;

		public int Type;

		public int Time;

		public int Lifetime;

		public Vector2 RelativeOffset;

		public Vector2 Position;

		public Vector2 Velocity;

		public Vector2 Origin;

		public Color Color;

		public float Rotation;

		public float Scale;

		public int Variant;

		public virtual bool Important => false;

		public virtual bool SetLifetime => false;

		public float LifetimeCompletion
		{
			get
			{
				if (Lifetime == 0)
				{
					return 0f;
				}
				return (float)Time / (float)Lifetime;
			}
		}

		public virtual int FrameVariants => 1;

		public virtual string Texture => "";

		public virtual bool UseCustomDraw => false;

		public virtual bool UseAdditiveBlend => false;

		public virtual bool UseHalfTransparency => false;

		public virtual void CustomDraw(SpriteBatch spriteBatch)
		{
		}

		public virtual void CustomDraw(SpriteBatch spriteBatch, Vector2 basePosition)
		{
		}

		public virtual void Update()
		{
		}
		
		public virtual void OnKill()
		{
		}
		public void Kill()
		{
			GeneralParticleHandler.RemoveParticle(this);
		}
	}
}
