using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Character
{
    public float X
    {
        get
        {
            return Mathf.Lerp(currTile.X, destTile.X, movementPercentage);
        }
    }
    public float Y
    {
        get
        {
            return Mathf.Lerp(currTile.Y, destTile.Y, movementPercentage);
        }
    }

    public Tile currTile { get; protected set; }
    Tile destTile;
    float movementPercentage;
    float speed = 4f; //TileSpriteController per second
    public float animaledtime = 0f;
    public float _animFrameLen = 0.1111f;
    public int frameCount = 9;
    public int frame = 0;
    public string direction = "N";

    Action<Character> cbCharacterChanged;

    Job myJob;

    public Character(Tile tile)
    {
        currTile = destTile = tile;
    }

    bool Update_DoJob(float deltaTime)
    {
        if (myJob == null)
        {
            myJob = currTile.world.jobQueue.Dequeue();

            if (myJob != null)
            {
                destTile = myJob.tile;
                myJob.RegisterJobCancelCallback(OnJobEnded);
                myJob.RegisterJobCompleteCallback(OnJobEnded);
            }
        }

        if (currTile == destTile)
        {
            if (myJob != null)
            {
                myJob.DoWork(deltaTime);
            }
            return false;
        }

        return true;
    }


    void Update_Movement(float deltaTime)
    {
        float distToTravel = Mathf.Sqrt(Mathf.Pow(currTile.X - destTile.X, 2) + Mathf.Pow(currTile.Y - destTile.Y, 2));

        float distThisFrame = speed * Time.deltaTime;

        float perceThisFrame = distThisFrame / distToTravel;

        movementPercentage += perceThisFrame;

        if(distToTravel != 0)
        {
            if(currTile.X == destTile.X)
            {
                if(currTile.Y > destTile.Y)
                {
                    direction = "S";
                }
                else
                {
                    direction = "N";
                }
            }
            if (currTile.Y == destTile.Y)
            {
                if (currTile.X > destTile.X)
                {
                    direction = "W";
                }
                else
                {
                    direction = "E";
                }
            }
            if (currTile.X > destTile.X & currTile.Y > destTile.Y)
            {
                if (currTile.X - destTile.X > currTile.Y - destTile.Y)
                {
                    direction = "W";
                }
                else
                {
                    direction = "S";
                }
            }
            if (currTile.X < destTile.X & currTile.Y < destTile.Y)
            {
                if (currTile.X - destTile.X < currTile.Y - destTile.Y)
                {
                    direction = "E";
                }
                else
                {
                    direction = "N";
                }
            }
            if (currTile.X > destTile.X & currTile.Y < destTile.Y)
            {
                if (currTile.X - destTile.X > destTile.Y- currTile.Y)
                {
                    direction = "W";
                }
                else
                {
                    direction = "N";
                }
            }
            if (currTile.X < destTile.X & currTile.Y > destTile.Y)
            {
                if (destTile.X - currTile.X  > currTile.Y - destTile.Y)
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
            currTile = destTile;
            movementPercentage = 0;
        }


        if (cbCharacterChanged != null)
        {

            cbCharacterChanged(this);
        }
    }

    public void Update(float deltaTime)
    {
        animaledtime += deltaTime;
        if(animaledtime >= _animFrameLen * frameCount)
        {
            animaledtime = 0f;
        }
        frame = (int)(animaledtime / _animFrameLen);
        if (Update_DoJob(deltaTime) == false)
        {
            if(direction != "idle")
            {
                direction = "idle";
                cbCharacterChanged(this);
            }
            return;
        }
        
        Update_Movement(deltaTime);
        
    }




    public void SetDestination(Tile tile)
    {
        if (currTile.IsNeighbour(tile,true) == false)
        {
            Debug.Log("Character :SetDestination Error ");
        }
        destTile = tile;
    }

    public void RegisterOnChangedCallback(Action<Character> cb)
    {
        cbCharacterChanged += cb;
    }

    public void UnregisterOnChangedCallback(Action<Character> cb)
    {
        cbCharacterChanged -= cb;
    }

    void OnJobEnded(Job j)
    {
        if (j != myJob)
        {
            Debug.LogError("Character being told about job that isn't his");
            return;
        }
        myJob = null;
        j.tile.pendingInstalledObject = null;
    }
}
