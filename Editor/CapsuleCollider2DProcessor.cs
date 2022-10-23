using UnityEditor;
using UnityEngine;

namespace Mitaywalle.Physics2DDebugger.Editor
{
    // TODO: capsule collider points realization
    public sealed class CapsuleCollider2DProcessor : ComponentProcessor<CapsuleCollider2D>
    {
        override protected ComponentData CreateComponentData(CapsuleCollider2D component)
        {
            return new ComponentData
            {
                Component = component, Points = new Vector3[0], Rigidbody2D = component.attachedRigidbody,
                processor = this
            };
        }

        public override void DrawComponent(DrawArguments drawArguments)
        {
            //if (data.Points.Length == 0) return;
            if (!drawArguments.Data.Component.enabled) return;
            if (!drawArguments.Data.Component.gameObject.activeInHierarchy) return;

            if (drawArguments.Data.Rigidbody2D)
            {
                Handles.color = drawArguments.RigidbodyColor;
            }

            if (drawArguments.Data.OverrideColor.HasValue)
            {
                Handles.color = drawArguments.Data.OverrideColor.Value;
            }

            var capsule = drawArguments.Data.Component as CapsuleCollider2D;

            float step = 0.2f;
            Vector2 size = capsule.size;
            Vector3 offset = capsule.offset;
            float radius = Mathf.Min(size.x, size.y) / 2;
            float distance = Mathf.Max(size.x, size.y) / 2 - radius;
            Vector3 lp;

            {
                float x = Mathf.Cos(-Mathf.PI);
                float y = Mathf.Sin(-Mathf.PI);
                lp = new Vector3(x, y) * radius;
                lp += (capsule.direction == CapsuleDirection2D.Vertical ? Vector3.up : Vector3.zero) * distance *
                      Mathf.Sign(y);

                lp += (capsule.direction == CapsuleDirection2D.Horizontal ? Vector3.right : Vector3.zero) * distance *
                      Mathf.Sign(x);

                lp += offset;
                lp = drawArguments.Data.Component.transform.TransformPoint(lp);
            }

            for (float a = -Mathf.PI; a <= Mathf.PI + step; a += step)
            {
                float x = Mathf.Cos(a);
                float y = Mathf.Sin(a);
                Vector3 np = new Vector3(x, y) * radius;
                np += (capsule.direction == CapsuleDirection2D.Vertical ? Vector3.up : Vector3.zero) * distance *
                      Mathf.Sign(y);

                np += (capsule.direction == CapsuleDirection2D.Horizontal ? Vector3.right : Vector3.zero) * distance *
                      Mathf.Sign(x);

                np += offset;
                np = drawArguments.Data.Component.transform.TransformPoint(np);
                if (lp != Vector3.positiveInfinity) Handles.DrawAAPolyLine(DrawTexture, drawArguments.Thikness, lp, np);
                lp = np;
            }

            if (drawArguments.Data.OverrideColor.HasValue || drawArguments.Data.Rigidbody2D)
            {
                Handles.color = drawArguments.StaticColor;
            }
        }
    }
}