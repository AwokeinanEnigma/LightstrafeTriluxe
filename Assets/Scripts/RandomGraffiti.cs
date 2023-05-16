#region

using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

#endregion

public class RandomGraffiti : MonoBehaviour
{
    public GameObject[] options;

    public float noGraffitiChance = 50f;

    private void Start()
    {

        // why would this ever be needed?
        //GetComponent<DecalProjector>().enabled = false;
        if (options.Length != 0 && !(Random.Range(0, 100) < noGraffitiChance))
        {
            GameObject obj = Instantiate(options[Random.Range(0, options.Length)], transform);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
        }
    }
}