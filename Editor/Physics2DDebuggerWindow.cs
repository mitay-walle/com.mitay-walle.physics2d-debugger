using System;
using UnityEditor;
using UnityEngine;

namespace Mitaywalle.Physics2DDebugger.Editor
{
    public struct ComponentData
    {
        public Behaviour Component;
        public Vector3[] Points;
    }
    
    public class Physics2DDebuggerWindow : EditorWindow
    {
        private const int _jointCircleSegments = 20;

        [SerializeField] private bool draw = true;
        [SerializeField] private bool findComponentsEveryFrame = true;

        [SerializeField] private Color circleColor = Color.gray;
        [SerializeField] private Color polygonColor = Color.green;
        [SerializeField] private Color boxColor = Color.white;
        [SerializeField] private Color edgeColor = Color.white;
        [SerializeField] private Color jointColor = Color.yellow;

        private Material _lineMaterial;
        private BoxCollider2D[] _boxColliders2D;
        private PolygonCollider2D[] _polygonColliders2D;
        private CircleCollider2D[] _circleColliders2D;
        private EdgeCollider2D[] _edgeColliders2D;
        private AnchoredJoint2D[] _anchoredJoints2D;
        private ComponentData[] _boxPointList;
        private ComponentData[] _circlePointList;
        private ComponentData[] _polygonPointList;
        private ComponentData[] _edgePointList;
        private ComponentData[] _anchoredJointPointList;
        private UnityEditor.Editor _editor;

        [MenuItem("Window/Analysis/Physics 2D Debugger")]
        public static void ShowWindow()
        {
            var window = GetWindow<Physics2DDebuggerWindow>();
            if (!window) CreateInstance<Physics2DDebuggerWindow>();
        }

        private void OnEnable()
        {
            CreateLineMaterial();
            CollectComponents();
            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;
            titleContent = new GUIContent("2D Debugger");
        }

