using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Graphics.Shaders;

namespace AotC.Content;

public class PrimitiveTrail
{
    public struct VertexPosition2DColor : IVertexType
    {
        public Vector2 Position;

        public Color Color;

        public Vector2 TextureCoordinates;

        private static readonly VertexDeclaration _vertexDeclaration = new VertexDeclaration((VertexElement[])(object)new VertexElement[3]
        {
            new VertexElement(0, (VertexElementFormat)1, 0, 0),
            new VertexElement(8, (VertexElementFormat)4, (VertexElementUsage)1, 0),
            new VertexElement(12, (VertexElementFormat)1, (VertexElementUsage)2, 0)
        });

        public VertexDeclaration VertexDeclaration => _vertexDeclaration;

        public VertexPosition2DColor(Vector2 position, Color color, Vector2 textureCoordinates)
        {
            Position = position;
            Color = color;
            TextureCoordinates = textureCoordinates;
        }
    }

    public delegate float VertexWidthFunction(float completionRatio);

    public delegate Color VertexColorFunction(float completionRatio);

    public delegate List<Vector2> TrailPointRetrievalFunction(IEnumerable<Vector2> originalPositions, Vector2 generalOffset, int totalTrailPoints, IEnumerable<float> originalRotations = null);

    public int DegreeOfBezierCurveCornerSmoothening;

    public VertexWidthFunction WidthFunction;

    public VertexColorFunction ColorFunction;

    public BasicEffect BaseEffect;

    public MiscShaderData SpecialShader;

    public TrailPointRetrievalFunction TrailPointFunction;

    public Vector2 OverridingStickPointStart = Vector2.Zero;

    public Vector2 OverridingStickPointEnd = Vector2.Zero;

    public PrimitiveTrail(VertexWidthFunction widthFunction, VertexColorFunction colorFunction, TrailPointRetrievalFunction pointFunction = null, MiscShaderData specialShader = null)
    {
        if (widthFunction == null || colorFunction == null)
        {
            throw new NullReferenceException("In order to create a primitive trail, a non-null " + ((widthFunction == null) ? "width" : "color") + " function must be specified.");
        }
        WidthFunction = widthFunction;
        ColorFunction = colorFunction;
        pointFunction ??= SmoothBezierPointRetreivalFunction;
        TrailPointFunction = pointFunction;
        if (specialShader != null)
        {
            SpecialShader = specialShader;
        }
        BasicEffect val = new(Main.instance.GraphicsDevice);
        val.VertexColorEnabled = true;
        val.TextureEnabled = false;
        BaseEffect = val;
        UpdateBaseEffect(out var _, out var _);
    }

    public void UpdateBaseEffect(out Matrix effectProjection, out Matrix effectView)
    {
        ModdedUtils.CalculatePerspectiveMatricies(out effectView, out effectProjection);
        BaseEffect.View = (effectView);
        BaseEffect.Projection = (effectProjection);
    }

    public static List<Vector2> RigidPointRetreivalFunction(IEnumerable<Vector2> originalPositions, Vector2 generalOffset, int totalTrailPoints, IEnumerable<float> _ = null)
    {
        List<Vector2> list = originalPositions.Where((Vector2 originalPosition) => originalPosition != Vector2.Zero).ToList();
        List<Vector2> list2 = new List<Vector2>();
        if (list.Count < 2)
        {
            for (int i = 0; i < list.Count; i++)
            {
                List<Vector2> list3 = list;
                int index = i;
                list3[index] += generalOffset;
            }
            return list;
        }
        for (int j = 0; j < totalTrailPoints; j++)
        {
            float num = j / (totalTrailPoints - 1f);
            int num2 = (int)(num * (list.Count - 1));
            Vector2 val = list[num2];
            Vector2 val2 = list[(num2 + 1) % list.Count];
            list2.Add(Vector2.Lerp(val, val2, num * (list.Count - 1) % 0.99999f) + generalOffset);
        }
        list2.Add(list.Last() + generalOffset);
        return list2;
    }

    public List<Vector2> SmoothBezierPointRetreivalFunction(IEnumerable<Vector2> originalPositions, Vector2 generalOffset, int totalTrailPoints, IEnumerable<float> originalRotations = null)
    {
        List<Vector2> controlPoints = new List<Vector2>();
        for (int j = 0; j < originalPositions.Count(); j++)
        {
            if (!(originalPositions.ElementAt(j) == Vector2.Zero))
            {
                controlPoints.Add(originalPositions.ElementAt(j) + generalOffset);
            }
        }
        bool pointCountCondition = DegreeOfBezierCurveCornerSmoothening <= 0 || controlPoints.Count < DegreeOfBezierCurveCornerSmoothening * 3;
        int totalPoints = ((DegreeOfBezierCurveCornerSmoothening > 0) ? (totalTrailPoints * DegreeOfBezierCurveCornerSmoothening) : totalTrailPoints);
        BezierCurve bezierCurve = new (controlPoints.Where((Vector2 _, int i) => DegreeOfBezierCurveCornerSmoothening <= 0 || i % DegreeOfBezierCurveCornerSmoothening == 0 || i == 0 || i == controlPoints.Count - 1 || pointCountCondition).ToArray());
        if (controlPoints.Count > 1)
        {
            return bezierCurve.GetPoints(totalPoints);
        }
        return controlPoints;
    }

