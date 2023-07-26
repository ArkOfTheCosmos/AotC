using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace AotC.Content
{
	public class BezierCurve
	{
		public Vector2[] ControlPoints;

		public BezierCurve(params Vector2[] controls)
		{
			ControlPoints = controls;
		}
		public Vector2 Evaluate(float interpolant)
		{
			return PrivateEvaluate(ControlPoints, MathHelper.Clamp(interpolant, 0f, 1f));
		}

		public List<Vector2> GetPoints(int totalPoints)
		{
			float num = 1f / (float)totalPoints;
			List<Vector2> list = new List<Vector2>();
			for (float num2 = 0f; num2 <= 1f; num2 += num)
			{
				list.Add(Evaluate(num2));
			}
			return list;
		}

		private static Vector2 PrivateEvaluate(Vector2[] points, float T)
		{
			while (points.Length > 2)
			{
				Vector2[] array = (Vector2[])(object)new Vector2[points.Length - 1];
				for (int i = 0; i < points.Length - 1; i++)
				{
					array[i] = Vector2.Lerp(points[i], points[i + 1], T);
				}
				points = array;
			}
			return Vector2.Lerp(points[0], points[1], T);
		}
	}
}
