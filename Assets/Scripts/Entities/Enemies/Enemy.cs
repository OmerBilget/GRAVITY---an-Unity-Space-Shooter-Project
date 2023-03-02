using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public float health;
    public float MaxHealth;
    public Transform target;
    public SolarSystem sol;
    public Transform Player;
    public PlayerHealth playerHealth;
    public abstract void takeDamage(float damage);
    public abstract void onDeath();

}
