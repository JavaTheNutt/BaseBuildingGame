using UnityEngine;
using System.Collections;
using System;

public enum TileType { Empty, Floor };
/**
*This will represent an instance of a single tile
*/
public class Tile
{

    TileType _type = TileType.Empty;

    private Action<Tile> CbTileTypeChanged; //actions are technically an array of methods, so multiple methods can be added here to be executed sequentially 

    public TileType Type {
        get { return _type; }
        set {
            TileType oldType = _type;
            _type = value;
            if (CbTileTypeChanged != null && _type != oldType) {
                CbTileTypeChanged(this);
            }
        }
    }

    LooseObject looseObject;
    InstalledObject installedObject;

    World world;


    public int X { get; protected set; }


    public int Y { get; protected set; }

    public Tile(World world, int x, int y)
    {
        this.world = world;
        this.X = x;
        this.Y = y;
    }
    /*This  will add the specified action to the function passed*/
    public void RegisterTileTypeChangedCallback(Action<Tile> callback)
    {
        CbTileTypeChanged += callback;
    }
    /*This will remove a function from the list of callbacks, may throw an errror if specifed action does not exist*/
    public void UnregisterTileTypeChangedCallback(Action<Tile> callback)
    {
        CbTileTypeChanged -= callback;
    }

    public bool PlaceObject(InstalledObject objInstance)
    {
        /*This will uninstall an installed object if null is passed*/
        if (objInstance == null) {
            installedObject = null;
            return true;
        }

        if (installedObject != null) {
            Debug.LogError("You are trying to add an object to a tile that already has an object installed");
            return false;
        }
        installedObject = objInstance;
        return true;
    }
}
