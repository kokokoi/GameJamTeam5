using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButton : MonoBehaviour
{
    [SerializeField] private float showButtonSpeed;
    [SerializeField] private float showButtonTimer;

    public Image image;
    public Image shadow;

    public float ImageAlpha = 0.0f;
    public float ShadowAlpha = 0.0f;

    public void ButtonDown()
    {
        if (ImageAlpha <= 1.0f)
        {
            shadow.color = new Color(1.0f, 1.0f, 214.0f / 255.0f, 0.5f);
            image.color = new Color(1.0f, 1.0f, 214.0f / 255.0f, 0.2f);
        }
    }

    public void ButtonRelease()
    {
        if (ImageAlpha <= 1.0f)
        {
            shadow.color = new Color(1.0f, 1.0f, 214.0f / 255.0f, 1.0f);
            image.color = new Color(1.0f, 1.0f, 214.0f / 255.0f, 0.5f);
        }
    }

    public void ShowButton()
    {
        ShadowAlpha += Time.deltaTime * showButtonSpeed;
        ImageAlpha += Time.deltaTime * showButtonSpeed;
        if (ImageAlpha >= 1.0f)
            ImageAlpha = 1.0f;
        if (ShadowAlpha >= 0.5f)
            ShadowAlpha = 0.5f;
    }

    // Start is called before the first frame update
    void Start()
    {
        shadow.color = new Color(1.0f, 1.0f, 214.0f / 255.0f, ShadowAlpha);
        image.color = new Color(1.0f, 1.0f, 214.0f / 255.0f, ImageAlpha);

    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= showButtonTimer && ImageAlpha < 1.0f)
        {
                ShowButton();
        }

        shadow.color = new Color(1.0f, 1.0f, 214.0f / 255.0f, ShadowAlpha);
        image.color = new Color(1.0f, 1.0f, 214.0f / 255.0f, ImageAlpha);

    }
}
