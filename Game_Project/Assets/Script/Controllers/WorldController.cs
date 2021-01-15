using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WorldController : MonoBehaviour
{
    public static WorldController Instance { get; protected set; }

    public World world { get; protected set; }


   
    // Start is called before the first frame update
    void OnEnable()
    {
        if(Instance != null)
        {
            Debug.LogError("There is two world controller");
        }
        Instance = this;
        world = new World();
    }

    //void DestroyAllTileGameObject()
    //{
    //    while (tileGameObjectMap.Count > 0)
    //    {
    //        Tile tile_data = tileGameObjectMap.Keys.First();
    //        GameObject tile_go = tileGameObjectMap[tile_data];

    //        tileGameObjectMap.Remove(tile_data);
    //        tile_data.UnregisterTileChangedCallback(OnTileChanged);
    //        Destroy(tile_go);
    //    }
    //}


    private void Update()
    {
        if (world.paused){
            return;
        }
        world.Update(Time.deltaTime);
    }

    public Tile GetTileAtWorldCoord(Vector3 coord)
    {
        int x = Mathf.FloorToInt(coord.x);
        int y = Mathf.FloorToInt(coord.y);

        return world.GetTileAt(x, y);
    }

}
