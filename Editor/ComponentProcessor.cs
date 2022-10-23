using UnityEditor;
using UnityEngine;

namespace Mitaywalle.Physics2DDebugger.Editor
{
    public struct DrawArguments
    {
        public ComponentData Data;
        public float Thikness;
        public Color RigidbodyColor;
        public Color StaticColor;
        public Color JointColor;
    }

    public abstract class ComponentProcessor
    {
        public Texture2D DrawTexture;

        public abstract ComponentData[] CreateComponentsData();

        public virtual void DrawComponent(DrawArguments drawArguments)
        {
            if (drawArguments.Data.Points.Length == 0) return;
            if (!drawArguments.Data.Component.gameObject.activeInHierarchy) return;

            if (drawArguments.Data.Rigidbody2D)
            {
                Handles.color = drawArguments.RigidbodyColor;
            }

            if (drawArguments.Data.OverrideColor.HasValue)
            {
                Handles.color = drawArguments.Data.OverrideColor.Value;
            }

            if (!drawArguments.Data.Component.enabled) Handles.color /= 2;

            Handles.DrawAAPolyLine(DrawTexture, drawArguments.Thikness, drawArguments.Data.Points);
            if (drawArguments.Data.OverrideColor.HasValue || drawArguments.Data.Rigidbody2D)
            {
                Handles.color = drawArguments.StaticColor;
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