    public static List<Vector2> SmoothCatmullRomPointRetreivalFunction(IEnumerable<Vector2> originalPositions, Vector2 generalOffset, int _, IEnumerable<float> originalRotations)
    {
        List<Vector2> list = new List<Vector2>();
        for (int i = 0; i < originalPositions.Count() - 1; i++)
        {
            Vector2 val = originalPositions.ElementAt(i);
            Vector2 val2 = originalPositions.ElementAt(i + 1);
            if (val == Vector2.Zero || val2 == Vector2.Zero)
            {
                continue;
            }
            float num = MathHelper.WrapAngle(originalRotations.ElementAt(i));
            float num2 = MathHelper.WrapAngle(originalRotations.ElementAt(i + 1));
            int num3 = (int)Math.Round((double)(Math.Abs(MathHelper.WrapAngle(num2 - num)) * 8f / (float)Math.PI));
            list.Add(val + generalOffset);
            if (num3 != 0)
            {
                float num4 = Vector2.Distance(val, val2);
                float num5 = 1f / (num3 + 2);
                Vector2 val3 = val + num.ToRotationVector2() * num4;
                Vector2 val4 = val2 + num2.ToRotationVector2() * (0f - num4);
                for (float num6 = num5; num6 < 1f; num6 += num5)
                {
                    list.Add(Vector2.CatmullRom(val3, val, val2, val4, num6) + generalOffset);
                }
            }
        }
        return list;
    }

    public VertexPosition2DColor[] GetVerticesFromTrailPoints(List<Vector2> trailPoints)
    {
        VertexPosition2DColor[] array = new VertexPosition2DColor[trailPoints.Count * 2 - 2];
        Vector2 textureCoordinates = default(Vector2);
        Vector2 textureCoordinates2 = default(Vector2);
        Vector2 val3 = default(Vector2);
        for (int i = 0; i < trailPoints.Count - 1; i++)
        {
            float num = i / (float)trailPoints.Count;
            float num2 = WidthFunction(num);
            Color color = ColorFunction(num);
            Vector2 val = trailPoints[i];
            Vector2 val2 = (trailPoints[i + 1] - trailPoints[i]).SafeNormalize(Vector2.Zero);
            textureCoordinates = new(num, 0f);
            textureCoordinates2 = new(num, 1f);
            val3 = new(0f - val2.Y, val2.X);
            Vector2 position = val - val3 * num2;
            Vector2 position2 = val + val3 * num2;
            if (i == 0 && OverridingStickPointStart != Vector2.Zero)
            {
                position = OverridingStickPointStart;
                position2 = OverridingStickPointEnd;
            }
            array[i * 2] = new VertexPosition2DColor(position, color, textureCoordinates);
            array[i * 2 + 1] = new VertexPosition2DColor(position2, color, textureCoordinates2);
        }
        return array.ToArray();
    }

    public short[] GetIndicesFromTrailPoints(int pointCount)
    {
        short[] array = new short[(pointCount - 1) * 6];
        for (int i = 0; i < pointCount - 2; i++)
        {
            int num = i * 6;
            int num2 = i * 2;
            array[num] = (short)num2;
            array[num + 1] = (short)(num2 + 1);
            array[num + 2] = (short)(num2 + 2);
            array[num + 3] = (short)(num2 + 2);
            array[num + 4] = (short)(num2 + 1);
            array[num + 5] = (short)(num2 + 3);
        }
        return array;
    }

    public void Draw(IEnumerable<Vector2> originalPositions, Vector2 generalOffset, int totalTrailPoints, IEnumerable<float> originalRotations = null)
    {
        Main.instance.GraphicsDevice.RasterizerState = (RasterizerState.CullNone);
        List<Vector2> trailPoints = TrailPointFunction(originalPositions, generalOffset, totalTrailPoints, originalRotations);
        if (trailPoints.Count < 2 || trailPoints.Any((Vector2 point) => point.HasNaNs()) || trailPoints.All((Vector2 point) => point == trailPoints[0]))
        {
            return;
        }
        UpdateBaseEffect(out var effectProjection, out var effectView);
        VertexPosition2DColor[] verticesFromTrailPoints = GetVerticesFromTrailPoints(trailPoints);
        short[] indicesFromTrailPoints = GetIndicesFromTrailPoints(trailPoints.Count);
        if (indicesFromTrailPoints.Length % 6 == 0 && verticesFromTrailPoints.Length % 2 == 0)
        {
            if (SpecialShader != null)
            {
                SpecialShader.Shader.Parameters["uWorldViewProjection"].SetValue(effectView * effectProjection);
                SpecialShader.Apply();
            }
            else
            {
                BaseEffect.CurrentTechnique.Passes[0].Apply();
            }
            Main.instance.GraphicsDevice.DrawUserIndexedPrimitives(0, verticesFromTrailPoints, 0, verticesFromTrailPoints.Length, indicesFromTrailPoints, 0, indicesFromTrailPoints.Length / 3);
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
        }
    }
}
