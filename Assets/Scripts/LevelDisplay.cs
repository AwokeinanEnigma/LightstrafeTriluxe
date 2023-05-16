#region

using System;
using Level_Editor_Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#endregion

public class LevelDisplay : MonoBehaviour
{
    public Text levelText;

    public string prefix;

    private void Start()
    {
        if (CustomMapLoader.CustomMap)
        {
            levelText.text = "Custom Map: " +  CustomMapLoader.MapName + Environment.NewLine + "By: " + CustomMapLoader.Author + Environment.NewLine + "Last Modified: " + CustomMapLoader.LastModified;
            return;
        }
        levelText.text = prefix + SceneManager.GetActiveScene().name;
    }
}