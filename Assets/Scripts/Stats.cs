using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Stats",menuName="GlobalSettings")]
public class Stats : ScriptableObject
{



    [Header("Player")]
    [Space(10)]
    public float playerMaxHealth;
    public float playerMaxShield;
    [Space(10)]
    public float playerShieldRegenRate;
    public float playerDamageTimer;
    [Space(10)]
    public float PlayerBulletDamage;
    public float PlayerBulletLife;
    public float PlayerBulletCooldown;
    public float PlayerBulletInýtialSpeed;
    public float PlayerMaxBulletSpeed;
    public float PlayerMaxBulletSpread;
    public float PlayerBulletAcceleration;
    [Space(10)]
    public float PlayerMineDamage;
    public int mineCount;
    public int maxMineCount;
    public float mineSpreadAngle;
    public float mineInitialVelocity;
    public float mineDeploySpeed;
    public float mineExplodeRadius;
    public float mineCoolDown;
    public float mineRegenTimer;
    public float mineFriction;
    public float mineLife;
    public float mineExplosionDelayTime;

    [Space(40)]
    [Header("FlockGunner")]
    [Space(10)]

    public FlockStats flockGunner;
    public float flockGunnerbulletSpeed;
    public float flockGunnerBulletLife;
    public float flockGunnerBulletSpread;

    [Space(40)]
    [Header("FlockSniper")]
    [Space(10)]
    

    public FlockStats flockSniper;
    public float laserFireTime;
    public float laserRotateSpeed;
    public float laserLength;
    public float maxAttackAngle;

    [Space(40)]
    [Header("FlockRocket")]
    [Space(10)]



    public FlockStats flockRocket;
    public float rocketSpreadAngle;
    public float rocketLife;
    public float rocketRotateSpeed;
    public float rocketMaxSpeed;
    public float rocketMalfunctionMax;
    public float RocketMalfunctionRate;
    public float RocketMaxHealth;


    [Space(40)]
    [Header("Boss")]
    [Space(10)]

    public BossStats bossStats;


    [System.Serializable]
    public class FlockStats
    {
        public float maxHealth;
        public float CoolDown;
        public float damage;
        public float attackRadius;
        public float approachRadius;
        public float malfunctionMax;
        public float flockingPower;
        public float driveSpeed;
        public float flockTimer;
        public float evasionPower;
        public float steeringPower;
        public float Nrange;
        public float mass;
        public float planetUpdateTimer;
        public int[] weights = new int[3];
        public float smoothTimer;
        public float smoothAmount;
        public float maxSpeed;
        public float defaultRotateSpeed;

    }


    [System.Serializable]
    public class BossStats
    {
        public float MaxHealth;
        public float turretBulletDamage;
        public int TurretCount;
        public float AttackRadius;
        public float approachRadius;
        public float moveSpeed;
        public float mainLaserDamage;
        public float turretLaserDamage;
        public float laserRotateSpeed;
        public float turretBulletFireRotateSpeed;
        public float turretBulletFireTime;
        public float laserFireTime;
        public float turretBulletFireCooldown;
        public float LaserCoolDown;
    }



    [Space(40)]
    [Header("BossTurret")]
    [Space(10)]

    public float bulletSpreadPhaseZero;
    public float bulletFireTimePhaseZero;
    public float bulletCoolDownPhaseZero;
    public float laserfireTimePhaseZero;

    [Space(10)]

    public float bulletSpreadPhaseOne;
    public float bulletFireTimePhaseOne;
    public float bulletCoolDownPhaseOne;
    public float laserfireTimePhaseOne;
    public float laserTrackSpeed;
    public float turretMaxAngleWave;
    public float turretRotateSpeedWave;



}
