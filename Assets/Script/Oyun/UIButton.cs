using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButton : MonoBehaviour
{
    public Image image;
    public Image shadow;

    public void ButtonDown()
    {
        shadow.color = new Color(1.0f, 1.0f, 214.0f / 255.0f, 0.5f);
        image.color = new Color(1.0f, 1.0f, 214.0f / 255.0f, 0.2f);
    }

    public void ButtonRelease()
    {
        shadow.color = new Color(1.0f, 1.0f, 214.0f / 255.0f , 1.0f);
        image.color = new Color(1.0f, 1.0f, 214.0f /255.0f, 0.5f);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
