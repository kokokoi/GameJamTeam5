using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UITimer : MonoBehaviour
{
    public float timerRemaining = 0;
    public bool timeIsRunning = true;
    public TMP_Text timeText;

    public static float minutes = 0;
    public static float seconds = 0;

    // Start is called before the first frame update
    void Start()
    {
        timeIsRunning = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (timeIsRunning)
        {
            if(timerRemaining >= 0)
            {
                timerRemaining += Time.deltaTime;
                DisplayTime(timerRemaining);
            }
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;
        minutes = Mathf.FloorToInt(timeToDisplay / 60);
        seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timeText.text = string.Format("{0:00} : {1:00}", minutes, seconds);
    }

    public static float GetMinutes()
    {
        return minutes;
    }

    public static float GetSeconds()
    {
        return seconds;
    }
}
