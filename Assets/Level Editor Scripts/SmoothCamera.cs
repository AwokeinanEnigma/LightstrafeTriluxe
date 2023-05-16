using Level_Editor_Scripts;
using UnityEngine;


public class SmoothCamera : MonoBehaviour
{
    
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 1.0f;
    [SerializeField] private float rotationSpeed = 10.0f;
    [SerializeField] private float zoomSpeed = 10.0f;
    
    
    [SerializeField] private KeyCode anchoredMoveKey = KeyCode.Mouse2;

    [SerializeField] private KeyCode anchoredRotateKey = KeyCode.Mouse1;

    private void LateUpdate()
    {
        Debug.Log(Input.GetKeyDown(KeyCode.LeftShift));
        if (!SessionManager.LockEditor && !Input.GetKey(KeyCode.LeftShift))
        {
            Vector3 move = Vector3.zero;

            //Move and rotate the camera

            float moveSpeed = this.moveSpeed;
            if (Input.GetKey(KeyCode.LeftControl))
            {
                moveSpeed *= 10;
            }

            if (Input.GetKey((KeyCode)PlayerInput.MoveForward))
                move += Vector3.forward * moveSpeed;
            if (Input.GetKey((KeyCode)PlayerInput.MoveBackward))
                move += Vector3.back * moveSpeed;
            if (Input.GetKey((KeyCode)PlayerInput.MoveLeft))
                move += Vector3.left * moveSpeed;
            if (Input.GetKey((KeyCode)PlayerInput.MoveRight))
                move += Vector3.right * moveSpeed;

            // move player on the horizontal plane
            if (Input.GetKey(KeyCode.LeftAlt))
            {
                float origY = transform.position.y;

                transform.Translate(move);
                transform.position = new Vector3(transform.position.x, origY, transform.position.z);
                return;
            }

            float mouseMoveY = Input.GetAxis("Mouse Y");
            float mouseMoveX = Input.GetAxis("Mouse X");

            //Move the camera when anchored
            if (Input.GetKey(anchoredMoveKey))
            {
                move += Vector3.up * mouseMoveY * -moveSpeed;
                move += Vector3.right * mouseMoveX * -moveSpeed;
            }

            //Rotate the camera when anchored
            if (Input.GetKey(anchoredRotateKey))
            {
                transform.RotateAround(transform.position, transform.right, mouseMoveY * -rotationSpeed);
                transform.RotateAround(transform.position, Vector3.up, mouseMoveX * rotationSpeed);
            }

            transform.Translate(move);

            //Scroll to zoom
            float mouseScroll = Input.GetAxis("Mouse ScrollWheel");
            transform.Translate(Vector3.forward * mouseScroll * zoomSpeed);
        }
    }
}