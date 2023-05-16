#region

using System;
using UnityEngine;
using UnityEngine.UI;

#endregion

public class KeyBindButton : MonoBehaviour
{
    public string bindName;

    public Text text;

    private bool rebinding;

    private PlayerInput input;

    private void Start()
    {
        input = Game.OnStartResolve<PlayerInput>();
        text.text = GetValue();
    }

    private void Update()
    {
        if (rebinding)
        {
            foreach (KeyCode value in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(value))
                {
                    PlayerInput.SetBind(bindName, value);
                    text.text = value.ToString();
                    rebinding = false;
                    break;
                }
            }

            return;
        }

        text.text = PlayerInput.GetBindName(PlayerInput.GetBindByName(bindName));
    }

    private string GetValue()
    {
        return typeof(PlayerInput).GetProperty(bindName).GetValue(null, null).ToString();
    }

    public void ReBind()
    {
        text.text = "Press a Key";
        rebinding = true;
    }
}