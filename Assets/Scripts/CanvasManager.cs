#region

using System.Collections.Generic;
using UnityEngine;

#endregion

public class CanvasManager : MonoBehaviour
{
    public GameObject textNotificationPrefab;

    public Canvas Chapter1Select;

    public Canvas Options;

    public Canvas Pause;

    public Canvas Replays;

    public Canvas CustomMap;

    public Canvas baseCanvas;

    public Canvas screenSizeCanvas;

    private bool menuActionThisFrame;

    public List<Canvas> UiTree { get; set; }

    public int MenuLayerCount => UiTree.Count;

    public Canvas GetCanvasAtLayer(int layer)
    {
        if (layer < MenuLayerCount)
        {
            return UiTree[layer];
        }

        return null;
    }

    public Canvas GetActiveCanvas()
    {
        if (MenuLayerCount <= 0)
        {
            return baseCanvas;
        }

        return UiTree[MenuLayerCount - 1];
    }

    private void Awake()
    {
        UiTree = new List<Canvas>();
        Game.OnAwakeBind(this);
    }

    public void SendNotification(string text, float duration = 5f, int fontSize = 40)
    {
        TextNotification component = Instantiate(textNotificationPrefab, baseCanvas.transform).GetComponent<TextNotification>();
        fontSize *= Screen.width / 1600;
        component.SetText(text, duration, fontSize);
    }
    
    public void SendNotification(string text, float duration = 5f, int fontSize = 40, Vector2 position = default(Vector2))
    {
        TextNotification component = Instantiate(textNotificationPrefab, baseCanvas.transform).GetComponent<TextNotification>();
        fontSize *= Screen.width / 1600;
        component.GetComponent<RectTransform>().position = position;
        component.SetText(text, duration, fontSize);
    }

    public void SendNotification(string text, IEnumerable<int> keys, int fontSize = 40)
    {
        TextNotification component = Instantiate(textNotificationPrefab, baseCanvas.transform).GetComponent<TextNotification>();
        fontSize *= Screen.width / 1600;
        component.SetText(text, keys, 1f, fontSize);
    }

    public void OpenMenuAndSetAsBaseCanvas(Canvas canvas)
    {
        GameObject obj = baseCanvas.gameObject;
        Transform parent = obj.transform.parent;
        Destroy(obj);
        baseCanvas = Instantiate(canvas, parent);
    }

    public void CloseMenu()
    {
        if (!menuActionThisFrame && UiTree.Count > 0)
        {
            Canvas canvas = UiTree[UiTree.Count - 1];
            UiTree.RemoveAt(UiTree.Count - 1);
            Destroy(canvas.gameObject);
            if (UiTree.Count > 0)
            {
                UiTree[UiTree.Count - 1].GetComponent<Canvas>().enabled = true;
            }
            else
            {
                baseCanvas.GetComponent<Canvas>().enabled = true;
            }

            menuActionThisFrame = true;
            MenuOpen = false;
        }
    }

    public void ForceCloseMenu()
    {
        if (UiTree.Count > 0)
        {


            Canvas canvas = UiTree[UiTree.Count - 1];
            UiTree.RemoveAt(UiTree.Count - 1);
            Destroy(canvas.gameObject);
            if (UiTree.Count > 0)
            {
                UiTree[UiTree.Count - 1].GetComponent<Canvas>().enabled = true;
            }
            else
            {
                baseCanvas.GetComponent<Canvas>().enabled = true;
            }

            menuActionThisFrame = true;
            MenuOpen = false;
        }
    }

    public static bool MenuOpen;
    public void OpenMenu(Canvas canvas)
    {
        if (menuActionThisFrame)
        {
            return;
        }

        foreach (Canvas item in UiTree)
        {
            item.gameObject.GetComponent<Canvas>().enabled = false;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        baseCanvas.GetComponent<Canvas>().enabled = false;
        UiTree.Add(Instantiate(canvas));
        menuActionThisFrame = true;
        MenuOpen = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown((KeyCode)PlayerInput.Pause))
        {
            CloseMenu();
        }
    }

    private void LateUpdate()
    {
        menuActionThisFrame = false;
    }
}