using System;
using System.IO;
using fNbt;
using RuntimeGizmos;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Object = System.Object;

namespace Level_Editor_Scripts
{
    public class SessionManager : MonoBehaviour
    {
        public enum EditorMode
        { 
            Place,
            Select,
        }

        public enum GameMode
        {
            Playing,
            Editing
        }

        public LevelEditorPrefabs Prefabs;
        
        public static SessionManager Instance { get; private set; }
        
        [Header("Editor")]
        public PlaceObject PlaceObject;
        public TransformGizmo Gizmo;
        [FormerlySerializedAs("ModeText")] public Text EditorModeText;
        public Camera EditorCamera;
        public KeyCode ModeSwitchKey = KeyCode.F1;
        public EditorMode CurrentMode { get; private set; } = EditorMode.Place;
        public CanvasManager CanvasManager;
        
        [Header("Game")]
        public GameMode CurrentGameMode = GameMode.Editing;
        public Text GameModeText;
        public GameObject Player;
        public MeshRenderer PlayerSpawnPoint;

        [Header("GameObject handling")]
        public GameObject[] GameObjectsToDisableOnPlay;
        public GameObject[] GameObjectsToDisableOnEdit;

        public static bool LockEditor;

        public InputField LoadMapName;
        public InputField MapName;
        public InputField AuthorName;
        public Text OpenWorldText;

        public Transform MapParent;
        public Transform CassetteParent;
        public Transform NotificationPoint;

        private bool _openWorld;
        
        public void SwitchOpenWorldMode()
        {
            _openWorld = !_openWorld;
            OpenWorldText.text = _openWorld ? "Open World" : "Linear";
        }
        
        public void UpdateOpenWorldText()
        {
            OpenWorldText.text = _openWorld ? "Open World" : "Linear";
        }

        public void LoadMap()
        {
            if (LoadMapName.text == string.Empty)
            {
                CanvasManager.SendNotification("Can't load map. No map name entered.", 5f, 40, NotificationPoint.transform.position);
                return;
            }

            try
            {


                string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Lightstrafe\\maps";
                Directory.CreateDirectory(path);

                if (File.Exists(path + "\\" + LoadMapName.text + ".lsm"))
                {
                    NbtFile mapFile = new NbtFile(path + "\\" + LoadMapName.text + ".lsm");
                    NbtCompound root = mapFile.RootTag;

                    NbtString mapName = root.Get<NbtString>("MapName");
                    NbtString authorName = root.Get<NbtString>("AuthorName");

                    MapName.text = mapName.Value;
                    AuthorName.text = authorName.Value;

                    // KILL children
                    foreach (Transform child in MapParent)
                    {
                        Destroy(child.gameObject);
                    }

                    NbtList spawnPoint = root.Get<NbtList>("SpawnPoint");

                    //todo: this is stupid
                    _openWorld = root.Get<NbtInt>("OpenWorld") != null && root.Get<NbtInt>("OpenWorld").Value == 1;
                    UpdateOpenWorldText();

                    PlayerSpawnPoint.transform.position = new Vector3(spawnPoint.Get<NbtFloat>(0).Value, spawnPoint.Get<NbtFloat>(1).Value, spawnPoint.Get<NbtFloat>(2).Value);

                    NbtCompound mapRoot = root.Get<NbtCompound>("MapObjects");
                    foreach (NbtTag tag in mapRoot.Tags)
                    {
                        NbtCompound compound = (NbtCompound)tag;

                        int catalogIndex = compound.Get<NbtInt>("CatalogIndex").Value;
                        NbtList position = compound.Get<NbtList>("Position");
                        NbtList rotation = compound.Get<NbtList>("Rotation");
                        NbtList scale = compound.Get<NbtList>("Scale");

                        Vector3 pos = new Vector3(position.Get<NbtFloat>(0).Value, position.Get<NbtFloat>(1).Value, position.Get<NbtFloat>(2).Value);
                        Vector3 rot = new Vector3(rotation.Get<NbtFloat>(0).Value, rotation.Get<NbtFloat>(1).Value, rotation.Get<NbtFloat>(2).Value);
                        Vector3 sca = new Vector3(scale.Get<NbtFloat>(0).Value, scale.Get<NbtFloat>(1).Value, scale.Get<NbtFloat>(2).Value);

                        GameObject go = Instantiate(Prefabs.Prefabs[catalogIndex], pos, Quaternion.Euler(rot), MapParent);
                        go.transform.localScale = sca;

                        go.AddComponent<BasicSerializableObject>().CatalogIndex = catalogIndex;
                    }

                    NbtCompound cassetteRoot = root.Get<NbtCompound>("Cassettes");
                    foreach (NbtTag tag in cassetteRoot.Tags)
                    {
                        NbtCompound compound = (NbtCompound)tag;

                        NbtList position = compound.Get<NbtList>("Position");
                        NbtList rotation = compound.Get<NbtList>("Rotation");
                        NbtList scale = compound.Get<NbtList>("Scale");

                        Vector3 pos = new Vector3(position.Get<NbtFloat>(0).Value, position.Get<NbtFloat>(1).Value, position.Get<NbtFloat>(2).Value);
                        Vector3 rot = new Vector3(rotation.Get<NbtFloat>(0).Value, rotation.Get<NbtFloat>(1).Value, rotation.Get<NbtFloat>(2).Value);
                        Vector3 sca = new Vector3(scale.Get<NbtFloat>(0).Value, scale.Get<NbtFloat>(1).Value, scale.Get<NbtFloat>(2).Value);

                        GameObject go = Instantiate(ObjectCatalog.Instance.Cassette, pos, Quaternion.Euler(rot), CassetteParent);
                        go.transform.localScale = sca;
                    }
                    CanvasManager.SendNotification("Map successfully loaded.", 5f, 40, NotificationPoint.transform.position);

                }

                else
                {
                    CanvasManager.SendNotification("Can't find map.", 5f, 40, NotificationPoint.transform.position);
                }
            }
            catch (Exception e)
            {
                CanvasManager.SendNotification("Can't load map. Error: " + e.Message, 5f, 40, NotificationPoint.transform.position);
            }
        }

