using System;
using System.Linq;
using RuntimeGizmos;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Level_Editor_Scripts
{
    public class PlaceObject : MonoBehaviour
    {
        public static GameObject Object;
        public GameObject MapParent;
        public Camera MainCamera;
        public TransformGizmo Gizmo;

        public bool Gizmode;
        
        public static bool CassetteMode;
        public GameObject Cassette;
        public Transform CassetteParent;
        public LevelEditorPrefabs Prefabs;

        public void Update()
        {
            if (!SessionManager.LockEditor)
            {


                if (Input.GetMouseButtonDown(0))
                {

                    Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);


                    if (Physics.Raycast(ray, out RaycastHit hit, 1000)) // && hit.transform.gameObject.name == "Flat Plane")
                    {
                        Debug.DrawLine(ray.origin, hit.point, Color.red, 25f);
                        if (!Gizmode)
                        {

          

                                Vector3 position = hit.point;
                                if (hit.transform.gameObject.name == "Flat Plane")
                                {
                                    position.y = 0f;
                                }
                                else
                                {
                                    // make sure we're on the surface of the hit object
                                    position += hit.normal * 3f;
                                }

                                if (!CassetteMode)
                                {
                                    GameObject obj = Instantiate(Object, position, Object.transform.rotation, MapParent.transform);
                                    obj.AddComponent<BasicSerializableObject>().CatalogIndex = Array.IndexOf(Prefabs.Prefabs, Object);
                                }
                                else
                                {
                                    Instantiate(Cassette, position, Cassette.transform.rotation, CassetteParent);
                                }
                            

                        }   
                        else
                        {
                            if (hit.transform.gameObject.name == "Flat Plane")
                            {
                                if (Physics.SphereCast(ray, 2f, out hit, 1000, LayerMask.GetMask("Default")))
                                {
                                    Gizmo.PassTarget(hit);
                                }
                                else if (Gizmo.nearAxis == Axis.None)
                                {
                                    Gizmo.ClearTargets();

                                }
                            }
                            else 
                            {
                                Gizmo.PassTarget(hit);
                            }
                        }
                    }
                    else if (Gizmode && Gizmo.nearAxis == Axis.None)
                    {
                        Gizmo.ClearTargets();
                    }
                }
                
                if (Input.GetKeyDown(KeyCode.Delete) && Gizmo.HasTarget)
                {
                    //bool canDelete = Gizmo.targetRoots.for
                    Gizmo.DeleteObject();
                }
                
                if (Input.GetKeyDown(KeyCode.D) && Input.GetKey(KeyCode.LeftShift) && Gizmo.HasTarget)
                {
                    Debug.Log("hi");
                    Gizmo.DuplicateObject();
                }
            }
        }
    }
}
    
