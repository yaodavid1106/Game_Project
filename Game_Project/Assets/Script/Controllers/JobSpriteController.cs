using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobSpriteController : MonoBehaviour
{


    InstalledObjectSpriteController iosc;
    Dictionary<Job, GameObject> jobGameObjectMap;

    // Start is called before the first frame update
    void Start()
    {
        jobGameObjectMap = new Dictionary<Job, GameObject>();
        iosc = GameObject.FindObjectOfType<InstalledObjectSpriteController>();

        WorldController.Instance.world.jobQueue.RegisterJobCreationCallback(OnJobCreated);


        
    }

    void OnJobCreated(Job job)
    {

        //Create sprite for job
        GameObject job_go = new GameObject();

        //Dict Mapping
        jobGameObjectMap.Add(job, job_go);

        job_go.name = "JOB_" + job.tile.X + "_" + job.tile.Y;
        job_go.transform.position = new Vector3(job.tile.X, job.tile.Y, 0);
        job_go.transform.SetParent(this.transform, true);


        if (job.jobObjectType == objectType.Empty)
        {
            SpriteRenderer sr = job_go.AddComponent<SpriteRenderer>();
            sr.sprite = iosc.GetSpriteForInstalledObject(job.tile.installedObject);
            sr.color = new Color(1f, 0f, 0f, 0.7f);
            sr.sortingLayerName = "Job";
        }
        else
        {
            SpriteRenderer sr = job_go.AddComponent<SpriteRenderer>();
            sr.sprite = iosc.GetSpriteForInstalledObject_Easy(job.jobObjectType);
            sr.color = new Color(0.5f, 1f, 1f, 0.5f);
            sr.sortingLayerName = "Job";
        }

        job.RegisterJobCompleteCallback(OnJobEnded);
        job.RegisterJobCancelCallback(OnJobEnded);
    }

    void OnJobEnded(Job job)
    {
        GameObject job_go = jobGameObjectMap[job];

        job.UnregisterJobCancelCallback(OnJobEnded);
        job.UnregisterJobCompleteCallback(OnJobEnded);

        Destroy(job_go);
    }


}
