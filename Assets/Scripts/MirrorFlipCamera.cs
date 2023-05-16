#region

using UnityEngine;

#endregion

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class MirrorFlipCamera : MonoBehaviour
{
    private Camera camera;

    private void Awake()
    {
        camera = GetComponent<Camera>();
    }

    private void OnPreCull()
    {
        camera.ResetWorldToCameraMatrix();
        camera.ResetProjectionMatrix();
        camera.projectionMatrix *= Matrix4x4.Scale(new Vector3(-1f, 1f, 1f));
    }

    private void OnPreRender()
    {
        GL.invertCulling = true;
    }

    private void OnPostRender()
    {
        GL.invertCulling = false;
    }
}