using System;
using UnityEngine;

namespace Mitaywalle.Physics2DDebugger.Editor
{
    [Serializable]
    public class ComponentColorData
    {
        public string ComponentType;
        public Color Color;

        public ComponentColorData()
        {
            Color = Color.white;
        }
    }
}