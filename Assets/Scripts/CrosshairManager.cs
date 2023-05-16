#region

using UnityEngine;
using UnityEngine.UI;

#endregion

public class CrosshairManager : MonoBehaviour
{
    public Image crosshair;

    private Player player;

    private float crosshairScale;

    private Vector4 crosshairGrey = new Color(0.39f, 0.39f, 0.39f, 0.39f);

    private Vector4 crosshairBlue = new Color(0f, 0.8f, 1f, 1f);

    private Vector4 crosshairWhite = new Color(1f, 1f, 1f, 1f);

    private void Awake()
    {
        Game.OnAwakeBind(this);
    }

    private void Start()
    {
        player = Game.OnStartResolve<Player>();
    }

    private void Update()
    {
        if (player == null)
        {
            Debug.Log("hey");
            crosshair.color = new Color(0f, 0f, 0f, 0f);
            return;
        }

        float num = 0f;
        float num2 = 25f;
        if (player.GrappleEnabled || player.DashEnabled)
        {
            num = !player.GrappleDashCast(out var _, out var howFarBeyond, num2) ? 1f - howFarBeyond / num2 : 1f;
        }

        if (player.IsDashing || player.GrappleHooked)
        {
            crosshair.color = crosshairWhite;
            crosshair.transform.rotation = Quaternion.Euler(0f, 0f, 45f);
        }
        else
        {
            crosshair.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(crosshair.transform.rotation.eulerAngles.z, 45f * num, Time.deltaTime * 20f));
            crosshairScale = Mathf.Lerp(crosshairScale, num >= 1f ? 1.35f : 1f, Time.deltaTime * 20f);
            crosshair.color = Vector4.Lerp(crosshair.color, num >= 1f ? crosshairBlue : crosshairGrey, Time.deltaTime * 20f);
        }

        crosshair.transform.localScale = new Vector3(crosshairScale, crosshairScale, crosshairScale);
    }
}