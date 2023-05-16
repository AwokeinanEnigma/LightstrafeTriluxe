#region

using UnityEngine;
using UnityEngine.UI;

#endregion

public class Collectible : MonoBehaviour
{
    public const int MARGIN = 50;

    public GameObject NoSprite;

    public GameObject GemSprite;

    public Collectible PreviousCollectible;

    private GameObject nosprite;

    private GameObject gemsprite;

    public GameObject visual;

    public Transform quickspawn;

    private Player player;

    private CanvasManager canvasManager;

    private Level level;

    public int LeftToCollect { get; set; }

    public bool RequirementsMet => PreviousCollectible == null;

    private void Start()
    {
        canvasManager = Game.OnStartResolve<CanvasManager>();
        level = Game.OnStartResolve<Level>();
        player = Game.OnStartResolve<Player>();
        LeftToCollect = FindObjectsOfType<Collectible>().Length;
    }

    private void LateUpdate()
    {
        if (player == null)
        {
            return;
        }

        visual.transform.Rotate(0f, 0f, 50f * Time.deltaTime);
        if (RequirementsMet)
        {
            if (gemsprite == null)
            {
                gemsprite = Instantiate(GemSprite, canvasManager.screenSizeCanvas.transform);
                gemsprite.transform.SetAsFirstSibling();
            }

            if (nosprite != null)
            {
                Destroy(nosprite);
            }

            gemsprite.GetComponent<Image>().color = LeftToCollect == 1 ? Color.yellow : Color.white;
            Vector3 position = player.camera.WorldToScreenPoint(transform.position);
            float num = Mathf.Clamp01(1f - (Vector3.Distance(transform.position, player.camera.transform.position) - 20f) / 20f);
            num = 1f - num;
            if (position.x < 50f || position.y < 50f || position.x > Screen.width - 50 || position.y > Screen.height - 50 || player.camera.WorldToViewportPoint(transform.position).z < 0f)
            {
                Vector3 vector = new Vector3(position.x, position.y, position.z);
                if (player.camera.WorldToViewportPoint(transform.position).z < 0f)
                {
                    vector = new Vector3(Screen.width - position.x, Screen.height - position.y, position.z);
                }

                vector.x -= Screen.width / 2f;
                vector.y -= Screen.height / 2f;
                float num2 = vector.x > 0f ? Screen.width / 2f : -Screen.width / 2f;
                Vector2 vector2 = new Vector2(num2, num2 * vector.y / vector.x);
                float num3 = vector.y > 0f ? Screen.height / 2f : -Screen.height / 2f;
                Vector2 vector3 = new Vector2(num3 * vector.x / vector.y, num3);
                if (vector2.sqrMagnitude > vector3.sqrMagnitude)
                {
                    vector.x = vector3.x;
                    vector.y = vector3.y;
                }
                else
                {
                    vector.x = vector2.x;
                    vector.y = vector2.y;
                }

                vector.x += Screen.width / 2f;
                vector.y += Screen.height / 2f;
                position = vector;
                float num4 = Mathf.Clamp01((0f - Vector3.Dot(player.CrosshairDirection, (transform.position - player.camera.transform.position).normalized)) * 10f);
                num *= 1f + num4;
                num *= 0f;
            }

            num /= 1.8f;
            if (position.x < 50f)
            {
                position.x = 50f;
            }

            if (position.y < 50f)
            {
                position.y = 50f;
            }

            if (position.x > Screen.width - 50)
            {
                position.x = Screen.width - 50;
            }

            if (position.y > Screen.height - 50)
            {
                position.y = Screen.height - 50;
            }

            gemsprite.transform.position = position;
            gemsprite.transform.localScale = new Vector3(num, num, 1f);
        }
        else
        {
            if (nosprite == null)
            {
                nosprite = Instantiate(NoSprite, canvasManager.screenSizeCanvas.transform);
                nosprite.transform.SetAsFirstSibling();
            }

            if (gemsprite != null)
            {
                Destroy(gemsprite);
            }

            Vector3 position2 = player.camera.WorldToScreenPoint(transform.position);
            nosprite.transform.position = position2;
            float num5 = Mathf.Clamp01(1f - Vector3.Distance(transform.position, player.camera.transform.position) / 70f);
            Vector3 vector4 = player.camera.transform.position - transform.position;
            if (Physics.Raycast(transform.position, vector4.normalized, out var hitInfo, vector4.magnitude, 1, QueryTriggerInteraction.Ignore) && hitInfo.collider.gameObject != player.gameObject)
            {
                num5 = 0f;
            }

            if (player.camera.WorldToViewportPoint(transform.position).z < 0f)
            {
                num5 = 0f;
            }

            nosprite.transform.localScale = new Vector3(num5, num5, 1f);
        }
    }

    public void Collect()
    {
        if (RequirementsMet)
        {
            if (gemsprite != null)
            {
                Destroy(gemsprite);
            }

            if (nosprite != null)
            {
                Destroy(nosprite);
            }

            Collectible[] array = FindObjectsOfType<Collectible>();
            Collectible[] array2 = array;
            for (int i = 0; i < array2.Length; i++)
            {
                array2[i].GetComponent<Collectible>().LeftToCollect = array.Length - 1;
            }

            if (array.Length == 1)
            {
                level.LevelFinished();
            }

            player.AudioManager.PlayOneShot(player.wow, false, 0.4f);
            player.Recharge();
            player.Recharge();
            player.DoubleJumpAvailable = true;
            if (quickspawn != null)
            {
                Debug.Log("Quick death?");
                // not really sure how this works exactly, but it seems that the third arg controls how much velocity the player has when they respawn
                player.SetQuickDeathPosition(quickspawn.position, quickspawn.rotation.eulerAngles.y, Vector3.zero);
            }

            Destroy(gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        if (quickspawn != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(quickspawn.position, quickspawn.forward);
            Gizmos.DrawSphere(quickspawn.position, 0.5f);
        }
    }
}