using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
public class BulletHandler : MonoBehaviour
{
    public SolarSystem sol;
    ObjectPool<Bullet> bulletPool;
    public Bullet bulletPrefab;
   
    public float bulletSpeed = 0.00008f;
    public float bulletSpeedRandomness = 0.0003f;
    public float spreadAngle = 5f;
    private float cooldown;
    public int maxBulletCount = 1000;
    public ParticleSystem muzzleflash;
    public ParticleSystem impactParticle;
   
    public Stats stats;
    private bool weaponPressed = false;
    float i = 0;
    // Start is called before the first frame update
    private void Awake()
    {
        bulletPool = new ObjectPool<Bullet>(CreateBullet, TakeFromPoolBullet, ReturnToPoolBullet, null, true, maxBulletCount, maxBulletCount);
    }

    private void Start()
    {
        spreadAngle = stats.PlayerMaxBulletSpread;
        bulletSpeed = stats.PlayerBulletInýtialSpeed;
        cooldown = stats.PlayerBulletCooldown;
    }
    public void enableWeapon()
    {
        muzzleflash.Play();
      
        weaponPressed = true;
    }
    public void disableWeapon()
    {
        muzzleflash.Stop();
        weaponPressed = false;
    }
    private void ReturnToPoolBullet(Bullet obj)
    {

        obj.gameObject.SetActive(false);
        obj.enabled = false;
    }

    private void TakeFromPoolBullet(Bullet obj)
    {
        obj.transform.position = transform.position;
        obj.gameObject.SetActive(true);
        obj.enabled = true;

        obj.velocity = RotateVector(transform.up, UnityEngine.Random.Range(-spreadAngle, spreadAngle));
        obj.speed = bulletSpeed+UnityEngine.Random.Range(-bulletSpeedRandomness,bulletSpeedRandomness);
        obj.gameObject.transform.up = obj.velocity;
        
    }
    public Vector2 RotateVector(Vector2 v, float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        float _x = v.x * Mathf.Cos(radian) - v.y * Mathf.Sin(radian);
        float _y = v.x * Mathf.Sin(radian) + v.y * Mathf.Cos(radian);
        return new Vector2(_x, _y);
    }



    private Bullet CreateBullet()
    {
        Bullet newBullet = Instantiate(bulletPrefab);
        newBullet.pool = bulletPool;
        newBullet.sol = sol;
        newBullet.impactPrefab = impactParticle;
        return newBullet;
    }

   

    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetMouseButtonDown(0))
        {
            muzzleflash.Play();
            playerEnergy.bulletFire();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            muzzleflash.Stop();
        }
        */
        if (weaponPressed)
        {
          
            if (i <= 0 && bulletPool.CountActive < maxBulletCount)
            {

                bulletPool.Get();
                i = cooldown;
            }
            
        }
      
        if (i > 0)
        {
            i -= Time.deltaTime*60;
        }
        
    }
}
