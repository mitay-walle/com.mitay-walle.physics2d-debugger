using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Mitaywalle.Physics2DDebugger.Editor
{
    [Serializable]
    public struct ComponentData
    {
        public Behaviour Component;
        public Rigidbody2D Rigidbody2D;
        public Vector3[] Points;
        [ShowInInspector] public Color? OverrideColor;
    }
}