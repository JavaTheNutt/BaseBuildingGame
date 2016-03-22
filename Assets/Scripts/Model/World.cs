using System;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

/**
*This will hold an array of tiles and perform all operations on this array of tiles
*/
public class World
{
    private Tile[,] tiles;

    public int Width { get; protected set; }

    public int Height { get; protected set; }

    private Dictionary<string, InstalledObject> installedObjectPrototypes;
    private Action<InstalledObject> cbInstalledObjectCreated;
    public World(int width = 100, int height = 100)
    {
        this.Width = width;
        this.Height = height;
        tiles = new Tile[width, height];
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                tiles[x, y] = new Tile(this, x, y);
            }
        }
        installedObjectPrototypes = new Dictionary<string, InstalledObject>();
        CreateInstalledObjectPrototypes();
        Debug.Log("World created with " + (width * height) + " tiles");
    }

    private void CreateInstalledObjectPrototypes()
    {
        installedObjectPrototypes.Add("Wall", InstalledObject.CreatePrototype("Wall", 0f, 1, 1));
    }
    public void RandomiseTiles()
    {
        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                if (Random.Range(0, 2) == 1) {
                    tiles[x, y].Type = TileType.Empty;
                } else {
                    tiles[x, y].Type = TileType.Floor;
                }
            }
        }
    }
    public Tile GetTileAt(int x, int y)
    {
        if (x > Width || y > Height || x < 0 || y < 0) {
            //Debug.Log("Tile (" + x + "," + y + ") is out of bounds" );
            return null;
        }
        return tiles[x, y];
    }

    public void PlaceInstalledObject(string objType, Tile t)
    {
        Debug.Log("Place Installed instance");
        if (!installedObjectPrototypes.ContainsKey(objType)) {
            Debug.LogError("installedObjectPrototypes dosen't contain a prototype for key: " + objType);
            return;
        }
        InstalledObject obj = InstalledObject.PlaceObject(t, installedObjectPrototypes[objType]);
        if (cbInstalledObjectCreated != null) {
            cbInstalledObjectCreated(obj);
        }
    }

    public void RegisterInstalledObjectCreated(Action<InstalledObject> callback)
    {
        cbInstalledObjectCreated += callback;
    }
    public void UnregisterInstalledObjectCreated(Action<InstalledObject> callback)
    {
        cbInstalledObjectCreated -= callback;
    }
}
