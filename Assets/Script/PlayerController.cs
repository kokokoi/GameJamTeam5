using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //通常スピード、ダッシュスピードの変数宣言
    [SerializeField] float speed, dashSpeed;
    //現在のスピードを保持しておく本数
    float currentSpeed;

    // Start is called before the first frame update
    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        //現在のスピードをスピードに固定
        currentSpeed = speed;
     }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float jump = Input.GetAxis("Jump");


        Vector2 position = transform.position;
        position.x = position.x + 3.0f * horizontal*Time.deltaTime;
        position.y = position.y + 3.0f * vertical * Time.deltaTime;
        
        position.y = position.y + 8.0f * jump * Time.deltaTime;



        transform.position = position;

    }

}
