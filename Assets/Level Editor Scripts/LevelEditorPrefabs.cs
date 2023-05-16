using UnityEngine;

namespace Level_Editor_Scripts
{
    [CreateAssetMenu(fileName = "LevelEditorPrefabs", menuName = "Level Editor/LevelEditorPrefabs")]
    public class LevelEditorPrefabs : ScriptableObject
    {
        public GameObject[] Prefabs;
    }
}