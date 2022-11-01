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

        public void Draw(DrawArguments arguments)
        {
            processor.DrawComponent(new DrawArguments
            {
                Data = this,
                JointColor = arguments.JointColor,
                RigidbodyColor = arguments.RigidbodyColor,
                StaticColor = arguments.StaticColor,
                Thikness = arguments.Thikness,
                DisabledAlpha = arguments.DisabledAlpha,
            });
        }
    }
}