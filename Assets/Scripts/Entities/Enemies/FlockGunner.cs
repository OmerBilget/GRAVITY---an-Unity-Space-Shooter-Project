using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockGunner : FlockingEnemy
{
    private int state = 0;

    //attack cooldown
    private float cooldown = 10;
    private float maxCooldown = 20;

    private float loopCounter = 3;
    private float counter = 0;

    //bullet spread
    public float spread = 1;
    public ParticleSystem particle;
 
    //death explosion
    ParticleSystem p;
    Vector2 vel;
    Vector2 playerDir;

    bool isActive = false;
    public override void onDeath()
    {
        p.transform.position = transform.position;
        p.Play();
        if (isActive)
        {
            pool.flockGunnerPool.Release(this);
            isActive = false;
        }
        //Destroy(gameObject);
       
    }

    public override void takeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            
            onDeath();
        }
    }

    // 0 follow target dist > chase radius
    //1 attack dist < chase radius
    //2 attack cooldown 
    //3 malfunctioning 
    //4 die
    private void OnEnable()
    {
        state = 0;
        headingDirection = Vector2.up;
        malfunctionCounter = 0;
        counter = loopCounter;
        timer = UnityEngine.Random.Range(0, maxTimer);
        isActive = true;
        health = Stats.flockGunner.maxHealth;
    }
    // Start is called before the first frame update
    private void Start()
    {
        //base.Start();
        agentCollider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();


        nRange = Stats.flockGunner.Nrange;
        malfunctionLimit = Stats.flockGunner.malfunctionMax;
        maxCooldown = Stats.flockGunner.CoolDown;
        attackRadius = Stats.flockGunner.attackRadius;
        approachRadius = Stats.flockGunner.approachRadius;
        driveFactor = Stats.flockGunner.driveSpeed;
        maxTimer = Stats.flockGunner.flockTimer;
        planetUpdateTimer = Stats.flockGunner.planetUpdateTimer;
        evasionPower = Stats.flockGunner.evasionPower;
        steeringPower = Stats.flockGunner.steeringPower;
        smoothTime = Stats.flockGunner.smoothTimer;
        smoothDampAmount = Stats.flockGunner.smoothAmount;
        maxVelocity = Stats.flockGunner.maxSpeed;
        flockingPower = Stats.flockGunner.flockingPower;
        defaultRotateSpeed = Stats.flockGunner.defaultRotateSpeed;
        mass = Stats.flockGunner.mass;

        velocity = Vector2.up;
        headingDirection = Vector2.up;
        malfunctionCounter = 0;
        counter = loopCounter;
        timer = UnityEngine.Random.Range(0, maxTimer);
        p = Instantiate(particle);
        ClosestPlanet = sol.getClosestPlanet(transform.position);
    }

    // Update is called once per frame

    private void Update()
    {



    }


    private void FixedUpdate()
    {
       
        
        //if given time is up update closest planet and use gravity
        if (planetUpdateTimer <= 0)
        {
            ClosestPlanet = sol.getClosestPlanet(transform.position);
            planetUpdateTimer = closestPlanetUpdateInterval;
        }
        else
        {
            planetUpdateTimer -= Time.deltaTime;
        }


        vel = sol.getGravity(ClosestPlanet, rb.transform.position, mass);
        //InstantMove(vel);
       
       

        //chase
        if (state == 0)
        {
            base.Update();
            rotateTowardsTarget(velocity, defaultRotateSpeed * Time.deltaTime);
            if (Vector2.Distance(transform.position, target.position) < attackRadius)
            {
                state = 1;
                
            }
            
        }
        //attack
        else if (state == 1)
        {
            //attack
            
            FlockGunnerBullet bullet = pool.bulletPool.Get();
            bullet.transform.position = transform.position;
            bullet.velocity = (Player.transform.position - transform.position).normalized * bullet.speed;
            bullet.velocity = RotateVector(bullet.velocity, UnityEngine.Random.Range(-spread, spread));
            cooldown = maxCooldown;
            state = 2;
        }

        //attack cooldown
        else if (state == 2)
        {
            playerDir = (Player.transform.position - transform.position).normalized;
            rotateTowardsTarget(playerDir, defaultRotateSpeed* Time.deltaTime);
            if (counter <= 0)
            {
                base.Update();
                counter = loopCounter;
            }
            else
            {
                counter -= Time.deltaTime;
            }
            
           
            rotateTowardsTarget((Player.transform.position - transform.position).normalized, defaultRotateSpeed * Time.deltaTime);
            cooldown -= Time.deltaTime * 60;
            if (cooldown < 0)
            {
                state = 1;
            }
            if (Vector2.Distance(transform.position, target.position) > attackRadius)
            {
                state = 0;
               
            }
        }
        //broken hit by pulse
        else if (state == 3)
        {

          
            malfunctionCounter -= Time.deltaTime;
            if (malfunctionCounter <= 0)
            {
                state = 0;
            }
            
        }
        else if (state == 4)
        {
            onDeath();
        }
        
        

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerBullet"))
        {
            takeDamage(Stats.PlayerBulletDamage);
        }
        if (collision.CompareTag("ForceField"))
        {
           
            state = 3;
            malfunctionCounter = malfunctionLimit;
           
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        
        if (collision.CompareTag("ForceField"))
        {
            //insideForceField = false;
            velocity = rb.velocity;
            Debug.Log(velocity.magnitude);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("TerrestialPlanet"))
        {
            onDeath();
        }
    }

    public Vector2 RotateVector(Vector2 v, float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        float _x = v.x * Mathf.Cos(radian) - v.y * Mathf.Sin(radian);
        float _y = v.x * Mathf.Sin(radian) + v.y * Mathf.Cos(radian);
        return new Vector2(_x, _y);
    }

}
