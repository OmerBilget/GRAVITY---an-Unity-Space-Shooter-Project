using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
public class Mine : MonoBehaviour
{

    float explodeRadius=3;
    public Rigidbody2D rb;
    Vector2 tmp;
    float smoothTime;
    float life = 1000;
    float timer;
    public ObjectPool<Mine> pool;
    public ParticleSystem explosionPrefab;
    float explosionDelayTime = 3;
    float explosionDelay = 3;
    int enemyLayerMask= 1<<6;
    Collider2D[] proximityEnemyList = new Collider2D[300];
    bool exploded = false;
    bool released = false;
    public Stats stats;
    ParticleSystem explosion;
    public ParticleSystem proximityDetectorPrefab;
    ParticleSystem proximityDetector;
    public float defaultSize;
    public float proximitySize;
    // Start is called before the first frame update
    void Start()
    {
        smoothTime = stats.mineFriction;
        life = stats.mineLife;
        explosionDelayTime = stats.mineExplosionDelayTime;
        explodeRadius = stats.mineExplodeRadius;
        explosion = Instantiate(explosionPrefab, rb.transform.position, Quaternion.identity);
        if (proximityDetector == null)
        {
            proximityDetector = Instantiate(proximityDetectorPrefab, rb.transform);
        }
        timer = life;
    }

    public void Explode()
    {
       
        //explosion.Play();
        exploded = true;
    }

    void ProximityDamage()
    {
        int size=Physics2D.OverlapCircleNonAlloc(rb.transform.position, explodeRadius, proximityEnemyList,enemyLayerMask);
        int c = 0;
        for(int i = 0; i < size; i++)
        {
            Enemy e = proximityEnemyList[i].gameObject.GetComponent<Enemy>();
            if (e != null)
            {
                c += 1;
                e.takeDamage(stats.PlayerMineDamage);
            }
        }
       
    }
    
    private void OnEnable()
    {
       
       
        timer = life;
        released = false;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (life <= 0)
        {
            Detonate();
        }
        else
        {
            life -= Time.deltaTime;
        }
        if (exploded && explosionDelay > 0)
        {
            explosionDelay -= Time.deltaTime;
            if (explosionDelay <= 0)
            {
                Detonate();
            }
        }
    }

    private void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Detonate();
    }

    private void Detonate()
    {
        explosion.transform.position = rb.transform.position;
        explosion.Play();
        ProximityDamage();
        explosionDelay = explosionDelayTime;
        exploded = false;
        if (released == false)
        {
            released = true;
            pool.Release(this);
        }
    }

    
}
