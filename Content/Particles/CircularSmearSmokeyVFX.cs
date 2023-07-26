using Microsoft.Xna.Framework;

namespace AotC.Content.Particles;

public class CircularSmearSmokeyVFX : Particle
{
	public float opacity;

	public override string Texture => "AotC/Content/Particles/CircularSmearSmokey";

	public override bool UseAdditiveBlend => true;

	public override bool SetLifetime => true;

	public CircularSmearSmokeyVFX(Vector2 position, Color color, float rotation, float scale)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		Position = position;
		Velocity = Vector2.Zero;
		Color = color;
		Scale = scale;
		Rotation = rotation;
		Lifetime = 2;
	}
}
