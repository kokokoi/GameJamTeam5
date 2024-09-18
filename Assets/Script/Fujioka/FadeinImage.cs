using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FadeinImage : MonoBehaviour
{
    [SerializeField] private Image fadeinImage;
    [SerializeField] private float durationSeconds;

    private void Awake()
    {
        this.fadeinImage.color = this.fadeinImage.color.ToOpaque();
    }

    private void Start()
    {
        this.fadeinImage.DOFade(0.0f, this.durationSeconds);
    }
}
