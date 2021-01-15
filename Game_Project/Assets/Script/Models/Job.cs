using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Job
{
    public Tile tile { get; protected set; }
    float jobTime;

    public objectType jobObjectType{ get; protected set; }

    Action<Job> cbJobComplete;
    Action<Job> cbJobCancel;

    public Job(Tile tile, objectType jobObjectType,Action<Job> cbJobComplete, float jobtime = 0.5f)
    {
        this.tile = tile;
        this.jobObjectType = jobObjectType;
        this.cbJobComplete += cbJobComplete;
        this.jobTime = jobTime;
    }

    public void RegisterJobCompleteCallback(Action<Job> cb)
    {
       cbJobComplete += cb;
    }

    public void UnregisterJobCompleteCallback(Action<Job> cb)
    {
        cbJobComplete -= cb;
    }

    public void RegisterJobCancelCallback(Action<Job> cb)
    {
        cbJobCancel += cb;
    }

    public void UnregisterJobCancelCallback(Action<Job> cb)
    {
        cbJobComplete -= cb;
    }

    public void DoWork(float workTime)
    {
        jobTime -= workTime;
        if(jobTime <= 0)
        {
            if(cbJobComplete != null)
            {
              cbJobComplete(this);
            }
        }
    }

    public void CancelJob()
    {
        if (cbJobCancel != null)
        {
            cbJobCancel(this);
        }
    }
}
