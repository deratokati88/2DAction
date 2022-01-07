using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCrab : MonoBehaviour
{
    #region//インスペクターで設定する
    [Header("加算するスコア")] public int myScore;
    [Header("移動速度")] public float speed;
    [Header("重力")] public float gravity;
    [Header("大きさ")] public float size = 8.0f;
    [Header("画面外でも行動する")] public bool nonVisibleAct;
    [Header("接触判定")] public EnemyCollisionCheck checkCollision;
    [Header("体力")] public int enemyHp = 3;
    [Header("やられた時に鳴らすSE")] public AudioClip deadSE;
    [Header("ダメージを受けた時に鳴らすSE")] public AudioClip hurtSE;
    [Header("死亡時のエフェクト")] public GameObject explosionPrefab;
    #endregion


    #region//プライベート変数
    private Rigidbody2D rb = null;
    private SpriteRenderer sr = null;
    private bool rightTleftF = false;
    private bool isDown = false;
    private float downTime = 0.5f;
    private float leftDownTime;
    private Animator anim;
    private Transform tf;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        tf = gameObject.GetComponent<Transform>();
        tf.transform.localScale = new Vector2(size, size);
    }

    void FixedUpdate()
    {
        if (sr.isVisible || nonVisibleAct)
        {
            isDown = calcDownTime();
            if (!isDown)
            {
                if (checkCollision.isOn)
                {
                    rightTleftF = !rightTleftF;
                }
                int xVector = -1;
                if (rightTleftF)
                {
                    xVector = 1;
                    transform.localScale = new Vector3(-size, size, 1);
                }
                else
                {
                    transform.localScale = new Vector3(size, size, 1);
                }
                rb.velocity = new Vector2(xVector * speed, -gravity);
            }

        }
        else
        {
            rb.Sleep();
        }

    }

    public void OnDamage(int damage)
    {
        enemyHp -= damage;
        GManager.instance.PlaySE(hurtSE);
        anim.SetTrigger("IsHurt");
        leftDownTime = downTime;
        if (enemyHp <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        enemyHp = 0;
        Destroy(gameObject);
        if (GManager.instance != null)
        {
            GManager.instance.PlaySE(deadSE);
            Instantiate(explosionPrefab, transform.position, transform.rotation);
            GManager.instance.score += myScore;
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
}
