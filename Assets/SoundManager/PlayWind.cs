using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayWind : MonoBehaviour
{
    [SerializeField]
    SoundManager soundManager;
    [SerializeField]
    AudioClip clip;

    void Start()
    {
        soundManager.StopBgm();
        soundManager.BgmVolume = 1.5f;
        soundManager.PlayBgm(clip);
    }
}
