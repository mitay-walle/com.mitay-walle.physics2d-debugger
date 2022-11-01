using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector.Editor;
#endif

namespace Mitaywalle.Physics2DDebugger.Editor
{
    public class Physics2DDebuggerWindow
        #if ODIN_INSPECTOR
        : OdinEditorWindow
            #else
            : EditorWindow
            #endif
    {
        [SerializeField, Range(1, 10)] private float thikness = 2;
        [SerializeField, Range(0, 1)] private float disabledAlpha = .5f;
        [SerializeField] private bool draw = true;
        [SerializeField] private bool soft;
        [SerializeField] private bool findComponentsEveryFrame = true;
        [SerializeField] private bool processCustomComponents = true;
        [SerializeField] private Color staticColliderColor = Color.green;
        [SerializeField] private Color rigidbodyColor = Color.red;
        [SerializeField] private Color jointColor = Color.yellow;
        [SerializeField] private ComponentColorData[] customComponentColors;

        private Material _lineMaterial;
        private Texture2D _textureSoft;
        private Texture2D _textureHard;

        private ComponentProcessor[] _processors =
        {
            new BoxCollider2DProcessor(),
            new CircleCollider2DProcessor(),
            new PolygonCollider2DProcessor(),
            new EdgeCollider2DProcessor(),
            new AnchoredJoint2DProcessor(),
            new CapsuleCollider2DProcessor(),
        };

        private List<ComponentData> _data = new List<ComponentData>();
        private UnityEditor.Editor _editor;

        private List<Component> _tempComponents = new List<Component>();
        private HashSet<string> _componentTypes = new HashSet<string>();
        private Dictionary<string, ComponentColorData> _componentMap = new Dictionary<string, ComponentColorData>();

        [MenuItem("Window/Analysis/Physics 2D Debugger")]
        public static void ShowWindow()
        {
            var window = GetWindow<Physics2DDebuggerWindow>();
            if (!window)
            {
                window = CreateInstance<Physics2DDebuggerWindow>();
            }

            window.Load();
        }

        private void OnEnable()
        {
            CreateLineMaterial();
            CollectData();
            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;
            titleContent = new GUIContent("2D Debugger");
            _textureHard = Texture2D.whiteTexture;
            if (_textureSoft)
            {
                _textureSoft = null;
            }
        }

#if !ODIN_INSPECTOR
        private void OnGUI()
        {
            if (!_editor)
            {
                _editor = UnityEditor.Editor.CreateEditor(this);
            }
            if (!_editor) return;
            _editor.OnInspectorGUI();
        }
#else
        override protected void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
            base.OnGUI();
            if (EditorGUI.EndChangeCheck())
            {
                Save();
            }
        }
#endif

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        private void OnDestroy()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        private void OnBecameVisible()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private void OnBecameInvisible()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        private void OnFocus()
        {
            CollectData();
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            if (!sceneView.drawGizmos) return;
            if (!draw) return;

            if (findComponentsEveryFrame)
            {
                CollectData();
            }

            DrawHandles();
            //OnPostRender();
        }

        private void Save()
        {
            string json = EditorJsonUtility.ToJson(this, true);
            File.WriteAllText("UserSettings/Physics2DDebuggerSettings.json", json);
        }

        private void Load()
        {
            string json = File.ReadAllText("UserSettings/Physics2DDebuggerSettings.json");
            EditorJsonUtility.FromJsonOverwrite(json, this);
        }

        private void DrawHandles()
        {
            if (_data == null) return;
            Handles.color = staticColliderColor;
            Color original = Handles.color;
            for (int i = 0; i < _processors.Length; i++)
            {
                _processors[i].DrawTexture = soft ? _textureSoft : _textureHard;
            }

            var arguments = new DrawArguments
            {
                Thikness = thikness,
                RigidbodyColor = rigidbodyColor,
                StaticColor = original,
                JointColor = jointColor,
                DisabledAlpha = disabledAlpha,
            };

            for (int i = 0; i < _data.Count; i++)
            {
                _data[i].Draw(arguments);
            }
        }

        private void ProcessCustomComponents()
        {
            if (customComponentColors == null) return;
            if (customComponentColors.Length == 0) return;
            _componentTypes.Clear();
            _componentMap.Clear();

            foreach (ComponentColorData componentColorData in customComponentColors)
            {
                var type = componentColorData.ComponentType;

                if (type != null)
                {
                    _componentTypes.Add(type);
                    _componentMap.Add(type, componentColorData);
                }
                else
                {
                    //Debug.LogError($"Component Type '{componentColorData.ComponentType}' not found");
                }
            }

            for (int i = 0; i < _data.Count; i++)
            {
                var componentData = _data[i];
                componentData.Component.GetComponentsInParent(false, _tempComponents);
                for (int j = 0; j < _tempComponents.Count; j++)
                {
                    var type = _tempComponents[j].GetType();
                    if (_componentTypes.Contains(type.Name))
                    {
                        componentData.OverrideColor = _componentMap[type.Name].Color;
                    }
                }

                _data[i] = componentData;
            }
        }

        private void CollectData()
        {
            _data.Clear();

            foreach (ComponentProcessor processor in _processors)
            {
                _data.AddRange(processor.CreateComponentsData());
            }

            if (processCustomComponents) ProcessCustomComponents();
        }

        private void RenderGL()
        {
            if (_data == null) return;
            Color originalColor = staticColliderColor;
            Color halfColor = staticColliderColor / 2;

            GL.Begin(GL.LINES);
            GL.Color(staticColliderColor);
            for (int i = 0; i < _data.Count; i++)
            {
                if (!_data[i].Component.enabled)
                {
                    GL.Color(halfColor);
                }

                if (!_data[i].Component.gameObject.activeInHierarchy) continue;

                Vector3[] points = _data[i].Points;

                for (int k = 1; k < points.Length; k++)
                {
                    Vector3 p1 = points[k - 1];
                    GL.Vertex3(p1.x, p1.y, p1.z);

                    Vector3 p2 = points[k];
                    GL.Vertex3(p2.x, p2.y, p2.z);
                }

                if (!_data[i].Component.enabled)
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