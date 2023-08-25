using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Board))]

public class ShowCurrentBoard : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Board myGameObject = (Board)target;

        if (GUILayout.Button("Init Board"))
        {
            myGameObject.InitBoard();
        }

        if (GUILayout.Button("Print Board"))
        {
            myGameObject.ShowCurrentBoard();
        }
    }
}
