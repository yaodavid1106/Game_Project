using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public enum TileType { Empty, Floor, River, StoneWall };

public class Tile
{

    TileType _type = TileType.Empty;

    Action<Tile> cbTileChanged;
    

    LosseObject losseObject;
    public InstalledObject installedObject { get; protected set; }

    public World world { get; protected set; }

    public Job pendingInstalledObject;

    public float movementCost 
    {
        get{
            if (Type == TileType.Empty || Type == TileType.StoneWall || Type == TileType.River)
            {
                return 0;
            }
            if (installedObject == null)
            {
                return 1;
            }
            else{
                return 1 * installedObject.movementCost;
            }
            }
    }


    public TileType Type
    {
        get
        {
            return _type;
        }
        set
        {
            TileType oldType = _type;
            _type = value;
            //Callback
            if (cbTileChanged != null && oldType != _type)
            {
                cbTileChanged(this);
            }
            if (_type == TileType.Empty)
            {
                installedObject = null;
            }
        }
    }

    public int X { get; protected set; }
    public int Y { get; protected set; }

    public Tile(World world, int x, int y )
    {
        this.world = world;
        this.X = x;
        this.Y = y;
    }

    public void RegisterTileChangedCallback(Action<Tile> callback)
    {
        cbTileChanged += callback;
    }

    public void UnregisterTileChangedCallback(Action<Tile> callback)
    {
        cbTileChanged -= callback;
    }




    public bool UninstallObject()

    {
        if (installedObject == null)
        {
            return false; 
        }

        InstalledObject obj = installedObject;

        for (int x_off = X; x_off < (X + obj.Width); x_off++)
        {
            for (int y_off = Y; y_off < (Y + obj.Height); y_off++)
            {
                Tile t = world.GetTileAt(x_off, y_off);
                t.installedObject = null;
            }
        }
        return true;
    }
  

    public bool PlaceObject(InstalledObject objInstance)
    {
        if(objInstance == null)
        {
            return UninstallObject();
        }
        if (objInstance.IsVaildPosition(this)== false)
        {
            Debug.LogError("PlaceObject -- invaildpostition");
            return false;
        }
                if (installedObject != null)
                {
                    //There is already a object on it
                    Debug.LogError("There is already a object on it");
                    return false;
                }
            for (int x_off = X; x_off < (X + objInstance.Width); x_off++)
           {
                for (int y_off = Y; y_off < (Y + objInstance.Height); y_off++)
                {
                
                Tile t = world.GetTileAt(x_off, y_off);
                t.installedObject = objInstance;
                objInstance.AddTileToTileSet(t);

                
                }
        }
        return true;
    }

    public bool IsNeighbour(Tile tile,bool diagOkay = false)
    {
        if(this.X == tile.X && (this.Y == tile.Y+1 || this.Y == tile.Y-1))
        {
            return true;
        }
        if (this.Y == tile.Y && (this.X == tile.X + 1 || this.X == tile.X - 1))
        {
            return true;
        }

        if (diagOkay)
        {
            if (this.X == tile.X+1 && (this.Y == tile.Y + 1 || this.Y == tile.Y - 1))
            {
                return true;
            }
            if (this.X == tile.X - 1 && (this.Y == tile.Y + 1 || this.Y == tile.Y - 1))
            {
                return true;
            }
        }
        return false;
    }
    public Tile[] GetNeighbours(bool diagOkay = false)
    {
        Tile[] ns;
        if(diagOkay == false)
        {
            ns = new Tile[4];
        }
        else
        {
            ns = new Tile[8];
        }
        Tile n;

        n = world.GetTileAt(X, Y + 1);
        ns[0] = n;
        n = world.GetTileAt(X + 1, Y);
        ns[1] = n;
        n = world.GetTileAt(X, Y - 1);
        ns[2] = n;
        n = world.GetTileAt(X - 1, Y);
        ns[3] = n; 
        
        if (diagOkay == true)
        {
            n = world.GetTileAt(X + 1, Y + 1);
        ns[4] = n;
            n = world.GetTileAt(X + 1, Y - 1);
        ns[5] = n;
            n = world.GetTileAt(X - 1, Y - 1);
        ns[6] = n;
            n = world.GetTileAt(X - 1, Y + 1);
        ns[7] = n; 
        }

        return ns;
    }

    public bool CheckNeighbours (Tile[] ns, objectType ot)
    {
        foreach(Tile t in ns)
        {
            if (t == null)
                continue;
            if (t.installedObject!= null)
            {
                if (t.installedObject.ObjectType == ot)
                {
                    return true;
                }
            }
        }
        return false;
    }


   }
