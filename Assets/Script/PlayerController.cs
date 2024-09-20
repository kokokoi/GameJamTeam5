using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    //�ʏ�X�s�[�h�A�_�b�V���X�s�[�h�̕ϐ��錾
    [SerializeField] float speed, dashSpeed;
    [SerializeField] float dashCoolTime;
    [SerializeField] float jumpPower;
    [SerializeField] float dashTime;
    [SerializeField] float acceleration;//�����x
    [SerializeField] float deceleration;//�����x
    [SerializeField] float maxSpeed;//�ō����x
    [SerializeField] float airControlFactor = 0.0f;//�󒆂ł̑��쐧��
    [SerializeField] float deathTimer;
    [SerializeField] float clearTimer;
    [SerializeField] SoundManager soundManager;
    [SerializeField] AudioClip clip_jump;
    [SerializeField] AudioClip clip_dash;
    [SerializeField] AudioClip clip_move;

    //���݂̃X�s�[�h��ێ����Ă����{��
    float currentSpeed;
    float DashCoolTime;
    float DashTime;

    //����ł���t���O�ƃ^�C�}�[
    public bool isDeath;
    float currentDeathTimer;

    public bool isClear;
    float currentClearTimer;

    public bool isDashing = false;
    bool isMovingHorizontally = false;

    bool hasShaken = false; // �J������U�����������ǂ����̃t���O

    public Shaker shaker;

   // public PlayerDeathFadeScript fadeScript;

    //�W�����v�֘A
    [SerializeField] bool isGrounded;//�n�ʂɂ��邩�ǂ���
    Rigidbody2D rb;

    Vector3 targetPosition;

    Animator animator;
    bool isJump;
    bool direction;

    public Afterimage afterimage;


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
        isDeath = false;
        isClear = false;
        currentDeathTimer = deathTimer;
        currentClearTimer = clearTimer;
    }

    // Update is called once per frame
    void Update()
    {
        afterimage.UpdateTransform(transform.position, transform.localScale);

        if (Input.GetKeyDown(KeyCode.A)|| Input.GetKeyDown(KeyCode.D))
        {
            soundManager.PlaySe(clip_move);
        } 
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            soundManager.PlaySe(clip_dash);
        }

        if (isDeath)
        {

            // �J�������܂��U�����Ă��Ȃ��Ȃ�A��񂾂��U��������
            if (!hasShaken)
            {
                animator.PlayInFixedTime("Death", 0);
               
                shaker.ShakeCamera();
              //  fadeScript.StartFade();
                hasShaken = true; // �U���������̂Ńt���O�𗧂Ă�
            }

            //�v���C���[�̑��x���O�ɂ���
            rb.velocity = Vector2.zero;

            currentDeathTimer -= Time.deltaTime;
            if (currentDeathTimer <= 0)
            {
                PlayerReset();
                isDeath = false;
                hasShaken = false; // ���Z�b�g���ɐU���t���O�����Z�b�g����
            }
        }
        else if(isClear)
        {
            //�v���C���[�̑��x���O�ɂ���
            rb.velocity = Vector2.zero;
            animator.PlayInFixedTime("Attack", 0);


            currentClearTimer -= Time.deltaTime;
            if (currentClearTimer <= 0)
            {
                SceneManager.LoadScene("ResultScene");
                isClear = false;
            }
        }
        else
        {
            PlayerUpdate();
            UpdateAnimation();
            UpdateUI();    
        }
    }


    void PlayerUpdate()
    {
        float x = Input.GetAxis("Horizontal");

        // LSHIFT �������Ă���Ƃ�����������̓��͂�����
        float y =  0;


        //�_�b�V������false��
        DashTime -= Time.deltaTime;
        if (DashTime <= 0)
        {
            isDashing = false;
            hasShaken = false; // ���Z�b�g���ɐU���t���O�����Z�b�g����
        }


        // �_�b�V���\�ł��� LSHIFT �������ꂽ���̂݃_�b�V���������s��
        if (DashCoolTime <= 0 && !isDashing && Input.GetKey(KeyCode.LeftShift)&& (Input.GetKey(KeyCode.A)|| Input.GetKey(KeyCode.D)))
        {
            isDashing = true;
            DashTime = dashTime;
            DashCoolTime = dashCoolTime;

            animator.PlayInFixedTime("Dash", 0);

            // �c�����g�p
            afterimage.UseAfterImage();

            // ���܂��͏c�����̓��͂ɉ����ă_�b�V������������
            Vector3 dashDirection = new Vector3(x, y, 0).normalized;

            // �_�b�V���O�Ɍ��݂̑��x�����Z�b�g
            rb.velocity = new Vector2(0, rb.velocity.y);  // �������̑��x�����Z�b�g���Ă����������͈ێ�

            // Rigidbody2D �� AddForce ���g���ă_�b�V������
            rb.AddForce(dashDirection * dashSpeed, ForceMode2D.Impulse);
      
        
        }
        // �_�b�V�����Ă��Ȃ��ꍇ�͒ʏ�̈ړ�����
        if (!isDashing)
        {

            float controlFactor = isGrounded ? 10.0f : airControlFactor;

            if (x != 0 || y != 0) 
            {
                currentSpeed += acceleration * Time.deltaTime * controlFactor;
                currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);
                deceleration = 10.0f;
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
            rb.velocity = new Vector2(rb.velocity.x, 0);  // �W�����v�O�ɐ��������̑��x�����Z�b�g
            rb.AddForce(new Vector2(0, jumpPower), ForceMode2D.Impulse);
            isGrounded = false;

            // �W�����v�t���O�𗧂Ă�
            isJump = true;
            animator.PlayInFixedTime("Jump", 0);

            soundManager.PlaySe(clip_jump);

        }

        DashCoolTime -= Time.deltaTime;

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

    public void Clear(bool clear)
    {
        isClear = clear;
        if(isClear)
        {
            currentClearTimer = clearTimer;
        }
    }



    public void Death(bool death)
    {
        isDeath = death;
       
        if (isDeath)
        {
            //�f�X���̃^�C�}�[���Z�b�g
            currentDeathTimer = deathTimer;
        }
    }
    private void PlayerReset()
    {
        this.transform.position = Vector3.zero;
    }

}
