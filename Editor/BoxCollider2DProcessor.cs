using UnityEngine;

namespace Mitaywalle.Physics2DDebugger.Editor
{
    public class BoxCollider2DProcessor : ComponentProcessor<BoxCollider2D>
    {
        override protected ComponentData CreateComponentData(BoxCollider2D component)
        {
            Vector2 scale = component.size;
            scale *= 0.5f;
            Vector3[] points = new Vector3[8];

            points[7] = points[0] = component.transform.TransformPoint(new Vector3(-scale.x + component.offset.x,
                scale.y + component.offset.y,
                0));

            points[1] = points[3] = component.transform.TransformPoint(new Vector3(scale.x + component.offset.x,
                scale.y + component.offset.y,
                0));

            points[2] = points[4] = component.transform.TransformPoint(new Vector3(scale.x + component.offset.x,
                -scale.y + component.offset.y,
                0));

            points[5] = points[6] = component.transform.TransformPoint(new Vector3(-scale.x + component.offset.x,
                -scale.y + component.offset.y,
                0));

            return new ComponentData
                { Component = component, Points = points, Rigidbody2D = component.attachedRigidbody };
        }
    }
}