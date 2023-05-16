#region

using UnityEngine;
using UnityEngine.UI;

#endregion

public class CollectibleCount : MonoBehaviour
{
    private Collectible[] collectibles;

    private Text text;

    private void Start()
    {
        collectibles = FindObjectsOfType<Collectible>();
        text = GetComponent<Text>();
    }
    
    public void RefreshCollectibles()
    {
        collectibles = FindObjectsOfType<Collectible>();
    }

    private void Update()
    {
        int num = collectibles.Length + 1;
        Collectible[] array = collectibles;
        foreach (Collectible collectible in array)
        {
            if (collectible != null && collectible.LeftToCollect < num)
            {
                num = collectible.LeftToCollect;
                break;
            }
        }

        if (num == collectibles.Length + 1)
        {
            num = 0;
        }

        text.text = collectibles.Length - num + "/" + collectibles.Length;
    }
}