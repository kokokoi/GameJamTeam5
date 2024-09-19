using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //�ʏ�X�s�[�h�A�_�b�V���X�s�[�h�̕ϐ��錾
    [SerializeField] float speed, dashSpeed;
    [SerializeField] float dashCoolTime;
    [SerializeField] float jumpPower;
    [SerializeField] float Yspeed;
    [SerializeField] float dashTime;
    [SerializeField] float acceleration;//�����x
    [SerializeField] float deceleration;//�����x
    [SerializeField] float maxSpeed;//�ō����x

    //���݂̃X�s�[�h��ێ����Ă����{��
    float currentSpeed;
    float DashCoolTime;
    float DashTime;

    bool isDashing = false;
    bool isMovingHorizontally=false;

    //�W�����v�֘A
    [SerializeField] bool isGrounded = true;//�n�ʂɂ��邩�ǂ���
    Rigidbody2D rb;

    Vector3 targetPosition;

    public enum Button
    {
        Right = 0, Left, Up, Down, Shift, Space, Max
    }

    public UIButton[] uiButton = new UIButton[(int)Button.Max];

    // Start is called before the first frame update
    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        //���݂̃X�s�[�h���X�s�[�h�ɌŒ�
        currentSpeed = speed;

        //RigidBody2d�R���|�[�l���g���擾
        rb=GetComponent<Rigidbody2D>();
        DashCoolTime = 0f;
        DashTime = 0f;
     }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = 0;

        if (DashCoolTime <= 0 && !isDashing && Input.GetKey(KeyCode.LeftShift))
        {
            isDashing = true;
            DashTime = dashTime;
            DashCoolTime = dashCoolTime;

            // �ڕW�ʒu���v�Z���ă_�b�V���̂��߂Ɉړ����J�n
            targetPosition = transform.position + new Vector3(x, y, 0) * dashSpeed * dashTime;

            // �C�[�W���O���g���Ĉړ�
            transform.DOMove(targetPosition, dashTime)
                .SetEase(Ease.OutQuad) // Ease�ݒ�i���R�ɕύX�\�j
                .OnComplete(() =>
                {
                    isDashing = false; // �_�b�V���I��
                    currentSpeed = speed; // �ʏ�X�s�[�h�ɖ߂�
                });
        }

        // �_�b�V�����Ă��Ȃ��ꍇ�͒ʏ�̈ړ�����
        if (!isDashing)
        {
            if (x != 0)
            {
                currentSpeed += acceleration * Time.deltaTime;
                currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);
            }
            else
            {
                currentSpeed -= deceleration * Time.deltaTime;
                currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);
            }

            // �ʏ�̈ړ��i�C�[�W���O�Ȃ��j
            transform.Translate(new Vector3(x, y, 0) * currentSpeed * Time.deltaTime);
        }

        // �X�y�[�X�L�[�ŃW�����v����
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(new Vector2(0, jumpPower), ForceMode2D.Impulse);
            isGrounded = false;
        }

        DashCoolTime -= Time.deltaTime;

        UpdateUI();
    }

    //�n�ʂƂ̐ڐG�𔻒肷��֐��i��Ƃ���OnCollisionEnter2D���g�p�j
    private void OnCollisionStay2D(Collision2D collision)
    {
        Debug.Log(isGrounded);


        // �v���C���[���n�ʂɐڐG���Ă���ꍇ
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;  // �n�ʂɖ߂�����ĂуW�����v�\�ɂ���
            Yspeed = 0.0f;
            rb.gravityScale = 10.0f;
            rb.AddForce(collision.contacts[0].normal * -1 * 20);
        }
        // �v���C���[���n�ʂɐڐG���Ă���ꍇ
        if (collision.gameObject.CompareTag("Slope"))
        {
            isGrounded = true;  // �n�ʂɖ߂�����ĂуW�����v�\�ɂ���
            Yspeed = 0.0f;
            rb.gravityScale = 0.5f;
        }
    }


    void UpdateUI()
    {
        ApplyDownArrow();
        ApplyUpArrow();
        ApplyRightArrow();
        ApplyLeftArrow();
        ApplyShiftKey();
        ApplySpaceKey();


    }

    void ApplyDownArrow()
    {
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            uiButton[(int)Button.Down].ButtonDown();
        else
            uiButton[(int)Button.Down].ButtonRelease();
    }
    void ApplyUpArrow()
    {
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            uiButton[(int)Button.Up].ButtonDown();
        else
            uiButton[(int)Button.Up].ButtonRelease();
    }
    void ApplyRightArrow()
    {
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            uiButton[(int)Button.Right].ButtonDown();
        else
            uiButton[(int)Button.Right].ButtonRelease();
    }
    void ApplyLeftArrow()
    {
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            uiButton[(int)Button.Left].ButtonDown();
        else
            uiButton[(int)Button.Left].ButtonRelease();
    }
    void ApplyShiftKey()
    {
        if (Input.GetKey(KeyCode.LeftShift))
            uiButton[(int)Button.Shift].ButtonDown();
        else
            uiButton[(int)Button.Shift].ButtonRelease();
    }
    void ApplySpaceKey()
    {
        if (Input.GetKey(KeyCode.Space))
            uiButton[(int)Button.Space].ButtonDown();
        else
            uiButton[(int)Button.Space].ButtonRelease();
    }

    void Restart()
    {
        this.transform.position = Vector3.zero;
    }

}
