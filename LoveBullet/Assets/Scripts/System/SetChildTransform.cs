using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SetChildTransform : MonoBehaviour
{
    public float dist;


}


#if UNITY_EDITOR
[CustomEditor(typeof(SetChildTransform))]
public class ChildEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("”z’u")) {
            var t = (SetChildTransform)target;
            int c = t.gameObject.transform.childCount;

            Vector3 p = new Vector3(0, t.dist, 0);
            float angle = 360f / (float)c;

            for (int i = 0; i < c; i++) {
                t.gameObject.transform.GetChild(i).transform.localPosition =
                Quaternion.Euler(0, 0, -angle * (float)i) * p;
            }
        }
    }
}
#endif
