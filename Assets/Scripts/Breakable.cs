#region

using UnityEngine;

#endregion

public class Breakable : MonoBehaviour
{
    public GameObject unbroken;

    public GameObject[] fragments;

    public void Break(Vector3 impulse, Vector3 playerPosition)
    {
        GetComponent<Collider>().enabled = false;
        unbroken.SetActive(false);
        GameObject[] array = fragments;
        foreach (GameObject obj in array)
        {
            obj.SetActive(true);
            Rigidbody component = obj.GetComponent<Rigidbody>();
            Vector3 center = obj.GetComponent<Renderer>().bounds.center;
            Vector3 vector = new Vector3(Random.Range(-60, 60), Random.Range(-60, 60), Random.Range(-60, 60));
            float magnitude = (center - playerPosition).magnitude;
            float num = (10f - magnitude) / 3f;
            if (num < 1f)
            {
                num = 1f;
            }

            component.AddForce(impulse * num * 25f + vector);
            float num2 = 200f * num;
            component.AddTorque(Random.Range(0f - num2, num2), Random.Range(0f - num2, num2), Random.Range(0f - num2, num2));
        }
    }
}