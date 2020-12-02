using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ScriptedMover))]
public class ScriptedMoverEditor : Editor
{
    protected virtual void OnSceneGUI()
    {
        var mover = (ScriptedMover)target;

        for (int i = 0; i < mover.points.Count; i++)
        {
            if (i == 0)
                continue;

            EditorGUI.BeginChangeCheck();
            Vector3 newPosition = Handles.PositionHandle(mover.transform.position + mover.points[i], Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(mover, "Change point position");
                mover.points[i] = newPosition - mover.transform.position;
            }
        }
    }
}