        public void SaveMap()
        {

            if (MapName.text != string.Empty && AuthorName.text != string.Empty)
            {
                try
                {


                    string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Lightstrafe\\maps";
                    Directory.CreateDirectory(path);



                    NbtFile lsmFile = new NbtFile();
                    NbtCompound root = lsmFile.RootTag;

                    NbtString name = new NbtString("MapName", MapName.text);
                    NbtString authorName = new NbtString("AuthorName", AuthorName.text);

                    string date = DateTime.Now.ToString("MM-dd-y hh:mmtt");
                    date = date.Replace("/", "-");
                    date = date.Replace(":", "-");
                    NbtString lastModified = new NbtString("LastModified", date);

                    root.Add(name);
                    root.Add(authorName);
                    root.Add(lastModified);

                    NbtCompound mapRoot = new NbtCompound("MapObjects");
                    NbtCompound cassetteRoot = new NbtCompound("Cassettes");

                    NbtList spawnPoint = new NbtList("SpawnPoint", NbtTagType.Float);
                    spawnPoint.Add(new NbtFloat(PlayerSpawnPoint.transform.position.x));
                    spawnPoint.Add(new NbtFloat(PlayerSpawnPoint.transform.position.y));
                    spawnPoint.Add(new NbtFloat(PlayerSpawnPoint.transform.position.z));
                    root.Add(spawnPoint);

                    root.Add(new NbtInt("OpenWorld", _openWorld ? 1 : 0));

                    int index = 0;
                    foreach (Transform child in MapParent)
                    {
                        NbtCompound compound = new NbtCompound("Object" + index);

                        Debug.Log(child);
                        compound.Add(new NbtInt("CatalogIndex", child.gameObject.GetComponent<BasicSerializableObject>().CatalogIndex));

                        NbtList list = new NbtList("Position", NbtTagType.Float);
                        list.Add(new NbtFloat(child.position.x));
                        list.Add(new NbtFloat(child.position.y));
                        list.Add(new NbtFloat(child.position.z));

                        compound.Add(list);

                        list = new NbtList("Rotation", NbtTagType.Float);

                        Vector3 euler = child.rotation.eulerAngles;
                        list.Add(new NbtFloat(euler.x));
                        list.Add(new NbtFloat(euler.y));
                        list.Add(new NbtFloat(euler.z));

                        compound.Add(list);

                        list = new NbtList("Scale", NbtTagType.Float);
                        list.Add(new NbtFloat(child.localScale.x));
                        list.Add(new NbtFloat(child.localScale.y));
                        list.Add(new NbtFloat(child.localScale.z));

                        compound.Add(list);

                        mapRoot.Add(compound);

                        index++;
                    }

                    index = 0;

                    foreach (Transform child in CassetteParent)
                    {
                        NbtCompound cassette = new NbtCompound("Cassette" + index);

                        NbtList list = new NbtList("Position", NbtTagType.Float);
                        list.Add(new NbtFloat(child.position.x));
                        list.Add(new NbtFloat(child.position.y));
                        list.Add(new NbtFloat(child.position.z));

                        cassette.Add(list);

                        list = new NbtList("Rotation", NbtTagType.Float);
                        Vector3 euler = child.rotation.eulerAngles;
                        list.Add(new NbtFloat(euler.x));
                        list.Add(new NbtFloat(euler.y));
                        list.Add(new NbtFloat(euler.z));

                        cassette.Add(list);

                        list = new NbtList("Scale", NbtTagType.Float);
                        list.Add(new NbtFloat(child.localScale.x));
                        list.Add(new NbtFloat(child.localScale.y));
                        list.Add(new NbtFloat(child.localScale.z));

                        cassette.Add(list);

                        cassetteRoot.Add(cassette);

                        index++;
                    }

                    root.Add(mapRoot);
                    root.Add(cassetteRoot);

                    lsmFile.SaveToFile(path + "\\" + MapName.text + ".lsm", NbtCompression.GZip);
                    CanvasManager.SendNotification("Map successfully saved to: " + path + "\\" + MapName.text + ".lsm", 5f, 40, NotificationPoint.transform.position);
                }
                catch (Exception e)
                {
                    CanvasManager.SendNotification("Error while saving map. Error: " + e.Message, 5f, 20, NotificationPoint.transform.position);
                }
            }
            else if (MapName.text == string.Empty)
            {
                CanvasManager.SendNotification("Can't save map. No map name entered.", 5f, 40, NotificationPoint.transform.position);
            }
            else
            {
                CanvasManager.SendNotification("Can't save map. No author name entered.", 5f, 40, NotificationPoint.transform.position);
            }

        }

