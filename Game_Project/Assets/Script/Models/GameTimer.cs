using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimer
{
    public float startTime { get; protected set; }

    public float timer1 { get; protected set; }

    public float buffer_time;



    public World world { get; protected set; }

    public GameTimer(World world)
    {
        this.world = world;
        startTime = 40f;
        timer1 = 10f;
    }


    public IEnumerator Add_Timer()
    {
        do
        {
            if(world.paused != true)
            {
                startTime += Time.deltaTime;
            }
            yield return null;
        }
        while (world.enemies[0].ReachDestination() == false);

        world.ReachtheEnd = true;
    }

    public IEnumerator Min_Timer()
    {
        do
        {
            if (world.paused != true)
            {
                timer1 += Time.deltaTime;
            }
            yield return null;
        } 
        while (timer1 < startTime);
    }

    public void setstarttime(float t)
    {
        startTime = t;
    }

    public void addtimer1(float t)
    {
        timer1 += t;
    }
}
