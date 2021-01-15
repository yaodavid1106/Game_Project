using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public GameObject Start_Menu;
    public GameObject Option_Menu;
    public GameObject Pause_Menu;
    public GameObject Option_Menu_in_game;
    public GameObject Retry_Menu;

    public GameObject BuildMenu;

    public Button Time_Button; 

    bool gamestart = false;
    bool paused = false;
    bool retry_pause = false;


    World world
    {
        get
        {
            return WorldController.Instance.world;
        }
    }



    private void Start()
    {
        Option_Menu.SetActive(false);
        Pause_Menu.SetActive(false);
        Option_Menu_in_game.SetActive(false);
        Retry_Menu.SetActive(false);

        

    }

    private void Update()
    {
        Pause_Menu_funciton();
        Retry_menu_function();
    }

    public void Pause_Menu_funciton()
    {
        if (Input.GetKeyDown("escape") && gamestart == true)
        {
            if (paused)
            {
                Pause_Menu.SetActive(false);
                paused = false;
                world.paused = false;
            }
            else
            {
                Pause_Menu.SetActive(true);
                paused = true;
                world.paused = true;
            }
        }
    }

    public void Retry_menu_function()
    {
        if (retry_pause == false)
        {
            if (world.ReachtheEnd)
            {
                retry_pause = true;
                Retry_Menu.SetActive(true);
                Time_Button.GetComponentInChildren<Text>().text = "Time :" +(world.gameTimer.startTime-world.gameTimer_copy).ToString("F2") + "s";

            }
        }
    }

    public void Retry_button()
    {
        world.enemies[0].ResetLocaiton();
        world.Reset_timer_tile();
        world.ReachtheEnd = false;
        retry_pause = false;
        Retry_Menu.SetActive(false);
        BuildMenu.SetActive(true);

    }

    public void Next_stage_button()
    {
        if(world.gameTimer.startTime >= 180)
        {
            Debug.Log("You win");
            return;
        }
        if(world.gameTimer.startTime-world.gameTimer.timer1 < 10)
        {
            Debug.Log("You lose");
            return;
        }
        world.enemies[0].ResetLocaiton();
        world.gameTimer.addtimer1(10);
        world.RandomizeTiles();
        world.ReachtheEnd = false;
        retry_pause = false;
        Retry_Menu.SetActive(false);
        BuildMenu.SetActive(true);
        
        

    }

    public void start_game_button()
    {
        Start_Menu.SetActive(false);
        gamestart = true;
        world.RandomizeTiles();
    }

    public void Option_button()
    {
        Option_Menu.SetActive(true);
        Start_Menu.SetActive(false);   
    }

    public void Option_game_button()
    {
        Option_Menu_in_game.SetActive(true);
        Pause_Menu.SetActive(false);
    }

    public void Pause_button()
    {
        Pause_Menu.SetActive(true);
    }

    public void Resume_button()
    {
        Pause_Menu.SetActive(false);
        paused = false;
        world.paused = false;
    }

    public void Mainmenu_game_button()
    {
        Pause_Menu.SetActive(false);
        Start_Menu.SetActive(true);
        gamestart = false;
    }

    public void back_button()
    {
        Start_Menu.SetActive(true);
        Option_Menu.SetActive(false);
        
    }

    public void back_game_button()
    {
        Option_Menu_in_game.SetActive(false);
        Pause_Menu.SetActive(true);

    }

    public void quit_button()
    {
        Debug.Log("You leave the game");
    }

    public void Disavble_button()
    {
        BuildMenu.SetActive(false);
    }


}
