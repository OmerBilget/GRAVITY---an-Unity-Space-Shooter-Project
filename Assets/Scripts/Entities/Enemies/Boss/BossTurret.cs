using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTurret : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float laserDamage = 0.01f;
    public float laserLength = 100.0f;
    public float maxHealth = 1000;
    public float health;
    public Transform target;
    public Transform parent;
    public float orbitRadius;
    public float orbitAngle;
    public float targetAngle;
    public int id;
    public Rigidbody2D rb;
    public float moveSpeed;
    public float velocity;
    public bool lockedRotation = false;
    public EnemyPool pool;
    public float turretCooldown = 0.1f;
    public float turretCooldownPhaseOne = 0.07f;
    float turretCoolDownTimer;
    public float spread = 0.1f;
    public float spreadWave = 0.05f;
    public Transform spawnPoint;
    public float maxRotateWaveAngle = 1f;
    public float laserTrackSpeed=0.1f;
    public Stats stats;

    public bool active = true;

    public float maxRebuildTimer = 20.0f;
    float rebuildTimer = 0.0f;
    Vector3[] points = new Vector3[2];
    RaycastHit2D hit;

    

    public int state = -1;
    public float angle = 0;
    public float rotateSpeed = 0.1f;
   
    // 0 idle
    // 1 laser fire
    // 2 bullet fire

    public int phase = 0;
    public Vector2 up;
    private int laserLayer = ~((1 << 6) | (1 << 9) | (1 << 10) | (1 << 13) | (1 << 15) | (1 << 8));
    // Start is called before the first frame update

    private void OnEnable()
    {
     

        health = maxHealth;

    }


    void Start()
    {
        health = maxHealth;
        spread = stats.bulletSpreadPhaseZero;
        spreadWave = stats.bulletSpreadPhaseOne;
        laserTrackSpeed = stats.laserTrackSpeed;
        maxRotateWaveAngle = stats.turretMaxAngleWave;
        turretCooldown = stats.bulletCoolDownPhaseZero;
        turretCooldownPhaseOne = stats.bulletCoolDownPhaseOne;
        rotateSpeed = stats.turretRotateSpeedWave;
        disableLaser();
    }

    // Update is called once per frame
    void Update()
    {
            if (active == false)
            {
                rebuildTimer -= Time.deltaTime;
                if (rebuildTimer <= 0)
                {
                      active = true; 
                }
                
               
               return;
            }
            if (state == 0)
            {
                 //direct turret fire


                //idle
                if (turretCoolDownTimer <= 0)
                {

                    BossBullet bullet = pool.bossBulletPool.Get();
                    bullet.transform.position = spawnPoint.position;
                    //bullet.velocity = (target.position - transform.position).normalized * bullet.speed;
                    bullet.velocity = rotate(transform.up * bullet.speed, UnityEngine.Random.Range(-spread, spread));
                    turretCoolDownTimer = turretCooldown;
                }
                else
                {
                    turretCoolDownTimer -= Time.deltaTime;
                }
                //state = 2;
            }
            else if (state == 1)
            {
                //direct fire
                fireLaser();
            }
            if (state == 2)
            {

                
                //wave spread

                transform.up = rotate((transform.position - parent.position).normalized, Mathf.Sin(angle)*maxRotateWaveAngle);
                angle += Time.deltaTime *rotateSpeed;
                //idle
                if (turretCoolDownTimer <= 0)
                {

                    BossBullet bullet = pool.bossBulletPool.Get();
                    bullet.transform.position = spawnPoint.position;
                    //bullet.velocity = (target.position - transform.position).normalized * bullet.speed;
                    bullet.velocity = rotate(transform.up * bullet.speed, UnityEngine.Random.Range(-spreadWave, spreadWave));
                    turretCoolDownTimer = turretCooldownPhaseOne;
                }
                else
                {
                    turretCoolDownTimer -= Time.deltaTime;
                }
                //state = 2;
            }
            else if (state == 3)
             {
                  
                  transform.up = Vector3.RotateTowards(transform.up, (target.position-transform.position).normalized,laserTrackSpeed*Time.deltaTime, 1.0f);
                  fireLaser();
             }
            
        
  
    }

    public void fireLaser()
    {
        points[0] = gameObject.transform.position;
        hit = Physics2D.Raycast(gameObject.transform.position, gameObject.transform.up, laserLength, laserLayer);
        if (hit)
        {
            points[1] = hit.point;
            if (hit.transform.gameObject.CompareTag("Player"))
            {
                hit.transform.gameObject.GetComponent<PlayerHealth>().TakeDamage(laserDamage);
                
            }
        }
        else
        {
            points[1] = gameObject.transform.position + gameObject.transform.up.normalized * laserLength;

        }
        lineRenderer.SetPositions(points);
    }

    public void activateLaser()
    {
        lineRenderer.enabled = true;
    }

    public void disableLaser()
    {
        lineRenderer.enabled = false;
    }


    public static Vector2 rotate(Vector2 v, float delta)
    {
        return new Vector2(
            v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
            v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
        );
    }

    public void takeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {

        }
    }

    public void destructed()
    {
        active = false;
        rebuildTimer = maxRebuildTimer;
        disableLaser();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerBullet"))
        {
            takeDamage(stats.PlayerBulletDamage);
        }
    }
}

