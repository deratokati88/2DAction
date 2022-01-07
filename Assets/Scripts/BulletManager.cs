using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    public GameObject impactPrefab;
    public float bulletSpeed = 10f;

    private Rigidbody2D rb;
   
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * bulletSpeed;    
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            EnemyManager enemy = collision.GetComponent<EnemyManager>();
            enemy.OnDamage(1);
            Instantiate(impactPrefab, transform.position, transform.rotation);
            Destroy(gameObject);
        }

       else if(collision.tag == "Ground" || collision.tag == "HitArea")
        {
            Destroy(gameObject);
        }
        
    }
}
