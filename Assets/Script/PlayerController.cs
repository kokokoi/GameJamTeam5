using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField] float airControlFactor = 0.0f;//空中での操作制限

    //現在のスピードを保持しておく本数
    float currentSpeed;
    float DashCoolTime;
    float DashTime;

    bool isDashing = false;
    bool isMovingHorizontally = false;

    //ジャンプ関連
    [SerializeField] bool isGrounded;//地面にいるかどうか
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
        //現在のスピードをスピードに固定
        currentSpeed = speed;

        //RigidBody2dコンポーネントを取得
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

            // 目標位置を計算してダッシュのために移動を開始
            targetPosition = transform.position + new Vector3(x, y, 0) * dashSpeed * dashTime;

            // イージングを使って移動
            transform.DOMove(targetPosition, dashTime)
                .SetEase(Ease.OutQuad) // Ease設定（自由に変更可能）
                .OnComplete(() =>
                {
                    isDashing = false; // ダッシュ終了
                    currentSpeed = speed; // 通常スピードに戻す
                });
        }

        // ダッシュしていない場合は通常の移動処理
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

            // 通常の移動（イージングなし）
            transform.Translate(new Vector3(x, y, 0) * currentSpeed * Time.deltaTime);
        }

        // スペースキーでジャンプ処理
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(new Vector2(0, jumpPower), ForceMode2D.Impulse);
            isGrounded = false;

            // ジャンプフラグを立てる
            isJump = true;
            animator.PlayInFixedTime("Jump", 0);
        }

        DashCoolTime -= Time.deltaTime;

        UpdateAnimation();

        UpdateUI();
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

    public void Death()
    {
        PlayerReset();
    }
    private void PlayerReset()
    {
        this.transform.position = Vector3.zero;
    }

}
