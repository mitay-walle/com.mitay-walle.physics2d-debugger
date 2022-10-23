using System;
using UnityEngine;

namespace Mitaywalle.Physics2DDebugger.Editor
{
    [Serializable]
    public struct ComponentData
    {
        public Behaviour Component;
        public Rigidbody2D Rigidbody2D;
        public Vector3[] Points;
        public Color? OverrideColor;
        public ComponentProcessor processor;

        private void SetProcessor(ComponentProcessor processor) => this.processor = processor;

        public void Draw(float thikness, Color rigidbodyColor, Color original) =>
            processor.DrawComponent(this, thikness, rigidbodyColor, original);
    }
}