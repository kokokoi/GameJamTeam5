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
    [SerializeField] float airControlFactor = 0.0f;//�󒆂ł̑��쐧��

    //���݂̃X�s�[�h��ێ����Ă����{��
    float currentSpeed;
    float DashCoolTime;
    float DashTime;

    bool isDashing = false;
    bool isMovingHorizontally = false;

    //�W�����v�֘A
    [SerializeField] bool isGrounded;//�n�ʂɂ��邩�ǂ���
    Rigidbody2D rb;

    Vector3 targetPosition;

    Animator animator;
    bool isJump;
    bool direction;


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
        DashCoolTime = 0f;
        DashTime = 0f;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
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
            float controlFactor = isGrounded ? 1.0f : airControlFactor;

            if (x != 0)
            {
                currentSpeed += acceleration * Time.deltaTime * controlFactor;
                currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);
            }
            else
            {
                currentSpeed -= deceleration * Time.deltaTime * controlFactor;
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

            // �W�����v�t���O�𗧂Ă�
            isJump = true;
            animator.PlayInFixedTime("Jump", 0);
        }

        DashCoolTime -= Time.deltaTime;

        UpdateAnimation();

        UpdateUI();
    }

    //�n�ʂƂ̐ڐG�𔻒肷��֐��iOnCollisionEnter2D��OnCollisionExit2D���g�p�j
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // �v���C���[���n�ʂɐڐG���Ă���ꍇ
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Slope"))
        {
            isGrounded = true;  // �n�ʂɐڐG������W�����v�\�ɂ���
            rb.gravityScale = collision.gameObject.CompareTag("Ground") ? 10.0f : 0.3f;  // Slope�̏ꍇ�ɂ͈قȂ�d�͂�ݒ�
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // �v���C���[���n�ʂ��痣�ꂽ�ꍇ
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Slope"))
        {
            isGrounded = false;  // �n�ʂ��痣�ꂽ��W�����v�s�ɂ���
        }
    }

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
            else if (horizontal < 0.0f)
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
            if (isJump)
            {
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                if (stateInfo.IsName("Jump"))
                {
                    if (stateInfo.normalizedTime >= 0.7f)
                    {
                        isJump = false;
                        animator.PlayInFixedTime("Fall", 0);
                    }
                }
            }

            return;
        }

        // ���݃_�b�V�����̂��߂����ŏI��
        if (isDashing) return;

        // �ړ��Ȃ�
        if (horizontal == 0.0f)
        {
            // ���݂̃X�e�[�g��Idle�ł͂Ȃ��Ƃ��Ɋe���ڐݒ�
            if (clipName != "Idle")
            {
                // ���݂̃A�j���[�V�������I����Idle�̃A�j���[�V�����𗬂�
                animator.PlayInFixedTime("Idle", 0);
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

    public void Death()
    {
        PlayerReset();
    }
    private void PlayerReset()
    {
        this.transform.position = Vector3.zero;
    }

}
