﻿#region

using System;
using System.IO;
using fNbt;
using FullSerializer;
using Level_Editor_Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#endregion

public class CustomMapMenu : MonoBehaviour
{
    public GameObject buttonPrefab;

    public Transform startPosition;

    private GameObject[] generatedButtons;

    private void Start()
    {
        string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Lightstrafe\\maps";
        Directory.CreateDirectory(path);
        string[] files = Directory.GetFiles(path);
        generatedButtons = new GameObject[files.Length];
        for (int i = 0; i < files.Length; i++)
        {
            string file = files[i];
            generatedButtons[i] = Instantiate(buttonPrefab, startPosition);
            generatedButtons[i].GetComponent<RectTransform>().localPosition = Vector3.down * 55f * i;
            
            
            
            Button component = generatedButtons[i].GetComponent<Button>();
            Text componentInChildren = generatedButtons[i].GetComponentInChildren<Text>();
            componentInChildren.fontSize = 18;
            componentInChildren.text = Path.GetFileNameWithoutExtension(file);
            component.onClick.AddListener(Action);

            void Action()
            {
                CustomMapLoader.MapName = Path.GetFileNameWithoutExtension(file);;
                SceneManager.LoadScene("Custom Map");

            }
        }
    }
    
    public void Refresh()
    {
        foreach (GameObject generatedButton in generatedButtons)
        {
            Destroy(generatedButton);
        }
        Start();
    }
}