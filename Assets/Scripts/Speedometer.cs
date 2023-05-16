#region

using UnityEngine;
using UnityEngine.UI;

#endregion

public class Speedometer : MonoBehaviour
{
    public Image leftLayer1;

    public Image rightLayer1;

    private float _layer1Lerp;

    private Player player;

    private void Start()
    {
        player = Game.OnStartResolve<Player>();
    }

    private void Update()
    {
        float b = Flatten(player.velocity).magnitude / 10f;
        float b2 = Mathf.Min(1f, Mathf.Max(0f, b));
        _layer1Lerp = Mathf.Lerp(_layer1Lerp, b2, Time.deltaTime * 5f);
        leftLayer1.fillAmount = _layer1Lerp / 2f;
        rightLayer1.fillAmount = _layer1Lerp / 2f;
    }

    private static Vector3 Flatten(Vector3 vec)
    {
        return new Vector3(vec.x, 0f, vec.z);
    }
}