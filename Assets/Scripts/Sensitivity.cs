#region

using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#endregion

public class Sensitivity : MonoBehaviour
{
    public InputField sensitivity;

    private string preventEscape = "";

    private void Start()
    {
        sensitivity.text = GameSettings.Sensitivity.ToString() ?? "";
    }

    private void Update()
    {
        sensitivity.text = sensitivity.text.Where((char c) => char.IsDigit(c) || c == '.').Aggregate("", (string current, char c) => current + c);
        if (float.TryParse(sensitivity.text, out var result))
        {
            GameSettings.Sensitivity = result;
        }
    }

    public void OnEditting()
    {
        if (!Input.GetKeyDown(KeyCode.Escape))
        {
            preventEscape = sensitivity.text;
        }
    }

    public void OnEditEnd()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            sensitivity.text = preventEscape;
        }

        EventSystem.current.SetSelectedGameObject(null);
    }
}