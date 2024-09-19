using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultTimer : MonoBehaviour
{
    public TMP_Text timeText;
    float minutes = 0;
    float seconds = 0;
   

    // Start is called before the first frame update
    void Start()
    {
        minutes = UITimer.GetMinutes();
        seconds = UITimer.GetSeconds();

        timeText.text = string.Format("{0:00} : {1:00}", minutes,seconds);
    }

    
}
