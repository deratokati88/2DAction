using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region//インスペクターで設定する
    [Header("移動速度")] public float speed;
    [Header("重力")] public float gravity;
    [Header("ジャンプ速度")] public float jumpSpeed;
    [Header("ジャンプする高さ")] public float jumpHeight;
    [Header("ジャンプする長さ")] public float jumpLimitTime;
    [Header("接地判定")] public GroundCheck ground;
    [Header("天井判定")] public GroundCheck head;
    [Header("ダッシュの速さ表現")] public AnimationCurve dashCurve;
    [Header("ジャンプの速さ表現")] public AnimationCurve jumpCurve;
    [Header("攻撃判定場所")] public Transform attackPoint;
    [Header("攻撃判定範囲")] public float attackRadius;
    [Header("攻撃する対象")] public LayerMask enemyLayer;
    [Header("攻撃力")] public int at = 1;
    [Header("発射する弾のオブジェクト")] public GameObject bulletPrefab;
    [Header("弾の発射位置")] public Transform shotPoint;
    [Header("ジャンプする時に鳴らすSE")] public AudioClip jumpSE; 
    [Header("やられた鳴らすSE")] public AudioClip downSE; 
    [Header("コンティニュー時に鳴らすSE")] public AudioClip continueSE;
    [Header("近距離攻撃時に鳴らすSE")] public AudioClip attackSE; 
    [Header("遠距離攻撃時に鳴らすSE")] public AudioClip shotSE;
    #endregion

    #region//プライベート変数
    private Animator anim = null;
    private Rigidbody2D rb = null;
    private SpriteRenderer sr = null;
    private bool isGround = false;
    private bool isJump = false;
    private bool isRun = false;
    private bool isHead = false;
    private bool isDown = false;
    private bool isDeath = false;
    private bool isContinue = false;
    private bool isClearMotion = false;
    private float downTime = 0.5f;
    private float leftDownTime;
    private float jumpPos = 0.0f;
    private float dashTime = 0.0f;
    private float jumpTime = 0.0f;
    private float beforeKey = 0.0f;
    private float coolTime = 1.0f;
    private float leftCoolTime;
    private float continueTime = 0.0f;
    private float blinkTime = 0.0f;
    private string enemyTag = "Enemy";
    private string deadAreaTag = "DeadArea";
    private string hitAreaTag = "HitArea";
    #endregion

    void Start()
    {
        //コンポーネントのインスタンスを捕まえる
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        isDown = calcDownTime();

        if (!isDown && !isDeath && !GManager.instance.isStageClear)
        {
            AttackSet();

            Shoot();

        }

        if (isContinue)
        {
            //明滅　ついている時に戻る
            if (blinkTime > 0.2f)
            {
                sr.enabled = true;
                blinkTime = 0.0f;
            }
            //明滅　消えているとき
            else if (blinkTime > 0.1f)
            {
                sr.enabled = false;
            }
            //明滅　ついているとき
            else
            {
                sr.enabled = true;
            }

            //1秒たったら明滅終わり
            if (continueTime > 1.0f)
            {
                isContinue = false;
                blinkTime = 0f;
                continueTime = 0f;
                sr.enabled = true;
            }
            else
            {
                blinkTime += Time.deltaTime;
                continueTime += Time.deltaTime;
            }
        }
    }

    void FixedUpdate()
    {
        if (!isDown && !isDeath && !GManager.instance.isStageClear)
        {
            //接地判定を得る
            isGround = ground.IsGround();
            isHead = head.IsGround();

            //各種座標軸の速度を求める
            float xSpeed = GetXSpeed();
            float ySpeed = GetYSpeed();

            //アニメーションを適用
            SetAnimation();

            //移動速度を設定
            rb.velocity = new Vector2(xSpeed, ySpeed);
        }

        else if(isDown)
        {
            rb.velocity = new Vector2(-1.5f, 1.0f);
        }

        else
        {
            if(!isClearMotion && GManager.instance.isStageClear)
            {
                anim.Play("player_win");
                isClearMotion = true;
            }
            rb.velocity = new Vector2(0, -gravity);
        }

    }

    /// <summary> 
    /// Y成分で必要な計算をし、速度を返す。 
    /// </summary> 
    /// <returns>Y軸の速さ</returns> 
    private float GetYSpeed()
    {
        float verticalKey = Input.GetAxis("Vertical");
        float ySpeed = -gravity;

        if (isGround)
        {
            if (verticalKey > 0)
            {
                if (!isJump)
                {
                    GManager.instance.PlaySE(jumpSE);  
                }
                ySpeed = jumpSpeed;
                jumpPos = transform.position.y; //ジャンプした位置を記録する
                isJump = true;
                jumpTime = 0.0f;
            }
            else
            {
                isJump = false;
            }
        }
        else if (isJump)
        {
            //上方向キーを押しているか
            bool pushUpKey = verticalKey > 0;
            //現在の高さが飛べる高さより下か
            bool canHeight = jumpPos + jumpHeight > transform.position.y;
            //ジャンプ時間が長くなりすぎてないか
            bool canTime = jumpLimitTime > jumpTime;

            if (pushUpKey && canHeight && canTime && !isHead)
            {
                ySpeed = jumpSpeed;
                jumpTime += Time.deltaTime;
            }
            else
            {
                isJump = false;
                jumpTime = 0.0f;
            }
        }

        if (isJump)
        {
            ySpeed *= jumpCurve.Evaluate(jumpTime);
        }

        return ySpeed;
    }

    /// <summary> 
    /// X成分で必要な計算をし、速度を返す。 
    /// </summary> 
    /// <returns>X軸の速さ</returns> 
    private float GetXSpeed()
    {
        float horizontalKey = Input.GetAxis("Horizontal");
        float xSpeed = 0.0f;

        if (horizontalKey > 0)
        {
            this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            isRun = true;
            dashTime += Time.deltaTime;
            xSpeed = speed;
        }
        else if (horizontalKey < 0)
        {
            this.transform.rotation = Quaternion.Euler(0.0f, 180f, 0.0f); 
            isRun = true;
            dashTime += Time.deltaTime;
            xSpeed = -speed;
        }
        else
        {
            isRun = false;
            xSpeed = 0.0f;
            dashTime = 0.0f;
        }

        //前回の入力からダッシュの反転を判断して速度を変える
        if (horizontalKey > 0 && beforeKey < 0)
        {
            dashTime = 0.0f;
        }
        else if (horizontalKey < 0 && beforeKey > 0)
        {
            dashTime = 0.0f;
        }

        beforeKey = horizontalKey;
        xSpeed *= dashCurve.Evaluate(dashTime);
        beforeKey = horizontalKey;
        return xSpeed;
    }

    /// <summary> 
    /// アニメーションを設定する 
    /// </summary> 
    private void SetAnimation()
    {
        anim.SetBool("jump", isJump);
        anim.SetBool("ground", isGround);
        anim.SetBool("run", isRun);
        if(!isDown)
        {
            anim.ResetTrigger("down");
        }
    }

    /// <summary> 
    /// 近距離攻撃 
    /// </summary> 
    void AttackSet()
    {
        leftCoolTime -= Time.deltaTime;
        if (leftCoolTime <= 0)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                anim.SetTrigger("IsAttack");
                leftCoolTime = coolTime - 0.5f;
            }
        }
    }

    void Attack()
    {
        GManager.instance.PlaySE(attackSE);
        Collider2D[] hitEnemys = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, enemyLayer);
        foreach (Collider2D hitEnemy in hitEnemys)
        {
            if (hitEnemy.tag == "Enemy")
            {
                hitEnemy.GetComponent<EnemyManager>().OnDamage(at);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }


    /// <summary> 
    /// 遠距離攻撃 
    /// </summary> 
    void Shoot()
    {
        leftCoolTime -= Time.deltaTime;
        if (leftCoolTime <= 0)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                GManager.instance.PlaySE(shotSE);
                anim.SetTrigger("shoot");
                Instantiate(bulletPrefab, shotPoint.position, transform.rotation);
                leftCoolTime = coolTime;
            }

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == deadAreaTag)
        {
            OnDamage(GManager.instance.maxHpNum);
        }
        
    }
    private bool calcDownTime()
    {
        leftDownTime -= Time.deltaTime;

        if (leftDownTime <= 0)
        {
            isDown = false;
        }

        else
        {
            isDown = true;
        }

        return isDown;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == enemyTag)
         {
                OnDamage(1);
         }

        else if (collision.collider.tag == hitAreaTag)
        {
            OnDamage(1);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.tag == enemyTag)
        {
            OnDamage(1);
        }

        else if (collision.collider.tag == hitAreaTag)
        {
            OnDamage(1);
        }
    }

    public void OnDamage(int damage)
    {
        if(!isDown && !isDeath && !GManager.instance.isStageClear)
        {
            GManager.instance.hpNum -= damage;
            anim.SetTrigger("down");
            leftDownTime = downTime;
            isDown = true;
            GManager.instance.PlaySE(downSE);
            if (GManager.instance.hpNum <= 0)
            {
                Die();
            }
        }

        else
        {
            return;
        }
    }

    private void Die()
    {
        GManager.instance.hpNum = 0;
        //死亡アニメーションをセット
        anim.Play("player_death");
        isDeath = true;
    }

    //プレイヤーの死亡アニメーションが終了しているか判定する
    private bool IsDeathAnimEnd()
    {
        if(isDeath && anim!= null)
        {
            AnimatorStateInfo currentState = anim.GetCurrentAnimatorStateInfo(0);
            if (currentState.IsName("player_death"))
            {
                if (currentState.normalizedTime >= 1)
                {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// コンティニュー待機状態か
    /// </summary>
    /// <returns></returns>
    public bool IsContinueWaiting()
    {
        return IsDeathAnimEnd();
    }

    /// <summary>
    /// コンティニューする
    /// </summary>
    public void ContinuePlayer()
    {
        GManager.instance.hpNum = GManager.instance.maxHpNum;
        isDown = false;
        isDeath = false;
        anim.Play("player_stand");
        isJump = false;
        isRun = false;
        isContinue = true;
        GManager.instance.SubHeartNum();
        GManager.instance.retryNum += 1;
        GManager.instance.PlaySE(continueSE);
    }
}