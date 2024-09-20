using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Video;
using DG.Tweening;

public class ChangeScene : MonoBehaviour
{
    [SerializeField]
    SoundManager soundManager;
    [SerializeField]
    AudioClip clip;
    public Image image;
    public Image shadow;
    //public Sprite titleSprite;
    private float buttonReleaseTimer = 1;

    [SerializeField] VideoPlayer videoPlayer;
    [SerializeField] GameObject video;

    private void Start()
    {
        videoPlayer.Pause();
    }

    public void OnClickStartButton()
   {

        ButtonDown();

        soundManager.PlaySe(clip);

        video.SetActive(true);
        //videoPlayer.loopPointReached += LoopPointReached;
        videoPlayer.Play();
        DOVirtual.DelayedCall(1.0f, () => {
            LoadScene();
        });
    }

    public void LoadScene()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void ButtonDown()
    {
        shadow.color = new Color(1.0f, 1.0f, 214.0f / 255.0f, 0.5f);
        image.color = new Color(1.0f, 1.0f, 214.0f / 255.0f, 0.2f);
        buttonReleaseTimer -= Time.deltaTime;
        if (buttonReleaseTimer <= 0)
        {
            buttonReleaseTimer = 1;
            ButtonRelease();
        }
    }

    public void ButtonRelease()
    {
        shadow.color = new Color(1.0f, 1.0f, 214.0f / 255.0f, 1.0f);
        image.color = new Color(1.0f, 1.0f, 214.0f / 255.0f, 0.5f);
    }

    // カーソルがボタンに重なったときの処理
    public void OnPointerEnter()
    {
        shadow.color = new Color(1.0f, 1.0f, 214.0f / 255.0f, 0.7f);
        image.color = new Color(1.0f, 1.0f, 214.0f / 255.0f, 0.5f);

    }

    // カーソルがボタンから離れたときの処理
    public void OnPointerExit()
    {
        shadow.color = new Color(1.0f, 1.0f, 214.0f / 255.0f, 1.0f);
        image.color = new Color(1.0f, 1.0f, 214.0f / 255.0f, 0.5f);

    }
}
