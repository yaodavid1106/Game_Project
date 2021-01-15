using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class TimerSpriteController : MonoBehaviour
{
    [SerializeField] Slider slider1;
    [SerializeField] Text timerText1;

    World world
    {
        get
        {
            return WorldController.Instance.world;
        }
    }

   void Start()
    {
        
    }


   void Update()
    {
        slider1.value = world.gameTimer.timer1 / world.gameTimer.startTime;
        FormatText1();

    }
           
    private void FormatText1()
    {
        float Time = world.gameTimer.startTime - world.gameTimer.timer1;

        int days = (int)(Time / 86400) % 365;
        int hours = (int)(Time / 3600)%24;
        int minutes = (int)(Time / 60) % 60;
        float seconds = (Time % 60);

        string secondString = seconds.ToString("F2");

        timerText1.text = "";
        if(days > 0) { timerText1.text += days + "d"; }
        if (hours > 0) { timerText1.text += hours + "h"; }
        if (minutes > 0) { timerText1.text += minutes + "m"; }
        timerText1.text += secondString + "s";

    }

}
