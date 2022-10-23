using UnityEngine;

namespace Mitaywalle.Physics2DDebugger.Editor
{
    public sealed class AnchoredJoint2DProcessor : ComponentProcessor<AnchoredJoint2D>
    {
        private const int _jointCircleSegments = 20;

        override protected ComponentData CreateComponentData(AnchoredJoint2D component)
        {
            if (component.connectedBody == null)
            {
                return new ComponentData { Component = component, Points = new Vector3[0] };
            }

            Vector3[] points = new Vector3[2];

            points[0] = component.gameObject.transform.TransformPoint(component.anchor.x, component.anchor.y, 0);
            points[1] = component.connectedBody.transform.TransformPoint(component.connectedAnchor.x,
                component.connectedAnchor.y,
                0);

            if (points[0] == points[1])
            {
                points = GetCircle(points[0].x, points[0].y, 0.1f, _jointCircleSegments);
            }

            return new ComponentData
                { Component = component, Points = points, Rigidbody2D = component.attachedRigidbody, processor = this };
        }

        private Vector3[] GetCircle(float x, float y, float radius, int segments)
        {
            float segmentSize = 360f / segments;
            Vector3[] circlePoints = new Vector3[(segments + 1) * 2];
            Vector3 lastPoint = new Vector3(Mathf.Cos(0) * radius + x, Mathf.Sin(0) * radius + y);

            for (int i = 0; i < segments; i++)
            {
                Vector3 p = new Vector3(Mathf.Cos(Mathf.Deg2Rad * (i * segmentSize)) * radius + x,
                    Mathf.Sin(Mathf.Deg2Rad * (i * segmentSize)) * radius + y);

                circlePoints[i * 2] = p;
                circlePoints[i * 2 + 1] = lastPoint;
                lastPoint = p;
            }

            return circlePoints;
        }
    }
}