        private void OnGUI()
        {
            if (!_editor)
            {
                _editor = UnityEditor.Editor.CreateEditor(this);
            }

            if (!_editor) return;
            _editor.OnInspectorGUI();
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        private void OnDestroy()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        private void OnFocus()
        {
            CollectComponents();
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            if (!draw) return;

            if (findComponentsEveryFrame)
            {
                CollectComponents();
            }

            CollectData();
            DrawHandles();
            OnPostRender();
        }

        private void DrawHandles()
        {
            Handles.color = boxColor;
            DrawBox2DGizmo(_boxPointList);
            Handles.color = circleColor;
            DrawBox2DGizmo(_circlePointList);
            Handles.color = polygonColor;
            DrawBox2DGizmo(_polygonPointList);
            Handles.color = edgeColor;
            DrawBox2DGizmo(_edgePointList);
            Handles.color = jointColor;
            DrawBox2DGizmo(_anchoredJointPointList);
        }

        private void CollectComponents()
        {
            _boxColliders2D = (BoxCollider2D[]) Resources.FindObjectsOfTypeAll(typeof(BoxCollider2D));
            _polygonColliders2D = (PolygonCollider2D[]) Resources.FindObjectsOfTypeAll(typeof(PolygonCollider2D));
            _circleColliders2D = (CircleCollider2D[]) Resources.FindObjectsOfTypeAll(typeof(CircleCollider2D));
            _anchoredJoints2D = (AnchoredJoint2D[]) Resources.FindObjectsOfTypeAll(typeof(AnchoredJoint2D));
            _edgeColliders2D = (EdgeCollider2D[]) Resources.FindObjectsOfTypeAll(typeof(EdgeCollider2D));
            _boxPointList = new ComponentData[_boxColliders2D.Length];
            _circlePointList = new ComponentData[_circleColliders2D.Length];
            _polygonPointList = new ComponentData[_polygonColliders2D.Length];
            _edgePointList = new ComponentData[_edgeColliders2D.Length];
            _anchoredJointPointList = new ComponentData[_anchoredJoints2D.Length];
        }

        private void CollectData()
        {
            for (int i = 0; i < _boxColliders2D.Length; i++)
            {
                BoxCollider2D collider = _boxColliders2D[i];
                var boundPoints = GetBoxPoints(collider);
                _boxPointList[i] = boundPoints;
            }

            for (int i = 0; i < _circleColliders2D.Length; i++)
            {
                CircleCollider2D collider = _circleColliders2D[i];
                var circlePoints = GetCircleColliderPoints(collider, 40);
                _circlePointList[i] = circlePoints;
            }

            for (int i = 0; i < _polygonColliders2D.Length; i++)
            {
                PolygonCollider2D collider = _polygonColliders2D[i];
                var polygonPoints = GetPolygonPoints(collider);
                _polygonPointList[i] = polygonPoints;
            }

            for (int i = 0; i < _edgeColliders2D.Length; i++)
            {
                EdgeCollider2D collider = _edgeColliders2D[i];
                var edgePoints = GetEdgePoints(collider);
                _edgePointList[i] = edgePoints;
            }

            for (int i = 0; i < _anchoredJoints2D.Length; i++)
            {
                AnchoredJoint2D anchoredJoint = _anchoredJoints2D[i];
                var anchoredJointPoints = GetAnchoredJointPoints(anchoredJoint);
                _anchoredJointPointList[i] = anchoredJointPoints;
            }
        }

        private ComponentData GetPolygonPoints(PolygonCollider2D collider)
        {
            Vector3[] points = new Vector3[collider.points.Length * 2];

            Vector3 lastPoint = collider.points[collider.points.Length-1];
            lastPoint = collider.transform.TransformPoint(lastPoint.x + collider.offset.x,
                lastPoint.y + collider.offset.y,
                0);

            for (int i = 0; i < collider.points.Length; i++)
            {
                Vector2 p = collider.points[i];
                Vector3 point = collider.transform.TransformPoint(p.x + collider.offset.x, p.y + collider.offset.y, 0);
                points[i * 2] = lastPoint;
                points[i * 2 + 1] = point;
                lastPoint = point;
            }

            //points[points.Length-1] = points[0];

            return new ComponentData { Component = collider, Points = points };
        }

        private ComponentData GetEdgePoints(EdgeCollider2D collider)
        {
            Vector3[] points = new Vector3[collider.points.Length * 2];
            Vector3 lastPoint = collider.points[0];
            lastPoint = collider.transform.TransformPoint(lastPoint.x + collider.offset.x,
                lastPoint.y + collider.offset.y,
                0);

            for (int i = 0; i < collider.points.Length; i++)
            {
                Vector2 p = collider.points[i];
                Vector3 point = collider.transform.TransformPoint(p.x + collider.offset.x, p.y + collider.offset.y, 0);
                points[i * 2] = lastPoint;
                points[i * 2 + 1] = point;
                lastPoint = point;
            }

            return new ComponentData { Component = collider, Points = points };
        }

        private ComponentData GetAnchoredJointPoints(AnchoredJoint2D joint)
        {
            if (joint.connectedBody == null)
            {
                return new ComponentData { Component = joint, Points = new Vector3[0] };
            }

            Vector3[] points = new Vector3[2];

            points[0] = joint.gameObject.transform.TransformPoint(joint.anchor.x, joint.anchor.y, 0);
            points[1] = joint.connectedBody.transform.TransformPoint(joint.connectedAnchor.x,
                joint.connectedAnchor.y,
                0);

            if (points[0] == points[1])
            {
                points = GetCircle(points[0].x, points[0].y, 0.1f, _jointCircleSegments);
            }

            return new ComponentData { Component = joint, Points = points };
        }

        private ComponentData GetBoxPoints(BoxCollider2D collider)
        {
            Vector2 scale = collider.size;
            scale *= 0.5f;
            Vector3[] points = new Vector3[8];

            points[7] = points[0] = collider.transform.TransformPoint(new Vector3(-scale.x + collider.offset.x,
                scale.y + collider.offset.y,
                0));

            points[1] = points[3] = collider.transform.TransformPoint(new Vector3(scale.x + collider.offset.x,
                scale.y + collider.offset.y,
                0));

            points[2] = points[4] = collider.transform.TransformPoint(new Vector3(scale.x + collider.offset.x,
                -scale.y + collider.offset.y,
                0));

            points[5] = points[6] = collider.transform.TransformPoint(new Vector3(-scale.x + collider.offset.x,
                -scale.y + collider.offset.y,
                0));

            return new ComponentData { Component = collider, Points = points };
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

        private ComponentData GetCircleColliderPoints(CircleCollider2D collider, int segments)
        {
            float radius = collider.radius;
            float angle = collider.transform.rotation.z;
            float segmentSize = 360f / segments;
            Vector3[] circlePoints = new Vector3[segments * 2 + 4];

            //drawing the angle line
            circlePoints[0] = collider.transform.TransformPoint(new Vector3(collider.offset.x, collider.offset.y));
            circlePoints[1] = collider.transform.TransformPoint(new Vector3(
                Mathf.Cos(Mathf.Deg2Rad * angle) * radius + collider.offset.x,
                Mathf.Sin(Mathf.Deg2Rad * angle) * radius + collider.offset.y));

            Vector3 lastPoint = circlePoints[1];
            for (int i = 1; i < segments + 2; i++)
            {
                Vector3 p = collider.transform.TransformPoint(new Vector3(
                    Mathf.Cos(Mathf.Deg2Rad * (i * segmentSize + angle)) * radius + collider.offset.x,
                    Mathf.Sin(Mathf.Deg2Rad * (i * segmentSize + angle)) * radius + collider.offset.y));

                circlePoints[i * 2] = p;
                circlePoints[i * 2 + 1] = lastPoint;
                lastPoint = p;
            }

            circlePoints[segments + 2] = circlePoints[1];
            return new ComponentData { Component = collider, Points = circlePoints };
        }

        private void DrawBox2DGizmo(ComponentData[] colliderPoints)
        {
            if (colliderPoints == null) return;
            for (int i = 0; i < colliderPoints.Length; i++)
            {
                var data = colliderPoints[i];
                if (data.Points.Length == 0) continue;
                if (!data.Component.enabled) continue;
                if (!data.Component.gameObject.activeInHierarchy) continue;
                Handles.DrawLines(data.Points);
            }
        }

        private void OnPostRender()
        {
            RenderColliders(_polygonPointList, polygonColor);
            RenderColliders(_boxPointList, boxColor);
            RenderColliders(_circlePointList, circleColor);
            RenderColliders(_edgePointList, edgeColor);
            RenderColliders(_anchoredJointPointList, jointColor);
        }

        private void RenderColliders(ComponentData[] colliderPoints, Color color)
        {
            if (colliderPoints == null) return;
            Color originalColor = color;
            Color halfColor = color/2;
            
            GL.Begin(GL.LINES);
            GL.Color(color);
            for (int i = 0; i < colliderPoints.Length; i++)
            {
                if (!colliderPoints[i].Component.enabled)
                {
                    GL.Color(halfColor);
                }
                if (!colliderPoints[i].Component.gameObject.activeInHierarchy) continue;
                
                Vector3[] points = colliderPoints[i].Points;

                for (int k = 1; k < points.Length; k++)
                {
                    Vector3 p1 = points[k - 1];
                    GL.Vertex3(p1.x, p1.y, p1.z);

                    Vector3 p2 = points[k];
                    GL.Vertex3(p2.x, p2.y, p2.z);
                }
                
                if (!colliderPoints[i].Component.enabled)
                {
                    GL.Color(originalColor);
                }
            }

            GL.End();
        }

        private void CreateLineMaterial()
        {
            if (_lineMaterial == null)
            {
                _lineMaterial = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended"))
                {
                    renderQueue = Int32.MaxValue,
                    hideFlags = HideFlags.HideAndDontSave,
                };
            }
        }
    }
}