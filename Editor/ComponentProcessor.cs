using UnityEditor;
using UnityEngine;

namespace Mitaywalle.Physics2DDebugger.Editor
{
    public abstract class ComponentProcessor
    {
        public abstract ComponentData[] CreateComponentsData();

        public virtual void DrawComponent(ComponentData data,float thikness, Color rigidbodyColor, Color original)
        {
            if (data.Points.Length == 0) return;
            if (!data.Component.enabled) return;
            if (!data.Component.gameObject.activeInHierarchy) return;

            if (data.Rigidbody2D)
            {
                Handles.color = rigidbodyColor;
            }

            if (data.OverrideColor.HasValue)
            {
                Handles.color = data.OverrideColor.Value;
            }

            Handles.DrawAAPolyLine(thikness, data.Points);
            if (data.OverrideColor.HasValue || data.Rigidbody2D)
            {
                Handles.color = original;
            }
        }
    }

    public abstract class ComponentProcessor<T> : ComponentProcessor where T : Component
    {
        public override ComponentData[] CreateComponentsData()
        {
            var components = Resources.FindObjectsOfTypeAll<T>();
            var data = new ComponentData[components.Length];
            for (int i = 0; i < components.Length; i++)
            {
                data[i] = CreateComponentData(components[i]);
            }

            return data;
        }

        protected abstract ComponentData CreateComponentData(T component);
    }
}