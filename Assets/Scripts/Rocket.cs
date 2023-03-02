using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
public class Rocket : MonoBehaviour
{
    public Transform target;
    float rotateSpeed =1;
    float maxLife = 500;
    float life;
    float maxSpeed=2;
    float maxHealth = 20;
    float health;
    public ObjectPool<Rocket> pool;
    public Vector2 velocity = Vector2.zero;
    Vector2 acc = Vector2.zero;
    Vector2 desiredVel;
    Vector2 steerForce;
    Vector2 dist;
    public ParticleSystem explosion;
    ParticleSystem p;
    public Rigidbody2D rb;
    bool isActive = false;
    float malfunctionMax=10;
    float malfunctionRate=5;
    float malfunctionCounter;
    public Stats stats;
    Vector2 tmp;
    // Start is called before the first frame update
    void Start()
    {
        p = Instantiate(explosion);
       
        rotateSpeed = stats.rocketRotateSpeed;
        maxLife = stats.rocketMaxSpeed;
        maxHealth = stats.RocketMaxHealth;
        maxSpeed = stats.rocketMaxSpeed;
        malfunctionMax = stats.rocketMalfunctionMax;
        malfunctionRate = stats.RocketMalfunctionRate;
        life = maxLife;
        health = maxHealth;
        rotateSpeed += UnityEngine.Random.Range(-0.1f, 0.1f);
   
    }

    private void OnEnable()
    {
        isActive = true;
        life = maxLife;
        health = maxHealth;
    }
    private void FixedUpdate()
    {

        if (life > 0)
        {
            life -= Time.deltaTime;
            if (life <= 0)
            {
                explode();
            }
        }
        if (malfunctionCounter > 0)
        {
            malfunctionCounter -= malfunctionRate*Time.deltaTime;
            return;
        }
        rb.transform.up = rb.velocity.normalized;
        

        
        if (target == null)
        {
            if (velocity.sqrMagnitude < maxSpeed * maxSpeed)
            {
                velocity *= 1.1f;
            }
            rb.velocity = velocity;
            return;
        }



        dist = (target.position - transform.position);
        if (dist.sqrMagnitude <= 36)
        {
            desiredVel = (target.position - transform.position).normalized * maxSpeed;
            steerForce = (desiredVel - velocity).normalized * rotateSpeed*3;
            velocity += steerForce * Time.deltaTime;
     
            rb.velocity = velocity;
        }
        else
        {
            desiredVel = (target.position - transform.position).normalized * maxSpeed;
            steerForce = (desiredVel - velocity).normalized * rotateSpeed;
            velocity += steerForce * Time.deltaTime;
            rb.velocity = velocity;
        }
       

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            
            PlayerHealth h = collision.gameObject.GetComponent<PlayerHealth>();
            h.TakeDamage(stats.flockRocket.damage);
            
        }
        explode();
        //planet layer

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerBullet"))
        {
           takeDamage(stats.PlayerBulletDamage);
        }
     
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("ForceField"))
        {
            velocity = rb.velocity.normalized*maxSpeed;
            malfunctionCounter = malfunctionMax;
        }
    }
    public void takeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            explode();
           
        }
    }

    void explode()
    {
        p.transform.position = transform.position; 
        p.Play();
        if (isActive == true)
        {
            isActive = false;
            pool.Release(this);
        }

    }
}
