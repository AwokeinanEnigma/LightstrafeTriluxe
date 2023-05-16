#region

using UnityEngine;
using UnityEngine.UI;

#endregion

public class SpeedChangeDisplay : MonoBehaviour
{
    public Image gain;

    public Image loss;

    private float prevSpeed;

    private Player player;

    public float interpolation { get; set; }

    private void Start()
    {
        player = Game.OnStartResolve<Player>();
    }

    private void Update()
    {
        float magnitude = Flatten(player.velocity).magnitude;
        float num = magnitude - prevSpeed;
        interpolation += num;
        gain.fillAmount = Mathf.Clamp01(interpolation / 10f);
        loss.fillAmount = Mathf.Clamp01((0f - interpolation) / 10f);
        interpolation = Mathf.Lerp(interpolation, 0f, Time.deltaTime * 3f);
        prevSpeed = magnitude;
    }

    private static Vector3 Flatten(Vector3 vec)
    {
        return new Vector3(vec.x, 0f, vec.z);
    }
}