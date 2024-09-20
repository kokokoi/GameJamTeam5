using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDeathFadeScript : MonoBehaviour
{
    public float speed = 0.01f; // フェードインする速度
    public float alfa = 0.0f;   // 初期のアルファ値
    float red, green, blue;
    public bool isFading = false; // プレイヤーが死んだかどうかのフラグ

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
            // アルファ値を増加させて徐々に暗転
            alfa += speed * Time.deltaTime;
            alfa = Mathf.Clamp(alfa, 0, 1); // アルファ値を0から1の範囲に制限

            // 画像の色にアルファ値を適用して暗転を実行
            GetComponent<Image>().color = new Color(red, green, blue, alfa);
        }

    }

     // 死亡時にフェードインを開始するためのメソッド
    public void StartFade()
    {
        isFading = true;  // 暗転を開始
    }
}
