using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
public class EnemySpawner : MonoBehaviour
{
    public SolarSystem sol;
    public EnemyPool pool;


    public List<FlockingEnemy> enemyTypes=new List<FlockingEnemy>();
    public FlockingEnemy prefab;
    List<FlockingEnemy> agentList = new List<FlockingEnemy>();

    public int[] maxCount = new int[4];
    Vector2 tmpVec;
    const float Density = 0.08f;
    public Transform spawnPoint1;
    public Transform spawnPoint2;

    float squareAvoidanceRadius;
    public int[] enemyCountArray = new int[4];
    public float SquareAvoidanceRadius { get { return squareAvoidanceRadius; } }
    public float spawn_radius=20.0f;

    // Start is called before the first frame update
    void Start()
    {
        //spawnSwarm(spawn_radius, spawnPoint1.position, enemyCountArray[0]);
    }

    // Update is called once per frame
    void Update()
    {
        //*
        if (pool.flockGunnerPool.CountActive <maxCount[0])
        {
            FlockGunner g=pool.flockGunnerPool.Get();
            g.transform.position = Random.insideUnitCircle * 1 +(Vector2)spawnPoint1.position;
            g.transform.rotation = Quaternion.Euler(Vector3.forward * Random.Range(0.0f, 360.0f));
        }
        
        if (pool.flockSniperPool.CountActive < maxCount[1])
        {
            FlockSniper s = pool.flockSniperPool.Get();
            s.transform.position = Random.insideUnitCircle * 1 + (Vector2)spawnPoint1.position;
            s.transform.rotation = Quaternion.Euler(Vector3.forward * Random.Range(0.0f, 360.0f));
        }
        
        if (pool.flockRocketPool.CountActive < maxCount[2])
        {
            FlockRocket r = pool.flockRocketPool.Get();
            r.transform.position = Random.insideUnitCircle * 1 + (Vector2)spawnPoint1.position;
            r.transform.rotation = Quaternion.Euler(Vector3.forward * Random.Range(0.0f, 360.0f));
        }
    }

    void spawnSwarm(float radius,Vector2 position, int count)
    {

        for (int i = 0; i < count; i++)
        {

            FlockGunner g=pool.flockGunnerPool.Get();
            g.transform.position = Random.insideUnitCircle * radius + position;
            g.transform.rotation = Quaternion.Euler(Vector3.forward * Random.Range(0.0f, 360.0f));
           // Vector2 v=Random.insideUnitCircle*radius+position,Quaternion.Euler(Vector3.forward * Random.Range(0.0f, 360.0f));





        }
        
    }

}