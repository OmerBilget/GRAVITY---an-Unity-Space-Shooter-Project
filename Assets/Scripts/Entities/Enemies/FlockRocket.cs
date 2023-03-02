using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockRocket : FlockingEnemy
{

    private int state = 0;

    //attack cooldown
    private float cooldown = 10;
    public float maxCooldown = 20;

    private float loopCounter = 3;
    private float counter = 0;

    //bullet spread

    public ParticleSystem particle;

    //death explosion
    ParticleSystem p;
    Vector2 vel;
    Vector2 tmpVec;
    bool isActive = false;
    public float rocketSpreadAngle = 70;
    public override void onDeath()
    {
        p.transform.position = transform.position;
        p.Play();
        if (isActive)
        {
            pool.flockRocketPool.Release(this);
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
        cooldown = Random.Range(0, maxCooldown);
        isActive = true;
        health = Stats.flockRocket.maxHealth;

    }
    // Start is called before the first frame update
    private void Start()
    {
        //base.Start();


        nRange = Stats.flockRocket.Nrange;
        malfunctionLimit = Stats.flockRocket.malfunctionMax;
        maxCooldown = Stats.flockRocket.CoolDown;
        attackRadius = Stats.flockRocket.attackRadius;
        approachRadius = Stats.flockRocket.approachRadius;
        driveFactor = Stats.flockRocket.driveSpeed;
        maxTimer = Stats.flockRocket.flockTimer;
        planetUpdateTimer = Stats.flockRocket.planetUpdateTimer;
        evasionPower = Stats.flockRocket.evasionPower;
        steeringPower = Stats.flockRocket.steeringPower;
        smoothTime = Stats.flockRocket.smoothTimer;
        smoothDampAmount = Stats.flockRocket.smoothAmount;
        maxVelocity = Stats.flockRocket.maxSpeed;
        flockingPower = Stats.flockRocket.flockingPower;
        defaultRotateSpeed = Stats.flockRocket.defaultRotateSpeed;
        mass = Stats.flockRocket.mass;



        agentCollider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        velocity = Vector2.zero;
        headingDirection = Vector2.up;
        malfunctionCounter = 0;
        counter = loopCounter;
        cooldown = Random.Range(0, maxCooldown);
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
       


        //chase
        if (state == 0)
        {
            base.Update();
            rotateTowardsTarget(velocity, defaultRotateSpeed * Time.deltaTime);
            if (Vector2.Distance(transform.position, target.position) < attackRadius)
            {
                state = 2;

            }

        }
        //attack
        else if (state == 1)
        {
            //attack
            Rocket r = pool.rocketPool.Get();
            tmpVec= (Player.position-transform.position).normalized*6;
            tmpVec = RotateVector(tmp, UnityEngine.Random.Range(-rocketSpreadAngle, rocketSpreadAngle));
            r.velocity = tmpVec;
            r.transform.position = transform.position;
            r.target = Player;
            cooldown = maxCooldown;
            state = 2;
        }

        //attack cooldown
        else if (state == 2)
        {
            base.Update();
            if (counter <= 0)
            {
                
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


            malfunctionCounter -=Time.deltaTime;
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
