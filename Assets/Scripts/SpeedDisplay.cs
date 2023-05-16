#region

using UnityEngine;
using UnityEngine.UI;

#endregion

public class SpeedDisplay : MonoBehaviour
{
    private Text speedText;

    public string prefix;

    public string suffix;

    public bool potential;

    private Player player;

    private float speedLerp;

    private void Start()
    {
        player = Game.OnStartResolve<Player>();
    }

    private void Awake()
    {
        speedText = GetComponent<Text>();
    }

    private void Update()
    {
        if (player == null)
        {
            speedText.color = new Color(1f, 1f, 1f, 0f);
            return;
        }

        float b = potential ? Mathf.Abs(player.velocity.y) : Flatten(player.velocity).magnitude;
        speedLerp = Mathf.Lerp(speedLerp, b, Time.deltaTime * 8f);
        speedText.text = prefix + Mathf.RoundToInt(speedLerp) + suffix;
    }

    private static Vector3 Flatten(Vector3 vec)
    {
        return new Vector3(vec.x, 0f, vec.z);
    }
}