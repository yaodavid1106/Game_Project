using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public enum objectType { Empty, Tree, Wall, Stone,Slow };

public class InstalledObject
{

    public Tile tile{ get; protected set; }

    public List<Tile> tile_set { get; protected set; }

    //defalut 1, 0 = blocked
    public float movementCost{ get; protected set; }

    public int Width { get; protected set; }
    public int Height { get; protected set; }

    public bool linksToNeighbour { get; protected set; }

    public Action<InstalledObject> cbOnChanged { get; protected set; }

    public Action<InstalledObject> cbOnRemovd; 

    Func<Tile, bool> funcPositionValidation;


    private objectType _objectType = objectType.Empty;

    public objectType ObjectType
    {
        get
        {
            return _objectType;
        }
        set
        {
            
            objectType oldType = _objectType;
            _objectType = value;
            //Callback
            if (cbOnChanged != null && oldType != _objectType)
            {
                cbOnChanged(this);
            }
        }
    }
    protected InstalledObject()
    {

    }

    static public InstalledObject CreatePrototype (objectType objectType, float movementCost = 1f, int width = 1, int height = 1, bool linksToNeighbour = true)
    {
        InstalledObject obj = new InstalledObject();
        obj.tile_set = new List<Tile>();
        obj.ObjectType = objectType;
        obj.movementCost = movementCost; 
        obj.Width = width;
        obj.Height = height;
        obj.linksToNeighbour = linksToNeighbour;
        obj.funcPositionValidation = obj.__IsVaildPosition;
        return obj;
    }

    static public InstalledObject PlaceInstance(InstalledObject proto, Tile tile)
    {
        if (proto.funcPositionValidation(tile) == false)
        {
            Debug.Log("PlaceInstance -- Position not valid");
            return null;
        }

        InstalledObject obj = new InstalledObject();
        obj.ObjectType = proto._objectType;
        obj.tile_set = proto.tile_set;
        obj.movementCost = proto.movementCost;
        obj.Width = proto.Width;
        obj.Height = proto.Height;
        obj.linksToNeighbour = proto.linksToNeighbour;
        obj.funcPositionValidation = proto.funcPositionValidation;

        obj.tile = tile;


        if (tile.PlaceObject(obj) == false)
        {
            return null;
        }

        if (obj.linksToNeighbour)
        {
            Tile t;
            int x = tile.X;
            int y = tile.Y;

            t = tile.world.GetTileAt(x + 1, y);
            if (t != null && t.installedObject != null && t.installedObject.ObjectType == obj.ObjectType)
            {
                t.installedObject.cbOnChanged(t.installedObject);
            }
            t = tile.world.GetTileAt(x, y - 1);
            if (t != null && t.installedObject != null && t.installedObject.ObjectType == obj.ObjectType)
            {
                t.installedObject.cbOnChanged(t.installedObject);
            }
            t = tile.world.GetTileAt(x - 1, y);
            if (t != null && t.installedObject != null && t.installedObject.ObjectType == obj.ObjectType)
            {
                t.installedObject.cbOnChanged(t.installedObject);
            }
            t = tile.world.GetTileAt(x, y + 1);
            if (t != null && t.installedObject != null && t.installedObject.ObjectType == obj.ObjectType)
            {
                t.installedObject.cbOnChanged(t.installedObject);
            }
        }



        return obj;
    }

    public void Deconstruct()
    {

        tile.UninstallObject();
        if(cbOnRemovd != null)
        {
            cbOnRemovd(this);
        } 
    }

    public void RegisterOnChangedCallback(Action<InstalledObject> callbackFunc)
    {
        cbOnChanged += callbackFunc;
    }
    public void UnregisterOnChangedCallback(Action<InstalledObject> callbackFunc)
    {
        cbOnChanged -= callbackFunc;
    }

    public void RegisterOnRemovedCallback(Action<InstalledObject> callbackFunc)
    {
        cbOnRemovd += callbackFunc;
    }
    public void UnregisterOnRemovedCallback(Action<InstalledObject> callbackFunc)
    {
        cbOnRemovd -= callbackFunc;
    }

    public void AddTileToTileSet(Tile t)
    {
        tile_set.Add(t);
    } 

    public bool IsVaildPosition(Tile t)
    {
        return funcPositionValidation(t);
    }

    public bool __IsVaildPosition(Tile t)
    {

        //Make sure is floor
        //Make sure no other installed object

        for (int x_off = t.X; x_off < (t.X + Width); x_off++)
        {
            for (int y_off = t.Y; y_off < (t.Y + Height); y_off++)
            {
                Tile t2 = t.world.GetTileAt(x_off, y_off);



                if (t.Type != TileType.Floor)
                {
                    return false;
                }
                if (t.installedObject != null)
                {
                    Debug.LogError(t.installedObject.ObjectType);
                    return false;
                }  
            }
        }
        return true;
    }

}