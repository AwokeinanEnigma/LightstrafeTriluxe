using System;
using UnityEngine;
using UnityEngine.UI;

namespace Level_Editor_Scripts
{
    public class ObjectCatalog : MonoBehaviour
    {
        public LevelEditorPrefabs Prefabs;
        public GameObject[] GeneratedButtons;
        
        public GameObject buttonPrefab;
        public Transform startPosition;
        
        public bool CatalogOpen;

        public static ObjectCatalog Instance { get; private set; }
        
        public void Awake()
        { 
            Instance = this;
            PlaceObject.Object = Prefabs.Prefabs[0];
        }

        public GameObject Cassette;
        
        public void OnDestroy()
        {
            Instance = null;
        }

        public void ShowCatalog()
        {

            CatalogOpen = true;
            if (GeneratedButtons.Length > 0)
            {
                for (int i = 0; i < GeneratedButtons.Length; i++)
                {
                    GeneratedButtons[i].SetActive(true);
                }
            }
            else
            {

                GeneratedButtons = new GameObject[Prefabs.Prefabs.Length];
                for (int i = 0; i < Prefabs.Prefabs.Length; i++)
                {
                    GameObject file = Prefabs.Prefabs[i];
                    GeneratedButtons[i] = Instantiate(buttonPrefab, startPosition);
                    GeneratedButtons[i].GetComponent<RectTransform>().localPosition = Vector3.down * 55 * i;
                    Button component = GeneratedButtons[i].GetComponent<Button>();
                    Text componentInChildren = GeneratedButtons[i].GetComponentInChildren<Text>();
                    componentInChildren.fontSize = 18;
                    componentInChildren.text = file.name;
                    component.onClick.AddListener(Action);

                    void Action()
                    {
                        PlaceObject.Object = file;
                    }
                }
            }
        }
        
        public void DestroyCatalog()
        {
            CatalogOpen = false;
            for (int i = 0; i < GeneratedButtons.Length; i++)
            {
                GeneratedButtons[i].SetActive(false);
            }
        }
        
        public void PopCatalog()
        {
            if (CatalogOpen)
            {
                DestroyCatalog();
                GetComponentInChildren<Text>().text = "Show Catalog";
            }
            else
            {
                ShowCatalog();
                GetComponentInChildren<Text>().text = "Hide Catalog";

                
            }
        }
    }
}