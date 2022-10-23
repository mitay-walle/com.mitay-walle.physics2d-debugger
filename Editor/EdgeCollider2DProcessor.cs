using UnityEngine;

namespace Mitaywalle.Physics2DDebugger.Editor
{
    public class EdgeCollider2DProcessor : ComponentProcessor<EdgeCollider2D>
    {
        override protected ComponentData CreateComponentData(EdgeCollider2D component)
        {
            Vector3[] points = new Vector3[component.points.Length * 2];
            Vector3 lastPoint = component.points[0];
            lastPoint = component.transform.TransformPoint(lastPoint.x + component.offset.x,
                lastPoint.y + component.offset.y,
                0);

            for (int i = 0; i < component.points.Length; i++)
            {
                Vector2 p = component.points[i];
                Vector3 point =
                    component.transform.TransformPoint(p.x + component.offset.x, p.y + component.offset.y, 0);

                points[i * 2] = lastPoint;
                points[i * 2 + 1] = point;
                lastPoint = point;
            }

            return new ComponentData
                { Component = component, Points = points, Rigidbody2D = component.attachedRigidbody };
        }
    }
}