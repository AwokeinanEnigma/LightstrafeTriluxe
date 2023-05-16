#region

using UnityEngine;

#endregion

public class HandsAnimation : MonoBehaviour
{
    public Animator animator;

    public Transform rightHand;

    public Transform leftHand;

    public LineRenderer grappleTether;

    public Transform grappleAttachLeft;

    public Transform grappleAttachRight;

    private Player player;

    private Quaternion startRotation;

    private bool wasDashing;

    private AnimatorStateInfo info;

    private bool setSpeed;

    private float totalDashTime;

    private float grapplePositionAmount;

    private int grappleHand = -1;

    private Quaternion rotation;

    private Quaternion targetRotation;

    private void Start()
    {
        player = Game.OnStartResolve<Player>();
        startRotation = transform.localRotation;
    }

    private void FixedUpdate()
    {
        info = animator.GetCurrentAnimatorStateInfo(0);
        if (info.IsName("Dash"))
        {
            if (animator.GetCurrentAnimatorClipInfo(0).Length != 0 && setSpeed)
            {
                totalDashTime = player.DashTime;
                animator.speed = Mathf.Min(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length / totalDashTime, 3f) - 0.1f;
                setSpeed = false;
            }
        }
        else
        {
            setSpeed = true;
        }

        if ((player.ApproachingWall || player.IsOnWall) && Vector3.Angle(Flatten(-player.WallNormal), Flatten(player.CrosshairDirection)) > 45f)
        {
            if (player.WallRightSide)
            {
                animator.SetBool("WallRunRight", true);
                animator.SetBool("WallRunLeft", false);
                if (info.IsName("Idle"))
                {
                    animator.Play("WallRunRight");
                }
            }
            else
            {
                animator.SetBool("WallRunLeft", true);
                animator.SetBool("WallRunRight", false);
                if (info.IsName("Idle"))
                {
                    animator.Play("WallRunLeft");
                }
            }
        }
        else if (!player.IsDashing)
        {
            if (info.IsName("WallRunRight"))
            {
                animator.Play("WallRunRightReverse");
            }

            if (info.IsName("WallRunLeft"))
            {
                animator.Play("WallRunLeftReverse");
            }

            animator.SetBool("WallRunRight", false);
            animator.SetBool("WallRunLeft", false);
        }

        animator.SetBool("Walking", !player.IsSliding && player.IsOnGround && player.Speed > 0.1f);
        animator.SetBool("GrappleHooked", player.GrappleHooked);
    }

    private void Update()
    {
        if (player.IsDashing)
        {
            transform.localRotation = startRotation;
            if (!wasDashing)
            {
                animator.Play("Dash");
            }
        }
        else
        {
            animator.speed = 1f;
            transform.localRotation = startRotation;
        }

        wasDashing = player.IsDashing;
    }

    private void LateUpdate()
    {
        if (player.IsDashing || info.IsName("Dash"))
        {
            float num = Vector3.Angle(player.camera.transform.right, player.DashTargetNormal) - 90f;
            num *= 0.7f;
            if (totalDashTime > 0f && Mathf.Abs(num) > 0f)
            {
                rightHand.RotateAround(transform.position, player.camera.transform.forward, num);
                leftHand.RotateAround(transform.position, player.camera.transform.forward, num);
            }
        }

        if (player.GrappleHooked)
        {
            grapplePositionAmount += Mathf.Min(Time.deltaTime * 6f, 0f - (grapplePositionAmount - 1f));
            Vector3[] array = new Vector3[2];
            if (grappleHand == -1)
            {
                Vector3 normalized = (player.transform.position - player.GrappleAttachPosition).normalized;
                float num2 = Vector3.Dot(Vector3.Cross(player.velocity, Vector3.up), normalized);
                grappleHand = !(num2 < 0f) ? 1 : 0;
                return;
            }

            if (grappleHand == 0)
            {
                Vector3 position = rightHand.transform.position;
                position = player.camera.transform.position + player.CrosshairDirection * 0.1f;
                position += (player.GrappleAttachPosition - position).normalized * 0.1f;
                position += -player.transform.right * 0.05f;
                position += Vector3.down * 0.05f;
                rightHand.transform.position = Vector3.Lerp(rightHand.transform.position + -player.transform.right + Vector3.down, position, grapplePositionAmount);
                Vector3 vector = player.GrappleAttachPosition - position;
                vector.y *= -1f;
                rightHand.LookAt(position + vector);
                rightHand.Rotate(Vector3.up, 180f, Space.World);
                rightHand.Rotate(Vector3.right, -110f, Space.Self);
                rightHand.Rotate(Vector3.up, 140f, Space.Self);
                array[1] = grappleAttachRight.position;
            }
            else
            {
                Vector3 position2 = leftHand.transform.position;
                position2 = player.camera.transform.position + player.CrosshairDirection * 0.1f;
                position2 += (player.GrappleAttachPosition - position2).normalized * 0.1f;
                position2 += player.transform.right * 0.05f;
                position2 += Vector3.down * 0.1f;
                leftHand.transform.position = Vector3.Lerp(leftHand.transform.position + player.transform.right + Vector3.down, position2, grapplePositionAmount);
                Vector3 vector2 = player.GrappleAttachPosition - position2;
                vector2.y *= -1f;
                leftHand.LookAt(position2 + vector2);
                leftHand.Rotate(Vector3.up, 180f, Space.World);
                leftHand.Rotate(Vector3.right, -110f, Space.Self);
                leftHand.Rotate(Vector3.up, 200f, Space.Self);
                array[1] = grappleAttachLeft.position;
            }

            if (!grappleTether.enabled && grapplePositionAmount > 0.8f)
            {
                grappleTether.enabled = true;
            }

            array[0] = player.GrappleAttachPosition;
            grappleTether.SetPositions(array);
        }
        else
        {
            grapplePositionAmount -= Mathf.Min(Time.deltaTime, grapplePositionAmount);
            if (grappleTether.enabled)
            {
                grappleTether.enabled = false;
            }

            grappleHand = -1;
        }
    }

    private static Vector3 Flatten(Vector3 vec)
    {
        return new Vector3(vec.x, 0f, vec.z);
    }
}