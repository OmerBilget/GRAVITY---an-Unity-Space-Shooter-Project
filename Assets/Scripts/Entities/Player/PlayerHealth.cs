using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    float maxHealth;
    float health;
    float maxShield;
    float shield;
    public float shieldResistanceMultiplier;
    public float healthResistanceMultiplier;
    private float timer;
    public float damageTimer;
    public float shieldRegenRate;
    public RectTransform healthBar;
    public RectTransform shieldBar;
    public Stats stats;
    // Start is called before the first frame update
    void Start()
    {
        maxHealth = stats.playerMaxHealth;
        health = maxHealth;
        maxShield = stats.playerMaxShield;
        shield = maxShield;

        damageTimer = stats.playerDamageTimer;
        shieldRegenRate = stats.playerShieldRegenRate;
    }

    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime*60;
            return;
        }
        if (health < 0)
        {
            return;
        }
        shield += shieldRegenRate*Time.deltaTime;
        if (shield > maxShield)
        {
            shield = maxShield;
        }
        updateShieldBar();
    }

    public void TakeDamage(float damage)
    {
        if (shield > 0)
        {
            if (shield < damage)
            {
                float remainingDamage = damage - shield;
                shield = 0;
                health -= remainingDamage;
                if (health < 0)
                {
                    health = 0;
                    onDeath();
                }
            }
            else
            {
                shield -= damage;
            }
        }
        else
        {
            health -= damage;
            if (health < 0)
            {
                health = 0;
                onDeath();
            }
        }
        timer = damageTimer;
        updateHealthbar();
        updateShieldBar();

    }
    
    void onDeath()
    {
        //gameObject.SetActive(false);
    }

    void updateHealthbar()
    {
        healthBar.transform.localScale = new Vector3(health / maxHealth, 1, 1);
    }

    void updateShieldBar()
    {
        shieldBar.transform.localScale = new Vector3(shield / maxShield, 1, 1);
    }
}
