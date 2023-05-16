#region

using UnityEngine;

#endregion

public class Projectile : MonoBehaviour
{
    public Vector3 velocity;

    public ParticleSystem explodeParticle;

    public AudioSource explodeSound;

    public GameObject visual;

    public Radial radial;

    public float d = 5f;

    private float fuse = 0.35f;

    private bool hit;

    private Player player;

    private CanvasManager canvasManager;

    public void Fire(Vector3 vel, Vector3 realPosition, Vector3 visualPosition, float drop = 5f)
    {
        transform.position = realPosition;
        visual.transform.position = visualPosition;
        velocity = vel;
        d = drop;
        visual.transform.rotation = Quaternion.LookRotation(-velocity);
    }

    private void Start()
    {
        canvasManager = Game.OnStartResolve<CanvasManager>();
        player = Game.OnStartResolve<Player>();
    }

    private void Update()
    {
        if (!hit)
        {
            visual.transform.rotation = Quaternion.Lerp(visual.transform.rotation, Quaternion.LookRotation(-velocity), Time.deltaTime * 10f);
            visual.transform.position = Vector3.Lerp(visual.transform.position, transform.position, Time.deltaTime * 8f);
        }
    }

    private void FixedUpdate()
    {
        if (hit)
        {
            if (!(fuse > 0f))
            {
                return;
            }

            fuse -= Time.fixedDeltaTime;
            if (fuse <= 0f)
            {
                if (explodeSound != null)
                {
                    explodeSound.Play();
                }

                Vector3 vec = player.transform.position - transform.position;
                player.velocity += Flatten(vec).normalized * 10f;
                Radial component = Instantiate(radial.gameObject, canvasManager.baseCanvas.transform).GetComponent<Radial>();
                Vector3 normalized = Flatten(vec).normalized;
                component.position = 0f - (57.29578f * Mathf.Atan2(normalized.x, normalized.z) - player.Yaw + 180f);
                Destroy(this, 3f);
            }

            return;
        }

        if (Physics.Raycast(transform.position, velocity.normalized, out var hitInfo, velocity.magnitude * Time.fixedDeltaTime, 1, QueryTriggerInteraction.Ignore))
        {
            if (hitInfo.collider.CompareTag("Player"))
            {
                return;
            }

            transform.position = hitInfo.point;
            hit = true;
            visual.transform.localPosition = default;
        }
        else
        {
            transform.position += velocity * Time.fixedDeltaTime;
        }

        velocity += Vector3.down * Time.fixedDeltaTime * d;
    }

    private static Vector3 Flatten(Vector3 vec)
    {
        return new Vector3(vec.x, 0f, vec.z);
    }
}