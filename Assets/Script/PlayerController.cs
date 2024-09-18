using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //�ʏ�X�s�[�h�A�_�b�V���X�s�[�h�̕ϐ��錾
    [SerializeField] float speed, dashSpeed;
    //���݂̃X�s�[�h��ێ����Ă����{��
    float currentSpeed;

    // Start is called before the first frame update
    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        //���݂̃X�s�[�h���X�s�[�h�ɌŒ�
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
