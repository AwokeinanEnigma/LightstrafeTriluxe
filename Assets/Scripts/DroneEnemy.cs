#region

using UnityEngine;

#endregion

public class DroneEnemy : MonoBehaviour
{
    public GameObject visual;

    private Vector3 visualStart;

    private Player player;

    private void Start()
    {
        player = Game.OnStartResolve<Player>();
        visualStart = visual.transform.position;
    }

    private void Update()
    {
        visual.transform.position = visualStart + Vector3.up * Mathf.Sin(Time.time * 2f);
        Quaternion rotation = visual.transform.rotation;
        visual.transform.LookAt(player.transform.position);
        Quaternion rotation2 = visual.transform.rotation;
        visual.transform.rotation = Quaternion.Lerp(rotation, rotation2, Time.deltaTime * 5f);
        Quaternion rotation3 = visual.transform.rotation;
        Vector3 eulerAngles = rotation3.eulerAngles;
        eulerAngles.x = 0f;
        eulerAngles.z = 0f;
        rotation3.eulerAngles = eulerAngles;
        visual.transform.rotation = rotation3;
    }
}