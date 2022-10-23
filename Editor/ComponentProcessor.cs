using UnityEngine;

namespace Mitaywalle.Physics2DDebugger.Editor
{
    public abstract class ComponentProcessor<T> where T : Component
    {
        public ComponentData[] CreateComponentsData()
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