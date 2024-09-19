using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySE : MonoBehaviour
{
    [SerializeField]
    SoundManager soundManager;
    [SerializeField]
    AudioClip clip;


    bool isPlaying = false;


    // Update is called once per frame
    void Update()
    {
        if (isPlaying)
        {
            return;
        }

        if(Input.GetMouseButtonDown(0))
        {
            isPlaying = true;

            soundManager.PlaySe(clip);

            isPlaying = false;
        }
    }
}
