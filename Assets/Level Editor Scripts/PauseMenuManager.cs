using UnityEngine;

namespace Level_Editor_Scripts
{
    public class PauseMenuManager : MonoBehaviour
    {
        public GameObject[] GameObjectsToDisableInEditor;
        public void Awake()
        {
            if (SessionManager.Instance != null)
            {
                for (int i = 0; i < GameObjectsToDisableInEditor.Length; i++)
                {
                    GameObjectsToDisableInEditor[i].SetActive(false);
                }
            }
        }
    }
}