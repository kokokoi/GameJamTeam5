using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Path;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    //�ʏ�X�s�[�h�A�_�b�V���X�s�[�h�̕ϐ��錾
    [SerializeField] float speed, dashSpeed;
    [SerializeField] float dashCoolTime;
    [SerializeField] float jumpPower;

    private Animator animator;        // �A�j���[�^�[
    private bool direction; // �i�s����.�E:true,��:false

    bool isMove_;
    bool isDash_;
    bool isJump_;


    //���݂̃X�s�[�h��ێ����Ă����{��
    float currentSpeed;
    float DashCoolTime;
    float Yspeed;

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
        rb = GetComponent<Rigidbody2D>();

        // Animator�擾
        animator = GetComponent<Animator>();
        direction = true;
    }

    // Update is called once per frame
    void Update()
    {
        // ���������i�������j�̓��͎󂯎��
        float x = Input.GetAxisRaw("Horizontal");        

        // �_�b�V������
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if(Input.GetKey(KeyCode.UpArrow)||Input.GetKey(KeyCode.W))
            {
                Yspeed = dashSpeed;
            }
            // �ʏ�X�s�[�h�������Ȃ�
            if (DashCoolTime <= 0)
            {
                currentSpeed = dashSpeed;

                // �_�b�V���̃t���O�𗧂Ă�(�A�j���[�V�������؂�ւ���)
                isDash_ = true;
                animator.PlayInFixedTime("Dash", 0);
            }
            DashCoolTime = dashCoolTime;
        }

        // �_�b�V���̃N�[���^�C������
        if (currentSpeed > speed)
        {
            currentSpeed -= 1.0f;
        }
        DashCoolTime -= 1.0f;

        // �ړ�����
        transform.Translate(new Vector3(x, Yspeed, 0) * currentSpeed * Time.deltaTime);

        // �X�y�[�X�L�[�������ăW�����v���鏈��
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            // Rigidbody�ɏ�����̗͂������ăW�����v����
            rb.AddForce(new Vector2(0, jumpPower), ForceMode2D.Impulse);
            isGrounded = false; // �W�����v���͒n�ʂɂ��Ȃ��Ɣ���

            // ���݃_�b�V�����ł͂Ȃ�
            if (isDash_ == false)
            {
                // �W�����v�̃t���O�𗧂Ă�(�A�j���[�V�������؂�ւ���)
                isJump_ = true;
                animator.PlayInFixedTime("Jump", 0);
            }
        }

        // Animator�X�V
        UpdateAnimation();

        UpdateUI();
    }

    //�n�ʂƂ̐ڐG�𔻒肷��֐��i��Ƃ���OnCollisionEnter2D���g�p�j
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // �v���C���[���n�ʂɐڐG���Ă���ꍇ
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;  // �n�ʂɖ߂�����ĂуW�����v�\�ɂ���
        }
    }

    // �A�j���[�V�����X�V
    void UpdateAnimation()
    {
        // ���݂̃X�e�[�g�����擾
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        string clipName = clipInfo[0].clip.name;

        float horizontal = Input.GetAxisRaw("Horizontal");
        // ----- �i�s�����ɍ��킹�ĉ摜�𔽓]���� -----
        {
            if (horizontal > 0.0f)
            {
                Vector3 scale = transform.localScale;
                scale.x = scale.x > 0.0f ? scale.x : scale.x * -1;
                transform.localScale = scale;
            }
            else if(horizontal < 0.0f)
            {
                Vector3 scale = transform.localScale;
                scale.x = scale.x < 0.0f ? scale.x : scale.x * -1;
                transform.localScale = scale;
            }
        }

        // �󒆂ɂ���
        if (isGrounded == false)
        {
            // ����JumpState
            if (isJump_)
            {
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                if (stateInfo.IsName("Jump"))
                {
                    if (stateInfo.normalizedTime >= 0.7f)
                    {
                        isJump_ = false;
                        animator.PlayInFixedTime("Fall", 0);
                    }
                }
            }

            return;
        }


        // �_�b�V���̑��x���Ȃ��Ȃ����烂�[�V�������I������
        if(currentSpeed <= speed && isDash_ == true)
        {
            isDash_ = false;
        }

        // ���݃_�b�V�����̂��߂����ŏI��
        if (isDash_) return;

        // �ړ��Ȃ�
        if (horizontal == 0.0f)
        {
            // ���݂̃X�e�[�g��Idle�ł͂Ȃ��Ƃ��Ɋe���ڐݒ�
            if (clipName != "Idle")
            {
                // ���݂̃A�j���[�V�������I����Idle�̃A�j���[�V�����𗬂�
                animator.PlayInFixedTime("Idle", 0);

                // �ړ��t���O��������
                isMove_ = false;
            }
        }
        // �E�����Ɉړ�
        else if (horizontal > 0.0f)
        {
            // ���݂̐i�s�������E�łȂ��Ƃ��A
            // RunState�ł͂Ȃ��Ƃ��Ɋe���ڂ�ݒ肷��
            if (direction != true || clipName != "Run")
            {
                // ���݂̃A�j���[�V�������I����Run�̃A�j���[�V�����𗬂�
                animator.PlayInFixedTime("Run", 0);

                // �i�s�������E�ɐݒ肷��
                direction = true;

                // �ړ��t���O�𗧂Ă�
                isMove_ = true;
            }
        }
        // �������Ɉړ�
        else
        {
            // ���݂̐i�s���������łȂ��Ƃ��A
            // RunState�ł͂Ȃ��Ƃ��Ɋe���ڂ�ݒ肷��
            if (direction != false || clipName != "Run")
            {
                // ���݂̃A�j���[�V�������I����Run�̃A�j���[�V�����𗬂�
                animator.PlayInFixedTime("Run", 0);

                // �i�s���������ɐݒ肷��
                direction = false;

                // �ړ��t���O�𗧂Ă�
                isMove_ = true;
            }
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
