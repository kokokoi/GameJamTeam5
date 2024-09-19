using System;
using System.Collections;
using System.Collections.Generic;
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
                currentSpeed = Mathf.Max(currentSpeed, 0);  // 最低速度は0に制限
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
        }

        if (!isGrounded)
        {
            rb.gravityScale = 10.0f;
        }

        isGrounded = false;

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
