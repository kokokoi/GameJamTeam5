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


    //���݂̃X�s�[�h��ێ����Ă����{��
    float currentSpeed;
    float DashCoolTime;

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


        if (DashCoolTime <= 0)
        {
            // �_�b�V������
            if (Input.GetKey(KeyCode.LeftShift))
            {
                // �ʏ�X�s�[�h�������Ȃ�
                currentSpeed = dashSpeed;
                DashCoolTime = dashCoolTime;

                // Y�����̃X�s�[�h���_�b�V���X�s�[�h�ɂ���
                if (y != 0)
                {
                   y= dashSpeed * Yspeed;  // �㉺�����ɂ��_�b�V��
                }
            }
        }

        // �_�b�V���̃N�[���^�C������
        if (currentSpeed > speed)
        {
            currentSpeed -= 1.0f;
        }
        DashCoolTime -= 1.0f;

        // �ړ�����
        transform.Translate(new Vector3(x, y, 0) * currentSpeed * Time.deltaTime);

        // �X�y�[�X�L�[�������ăW�����v���鏈��
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            // Rigidbody�ɏ�����̗͂������ăW�����v����
            rb.AddForce(new Vector2(0, jumpPower), ForceMode2D.Impulse);
            isGrounded = false; // �W�����v���͒n�ʂɂ��Ȃ��Ɣ���
        }

        UpdateUI();
    }

    //�n�ʂƂ̐ڐG�𔻒肷��֐��i��Ƃ���OnCollisionEnter2D���g�p�j
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // �v���C���[���n�ʂɐڐG���Ă���ꍇ
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;  // �n�ʂɖ߂�����ĂуW�����v�\�ɂ���
            Yspeed = 0.0f;
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
