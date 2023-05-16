#region

using UnityEngine;

#endregion

public class FollowTransform : MonoBehaviour
{
    public Transform transformToFollow;

    public Camera viewModel;

    private Player player;

    private void Start()
    {
        player = Game.OnStartResolve<Player>();
    }

    private void Update()
    {
        Vector3 position = viewModel.WorldToViewportPoint(transformToFollow.position);
        transform.position = player.camera.ViewportToWorldPoint(position);
    }
}