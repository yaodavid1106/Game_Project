using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class World
{
    Tile[,] tiles;
    List<Character> characters;
    public List<Enemy> enemies { get; protected set; }

    // For pathfinding
    public Path_TileGraph tileGraph;

    public bool paused = false;

    Dictionary<objectType, InstalledObject> installedObjectPrototypes;


    public int Width { get; protected set; }
    public int Height { get; protected set; }

    Action<InstalledObject> cbInstalledObjectCreated;
    Action<Character> cbCharacterCreated;
    Action<Enemy> cbEnemyCreated;
    Action<Tile> cbTileChanged;

    public JobQueue jobQueue;

    public GameTimer gameTimer;

    public objectType[,] tiles_Install_object_type_copy;

    public float gameTimer_copy;

    public int wood = 10;

    public int wood_copy = 10;

    public bool ReachtheEnd = false;

    public GameObject Wood_Btn = GameObject.Find("Button-Wood");

    public World(int width=12, int height = 12)
    {
        jobQueue = new JobQueue();
        gameTimer = new GameTimer(this);
        
        
        
        Width = width;
        Height = height;

        WoodChange(0);

        tiles = new Tile[width, height];
        tiles_Install_object_type_copy = new objectType[width, height];

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                tiles[x, y] = new Tile(this, x, y);
                tiles[x, y].RegisterTileChangedCallback(OnTileChanged);
                tiles[x, y].Type = TileType.Floor;
            }
            
        }

        Debug.Log("World created with" + width +" X " +height);


        CreateInstalledObjectPrototypes();

        characters = new List<Character>();
        enemies = new List<Enemy>();
      
    }

    public void Update(float deltatime)
    {
        foreach(Character c in characters)
        {
            c.Update(deltatime);
        }
        foreach (Enemy e in enemies)
        {
            e.Update(deltatime);
        }
    }

    public void WoodChange(int Number)
    {
        wood += Number;
        Wood_Btn.GetComponentInChildren<Text>().text = "Wood : " + wood;

    }

    public Character CreateCharacter(Tile t )
    {
        Character c = new Character(t);
        characters.Add(c);

        if (cbCharacterCreated != null)
        {
            cbCharacterCreated(c);
        }
        return c;
    }

    public Enemy CreateEnemey(Tile t)
    {
        Enemy e = new Enemy(t);
        enemies.Add(e);

        if (cbEnemyCreated != null)
        {
            cbEnemyCreated(e);
        }
        return e;
    }

    void CreateInstalledObjectPrototypes()
    {
        installedObjectPrototypes = new Dictionary<objectType, InstalledObject>();


        installedObjectPrototypes.Add(objectType.Wall, InstalledObject.CreatePrototype(
            objectType.Wall,
            0,
            1,
            1,
            true));

        installedObjectPrototypes.Add(objectType.Stone, InstalledObject.CreatePrototype(
            objectType.Stone,
            0,
            1,
            1,
            false));
        installedObjectPrototypes.Add(objectType.Slow, InstalledObject.CreatePrototype(
            objectType.Slow,
            0,
            1,
            1,
            false));
        installedObjectPrototypes.Add(objectType.Tree, InstalledObject.CreatePrototype(
            objectType.Tree,
            0,
            2,
            2,
            false));
    }

   
    public void RandomizeTiles()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if ((x == 0 || y == 0 || x == Width - 1 || y == Height - 1) && (x!=5 && x !=6))
                {
                    tiles[x, y].Type = TileType.StoneWall;
                }

                if (tiles[x,y].installedObject != null)
                {
                    tiles[x, y].installedObject.Deconstruct();
                }
                
            }

        }

        for (int x = 1; x < Width-2; x++)
        {
            for (int y = 1; y < Height - 2; y++)
            {
                if ((y == 10 || y == 1) && (x == 5 || x == 6))
                {
                    continue;
                }

                 if (UnityEngine.Random.Range(0, 5) == 2)
                {
                    if (UnityEngine.Random.Range(0, 20) == 1)
                    {
                        PlaceInstalledObject(objectType.Slow, tiles[x, y]);
                    }
                    else
                    {
                        if (UnityEngine.Random.Range(0, 2) == 1)
                        {
                            PlaceInstalledObject(objectType.Tree, tiles[x, y]);
                        }
                        else
                        {
                            PlaceInstalledObject(objectType.Stone, tiles[x, y]);
                        }
                    }
                 }
            }

        }

        InvalidTileGraph();
        Copy_timer_tile();

    }

    public void SetupPathfindingExample()
    {
        int l = Width / 2 - 3;
        int b = Height / 2 - 3;

        for (int x = l - 3; x < 1 + 11; x++)
            for (int y = b - 3; y < b + 9; y++)
            {
                tiles[x, y].Type = TileType.Floor;

                if (x == l || x == (l + 6) || y == b || y == (b + 6))
                {
                    if (x != (l + 6) && y != (b + 1))
                    {
                        tiles[x, y].PlaceObject(installedObjectPrototypes[objectType.Tree]);

                    }
                }
            }
    }




    public Tile GetTileAt(int x, int y)
    {
        if (x > Width-1 || x < 0 || y > Height-1 || y < 0)
        {
            //Debug.LogError("Tile  (" + x + "," + y + ") is out of range.");
            return null;
        }
        return tiles[x,y];
    }


    public void PlaceInstalledObject(objectType objectType, Tile t)
    {
     
        if(objectType == objectType.Empty)
        {
            t.PlaceObject(null);
            InvalidTileGraph();
            return;

        }

        if (installedObjectPrototypes.ContainsKey(objectType) == false)
        {
            Debug.LogError("installedObjectPrototypes Dictionary doesn't have that objecttype");
            return;
        }
        
        InstalledObject obj = InstalledObject.PlaceInstance(installedObjectPrototypes[objectType], t);

        if (obj == null)
        {
            //Failed to place object -- probably have object already.
            return;
        }

        obj.RegisterOnRemovedCallback(OnInstalledObjectRemoved);

        if(cbInstalledObjectCreated != null)
        {
            cbInstalledObjectCreated(obj);

            InvalidTileGraph();
        }

    }

    void OnInstalledObjectRemoved( InstalledObject obj)
    {

    }

    public void RegisterInstalledObjectCreated(Action<InstalledObject> callbackfunc)
    {
        cbInstalledObjectCreated += callbackfunc;
    }
    public void UnregisterInstalledObjectCreated(Action<InstalledObject> callbackfunc)
    {
        cbInstalledObjectCreated -= callbackfunc;
    }

    public void RegisterCharacterCreated(Action<Character> callbackfunc)
    {
        cbCharacterCreated += callbackfunc;
    }
    public void UnregisterCharacterCreated(Action<Character> callbackfunc)
    {
        cbCharacterCreated -= callbackfunc;
    }

    public void RegisterEnemyCreated(Action<Enemy> callbackfunc)
    {
        cbEnemyCreated += callbackfunc;
    }
    public void UnregisterEnemyCreated(Action<Enemy> callbackfunc)
    {
        cbEnemyCreated -= callbackfunc;
    }


    public void RegisterTileChanged(Action<Tile> callbackfunc)
    {
        cbTileChanged += callbackfunc;
    }
    public void UnregisterTileChanged(Action<Tile> callbackfunc)
    {
        cbTileChanged -= callbackfunc;
    }

    void OnTileChanged(Tile t)
    {
        if (cbTileChanged == null)
            return;

        cbTileChanged(t);

        InvalidTileGraph();
    }

    public void Copy_timer_tile()
    {
        Debug.Log("Copy_timer_tile");
        gameTimer_copy = gameTimer.startTime-gameTimer.timer1+ gameTimer.timer1;
        wood_copy = wood+1-1;

        for (int x = 0; x < Width-1; x++)
        {
            for (int y = 0; y < Height-1; y++)
            {
                if (tiles[x, y].installedObject == null)
                {
                    tiles_Install_object_type_copy[x, y] = objectType.Empty;
                }
                else
                {
                    tiles_Install_object_type_copy[x, y] = tiles[x, y].installedObject.ObjectType;
                }
            }
                

        }
    }

    public void Reset_timer_tile()
    {
        gameTimer.setstarttime(gameTimer_copy);
        wood = wood_copy + 1 - 1;
        WoodChange(0);

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if ((x == 0 || y == 0 || x == Width - 1 || y == Height - 1) && (x != 5 && x != 6))
                {
                    tiles[x, y].Type = TileType.StoneWall;
                }

                if (tiles[x, y].installedObject != null)
                {
                    tiles[x, y].installedObject.Deconstruct();
                }

            }

        }
        for (int x = 0; x < Width-1; x++)
        {
            for (int y = 0; y < Height-1; y++)
            {
                if(tiles_Install_object_type_copy[x,y] == objectType.Empty)
                {
                    continue;
                }
                PlaceInstalledObject(tiles_Install_object_type_copy[x, y], tiles[x, y]);
                    
                
            }

        }
    }

    public void InvalidTileGraph()
    {
        tileGraph = null; 
    }


    public bool IsInstalledObjectPlacementValid(objectType installOBjectType, Tile t)
    {
        return installedObjectPrototypes[installOBjectType].IsVaildPosition(t);
    }


    public InstalledObject GetInstalledObjectPrototype(objectType objectType)
    {
        if (installedObjectPrototypes.ContainsKey(objectType) == false)
        {
            Debug.LogError("No furniture with type : " + objectType);
            return null;
        }

        return installedObjectPrototypes[objectType];
    }
}
