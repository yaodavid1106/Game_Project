using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpriteController : MonoBehaviour
{
    // Start is called before the first frame update

    Dictionary<Character, GameObject> characterGameObjectMap;
    Dictionary<Enemy, GameObject> enemyGameObjectMap;
    Dictionary<string, Sprite> characterSprites;

    World world
    {
        get
        {
            return WorldController.Instance.world;
        }
    }

    void Start()
    {
        LoadSprites();

        characterGameObjectMap = new Dictionary<Character, GameObject>();
        enemyGameObjectMap = new Dictionary<Enemy, GameObject>();

        world.RegisterCharacterCreated(OnCharacterCreated);

        world.RegisterEnemyCreated(OnEnemyCreated);

        Character c =  world.CreateCharacter(world.GetTileAt(world.Width / 2, world.Height / 2));

        Enemy e = world.CreateEnemey(world.GetTileAt(world.Width / 2, 0)) ;
        

    }

    void LoadSprites()
    {
        //Load sprites from resourses
        characterSprites = new Dictionary<string, Sprite>();
        Sprite[] sprites = Resources.LoadAll<Sprite>("Character/");

        foreach (Sprite s in sprites)
        {
            characterSprites[s.name] = s;
        }
        Sprite[] sprites_2 = Resources.LoadAll<Sprite>("Enemy/");

        foreach (Sprite s in sprites_2)
        {
            characterSprites[s.name] = s;
        }
    }

    public void OnCharacterCreated(Character character)
    {
        GameObject char_go = new GameObject();
        characterGameObjectMap.Add(character, char_go);

        char_go.name = "Character";
        char_go.transform.position = new Vector3(character.X, character.Y, 0);
        SpriteRenderer sr = char_go.AddComponent<SpriteRenderer>();
        sr.sprite = characterSprites["char_0"];
        sr.sortingLayerName = "Character";
        character.RegisterOnChangedCallback(OnCharacterChanged);
    }


    public void OnCharacterChanged(Character c)
    {
        GameObject char_go = characterGameObjectMap[c];

        SpriteRenderer sr = char_go.GetComponent<SpriteRenderer>();
        if (c.direction == "idle")
        {
            
            sr.sprite = characterSprites["char_0"];
        }
        else
        {
            int frame = c.frame + 1; 
            sr.sprite = characterSprites["char_" + c.direction + "_" + frame];
        }
        char_go.transform.position = new Vector3(c.X, c.Y, 0);


    }

    public void OnEnemyCreated(Enemy enemy)
    { 
        GameObject char_go = new GameObject();
        enemyGameObjectMap.Add(enemy, char_go);

        char_go.name = "Enemy";
        char_go.transform.position = new Vector3(enemy.X, enemy.Y, 0);
        SpriteRenderer sr = char_go.AddComponent<SpriteRenderer>();
        sr.sprite = characterSprites["Zombie_N_0"];
        sr.sortingLayerName = "Character";
        enemy.RegisterOnChangedCallback(OnEnemyChanged);
    }


    public void OnEnemyChanged(Enemy c)
    {
        GameObject char_go = enemyGameObjectMap[c];
        SpriteRenderer sr = char_go.GetComponent<SpriteRenderer>();
        if (c.direction == "idle")
        {

            sr.sprite = characterSprites["Zombie_N_0"];
        }
        else
        {
            sr.sprite = characterSprites["Zombie_" + c.direction + "_" + c.frame];
        }
        if (c.slowcooldown != 0)
        {
            sr.color = new Color(0.5f, 0.5f, 1f, 1f);
        }
        else
        {
            sr.color = new Color(1f, 1f, 1f, 1f);
        }

        char_go.transform.position = new Vector3(c.X, c.Y, 0);
    }

    public void EnemyPathfindTest()
    {
        foreach(Enemy e in enemyGameObjectMap.Keys)
        {

            if (e.X == 6 && e.Y == 11)
            {
                StartCoroutine(world.gameTimer.Add_Timer());
                e.SetDestination(WorldController.Instance.world.GetTileAt(6, 0));
            }
            else
            {
                StartCoroutine(world.gameTimer.Add_Timer());
                e.SetDestination(WorldController.Instance.world.GetTileAt(6, 11));
            }   
        }
    }
}