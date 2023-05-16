using System;
using System.IO;
using fNbt;
using UnityEngine;

namespace Level_Editor_Scripts
{
    public class CustomMapLoader : MonoBehaviour
    {
        public GameObject[] EnableOnLoad;
        public GameObject Cassette;
        public Transform MapParent;

        public Player player;
        private GameObject _lastCasstte;
        public Vector3 PlayerPosition;
        public static string MapName;

        public GameObject[] Prefabs;
        public static bool CustomMap;
        
        public static string Author;
        public static string LastModified;
        
        public void OnEnable()
        {
            // load map
            LoadMap();
            // load objects
            foreach (GameObject go in EnableOnLoad)
            {
                go.SetActive(true);
            }
            player.transform.position = PlayerPosition;
            CustomMap = true;
            player.SetQuickDeathPosition(PlayerPosition, 0, Vector3.forward);
        }

        public void OnDestroy()
        {

            CustomMap = false;
        }
        
        public void LoadMap()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Lightstrafe\\maps";
            Directory.CreateDirectory(path);

            if (File.Exists(path + "\\" + MapName + ".lsm"))
            {
                Debug.Log("Found map!!!");
                NbtFile mapFile = new NbtFile(path + "\\" + MapName + ".lsm");
                NbtCompound root = mapFile.RootTag;

                NbtCompound mapRoot = root.Get<NbtCompound>("MapObjects");
                NbtList spawnPosition = root.Get<NbtList>("SpawnPoint");
                PlayerPosition = new Vector3(spawnPosition.Get<NbtFloat>(0).Value, spawnPosition.Get<NbtFloat>(1).Value, spawnPosition.Get<NbtFloat>(2).Value);
                
                Author = root.Get<NbtString>("AuthorName").Value;
                LastModified = root.Get<NbtString>("LastModified").Value;
                
                foreach (NbtTag tag in mapRoot.Tags)
                {
                    NbtCompound compound = (NbtCompound) tag;
                    
                    int catalogIndex = compound.Get<NbtInt>("CatalogIndex").Value;
                    NbtList position = compound.Get<NbtList>("Position");
                    NbtList rotation = compound.Get<NbtList>("Rotation");
                    NbtList scale = compound.Get<NbtList>("Scale");
                    
                    Vector3 pos = new Vector3(position.Get<NbtFloat>(0).Value, position.Get<NbtFloat>(1).Value, position.Get<NbtFloat>(2).Value);
                    Vector3 rot = new Vector3(rotation.Get<NbtFloat>(0).Value, rotation.Get<NbtFloat>(1).Value, rotation.Get<NbtFloat>(2).Value);
                    Vector3 sca = new Vector3(scale.Get<NbtFloat>(0).Value, scale.Get<NbtFloat>(1).Value, scale.Get<NbtFloat>(2).Value);
                    
                    GameObject go = Instantiate(Prefabs[catalogIndex], pos, Quaternion.Euler(rot), MapParent);
                    go.transform.localScale = sca;
                    
                    go.AddComponent<BasicSerializableObject>().CatalogIndex = catalogIndex;
                }
                
                NbtInt openWorld = root.Get<NbtInt>("OpenWorld");
                bool open = openWorld != null && openWorld.Value == 1;
                
                NbtCompound cassetteRoot = root.Get<NbtCompound>("Cassettes");
                foreach (NbtTag nbtTag in cassetteRoot.Tags)
                {
                    NbtCompound compound = (NbtCompound) nbtTag;
                    
                    NbtList position = compound.Get<NbtList>("Position");
                    NbtList rotation = compound.Get<NbtList>("Rotation");
                    NbtList scale = compound.Get<NbtList>("Scale");
                    
                    Vector3 pos = new Vector3(position.Get<NbtFloat>(0).Value, position.Get<NbtFloat>(1).Value, position.Get<NbtFloat>(2).Value);
                    Vector3 rot = new Vector3(rotation.Get<NbtFloat>(0).Value, rotation.Get<NbtFloat>(1).Value, rotation.Get<NbtFloat>(2).Value);
                    Vector3 sca = new Vector3(scale.Get<NbtFloat>(0).Value, scale.Get<NbtFloat>(1).Value, scale.Get<NbtFloat>(2).Value);
                    
                    GameObject go = Instantiate(Cassette, pos, Quaternion.Euler(rot), MapParent);
                    go.transform.localScale = sca;
                    
                    //linear
                    if (!open)
                    {
                        go.GetComponent<Collectible>().PreviousCollectible = _lastCasstte ? _lastCasstte.GetComponent<Collectible>() : null;
                        _lastCasstte = go;
                    }

                }
                
            

            }
            else
            {
                Debug.Log("Error loading map.");
            }
        }
    }
}