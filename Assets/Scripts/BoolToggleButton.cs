#region

using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

#endregion

public class BoolToggleButton : MonoBehaviour
{
    public Text text;

    public string boolName;

    private PropertyInfo property;

    private void Start()
    {
        PropertyInfo[] properties = typeof(GameSettings).GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public);
        foreach (PropertyInfo propertyInfo in properties)
        {
            if (propertyInfo.Name == boolName)
            {
                property = propertyInfo;
            }
        }
    }

    private void Update()
    {
        bool flag = (bool)property.GetValue(null);
        text.text = flag ? "Yes" : "No";
    }

    public void Toggle()
    {
        property.SetValue(null, !(bool)property.GetValue(null));
    }
}