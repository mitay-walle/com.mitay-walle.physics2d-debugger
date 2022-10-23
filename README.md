# Unity Physics 2D Debugger

Instantly renders Unity's 2D physics colliders and joints as wireframes for rapid prototyping.

![](https://github.com/mitay-walle/com.mitay-walle.physics2d-debugger/blob/master/Documentation~/screenshot.png)
![](https://github.com/mitay-walle/com.mitay-walle.physics2d-debugger/blob/master/Documentation~/screenshot.png)

## Installation
Install UPM-Package [by Git URL](https://docs.unity3d.com/Manual/upm-ui-giturl.html)

## Usage

- open window by click 'Window/Analysis/Physics 2D Debugger'
- ensure Gizmos is turn on
- The colors for the various types of colliders can be changed in the '2D Debugger' window
# Added
- hide by SceneView.DrawGizmos
- hide by EditorWindow.OnBecameVisible
- hide by Draw-flag
- hide by Collider.gameObject.activeInHierarcy 
- Color by has/no Rigidbody2D
- Collider.enabled = false make Gizmo.Color half-visible
- Settings saved in Project/UserSettings
- MonoBehaviour can override color By its name
# Changed
- converted to UPM-package
- MonoBehaviour converted to EditorWindow
- Gizmos.DrawLine() replaced with Handles.DrawLines()
# Known Issues
- [ ] [#1 non-uniform scaled CircleCollider2D is drawing wrong](/../../issues/1) 
- [ ] [#2 GameView draw removed](/../../issues/2) 
- [ ] [#3 no CapsuleCollider2 support ](/../../issues/3) 
