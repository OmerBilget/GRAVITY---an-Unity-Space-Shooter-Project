using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
public class EnemyPool : MonoBehaviour
{
    public SolarSystem sol;

    //Weapon pools
    public ObjectPool<FlockGunnerBullet> bulletPool;
    public ObjectPool<BossBullet> bossBulletPool;
    public ObjectPool<Rocket> rocketPool;
    //enemy type pools
    public ObjectPool<FlockGunner> flockGunnerPool;
    public ObjectPool<FlockSniper> flockSniperPool;
    public ObjectPool<FlockRocket> flockRocketPool;
    public int[] poolCount = new int[3] { 1000, 500, 500 };


    public FlockGunnerBullet bulletPrefab;
    public BossBullet bossBulletPrefab;
    public Rocket rocketPrefab;
    public FlockGunner flockGunnerPrefab;
    public FlockSniper flockSniperPrefab;
    public FlockRocket flockRocketPrefab;

    public GameObject target;
    public Transform player;
    public PlayerHealth PlayerHealth;
    public Boss Boss;

    public int maxBulletCount = 1000;

    int i = 0;
    // Start is called before the first frame update
    private void Awake()
    {

        bulletPool = new ObjectPool<FlockGunnerBullet>(CreateBullet, TakeFromPoolBullet, ReturnToPoolBullet, null, true, maxBulletCount, maxBulletCount);
        rocketPool = new ObjectPool<Rocket>(CreateRocket, TakeFromPoolRocket, ReturnToPoolRocket, null, true, maxBulletCount, maxBulletCount);

        flockGunnerPool = new ObjectPool<FlockGunner>(CreateFlockGunner,TakeFromPoolFlockGunner,ReturnToPoolFlockGunner,null,true,poolCount[0],poolCount[0]);
        flockSniperPool = new ObjectPool<FlockSniper>(CreateFlockSniper, TakeFromPoolFlockSniper, ReturnToPoolFlockSniper, null, true, poolCount[1], poolCount[1]);
        flockRocketPool = new ObjectPool<FlockRocket>(CreateFlockRocket, TakeFromPoolFlockRocket, ReturnToPoolFlockRocket, null, true, poolCount[2], poolCount[2]);

        bossBulletPool = new ObjectPool<BossBullet>(CreateBossBullet, TakeFromPoolBossBullet, ReturnToPoolBossBullet, null, true, maxBulletCount, maxBulletCount);
    }




    private FlockRocket CreateFlockRocket()
    {
        FlockRocket flockRocket = Instantiate(flockRocketPrefab);
        flockRocket.pool = this;
        flockRocket.sol = sol;
        flockRocket.Boss = Boss;
        flockRocket.target = target.transform;
        flockRocket.Player = player;
        flockRocket.playerHealth = PlayerHealth;
        return flockRocket;
    }


    private void ReturnToPoolFlockRocket(FlockRocket obj)
    {

        obj.gameObject.SetActive(false);
        obj.enabled = false;
    }

    private void TakeFromPoolFlockRocket(FlockRocket obj)
    {
        obj.gameObject.SetActive(true);
        obj.enabled = true;
    }







    private FlockSniper CreateFlockSniper()
    {
        FlockSniper flockSniper = Instantiate(flockSniperPrefab);
        flockSniper.pool = this;
        flockSniper.sol = sol;
        flockSniper.Boss = Boss;
        flockSniper.target = target.transform;
        flockSniper.Player = player;
        flockSniper.playerHealth = PlayerHealth;
        return flockSniper;
    }


    private void ReturnToPoolFlockSniper(FlockSniper obj)
    {

        obj.gameObject.SetActive(false);
        obj.enabled = false;
    }

    private void TakeFromPoolFlockSniper(FlockSniper obj)
    {
        obj.gameObject.SetActive(true);
        obj.enabled = true;
    }











    private FlockGunner CreateFlockGunner()
    {
        FlockGunner flockGunner = Instantiate(flockGunnerPrefab);
        flockGunner.pool = this;
        flockGunner.sol = sol;
        flockGunner.Boss = Boss;
        flockGunner.target = target.transform;
        flockGunner.Player = player;
        flockGunner.playerHealth = PlayerHealth;
        return flockGunner;
    }


    private void ReturnToPoolFlockGunner(FlockGunner obj)
    {

        obj.gameObject.SetActive(false);
        obj.enabled = false;
    }

    private void TakeFromPoolFlockGunner(FlockGunner obj)
    {
        obj.gameObject.SetActive(true);
        obj.enabled = true;
    }

    private FlockGunnerBullet CreateBullet()
    {
        FlockGunnerBullet newBullet = Instantiate(bulletPrefab);
        newBullet.pool = bulletPool;
        newBullet.sol = sol;
        return newBullet;
    }


    private void ReturnToPoolBullet(FlockGunnerBullet obj)
    {

        obj.gameObject.SetActive(false);
        obj.enabled = false;
    }

    private void TakeFromPoolBullet(FlockGunnerBullet obj)
    {
        obj.gameObject.SetActive(true);
        obj.enabled = true;
    }


    private BossBullet CreateBossBullet()
    {
        BossBullet newBullet = Instantiate(bossBulletPrefab);
        newBullet.pool = bossBulletPool;
        newBullet.sol = sol;
        return newBullet;
    }


    private void ReturnToPoolBossBullet(BossBullet obj)
    {

        obj.gameObject.SetActive(false);
        obj.enabled = false;
    }

    private void TakeFromPoolBossBullet(BossBullet obj)
    {
        obj.gameObject.SetActive(true);
        obj.enabled = true;
    }


    private Rocket CreateRocket()
    {
        Rocket newRocket = Instantiate(rocketPrefab);
        newRocket.pool = rocketPool;
        return newRocket;
    }


    private void ReturnToPoolRocket(Rocket obj)
    {

        obj.gameObject.SetActive(false);
        obj.enabled = false;
    }

    private void TakeFromPoolRocket(Rocket obj)
    {
        obj.gameObject.SetActive(true);
        obj.enabled = true;
    }
    public Vector2 RotateVector(Vector2 v, float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        float _x = v.x * Mathf.Cos(radian) - v.y * Mathf.Sin(radian);
        float _y = v.x * Mathf.Sin(radian) + v.y * Mathf.Cos(radian);
        return new Vector2(_x, _y);
    }




   
}
