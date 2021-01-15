using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TileSpriteController : MonoBehaviour
{
    public Sprite floorSprite;
    public Sprite treeSprite;
    public Sprite stoneSprite;
    public Sprite slowSprite; 
    
    Dictionary<Tile, GameObject> tileGameObjectMap;
    

    World world { get {
            return WorldController.Instance.world; }
    }

    // Start is called before the first frame update
    void Start()
    {


        tileGameObjectMap = new Dictionary<Tile, GameObject>();
        

        //Create all tile
        for (int x = 0; x < world.Width; x++)
        {
            for (int y = 0; y < world.Height; y++)
            {
                Tile tile_data = world.GetTileAt(x, y);

                GameObject tile_go = new GameObject();

                //Dict Mapping
                tileGameObjectMap.Add(tile_data, tile_go);

                tile_go.name = "Tile_" + x + "_" + y;
                tile_go.transform.position = new Vector3(x, y, 0);
                tile_go.transform.SetParent(this.transform, true);

                SpriteRenderer sr = tile_go.AddComponent<SpriteRenderer>();
                sr.sprite = floorSprite;
                sr.sortingLayerName = "Tile";


            }
        }


       world.RegisterTileChanged(OnTileChanged);
    }


    public void RandomizeTile_Button()
    {
        world.RandomizeTiles();
    }

    void DestroyAllTileGameObject()
    {
        while (tileGameObjectMap.Count > 0)
        {
            Tile tile_data = tileGameObjectMap.Keys.First();
            GameObject tile_go = tileGameObjectMap[tile_data];

            tileGameObjectMap.Remove(tile_data);
            tile_data.UnregisterTileChangedCallback(OnTileChanged);
            Destroy(tile_go);
        }
    }

    void OnTileChanged(Tile tile_data)
    {
        //Debug.Log("OnTileChanged");
        if (tileGameObjectMap.ContainsKey(tile_data) == false)
        {
            Debug.LogError("Tile_data unmapped");
            return;
        }

        GameObject tile_go = tileGameObjectMap[tile_data];

        if (tile_go == null)
        {
            Debug.LogError("No tile_go ");
            return;
        }

        if (tile_data.Type == TileType.Floor)
        {
            tile_go.GetComponent<SpriteRenderer>().sprite = floorSprite;
        }
        else if (tile_data.Type == TileType.StoneWall)
        {
            tile_go.GetComponent<SpriteRenderer>().sprite = stoneSprite;
        }
        else if (tile_data.Type == TileType.River)
        {
            tile_go.GetComponent<SpriteRenderer>().sprite = treeSprite;
        }
        else
        {
            Debug.LogError("OnTileTypeChanged -- Unreconginzed tile type");
        }
    }

   

}
