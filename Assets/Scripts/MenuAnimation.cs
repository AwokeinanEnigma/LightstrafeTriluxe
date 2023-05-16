#region

using UnityEngine;

#endregion

public class MenuAnimation : MonoBehaviour
{
    public struct PosRot
    {
        public Vector3 position;

        public Quaternion rotation;
    }

    public Camera MenuCamera;

    public Transform OptionsPosition;

    public Transform OtherPosition;

    public float lerpSpeed = 2f;

    private PosRot startPosition;

    private PosRot nextPosition;

    private PosRot previousPosition;

    private float currentLerpValue;

    private CanvasManager canvasManager;

    private string previousCanvas;

    private void Start()
    {
        canvasManager = Game.OnStartResolve<CanvasManager>();
        Transform transform = MenuCamera.transform;
        startPosition = new PosRot
        {
            position = transform.position,
            rotation = transform.rotation
        };
        nextPosition = startPosition;
        previousPosition = startPosition;
    }

    public void SendToOptionsPosition()
    {
        SendToTransform(OptionsPosition);
    }

    public void SendToOtherPosition()
    {
        SendToTransform(OtherPosition);
    }

    public void SendToStartPosition()
    {
        SendToPosRot(startPosition);
    }

    public void SendToTransform(Transform trans)
    {
        SendToPosRot(new PosRot
        {
            position = trans.position,
            rotation = trans.rotation
        });
    }

    public void SendToPosRot(PosRot posRot)
    {
        currentLerpValue = 0f;
        Transform transform = MenuCamera.transform;
        PosRot posRot2 = default;
        posRot2.position = transform.position;
        posRot2.rotation = transform.rotation;
        PosRot posRot3 = posRot2;
        previousPosition = posRot3;
        nextPosition = posRot;
    }

    private void Update()
    {
        string text = canvasManager.GetActiveCanvas().name;
        if (text != previousCanvas)
        {
            if (text.ToLower().Contains("options"))
            {
                SendToOptionsPosition();
            }
            else if (canvasManager.UiTree.Count > 0)
            {
                SendToOtherPosition();
            }
            else
            {
                SendToStartPosition();
            }
        }

        previousCanvas = text;
        float num = currentLerpValue;
        float t = 1f - Mathf.Pow(1f - num, 3f);
        MenuCamera.transform.position = Vector3.Lerp(previousPosition.position, nextPosition.position, t);
        MenuCamera.transform.rotation = Quaternion.Lerp(previousPosition.rotation, nextPosition.rotation, t);
        currentLerpValue += Mathf.Min(1f - currentLerpValue, Time.deltaTime * lerpSpeed);
    }
}