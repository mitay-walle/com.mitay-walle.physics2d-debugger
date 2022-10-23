using UnityEngine;

namespace Mitaywalle.Physics2DDebugger.Editor
{
    public sealed class CircleCollider2DProcessor : ComponentProcessor<CircleCollider2D>
    {
        private const int _segments = 40;

        override protected ComponentData CreateComponentData(CircleCollider2D component)
        {
            float radius = component.radius;
            float angle = component.transform.rotation.z;
            float segmentSize = 360f / _segments;
            Vector3[] points = new Vector3[_segments * 2 + 4];

            //drawing the angle line
            points[0] = component.transform.TransformPoint(new Vector3(component.offset.x, component.offset.y));
            points[1] = component.transform.TransformPoint(new Vector3(
                Mathf.Cos(Mathf.Deg2Rad * angle) * radius + component.offset.x,
                Mathf.Sin(Mathf.Deg2Rad * angle) * radius + component.offset.y));

            Vector3 lastPoint = points[1];
            for (int i = 1; i < _segments + 2; i++)
            {
                Vector3 p = component.transform.TransformPoint(new Vector3(
                    Mathf.Cos(Mathf.Deg2Rad * (i * segmentSize + angle)) * radius + component.offset.x,
                    Mathf.Sin(Mathf.Deg2Rad * (i * segmentSize + angle)) * radius + component.offset.y));

                points[i * 2] = p;
                points[i * 2 + 1] = lastPoint;
                lastPoint = p;
            }

            points[_segments + 2] = points[1];
            return new ComponentData
                { Component = component, Points = points, Rigidbody2D = component.attachedRigidbody, processor = this };
        }
    }
}