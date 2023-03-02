using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockSniper : FlockingEnemy
{
    int state = 0;

    //laser
    public LineRenderer lineRenderer;
    public float laserFireTime = 200f;
    float timerSniper;


    public float cooldown = 1000f;
    public float laserFireRotationSpeed = 0.1f;
    public float laserLength = 20.0f;
    public float damage = 0.01f;
   
    private Vector2 playerDir;
    private float playerDirAngle;
    private bool attackLock;
    public float maxAttackAngle = 40;
    Vector2 vel;
    RaycastHit2D hit;
    Vector3[] points = new Vector3[2];
    bool isActive = false;

    private int laserLayer = ~((1 << 6) | (1 << 9) | (1<<10) | (1<<13) |(1<<15));


    public ParticleSystem particle;

    //death explosion
    ParticleSystem p;

    public override void onDeath()
    {
        p.transform.position = transform.position;
        p.Play();
        disableLaser();
        if (isActive)
        {
            pool.flockSniperPool.Release(this);
            isActive = false;
        }
    }

    private void OnEnable()
    {
        state = 0;
        isActive = true;
        health = Stats.flockSniper.maxHealth;
    }
    public override void takeDamage(float damage)
    {
        health -= damage;
        if (health < 0)
        {
            onDeath();
        }
    }

    // 0 follow target dist > chase radius
    //1 attack dist < chase radius
    //2 attack cooldown 
    //3 malfunctioning 
    //4 panic escape
    //5 die

    // Start is called before the first frame update
    private void Start()
    {
        //base.Start();
        damage = Stats.flockSniper.damage;
        agentCollider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        nRange = Stats.flockSniper.Nrange;
        malfunctionLimit = Stats.flockSniper.malfunctionMax;
        cooldown = Stats.flockSniper.CoolDown;
        attackRadius = Stats.flockSniper.attackRadius;
        approachRadius = Stats.flockSniper.approachRadius;
        driveFactor = Stats.flockSniper.driveSpeed;
        maxTimer = Stats.flockSniper.flockTimer;
        closestPlanetUpdateInterval = Stats.flockSniper.planetUpdateTimer;
        evasionPower = Stats.flockSniper.evasionPower;
        steeringPower = Stats.flockSniper.steeringPower;
        smoothTime = Stats.flockSniper.smoothTimer;
        smoothDampAmount = Stats.flockSniper.smoothAmount;
        maxVelocity = Stats.flockSniper.maxSpeed;
        flockingPower = Stats.flockSniper.flockingPower;
        defaultRotateSpeed = Stats.flockSniper.defaultRotateSpeed;



        velocity = Vector2.zero;
        flockVelocity = Vector2.up * driveFactor;
        headingDirection = Vector2.up;
        weights = Stats.flockSniper.weights;
        lineRenderer.positionCount = 2;


        rb.velocity = Vector2.up * maxVelocity;
        disableLaser();
        timerSniper = UnityEngine.Random.Range(0,cooldown);
        state = 0;
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
        InstantMove(vel);
        //velocity += vel;

        if (state == 0)
        {
            base.Update();

            rotateTowardsTarget(velocity, defaultRotateSpeed * Time.deltaTime);
            //attack 
            if (Vector2.Distance(transform.position, target.position) < attackRadius)
            {
                state = 2;
               
            }
        }
        else if (state == 1)
        {
            playerDir = (Player.transform.position - transform.position).normalized;
            rotateTowardsTarget(playerDir, laserFireRotationSpeed * Time.deltaTime);

            base.Update();



            if (timerSniper <= 0 )
            {
                state = 2;
                disableLaser();
             
                timerSniper = cooldown;
            }
            else
            {
                timerSniper -= (Time.deltaTime * 60);
                fireLaser();
            }
            
        }
        else if (state == 2)
        {
            base.Update();
            playerDir = (Player.transform.position - transform.position).normalized;
            rotateTowardsTarget(playerDir, defaultRotateSpeed * Time.deltaTime);
            attackLock = false;

            playerDirAngle = Vector2.Angle(playerDir, headingDirection);
            if (playerDirAngle > -maxAttackAngle && playerDirAngle < maxAttackAngle)
            {
                attackLock = true;
            }
            if (timerSniper < 0 && attackLock)
            {
            
                state = 1;
                disableLaser();
                activateLaser();
                fireLaser();
                timerSniper = laserFireTime;
            }
            else
            {
                timerSniper -= (Time.deltaTime * 60);
            }
            if (Vector2.Distance(transform.position, target.position) > attackRadius)
            {
                state = 0;
                disableLaser();
            }
        }
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


    private void fireLaser()
    {
        points[0] = gameObject.transform.position;
        hit = Physics2D.Raycast(gameObject.transform.position, gameObject.transform.up, laserLength, laserLayer );
        if (hit)
        {
            points[1] = hit.point;
            if (hit.transform.gameObject.CompareTag("Player"))
            {
                playerHealth.TakeDamage(damage);
            }
        }
        else
        {
            points[1] = gameObject.transform.position  + gameObject.transform.up.normalized*laserLength;

        }
        lineRenderer.SetPositions(points);
    }

    private void activateLaser()
    {
        lineRenderer.enabled = true;
    }

    private void disableLaser()
    {
        lineRenderer.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerBullet"))
        {
            takeDamage(Stats.PlayerBulletDamage);
        }
        if (collision.CompareTag("ForceField"))
        {
            disableLaser();
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
            //Destroy(gameObject);
            onDeath();
        }
    }
}
