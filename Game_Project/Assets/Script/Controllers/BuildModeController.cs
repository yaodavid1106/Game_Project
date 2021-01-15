using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildModeController : MonoBehaviour
{
    

 
    //Build_Mode
    enum BuildMode{view,wall,cut,deconstruct }

    BuildMode CurrBuildMode = BuildMode.view;
    objectType CurrBuildObject = objectType.Empty;

    void Start()
    {

    }

    public void SetMode_BuildInstalledObject()
    {
        if (CurrBuildMode == BuildMode.wall)
        {
            ResetBuildMode();
        }
        else
        {
            CurrBuildMode = BuildMode.wall;
            CurrBuildObject = objectType.Wall;
            
        }
    }

    public void SetMode_CutTree()
    {
        if (CurrBuildMode == BuildMode.cut)
        {
            CurrBuildMode = BuildMode.view;
        }
        else
        {
            CurrBuildMode = BuildMode.cut;
            CurrBuildObject = objectType.Empty;
        }
    }

    public void SetMode_Deconstruct()
    {
        if (CurrBuildMode == BuildMode.deconstruct)
        {
            CurrBuildMode = BuildMode.view;
        }
        else
        {
            CurrBuildMode = BuildMode.deconstruct;
            CurrBuildObject = objectType.Empty;
        }
    }

    public void DoPathfindingTest()
    {
        WorldController.Instance.world.SetupPathfindingExample();

        Path_TileGraph tileGraph = new Path_TileGraph(WorldController.Instance.world);
    }


    public void DoBuild(Tile t)
    {
        if (t.world.paused)
        {
            return;
        }

        if (CurrBuildMode == BuildMode.cut)
        {
            if (t != null)
            {
                if (t.installedObject != null)
                {
                    if (t.installedObject.ObjectType == objectType.Tree && t.installedObject.tile.pendingInstalledObject == null)
                    {
                        Tile t2 = t.world.GetTileAt(t.installedObject.tile.X, t.installedObject.tile.Y);

                        Job j = new Job(t2, objectType.Empty, (theJob) =>
                        {
                            OnCallTreeJobComplete(t2);
                        });


                        t.installedObject.tile.pendingInstalledObject = j;
                        j.RegisterJobCancelCallback((theJob) => { theJob.tile.pendingInstalledObject = null; });

                        WorldController.Instance.world.jobQueue.Enqueue(j);
                    }
                }
            }
        }

        if (CurrBuildMode == BuildMode.deconstruct)
        {
            if (t != null)
            {
                if (t.installedObject != null)
                {
                    if (t.installedObject.ObjectType == objectType.Wall && t.installedObject.tile.pendingInstalledObject == null)
                    {
                        Job j = new Job(t, objectType.Empty, (theJob) =>
                        {
                            OnCallWallDesJobComplete(t);
                        });
                        t.installedObject.tile.pendingInstalledObject = j;
                        j.RegisterJobCancelCallback((theJob) => { theJob.tile.pendingInstalledObject = null; });

                        WorldController.Instance.world.jobQueue.Enqueue(j);
                    }
                }
            }
        }

        if (CurrBuildObject != objectType.Empty && t.world.wood > 0) 
        {
            if (WorldController.Instance.world.IsInstalledObjectPlacementValid(CurrBuildObject, t)
                && t.pendingInstalledObject == null)
            {


                Job j = new Job(t,objectType.Wall, (theJob) =>
                {
                    OnInstallObjectJobComplete(t);
                });


                t.world.WoodChange(-1);
                t.pendingInstalledObject = j;
                j.RegisterJobCancelCallback((theJob) => { theJob.tile.pendingInstalledObject = null; });

                WorldController.Instance.world.jobQueue.Enqueue(j);
               
            }
        }
    }


    void OnInstallObjectJobComplete(Tile t)
    {
        WorldController.Instance.world.PlaceInstalledObject(CurrBuildObject, t);
    }

    void OnCallTreeJobComplete(Tile t)
    {
        t.installedObject.Deconstruct();
        t.world.WoodChange(2);
    }

    void OnCallWallDesJobComplete(Tile t)
    {
        t.installedObject.Deconstruct();
        t.world.WoodChange(1);
    }

    public void ResetBuildMode()
    {
        CurrBuildMode = BuildMode.view;
        CurrBuildObject = objectType.Empty;
    }
}
