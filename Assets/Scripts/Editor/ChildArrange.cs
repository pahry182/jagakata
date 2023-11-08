using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ArrangeChildren))]

public class ChildArrange : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ArrangeChildren myGameObject = (ArrangeChildren)target;

       
    }
}
