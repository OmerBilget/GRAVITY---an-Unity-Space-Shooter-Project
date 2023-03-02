using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
public class Bullet : MonoBehaviour
{
    public Vector2 velocity=Vector2.up;
    float life = 100f;
    float timer;
    public ObjectPool<Bullet> pool;
    private bool released = true;
    float maxSpeed = 0.4f;
    public float speed = 0.02f;
    float acc = 0.0001f;
    public ParticleSystem impactPrefab;
    public Stats stats;
    ParticleSystem impact;
    public SolarSystem sol;
    // Start is called before the first frame update
    void Start()
    {
        life = stats.PlayerBulletLife;
        maxSpeed = stats.PlayerMaxBulletSpeed;
        acc = stats.PlayerBulletAcceleration;

        timer = life;
       
        
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        velocity = (Time.deltaTime * 60) * 0.07f * speed * velocity.normalized;
        velocity += sol.getGravity(transform.position, 0.0002f) * (Time.deltaTime * 60);
        transform.position += (Vector3)velocity;
        if (speed < maxSpeed)
        {
            speed += acc * (Time.deltaTime * 60);
        }
        gameObject.transform.up = velocity;
        timer -= (Time.deltaTime * 600);
        if (timer <= 0)
        {


            if (released == false && pool != null)
            {

                impact = Instantiate(impactPrefab, transform.position, Quaternion.identity);
                impact.Play();
                pool.Release(this);
                released = true;
            }




        }
    }


    private void OnEnable()
    {
        timer = life;
        released = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {


        if (released == false &  pool != null)
        {

            impact = Instantiate(impactPrefab, transform.position, Quaternion.identity);
            impact.Play();
            pool.Release(this);
            released = true;
          
        }
        

    }



}
