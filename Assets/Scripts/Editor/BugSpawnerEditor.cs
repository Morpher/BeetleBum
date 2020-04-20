using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
namespace Editor
{
    [CustomEditor(typeof(BugSpawner))]
    public class BugSpawnerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (EditorApplication.isPlaying 
                && GUILayout.Button("Spawn Bugs"))
            {
                (target as BugSpawner)?.Spawn();
            }
        }
    }
}
#endif