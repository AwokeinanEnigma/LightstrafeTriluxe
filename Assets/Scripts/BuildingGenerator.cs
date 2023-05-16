#region

using UnityEngine;

#endregion

[SelectionBase]
public class BuildingGenerator : MonoBehaviour
{
    public GameObject top;

    public GameObject[] slicePrefabs;

    public GameObject sliceContainer;

    public bool randomizeRotation = true;

    public string pattern = "000000000000";
}