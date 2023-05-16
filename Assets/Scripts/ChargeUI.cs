#region

using UnityEngine;
using UnityEngine.UI;

#endregion

public class ChargeUI : MonoBehaviour
{
    public GameObject UiBar;

    private GameObject[] bars;

    private float[] fillAmounts;

    private Player player;

    private void Start()
    {
        player = Game.OnStartResolve<Player>();
        bars = new GameObject[2];
        fillAmounts = new float[2];
        int num = 5;
        for (int i = 0; i < 2; i++)
        {
            bars[i] = Instantiate(UiBar, transform);
            float num2 = num + bars[i].GetComponent<RectTransform>().rect.width;
            float num3 = 1f * num2 / 2f;
            bars[i].transform.localPosition = new Vector3(i * num2 - num3, 0f, 0f);
        }
    }

    private void Update()
    {
        bool flag = player != null && (player.GrappleEnabled || player.DashEnabled);
        for (int i = 0; i < 2; i++)
        {
            float num = Mathf.Clamp01((player == null ? 0f : player.Charges) - i);
            Image component = bars[i].GetComponent<Image>();
            component.fillAmount = num;
            if (num >= 1f && fillAmounts[i] < 1f)
            {
                component.color = Color.white;
            }
            else
            {
                component.color = Color.Lerp(component.color, Color.gray, Time.deltaTime * 3f);
            }

            fillAmounts[i] = num;
            if (bars[i].activeSelf && !flag)
            {
                bars[i].SetActive(false);
            }

            if (!bars[i].activeSelf && flag)
            {
                bars[i].SetActive(true);
            }
        }
    }
}