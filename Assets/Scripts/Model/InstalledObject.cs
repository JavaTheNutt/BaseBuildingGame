using System;
using UnityEngine;
using System.Collections;

/**
*This will represent an instance of an installed object such as a wall or door
*/
public class InstalledObject
{
    public Tile Tile { get; protected set; }
    public string ObjectType { get; protected set; }
    public float MovementCost { get; protected set; }

    private Action<InstalledObject> cbOnChange;
    public int Width { get; protected set; }
    public int Height { get; protected set; }

    protected InstalledObject()
    {

    }

    public static InstalledObject CreatePrototype(string objectType, float movementCost = 1f, int width = 1, int height = 1)
    {
        InstalledObject obj = new InstalledObject();
        obj.ObjectType = objectType;
        obj.MovementCost = movementCost;
        obj.Width = width;
        obj.Height = height;
        return obj;
    }
    public static InstalledObject PlaceObject(Tile tile, InstalledObject proto)
    {
        InstalledObject obj = new InstalledObject();
        obj.Tile = tile;
        obj.ObjectType = proto.ObjectType;
        obj.MovementCost = proto.MovementCost;
        obj.Width = proto.Width;
        obj.Height = proto.Height;
        if (!tile.PlaceObject(obj)) {
            return null;
        }
        return obj;
    }

    public void RegisterOnChangedCallback(Action<InstalledObject> callback)
    {
        cbOnChange += callback;
    }
    public void UnregisterOnChangedCallback(Action<InstalledObject> callback)
    {
        cbOnChange -= callback;
    }

    private void OnInstalledObjectChanged()
    {

    }
}
