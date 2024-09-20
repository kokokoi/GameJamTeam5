using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    //通常スピード、ダッシュスピードの変数宣言
    [SerializeField] float speed, dashSpeed;
    [SerializeField] float dashCoolTime;
    [SerializeField] float jumpPower;
    [SerializeField] float dashTime;
    [SerializeField] float acceleration;//加速度
    [SerializeField] float deceleration;//減速度
    [SerializeField] float maxSpeed;//最高速度
    [SerializeField] float airControlFactor = 0.0f;//空中での操作制限
    [SerializeField] float deathTimer;
    [SerializeField] float clearTimer;
    [SerializeField] SoundManager soundManager;
    [SerializeField] AudioClip clip_jump;
    [SerializeField] AudioClip clip_dash;
    [SerializeField] AudioClip clip_move;

    //現在のスピードを保持しておく本数
    float currentSpeed;
    float DashCoolTime;
    float DashTime;

    //死んでからフラグとタイマー
    public bool isDeath;
    float currentDeathTimer;

    public bool isClear;
    float currentClearTimer;

    public bool isDashing = false;
    bool isMovingHorizontally = false;

    bool hasShaken = false; // カメラを振動させたかどうかのフラグ

    public Shaker shaker;

   // public PlayerDeathFadeScript fadeScript;

    //ジャンプ関連
    [SerializeField] bool isGrounded;//地面にいるかどうか
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
        //現在のスピードをスピードに固定
        currentSpeed = speed;

        //RigidBody2dコンポーネントを取得
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

            // カメラがまだ振動していないなら、一回だけ振動させる
            if (!hasShaken)
            {
                animator.PlayInFixedTime("Death", 0);
               
                shaker.ShakeCamera();
              //  fadeScript.StartFade();
                hasShaken = true; // 振動させたのでフラグを立てる
            }

            //プレイヤーの速度を０にする
            rb.velocity = Vector2.zero;

            currentDeathTimer -= Time.deltaTime;
            if (currentDeathTimer <= 0)
            {
                PlayerReset();
                isDeath = false;
                hasShaken = false; // リセット時に振動フラグもリセットする
            }
        }
        else if(isClear)
        {
            //プレイヤーの速度を０にする
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

        // LSHIFT を押しているときだけ上方向の入力を許可
        float y =  0;


        //ダッシュ中をfalseに
        DashTime -= Time.deltaTime;
        if (DashTime <= 0)
        {
            isDashing = false;
            hasShaken = false; // リセット時に振動フラグもリセットする
        }


        // ダッシュ可能でかつ LSHIFT が押された時のみダッシュ処理を行う
        if (DashCoolTime <= 0 && !isDashing && Input.GetKey(KeyCode.LeftShift)&& (Input.GetKey(KeyCode.A)|| Input.GetKey(KeyCode.D)))
        {
            isDashing = true;
            DashTime = dashTime;
            DashCoolTime = dashCoolTime;

            animator.PlayInFixedTime("Dash", 0);

            // 残像を使用
            afterimage.UseAfterImage();

            // 横または縦方向の入力に応じてダッシュ方向を決定
            Vector3 dashDirection = new Vector3(x, y, 0).normalized;

            // ダッシュ前に現在の速度をリセット
            rb.velocity = new Vector2(0, rb.velocity.y);  // 横方向の速度をリセットしても垂直方向は維持

            // Rigidbody2D の AddForce を使ってダッシュする
            rb.AddForce(dashDirection * dashSpeed, ForceMode2D.Impulse);
      
        
        }
        // ダッシュしていない場合は通常の移動処理
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

            // 通常の移動（イージングなし）
            transform.Translate(new Vector3(x, y, 0) * currentSpeed * Time.deltaTime);
        }

        // スペースキーでジャンプ処理
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);  // ジャンプ前に垂直方向の速度をリセット
            rb.AddForce(new Vector2(0, jumpPower), ForceMode2D.Impulse);
            isGrounded = false;

            // ジャンプフラグを立てる
            isJump = true;
            animator.PlayInFixedTime("Jump", 0);

            soundManager.PlaySe(clip_jump);

        }

        DashCoolTime -= Time.deltaTime;

    }


    //地面との接触を判定する関数（OnCollisionEnter2DとOnCollisionExit2Dを使用）
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // プレイヤーが地面に接触している場合
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Slope"))
        {
            isGrounded = true;  // 地面に接触したらジャンプ可能にする
            rb.gravityScale = collision.gameObject.CompareTag("Ground") ? 10.0f : 0.3f;  // Slopeの場合には異なる重力を設定
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // プレイヤーが地面から離れた場合
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Slope"))
        {
            isGrounded = false;  // 地面から離れたらジャンプ不可にする
        }
    }



    void UpdateAnimation()
    {
        // 現在のステート名を取得
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        string clipName = clipInfo[0].clip.name;

        float horizontal = Input.GetAxisRaw("Horizontal");
        // ----- 進行方向に合わせて画像を反転する -----
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

        // 空中にいる
        if (isGrounded == false)
        {
            // 現在JumpState
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

        // 現在ダッシュ中のためここで終了
        if (isDashing) return;

        // 移動なし
        if (horizontal == 0.0f)
        {
            // 現在のステートがIdleではないときに各項目設定
            if (clipName != "Idle")
            {
                // 現在のアニメーションを終了しIdleのアニメーションを流す
                animator.PlayInFixedTime("Idle", 0);
            }
        }
        // 右方向に移動
        else if (horizontal > 0.0f)
        {
            // 現在の進行方向が右でないとき、
            // RunStateではないときに各項目を設定する
            if (direction != true || clipName != "Run")
            {
                // 現在のアニメーションを終了しRunのアニメーションを流す
                animator.PlayInFixedTime("Run", 0);

                // 進行方向を右に設定する
                direction = true;
            }
        }
        // 左方向に移動
        else
        {
            // 現在の進行方向が左でないとき、
            // RunStateではないときに各項目を設定する
            if (direction != false || clipName != "Run")
            {
                // 現在のアニメーションを終了しRunのアニメーションを流す
                animator.PlayInFixedTime("Run", 0);

                // 進行方向を左に設定する
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
            //デス時のタイマーリセット
            currentDeathTimer = deathTimer;
        }
    }
    private void PlayerReset()
    {
        this.transform.position = Vector3.zero;
    }

}
