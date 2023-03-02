using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    public Rigidbody2D rb;

    
    public Transform[] spawnPoints;
    public GameObject shockArea;
  
    float orbitRadius;
    public float orbitalSpeed=0.0001f;
    public float angle=0;
    float PIx2 = Mathf.PI*2;
    [Range(0,1)]
    public int mode=0;
    public LineRenderer lineRenderer;
    public float laserLength = 100;
    public float laserDamage = 0.01f;
    public float laserRotateSpeed = 0.07f;
    public float laserPushForce = 0.3f;
    public float avoidanceRadius;
    public Stats stats;
    public int turretCount = 7;
    public bool ShieldOn=false;
    public float evasionPower = 2.0f;
    public float attackRadius = 10;


    public EnemyPool pool;



    public int[] maxCount = new int[4];
    Vector2 tmpVec;
    const float Density = 0.08f;

    float squareAvoidanceRadius;
    public int[] enemyCountArray = new int[4];
    public float SquareAvoidanceRadius { get { return squareAvoidanceRadius; } }
    public float spawn_radius = 20.0f;


    float laserCounter;
    public float maxlaserCounter = 100;

    public float maxLaserCooldown = 100;
    public float laserCooldown = 0;

    public float maxTurretFireTime = 3;
    float TurretFireTimer = 0;
    public float turretRotateSpeed = 0;


    public float maxTurretBulletFireCoolDown = 3;
    float bulletFireCoolDownTimer = 0;
    Vector3[] points = new Vector3[2];
    RaycastHit2D hit;
    //mode 0 spawn flock orbit 
    //mode 1 attack directly
    //mode 2 death
    // Start is called before the first frame update
    public BossTurret bossTurretPrefab;
    BossTurret[] turrets;
    int state = 0;
    int modeZeroState = 0;
    public GameObject shield;
    //0 chase player
    //1 fire central laser and star shape turrets
    //2 fire turrets
    //3 laser cooldown
    //4 change attackType
    //5 eye shot damaged disable
    public int waveCount = 5;
    public int waveIndex = 0;
    public int[] spawnCounts = new int[5];
    public int[] spawnLow = new int[5];
    public int startRadius=50;
    //phase 0  star laser slow rotate turret //pull toward self 
    //phase1 rotating towards player laser sin wave turret spawn flocks 

    //20->5  50 -> 10  150->50  500->150 1000  -> 0 mode 1 phase0-> health below%70  phase1 health %10 ->phase 2

    public bool spawned = false;
    public RectTransform healthBar;
    private int laserLayer = ~((1 << 9) | (1 << 10) | (1 << 13) | (1 << 15)|(1<<8)|(1<<16));

    public int spawnCount = 0;
    public bool started = false;

    public float maxVelocity;
    public int phase = 0;
    Vector2 velocity;
    void Start()
    {

        MaxHealth = stats.bossStats.MaxHealth;
        laserDamage = stats.bossStats.mainLaserDamage;
        turretCount = stats.bossStats.TurretCount;
        attackRadius = stats.bossStats.AttackRadius;
        laserRotateSpeed = stats.bossStats.laserRotateSpeed;
        turretRotateSpeed = stats.bossStats.turretBulletFireRotateSpeed;
        laserCounter = stats.bossStats.laserFireTime;
        maxTurretFireTime = stats.bossStats.turretBulletFireTime;
        maxLaserCooldown = stats.bossStats.LaserCoolDown;
        maxTurretBulletFireCoolDown = stats.bossStats.turretBulletFireCooldown;



        turrets = new BossTurret[turretCount];
        
        orbitRadius = transform.position.magnitude;
        health = MaxHealth;
        lineRenderer.enabled = true;
        float turretAngle = 360 / (turretCount + 1);

        float spawnAngle = turretAngle;
        velocity = Vector2.zero;
        for(int i = 0; i < turretCount; i++)
        {
            Vector3 spawnPoint = rotate(transform.up, Mathf.Deg2Rad*spawnAngle)*10;
            spawnAngle += turretAngle;
            turrets[i] = Instantiate(bossTurretPrefab,transform);
            turrets[i].transform.localPosition = spawnPoint;
            turrets[i].parent = transform;
            turrets[i].target = Player;
            turrets[i].state = 1;
            turrets[i].pool = pool;
           // turrets[i].activateLaser();
            turrets[i].transform.up = (turrets[i].transform.position - transform.position).normalized;
        }
        shield.SetActive(false);
        disableShockArea();
    }

    // Update is called once per frame
    void Update()
    {
       
    }



    private void FixedUpdate()
    {

        //orbit around sun and flock spawn
        if (mode == 0)
        {

            //move
            Vector2 targetPosition = (rotate(Vector2.right, (float)angle) * orbitRadius);
            angle += orbitalSpeed * Time.deltaTime;
            angle %= PIx2;
            rb.MovePosition(targetPosition);
            rb.transform.up = -transform.position.normalized;

            if (modeZeroState == 0)
            {
                if (Vector2.Distance(transform.position, Player.position) < startRadius)
                {
                    shield.SetActive(true);
                    modeZeroState = 1;
                    

                }
                
            }
            else if(modeZeroState == 1)
            {

                //spawn flock 
                if (spawnCount < spawnCounts[waveIndex])
                {
                    spawnFlock();
                }
                else
                {
                    //if all wave spawned and spawn_low amount flock remained -> next wave
                    int activeCount = pool.flockGunnerPool.CountActive + pool.flockSniperPool.CountActive + pool.flockRocketPool.CountActive;
                    if (activeCount <= spawnLow[waveIndex])
                    {
                        waveIndex += 1;
                        spawnCount = 0;
                    }
                    //all waves spawned boss time
                    if (waveIndex >= waveCount)
                    {
                        mode = 1;
                        shield.SetActive(false);
                        return;
                    }
                }
                
            }
            

            
        }
        else
        {



            if (phase == 0)
            {


                //Debug.Log(state);
                if (state == 0)
                {

                    //player is far away chase him
                    if (Vector2.Distance(transform.position, Player.position) < attackRadius)
                    {

                        //if player is close decide which attack to perform
                        state = 4;



                    }
                    else
                    {
                        //Vector2 targetVector = ( player.transform.position - transform.position).normalized * maxVelocity;

                        //Vector2 tmpvelocity = (GetObstacleAvoidVector() + targetVector) * maxVelocity ;

                        rb.velocity = velocity;
                    }

                }
                else if (state == 1)
                {
                    //fire laser
                    Vector2 targetDir = (Player.position - transform.position).normalized;
                    rb.transform.up = Vector3.RotateTowards(transform.up, targetDir, laserRotateSpeed * Time.deltaTime, 1.0f);
                    fireLaser();
                    laserCounter -= Time.deltaTime;
                    if (laserCounter <= 0)
                    {
                        state = 2;
                        stopLaserStar();
                        laserCooldown = maxLaserCooldown;
                    }

                }
                else if (state == 2)
                {
                    //fire main laser and turret laser
                    laserCooldown -= Time.deltaTime;
                    if (laserCooldown <= 0)
                    {
                        state = 0;
                    }
                    //cooldown;
                }
                else if (state == 3)
                {
                    //bullet fire
                    rb.rotation += turretRotateSpeed * Time.deltaTime;
                    TurretFireTimer -= Time.deltaTime;


                    //timer 0 stop turret
                    if (TurretFireTimer <= 0)
                    {
                        stopTurrets();
                        state = 5;
                        bulletFireCoolDownTimer = maxTurretBulletFireCoolDown;
                    }

                }
                else if (state == 4)
                {

                    if (Vector2.Distance(transform.position, Player.position) > attackRadius)
                    {

                        //if player is close decide which attack to perform
                        state = 0;
                        return;


                    }
                    //decide attack 
                    int randNum = Random.Range(0, 2);
                    if (randNum == 0)
                    {
                        startLaserStar();
                        laserCounter = maxlaserCounter;
                        state = 1;
                    }
                    else
                    {
                        startTurrets();
                        TurretFireTimer = maxTurretFireTime;
                        state = 3;
                    }
                }
                else if (state == 5)
                {

                    //bullet fire cooldown
                    bulletFireCoolDownTimer -= Time.deltaTime;
                    if (bulletFireCoolDownTimer <= 0)
                    {
                        state = 0;
                    }
                }













            }else if (phase == 1)
            {





                //Debug.Log(state);
                if (state == 0)
                {

                    //player is far away chase him
                    if (Vector2.Distance(transform.position, Player.position) < attackRadius)
                    {

                        //if player is close decide which attack to perform
                        state = 4;



                    }
                    

                }
                else if (state == 1)
                {
                    //fire laser
                    Vector2 targetDir = (Player.position - transform.position).normalized;
                    rb.transform.up = Vector3.RotateTowards(transform.up, targetDir, laserRotateSpeed * Time.deltaTime, 1.0f);
                    fireLaser();
                    laserCounter -= Time.deltaTime;
                    if (laserCounter <= 0)
                    {
                        state = 2;
                        stopLaserStar();
                        laserCooldown = maxLaserCooldown;
                    }

                }
                else if (state == 2)
                {
                    //fire main laser and turret laser
                    laserCooldown -= Time.deltaTime;
                    if (laserCooldown <= 0)
                    {
                        state = 0;
                    }
                    //cooldown;
                }
                else if (state == 3)
                {
                    //bullet fire
                    rb.rotation += turretRotateSpeed* 1.5f* Time.deltaTime;
                    TurretFireTimer -= Time.deltaTime;
                    


                    //timer 0 stop turret
                    if (TurretFireTimer <= 0)
                    {

                        stopTurretsWave();
                        state = 5;
                        bulletFireCoolDownTimer = maxTurretBulletFireCoolDown;
                    }

                }
                else if (state == 4)
                {

                    if (Vector2.Distance(transform.position, Player.position) > attackRadius)
                    {

                        //if player is close decide which attack to perform
                        state = 0;
                        return;


                    }
                    //decide attack 

                    //startTurretsWave();
                        startLaserTrack();
                 
                    laserCounter = maxlaserCounter;
                    state = 1;

                    
                    
                }
                else if (state == 5)
                {
                   
                    //bullet fire cooldown
                    
                    bulletFireCoolDownTimer -= Time.deltaTime;
                    if (bulletFireCoolDownTimer <= 0)
                    {
                        state = 0;
                    }
                    
                }










            }
            
            
            
        }
    }


   void spawnFlock()
    {
        int randNum = Random.Range(0, spawnPoints.Length);
        int spawnID = Random.Range(0, 100);
        Vector2 dir = (transform.position - spawnPoints[randNum].position).normalized;
        if (spawnID < 28)
        {    
            spawnFlockRocket(spawnPoints[randNum], dir);
        }else if(spawnID< 57)
        {
            spawnFlockSniper(spawnPoints[randNum], dir);
        }
        else
        {
            spawnFlockGunner(spawnPoints[randNum], dir);
        }
        spawnCount += 1;
    }



   public void spawnFlockGunner(Transform spawnPoint,Vector2 direction)
    {
        FlockGunner g = pool.flockGunnerPool.Get();
        g.transform.position = Random.insideUnitCircle * 1 + (Vector2)spawnPoint.position;
        g.transform.up = direction;
    }

    public void spawnFlockSniper(Transform spawnPoint, Vector2 direction)
    {
        FlockSniper s = pool.flockSniperPool.Get();
        s.transform.position = Random.insideUnitCircle * 1 + (Vector2)spawnPoint.position;
        s.transform.up = direction;
    }

    public void spawnFlockRocket(Transform spawnPoint, Vector2 direction)
    {
        FlockRocket r = pool.flockRocketPool.Get();
        r.transform.position = Random.insideUnitCircle * 1 + (Vector2)spawnPoint.position;
        r.transform.up = direction;
    }

    public void disableShield()
    {
        shield.SetActive(false);
    }

    public void enableShield()
    {
        shield.SetActive(true);
    }
    public static Vector2 rotate(Vector2 v, float delta)
    {
        return new Vector2(
            v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
            v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
        );
    }

 
    void startLaserStar()
    {
        activateLaser();
        for (int i = 0; i < turretCount; i++)
        {
            if (turrets[i].isActiveAndEnabled == false)
            {
                continue;
            }
            turrets[i].activateLaser();
            turrets[i].state = 1;
            turrets[i].transform.up = (turrets[i].transform.position - transform.position).normalized;
        }
       
    }
    void startLaserTrack()
    {
        activateLaser();
        for (int i = 0; i < turretCount; i++)
        {
            if (turrets[i].isActiveAndEnabled == false)
            {
                continue;
            }
            turrets[i].activateLaser();
            turrets[i].state = 3;
            turrets[i].transform.up = (turrets[i].transform.position - transform.position).normalized;
        }

    }
    void startTurrets()
    {
        for (int i = 0; i < turretCount; i++)
        {
          
            turrets[i].state = 0;
        }
    }
    void startTurretsWave()
    {
        for (int i = 0; i < turretCount; i++)
        {
            turrets[i].angle = 0;
            turrets[i].up = turrets[i].transform.up;
            turrets[i].state = 2;
        }
    }
    void UpdatePhaseOne()
    {
        phase = 1;
        state = 0;
        disableLaser();
        stopLaserStar();
        stopTurrets();
        enableShockArea();
        for (int i = 0; i < turretCount; i++)
        {

            turrets[i].phase = 1;
        }
    }

    void stopTurrets()
    {
        for (int i = 0; i < turretCount; i++)
        {
            
            turrets[i].state = -1;
        }
    }

    void stopTurretsWave()
    {
        for (int i = 0; i < turretCount; i++)
        {
            turrets[i].transform.up = (turrets[i].transform.position - transform.position).normalized;
            turrets[i].state = -1;
        }
    }
    void stopLaserStar()
    {
        disableLaser();
        for (int i = 0; i < turretCount; i++)
        {
            if (turrets[i].isActiveAndEnabled == false)
            {
                continue;
            }
            turrets[i].disableLaser();
            turrets[i].state = -1;
        }
    }

    private void fireLaser()
    {
        points[0] = gameObject.transform.position;
        hit = Physics2D.Raycast(gameObject.transform.position, gameObject.transform.up, laserLength, laserLayer);
        if (hit)
        {
            points[1] = hit.point;
            if (hit.transform.gameObject.CompareTag("Player"))
            {
                playerHealth.TakeDamage(laserDamage);
                Rigidbody2D playerRb = hit.transform.gameObject.GetComponent<Rigidbody2D>();
                playerRb.AddForce(transform.up * laserPushForce, ForceMode2D.Impulse);
            }else if (hit.transform.gameObject.layer == 6)
            {
                hit.transform.gameObject.GetComponent<Enemy>().takeDamage(laserDamage);
            }
        }
        else
        {
            points[1] = gameObject.transform.position + gameObject.transform.up.normalized * laserLength;

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



    protected Vector2 GetObstacleAvoidVector()
    {
        Vector2 avoidanceVector = Vector2.zero;
        List<Planet> planets = sol.planetarySystemList[sol.getClosestPlanet(rb.transform.position).PlanetarySystemIndex];
        int count = planets.Count;
        Vector2 dist;
        for (int i = 0; i < count; i++)
        {
            dist = (rb.transform.position - planets[i].transform.position);
            if (dist.sqrMagnitude < planets[i].avoidRadius * planets[i].avoidRadius*2)
            {
                avoidanceVector += (Vector2)dist.normalized * evasionPower;
            }
        }
        if (transform.position.sqrMagnitude < sol.sunAvoidRadius * sol.sunAvoidRadius*2)
        {
            avoidanceVector += (Vector2)transform.position.normalized * evasionPower;
        }
       

        return avoidanceVector;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerBullet"))
        {
            takeDamage(stats.PlayerBulletDamage);
        }
    }

    void updateHealthbar()
    {
        healthBar.transform.localScale = new Vector3(health / MaxHealth, 1, 1);
    }


  

    public override void takeDamage(float damage)
    {
        if (ShieldOn)
        {
            return;
        }
        health -= damage;
        updateHealthbar();
        if (health / MaxHealth <= 0.75 && phase==0)
        {
            UpdatePhaseOne();
        }
        if (health <= 0)
        {
            onDeath();
        }
    }
    public void KillFlock()
    {
       FlockingEnemy[] e= GameObject.FindObjectsOfType<FlockingEnemy>();
        int len = e.Length;
        for(int i = 0; i < len; i++)
        {
            e[i].onDeath();
        }
        
    }
    public override void onDeath()
    {
        KillFlock();
        healthBar.gameObject.SetActive(false);
        Destroy(gameObject);
    }

    void enableShockArea()
    {
        shockArea.SetActive(true);
    }

    void disableShockArea()
    {
        shockArea.SetActive(false);
    }
}
