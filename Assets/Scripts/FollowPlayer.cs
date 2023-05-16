#region

using UnityEngine;

#endregion

public class FollowPlayer : MonoBehaviour
{
    public bool x;

    public bool y;

    public bool z;

    private Player player;

    private void Start()
    {
        player = Game.OnStartResolve<Player>();
    }

    private void Update()
    {
        if (!(player == null))
        {
            Vector3 position = transform.position;
            if (x)
            {
                position.x = player.camera.transform.position.x;
            }

            if (y)
            {
                position.y = player.camera.transform.position.y;
            }

            if (z)
            {
                position.z = player.camera.transform.position.z;
            }

            transform.position = position;
        }
    }
}