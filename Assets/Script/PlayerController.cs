using System;
using System.Collections;
using System.Collections.Generic;
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
        // ���������i�������j�̓��͎󂯎��

        float x = Input.GetAxisRaw("Horizontal");
        float y = 0;


        if (Input.GetKey(KeyCode.LeftShift))
        {
            // �㉺�����̓��͂��󂯎��
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            {
                y = 1; // �����
            }

            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            {
                y = -1; // ������
            }
        }


        if(DashCoolTime<=0&&!isDashing)
        {
            if(Input.GetKey(KeyCode.LeftShift))
            {
                isDashing = true;
                DashTime = dashTime;
                DashCoolTime = dashCoolTime;
                currentSpeed = dashSpeed;
            }
        }

        // �_�b�V�����̏���
        if (isDashing)
        {
            DashTime -= Time.deltaTime;  // �_�b�V�����Ԃ����炷

            // �_�b�V�����I��������ʏ푬�x�ɖ߂�
            if (DashTime <= 0)
            {
                isDashing = false;
            }
        }

        // �_�b�V�����Ă��Ȃ��Ƃ��ɉ����x���g�p�����ʏ�ړ�����
        if (!isDashing)
        {
            if (x != 0) // �v���C���[�����E�ɓ����Ă���Ƃ�
            {
                // �����x�Ɋ�Â��ăX�s�[�h�𑝉�������
                currentSpeed += acceleration * Time.deltaTime;
                currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);  // �ō����x�ɐ���
            }
            else
            {
                // �v���C���[�������Ă��Ȃ��Ƃ��͌���������
                currentSpeed -= deceleration * Time.deltaTime;
                currentSpeed = Mathf.Max(currentSpeed, 0);  // �Œᑬ�x��0�ɐ���
            }
        }

        // �L�[���͂Ɋւ�炸�A�_�b�V�����̓_�b�V���X�s�[�h�ňړ��𑱂���
        transform.Translate(new Vector3(x, y, 0) * currentSpeed * Time.deltaTime);

        // �X�y�[�X�L�[�������ăW�����v���鏈��
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(new Vector2(0, jumpPower), ForceMode2D.Impulse);
            isGrounded = false;
        }

        // �N�[���^�C�������炷
        DashCoolTime -= Time.deltaTime;



        // �X�y�[�X�L�[�������ăW�����v���鏈��
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            // Rigidbody�ɏ�����̗͂������ăW�����v����
            rb.AddForce(new Vector2(0, jumpPower), ForceMode2D.Impulse);
            isGrounded = false; // �W�����v���͒n�ʂɂ��Ȃ��Ɣ���
        }

        if (!isGrounded)
        {
            rb.gravityScale = 10.0f;
        }

        isGrounded = false;

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

}
