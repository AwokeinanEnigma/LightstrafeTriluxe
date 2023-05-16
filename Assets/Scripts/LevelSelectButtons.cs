#region

using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

#endregion

public class LevelSelectButtons : MonoBehaviour
{
    public GameObject buttonPrefab;

    public Transform startPosition;

    private GameObject[] generatedButtons;

    public List<string> levels;

    private void Start()
    {
        generatedButtons = new GameObject[levels.Count];
        for (int i = 0; i < levels.Count; i++)
        {
            generatedButtons[i] = Instantiate(buttonPrefab, startPosition);
            generatedButtons[i].GetComponent<RectTransform>().localPosition = Vector3.down * 55f * i;
            Button component = generatedButtons[i].GetComponent<Button>();
            Text componentInChildren = generatedButtons[i].GetComponentInChildren<Text>();
            string level = levels[i];
            componentInChildren.text = Path.GetFileNameWithoutExtension(level);
            component.onClick.AddListener(Action);

            void Action()
            {
                Game.StartLevel(level);
            }
        }
    }
}