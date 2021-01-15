using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy
{
    public float X
    {
        get
        {
            return Mathf.Lerp(currTile.X, nextTile.X, movementPercentage);
        }
    }
    public float Y
    {
        get
        {
            return Mathf.Lerp(currTile.Y, nextTile.Y, movementPercentage);
        }
    }

    public Tile currTile { get; protected set; }
    Tile destTile;
    Tile nextTile; // next tile to walk
    Path_AStar pathAStar;
    float movementPercentage;
    float speed = 4f; //TileSpriteController per second
    Action<Enemy> cbCharacterChanged;
    public float _animFrameLen = 0.25f;
    public int frameCount = 4;
    public int frame = 0;
    public string direction = "N";
    public float animaledtime = 0f;
    public float slowcooldown = 0f; 


    public Enemy(Tile tile)
    {
        currTile = destTile = nextTile = tile;
    }

    public void Update(float deltaTime)
    {

        if (currTile == destTile)
        {
            pathAStar = null;
            return;
        }

        animaledtime += deltaTime;
        if (animaledtime >= _animFrameLen * frameCount)
        {
            animaledtime = 0f;
        }
        frame = (int)(animaledtime / _animFrameLen);


        if (nextTile == null || nextTile == currTile)
        {
            if(pathAStar == null)
            {
                pathAStar = new Path_AStar(currTile.world, currTile, destTile);
                if(pathAStar.Length() == 0)
                {
                    Debug.LogError("PathAStar -- No path found");
                    
                    //TODO
                    //Break the wall and walk stright line
                    nextTile = currTile.world.GetTileAt(currTile.X, currTile.Y + 1);
                    pathAStar = null;
                    if(currTile.installedObject != null)
                    {
                        currTile.installedObject.Deconstruct();
                        
                    }
                    return;
                }

            }
            nextTile = pathAStar.Dequeue();

            if (nextTile == currTile)
            {
                
                Debug.LogError("Enemy Update -- nextTile is currTile?");
            }
        }

        if (slowcooldown == 0)
        {
            if (currTile.CheckNeighbours(currTile.GetNeighbours(true), objectType.Slow))
            {
                speed = 2f;
                slowcooldown = 500f;
            }
            else
            {
                speed = 4f;
            }
        }
        else
        {
            slowcooldown -= 1;
        }
        

        float distToTravel = Mathf.Sqrt(Mathf.Pow(currTile.X - nextTile.X, 2) + Mathf.Pow(currTile.Y - nextTile.Y, 2));

        float distThisFrame = speed * Time.deltaTime;

        float perceThisFrame = distThisFrame / distToTravel;

        movementPercentage += perceThisFrame;

        if (distToTravel != 0)
        {
            if (currTile.X == nextTile.X)
            {
                if (currTile.Y > nextTile.Y)
                {
                    direction = "S";
                }
                else
                {
                    direction = "N";
                }
            }
            if (currTile.Y == nextTile.Y)
            {
                if (currTile.X > nextTile.X)
                {
                    direction = "W";
                }
                else
                {
                    direction = "E";
                }
            }
            if (currTile.X > nextTile.X & currTile.Y > nextTile.Y)
            {
                if (currTile.X - nextTile.X > currTile.Y - nextTile.Y)
                {
                    direction = "W";
                }
                else
                {
                    direction = "S";
                }
            }
            if (currTile.X < nextTile.X & currTile.Y < nextTile.Y)
            {
                if (currTile.X - nextTile.X < currTile.Y - nextTile.Y)
                {
                    direction = "E";
                }
                else
                {
                    direction = "N";
                }
            }
            if (currTile.X > nextTile.X & currTile.Y < nextTile.Y)
            {
                if (currTile.X - nextTile.X > nextTile.Y - currTile.Y)
                {
                    direction = "W";
                }
                else
                {
                    direction = "N";
                }
            }
            if (currTile.X < nextTile.X & currTile.Y > nextTile.Y)
            {
                if (nextTile.X - currTile.X > currTile.Y - nextTile.Y)
                {
                    direction = "E";
                }
                else
                {
                    direction = "S";
                }
            }
        }

        if (movementPercentage >= 1)
        {
            currTile = nextTile;
            movementPercentage = 0;
        }
        if (cbCharacterChanged != null)
        {
            cbCharacterChanged(this);
        }
    }




    public void SetDestination(Tile tile)
    {
        if (currTile.IsNeighbour(tile, true) == false)
        {
            Debug.Log("Character :SetDestination Error ");
        }
        Debug.Log("SetDestination");
        destTile = tile;
    }

    public void RegisterOnChangedCallback(Action<Enemy> cb)
    {
        cbCharacterChanged += cb;
    }

    public void UnregisterOnChangedCallback(Action<Enemy> cb)
    {
        cbCharacterChanged -= cb;
    }

    public bool ReachDestination()
    {
        return currTile == destTile;
    }

    public void ResetLocaiton()
    {
        currTile = currTile.world.GetTileAt(6, 0);
        destTile = currTile;
        nextTile = currTile;
        cbCharacterChanged(this);
    }
}
