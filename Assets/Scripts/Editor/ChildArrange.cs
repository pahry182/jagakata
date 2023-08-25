using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ChildrenArranger))]

public class ChildArrange : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ChildrenArranger myGameObject = (ChildrenArranger)target;

        if (GUILayout.Button("Arrange Childs"))
        {
            myGameObject.ArrangeChilds();
        }
    }
}
