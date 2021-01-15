using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InstalledObjectSpriteController : MonoBehaviour
{
    
    Dictionary<InstalledObject, GameObject> installedObjectGameObjectMap;

    Dictionary<string, Sprite> installedObjectSprites;

    World world { get {
            return WorldController.Instance.world; }
    }

    // Start is called before the first frame update
    void Start()
    {

        LoadSprites();

        
        installedObjectGameObjectMap = new Dictionary<InstalledObject, GameObject>();



       
       world.RegisterInstalledObjectCreated(OnInstalledObjectCreated);
      
    }


    void LoadSprites()
    {
        //Load sprites from resourses
        installedObjectSprites = new Dictionary<string, Sprite>();
        Sprite[] sprites = Resources.LoadAll<Sprite>("Images/Wall");

        foreach (Sprite s in sprites)
        {
            installedObjectSprites[s.name] = s;
        }

        Sprite[] sprites_2 = Resources.LoadAll<Sprite>("Images/Tree");

        foreach (Sprite s in sprites_2)
        {
            installedObjectSprites[s.name] = s;
        }

        Sprite[] sprites_3 = Resources.LoadAll<Sprite>("Images/Slow");

        foreach (Sprite s in sprites_3)
        {
            installedObjectSprites[s.name] = s;
        }


    }

    public void OnInstalledObjectCreated(InstalledObject obj)
    {
        // Create a visual obj
        GameObject obj_go = new GameObject();

        //Dict Mapping
        installedObjectGameObjectMap.Add(obj, obj_go);

        obj_go.name = obj.ObjectType+"_"+ obj.tile.X + "_" + obj.tile.Y;
        obj_go.transform.position = new Vector3(obj.tile.X, obj.tile.Y, 0);
        obj_go.transform.SetParent(this.transform, true);

        SpriteRenderer obj_sr = obj_go.AddComponent<SpriteRenderer>();
        obj_sr.sprite = GetSpriteForInstalledObject(obj);
        obj_sr.sortingLayerName = "InstalledObject";


        obj.RegisterOnChangedCallback(OnInstalledChanged);
        obj.RegisterOnRemovedCallback(OnFurnitureRemoved);
    }

    void OnFurnitureRemoved(InstalledObject obj)
    {

        if(installedObjectGameObjectMap.ContainsKey(obj) == false)
        {
            Debug.Log("OnFurnitureRemoved -- trying to change visuals for installed object not in our map");
            return;
        }

        GameObject obj_go = installedObjectGameObjectMap[obj];
        Destroy(obj_go);
    }

    void OnInstalledChanged(InstalledObject obj)
    {
        //Update installed object near by.
        if(installedObjectGameObjectMap.ContainsKey(obj) == false)
        {
            Debug.LogError("OnInstalledChanged -- obj not mapped");
            return;
        }

        GameObject obj_go = installedObjectGameObjectMap[obj];
        obj_go.GetComponent<SpriteRenderer>().sprite = GetSpriteForInstalledObject(obj);

    }



    public Sprite GetSpriteForInstalledObject(InstalledObject obj)
    {
        if(obj.linksToNeighbour == false)
        {
            return installedObjectSprites[obj.ObjectType.ToString()];
        }

        string spriteNmae = obj.ObjectType.ToString() + "_";

        Tile t;
        int x = obj.tile.X;
        int y = obj.tile.Y;

        t = world.GetTileAt(x +1, y);
        if (t != null && t.installedObject != null && t.installedObject.ObjectType == obj.ObjectType)
        {
            spriteNmae += "E";
        }
        t = world.GetTileAt(x, y - 1);
        if (t != null && t.installedObject != null && t.installedObject.ObjectType == obj.ObjectType)
        {
            spriteNmae += "S";
        }
        t = world.GetTileAt(x-1, y);
        if(t != null && t.installedObject!= null && t.installedObject.ObjectType == obj.ObjectType)
        {
            spriteNmae += "W";
        }
        t = world.GetTileAt(x, y + 1);
        if (t != null && t.installedObject != null && t.installedObject.ObjectType == obj.ObjectType)
        {
            spriteNmae += "N";
        }

        if (installedObjectSprites.ContainsKey(spriteNmae) == false)
        {
            Debug.LogError("GetSpriteForInstalledObject -- No that sprite" + spriteNmae);
            return null;
        }

        return installedObjectSprites[spriteNmae];

    }

    public Sprite GetSpriteForInstalledObject_Easy(objectType objectType)
    {
        if (installedObjectSprites.ContainsKey(objectType+"_"))
        {
            return installedObjectSprites[objectType+"_"];
        }

        Debug.LogError("GetSpriteForInstalledObject -- No that sprite" + objectType+"_");
        return null;
    }


}
