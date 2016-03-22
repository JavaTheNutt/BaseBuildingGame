using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class WorldController : MonoBehaviour
{
    public static WorldController Instance { get; protected set; }

    [SerializeField]
    private Sprite floorSprite; // reference to the floor sprite
    [SerializeField]
    private Sprite wallSprite;
    [SerializeField]
    private GameObject walls;

    public World World { get; protected set; } //reference to the World

    private Dictionary<Tile, GameObject> tileGameObjectMap;
    private Dictionary<InstalledObject, GameObject> installedObjectGameObjectMap;
    // Use this for initialization
    void Start()
    {
        if (Instance != null) {
            Debug.LogError("There should never be two worldcontroller instances");
        }
        Instance = this;
        World = new World();
        World.RegisterInstalledObjectCreated(OnInstalledObjectCreated);
        tileGameObjectMap = new Dictionary<Tile, GameObject>();
        installedObjectGameObjectMap = new Dictionary<InstalledObject, GameObject>();
        for (int x = 0; x < World.Width; x++) {
            for (int y = 0; y < World.Height; y++) {
                Tile tile_data = World.GetTileAt(x, y); //get the tile data
                GameObject tile_go = new GameObject(); // create a gameobject for each tile
                tile_go.name = "Tile_" + x + "_" + y; // name the tile
                tile_go.transform.position = new Vector3(tile_data.X, tile_data.Y, 0); //move the game object to its correct position
                tileGameObjectMap.Add(tile_data, tile_go);
                tile_go.AddComponent<SpriteRenderer>(); // add an empty sprite renderer
                tile_go.transform.SetParent(this.transform, true);
                /*This will register the callback passing only the tile like it expects and inside call the method in this class passing expected parameters*/
                tile_data.RegisterTileTypeChangedCallback(OnTileTypeChanged);
            }
        }
        /*World.RandomiseTiles();*/
    }
    //NOT CURRENTLY IN USE
    //this will remove everything from the dictionary and destroy all visual gameobjects leaving the data intact
    void DestroyAllTileGameObjects()
    {
        while (tileGameObjectMap.Count > 0) {
            /*Get first item*/
            Tile tile_data = tileGameObjectMap.Keys.First();
            /*get associated game data*/
            GameObject tile_go = tileGameObjectMap[tile_data];
            /*Remove from dictionary*/
            tileGameObjectMap.Remove(tile_data);
            /*Unregister callback*/
            tile_data.UnregisterTileTypeChangedCallback(OnTileTypeChanged);
            //Destroy visual GameObject
        }
    }

    void OnTileTypeChanged(Tile tile_data)
    {
        if (!tileGameObjectMap.ContainsKey(tile_data)) {
            Debug.LogError("The dictionary does not contain this tile. Did you forget to add it? Or register a callback?");
            return;
        }
        GameObject tile_go = tileGameObjectMap[tile_data];
        if (tile_go == null) {
            Debug.LogError("The returned GameObject is null");
            return;
        }
        if (tile_data.Type == TileType.Floor) {
            tile_go.GetComponent<SpriteRenderer>().sprite = floorSprite;
        } else if (tile_data.Type == TileType.Empty) {
            tile_go.GetComponent<SpriteRenderer>().sprite = null;
        } else {
            Debug.LogError("OnTileTypeChanged -- Invalid Tile Type");
        }
    }
    public Tile GetTileAtWorldCoord(Vector3 coord)
    {
        int x = Mathf.FloorToInt(coord.x);
        int y = Mathf.FloorToInt(coord.y);

        return World.GetTileAt(x, y);
    }

    public void OnInstalledObjectCreated(InstalledObject obj)
    {
        if (obj == null) {
            return;
        }
        //todo does not consider multi-tile objects or rotated objects
        GameObject objGO = new GameObject(); // create a gameobject for each tile
        installedObjectGameObjectMap.Add(obj, objGO);
        objGO.name = obj.ObjectType + "_" + obj.Tile.X + "_" + obj.Tile.Y; // name the tile
        objGO.transform.position = new Vector3(obj.Tile.X, obj.Tile.Y, 0); //move the game object to its correct position
        objGO.AddComponent<SpriteRenderer>().sprite = wallSprite; // todo add sprites
        objGO.transform.SetParent(walls.transform, true);
        obj.RegisterOnChangedCallback(OnInstalledObjectChanged);
    }

    void OnInstalledObjectChanged(InstalledObject obj)
    {
        Debug.LogError("Error -- OnInstalledObjectChanged not implemented");
    }
}
