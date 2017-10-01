using UnityEditor;
using UnityEngine;

namespace Editor {
    public class DropObjectsEditor : EditorWindow {
        public RaycastHit Hit;
        public float YOffset;
        public int SavedLayer;
        public bool AlignNormals;
        public Vector3 UpVector = new Vector3(0, 90, 0);

        [MenuItem("Window/Drop Object")] // add menu item
        private static void Awake() {
            GetWindow<DropObjectsEditor>().Show(); // Get existing open window or if none, make a new one
        }

        void OnGUI() {
            GUILayout.Label("Drop using: ", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Bottom")) {
                DropObjects("Bottom");
            }

            if (GUILayout.Button("Origin")) {
                DropObjects("Origin");
            }

            if (GUILayout.Button("Center")) {
                DropObjects("Center");
            }
            EditorGUILayout.EndHorizontal();
            AlignNormals =
                EditorGUILayout.ToggleLeft("Align Normals",
                    AlignNormals); // toggle to align the object with the normal direction of the surface
            if (AlignNormals) {
                EditorGUILayout.BeginHorizontal();
                UpVector = EditorGUILayout.Vector3Field("Up Vector",
                    UpVector); // Vector3 helping to specify the Up vector of the object
                // default has 90° on the Y axis, this is because by default
                // the objects I import have a rotation.
                // If anyone has a better way to do this I'd be happy
                // to see a better solution!
                GUILayout.EndHorizontal();
            }
        }

        private void DropObjects(string method) {
            foreach (Transform t in Selection.transforms) {
                GameObject go = t.gameObject; // get the gameobject
                if (!go) {
                    continue;
                } // if there's no gameobject, skip the step — probably unnecessary but hey…

                Bounds bounds = go.GetComponent<Renderer>().bounds; // get the renderer's bounds
                SavedLayer = go.layer; // save the gameobject's initial layer
                go.layer = 2; // set the gameobject's layer to ignore raycast

                if (Physics.Raycast(go.transform.position, -Vector3.up, out Hit)) // check if raycast hits something
                {
                    switch (method) // determine how the y position will need to be adjusted
                    {
                        case "Bottom":
                            YOffset = go.transform.position.y - bounds.min.y;
                            break;
                        case "Origin":
                            YOffset = 0f;
                            break;
                        case "Center":
                            YOffset = bounds.center.y - go.transform.position.y;
                            break;
                    }
                    if (AlignNormals) // if "Align Normals" is checked, set the gameobject's rotation
                        // to match the raycast's hit position's normal, and add the specified offset.
                    {
                        go.transform.up = Hit.normal + UpVector;
                    }
                    go.transform.position = new Vector3(Hit.point.x, Hit.point.y + YOffset, Hit.point.z);
                }
                go.layer = SavedLayer; // restore the gameobject's layer to it's initial layer
            }
        }
    }
}