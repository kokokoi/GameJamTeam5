using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Path;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    //通常スピード、ダッシュスピードの変数宣言
    [SerializeField] float speed, dashSpeed;
    [SerializeField] float dashCoolTime;
    [SerializeField] float jumpPower;
    [SerializeField] float Yspeed;
    [SerializeField] float dashTime;
    [SerializeField] float acceleration;//加速度
    [SerializeField] float deceleration;//減速度
    [SerializeField] float maxSpeed;//最高速度

    private Animator animator;        // アニメーター
    private bool direction; // 進行方向.右:true,左:false

    bool isMove_;
    bool isDash_;
    bool isJump_;


    //現在のスピードを保持しておく本数
    float currentSpeed;
    float DashCoolTime;
    float DashTime;

    bool isDashing = false;
    bool isMovingHorizontally=false;

    //ジャンプ関連
    [SerializeField] bool isGrounded = true;//地面にいるかどうか
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
        //現在のスピードをスピードに固定
        currentSpeed = speed;


        // Animator取得
        animator = GetComponent<Animator>();
        direction = true;
   
        //RigidBody2dコンポーネントを取得
        rb=GetComponent<Rigidbody2D>();
        DashCoolTime = 0f;
        DashTime = 0f;
     }

    // Update is called once per frame
    void Update()
    {
        // 水平方向（横向き）の入力受け取り
        float x = Input.GetAxisRaw("Horizontal");        
        float y = 0;


        if (Input.GetKey(KeyCode.LeftShift))
        {
            // 上下方向の入力を受け取る
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            {
                y = 1; // 上方向
            }

            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            {
                y = -1; // 下方向
            }
        }


        if (DashCoolTime <= 0 && !isDashing) 
        {
            if(Input.GetKey(KeyCode.LeftShift))
            {
                isDashing = true;
                DashTime = dashTime;
                DashCoolTime = dashCoolTime;
                currentSpeed = dashSpeed;

                // ダッシュのフラグを立てる(アニメーションも切り替える)
                isDash_ = true;
                animator.PlayInFixedTime("Dash", 0);
            }
        }

        // ダッシュ中の処理
        if (isDashing)
        {
            DashTime -= Time.deltaTime;  // ダッシュ時間を減らす

            // ダッシュが終了したら通常速度に戻す
            if (DashTime <= 0)
            {
                isDashing = false;
            }
        }

        // ダッシュしていないときに加速度を使用した通常移動処理
        if (!isDashing)
        {
            if (x != 0) // プレイヤーが左右に動いているとき
            {
                // 加速度に基づいてスピードを増加させる
                currentSpeed += acceleration * Time.deltaTime;
                currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);  // 最高速度に制限
            }
            else
            {
                // プレイヤーが動いていないときは減速させる
                currentSpeed -= deceleration * Time.deltaTime;
                currentSpeed = Mathf.Clamp(currentSpeed, 0,maxSpeed);  // 最低速度は0に制限
            }
        }

        // キー入力に関わらず、ダッシュ中はダッシュスピードで移動を続ける
        transform.Translate(new Vector3(x, y, 0) * currentSpeed * Time.deltaTime);

        // スペースキーを押してジャンプする処理
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(new Vector2(0, jumpPower), ForceMode2D.Impulse);
            isGrounded = false;
        }

        // クールタイムを減らす
        DashCoolTime -= Time.deltaTime;



        // スペースキーを押してジャンプする処理
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            // Rigidbodyに上方向の力を加えてジャンプする
            rb.AddForce(new Vector2(0, jumpPower), ForceMode2D.Impulse);
            isGrounded = false; // ジャンプ中は地面にいないと判定

            // 現在ダッシュ中ではない
            if (isDash_ == false)
            {
                // ジャンプのフラグを立てる(アニメーションも切り替える)
                isJump_ = true;
                animator.PlayInFixedTime("Jump", 0);
            }
        }
        

        if (!isGrounded)
        {
            rb.gravityScale = 10.0f;
        }

        isGrounded = false;

        // Animator更新
        UpdateAnimation();

        UpdateUI();
    }

    //地面との接触を判定する関数（例としてOnCollisionEnter2Dを使用）
    private void OnCollisionStay2D(Collision2D collision)
    {
        Debug.Log(isGrounded);


        // プレイヤーが地面に接触している場合
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;  // 地面に戻ったら再びジャンプ可能にする
            Yspeed = 0.0f;
            rb.gravityScale = 10.0f;
            rb.AddForce(collision.contacts[0].normal * -1 * 20);
        }
        // プレイヤーが地面に接触している場合
        if (collision.gameObject.CompareTag("Slope"))
        {
            isGrounded = true;  // 地面に戻ったら再びジャンプ可能にする
            Yspeed = 0.0f;
            rb.gravityScale = 0.5f;
        }
    }

    // アニメーション更新
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
            else if(horizontal < 0.0f)
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


        // ダッシュの速度がなくなったらモーションを終了する
        if(currentSpeed <= speed && isDash_ == true)
        {
            isDash_ = false;
        }

        // 現在ダッシュ中のためここで終了
        if (isDash_) return;

        // 移動なし
        if (horizontal == 0.0f)
        {
            // 現在のステートがIdleではないときに各項目設定
            if (clipName != "Idle")
            {
                // 現在のアニメーションを終了しIdleのアニメーションを流す
                animator.PlayInFixedTime("Idle", 0);

                // 移動フラグを下げる
                isMove_ = false;
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

                // 移動フラグを立てる
                isMove_ = true;
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

                // 移動フラグを立てる
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
