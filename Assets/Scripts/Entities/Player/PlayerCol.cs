using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCol : MonoBehaviour
{


    public PlayerHealth PlayerHealth;
    public PlayerMovement playerMovement;
    public Rigidbody2D rb;



    public float crashMinSpeed = 2;
    float crashSpeed;
    public float CrashDamageMultiplier=1;
    public float CrashMinDamage = 1;
    bool onContactPlanet = false;
    bool land = false;

    float landCounter;
    public float maxLandCounter=4;
    public Stats Stats;

    private void Start()
    {
        landCounter = maxLandCounter;
    }
    private void FixedUpdate()
    {
        if (onContactPlanet == false && land==true)
        {
            landCounter -= Time.deltaTime;
            if (landCounter <= 0)
            {
                land = false;
                Debug.Log("LIFTOFF");
                playerMovement.landed = false;
            }
        }
       
    }

    // Start is called before the first frame update
    public void OnCollisionEnter2D(Collision2D collision)
    {
        
           
        if (collision.gameObject.CompareTag("TerrestialPlanet"))
        {

            crashSpeed = playerMovement.getAvgSpeed();
            Debug.Log(crashSpeed);
            if (crashSpeed> crashMinSpeed )
            {
                PlayerHealth.TakeDamage(CrashMinDamage+CrashDamageMultiplier*(crashSpeed-crashMinSpeed));
            }else
            {
                landCounter = maxLandCounter;
                if (land == false)
                {
                    Debug.Log("LAND");
                    land = true;
                    playerMovement.landed = true;                
                    onContactPlanet = true;
                    playerMovement.LandedPlanet = collision.gameObject.GetComponent<Planet>();
                 
                }
               
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("TerrestialPlanet"))
        {

            onContactPlanet = false;
        
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("EnemyBullet"))
        {
            PlayerHealth.TakeDamage(Stats.flockGunner.damage);
        }
        if (collision.gameObject.CompareTag("BossBullet"))
        {
            PlayerHealth.TakeDamage(Stats.bossStats.turretBulletDamage);
        }

    }

  
}
