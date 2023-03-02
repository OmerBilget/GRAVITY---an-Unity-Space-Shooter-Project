using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
public class FlockGunnerBullet : MonoBehaviour
{
    public Vector2 velocity = Vector2.up;
    public float life = 20;
    float timer;
    public ObjectPool<FlockGunnerBullet> pool;
    private bool released = true;
    public float maxSpeed = 0.5f;
    public float speed = 0.2f;
    public float acc = 0.001f;
    public SolarSystem sol;
    // Start is called before the first frame update
    void Start()
    {
        timer = life;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        velocity = velocity.normalized * speed * (Time.deltaTime * 60);
        //velocity += sol.getGravity(transform.position, 0.002f) * (Time.deltaTime * 60);
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

        
        if (released == false & pool != null)
        {
            pool.Release(this);
            released = true;

        }


    }
}