        public void SetCasstteMode()
        { 
            PlaceObject.CassetteMode = !PlaceObject.CassetteMode;
            CanvasManager.SendNotification("Cassette mode is on.", 5f, 40, NotificationPoint.transform.position);
        }
        
        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
                return;
            }
            
            // Manually bind player
            // ( because awake isn't triggered for the player, they start out disabled )
            Game.OnAwakeBind(Player.GetComponent<Player>());
            Game.OnAwakeBind(Player.GetComponent<PlayerAudioManager>());

            // ensure that we can actually do shit when the map loads in
            LockEditor = false;
            
            OnEditorModeSwitch(EditorMode.Select, EditorMode.Place);
            CurrentMode = EditorMode.Place;
            
            for (int i = 0; i < GameObjectsToDisableOnEdit.Length; i++)
            {
                GameObjectsToDisableOnEdit[i].SetActive(false);
            }
        }

        public void OnDestroy()
        {
            Instance = null;
        }

        public void OnEditorModeSwitch(EditorMode lastMode, EditorMode newMode)
        {
            if (lastMode == EditorMode.Select)
            {
                Gizmo.RemoveAllTargets();
                Gizmo.enabled = true;
                
                PlaceObject.enabled = true;
                PlaceObject.Gizmode = false;
            }
            else
            {
                //PlaceObject.enabled = false;
                Gizmo.enabled = true;
                PlaceObject.Gizmode = true;
            }

            EditorModeText.text = $"Mode: {newMode} ";

        }

        public void Update()
        {

            switch (CurrentGameMode)
            {
                case GameMode.Editing:
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.Confined;
                    
                    if (Input.GetKeyDown(ModeSwitchKey))
                    {
                        EditorMode newMode = CurrentMode == EditorMode.Place ? EditorMode.Select : EditorMode.Place;
                        OnEditorModeSwitch(CurrentMode, newMode);
                        CurrentMode = newMode;
                    }
                    break;
                case GameMode.Playing:
                    // hide cursor
                    if (!CanvasManager.MenuOpen)
                    {
                        Cursor.visible = false;
                        Cursor.lockState = CursorLockMode.Locked;
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        public void SwitchGameMode()
        {
            Player.transform.position = PlayerSpawnPoint.transform.position;
            Player.GetComponent<global::Player>().velocity = Vector3.zero;
            Player.GetComponent<global::Player>().Charges = 2;
            Player.GetComponent<global::Player>().SetQuickDeathPosition(PlayerSpawnPoint.transform.position, 0, Vector3.zero);
            
            switch (CurrentGameMode)
            {
                case GameMode.Editing:
                    // go to playing
                    CurrentGameMode = GameMode.Playing;
                    
                    Gizmo.enabled = false;
                    PlaceObject.enabled = false;

                    GameModeText.text = "Return";

                    EditorCamera.GetComponent<Camera>().enabled = false;
                    EditorCamera.GetComponent<SmoothCamera>().enabled = false;
                    EditorCamera.GetComponent<AudioListener>().enabled = false;
                    Player.gameObject.SetActive(true);
                    PlayerSpawnPoint.gameObject.SetActive(false);
                    
                    for (int i = 0; i < GameObjectsToDisableOnPlay.Length; i++)
                    {
                        GameObjectsToDisableOnPlay[i].SetActive(false);
                    }
                    for (int i = 0; i < GameObjectsToDisableOnEdit.Length; i++)
                    {
                        GameObjectsToDisableOnEdit[i].SetActive(true);
                    }
                    break;
                
                case GameMode.Playing:
                    CurrentGameMode = GameMode.Editing;
                    
                    PlaceObject.enabled = true;
                    Gizmo.enabled = true;
                    
                    EditorCamera.GetComponent<Camera>().enabled = true;
                    EditorCamera.GetComponent<SmoothCamera>().enabled = true;
                    EditorCamera.GetComponent<AudioListener>().enabled = true;
                    //EditorCamera.gameObject.SetActive(true);
                    Player.gameObject.SetActive(false);
                    PlayerSpawnPoint.gameObject.SetActive(true);

                    LockEditor = false;

                    for (int i = 0; i < GameObjectsToDisableOnPlay.Length; i++)
                    {
                        GameObjectsToDisableOnPlay[i].SetActive(true);
                    }
                    for (int i = 0; i < GameObjectsToDisableOnEdit.Length; i++)
                    {
                        GameObjectsToDisableOnEdit[i].SetActive(false);
                    }
                    
                    GameModeText.text = "Play";

                    CanvasManager.ForceCloseMenu();
                    break;
            }
        }
    }
}