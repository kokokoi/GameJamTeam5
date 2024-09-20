using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDeathFadeScript : MonoBehaviour
{
    public float speed = 0.01f; // �t�F�[�h�C�����鑬�x
    public float alfa = 0.0f;   // �����̃A���t�@�l
    float red, green, blue;
    public bool isFading = false; // �v���C���[�����񂾂��ǂ����̃t���O

    // Start is called before the first frame update
    void Start()
    {
        red = GetComponent<Image>().color.r;
        green = GetComponent<Image>().color.g;
        blue = GetComponent<Image>().color.b;
    }

    // Update is called once per frame
    void Update()
    {
        if (isFading)
        {
            // �A���t�@�l�𑝉������ď��X�ɈÓ]
            alfa += speed * Time.deltaTime;
            alfa = Mathf.Clamp(alfa, 0, 1); // �A���t�@�l��0����1�͈̔͂ɐ���

            // �摜�̐F�ɃA���t�@�l��K�p���ĈÓ]�����s
            GetComponent<Image>().color = new Color(red, green, blue, alfa);
        }

    }

     // ���S���Ƀt�F�[�h�C�����J�n���邽�߂̃��\�b�h
    public void StartFade()
    {
        isFading = true;  // �Ó]���J�n
    }
}
