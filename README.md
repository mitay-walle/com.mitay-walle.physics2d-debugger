# Unity Physics 2D Debugger

Instantly renders Unity's 2D physics colliders and joints as wireframes for rapid prototyping.

![](https://github.com/mitay-walle/com.mitay-walle.physics2d-debugger/blob/master/Documentation~/Screenshot_3.png)
![](https://github.com/mitay-walle/com.mitay-walle.physics2d-debugger/blob/master/Documentation~/Screenshot_2.png)
![](https://github.com/mitay-walle/com.mitay-walle.physics2d-debugger/blob/master/Documentation~/Screenshot_1.png)
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
- Collider.enabled = false make Gizmo.Color half-visible
# Changed
- converted to UPM-package
- MonoBehaviour converted to EditorWindow
- Gizmos.DrawLine() replaced with Handles.DrawLines()
# Known Issues
- [ ] [#1 non-uniform scaled CircleCollider2D is drawing wrong](/../../issues/1) 
- [ ] [#2 GameView draw removed](/../../issues/2) 
