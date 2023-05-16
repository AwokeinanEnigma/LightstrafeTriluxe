#region

using System.Collections.Generic;
using UnityEngine;

#endregion

public class AbilityUiTutorial : MonoBehaviour
{
    private Player player;

    private CanvasManager canvasManager;

    private void Start()
    {
        player = Game.OnStartResolve<Player>();
        canvasManager = Game.OnStartResolve<CanvasManager>();
        if (player == null || canvasManager == null)
        {
            return;
        }

        string text = "";
        List<int> list = new List<int>();
        if (player.DashEnabled)
        {
            text = text + "Press " + ((KeyCode)PlayerInput.SecondaryInteract) + " to dash";
            list.Add(PlayerInput.SecondaryInteract);
        }

        if (player.GrappleEnabled)
        {
            if (player.DashEnabled)
            {
                text += "\n";
            }

            text = text + "Press " + ((KeyCode)PlayerInput.PrimaryInteract) + " to grapple";
            list.Add(PlayerInput.PrimaryInteract);
        }

        if (text.Length > 0)
        {
            canvasManager.SendNotification(text, list);
        }
    }
}