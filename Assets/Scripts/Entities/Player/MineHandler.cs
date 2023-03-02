using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
public class MineHandler : MonoBehaviour
{
    public Mine minePrefab;
    public PlayerMovement playerMovement;
    ObjectPool<Mine> minePool;
    public int amount=5;
    public int maxMine=20;
    float ejectForce = 10;
    float spreadAngle = 30;
    public Rigidbody2D rb;
    public float initialVelocity = 20;
    public Stats Stats;
    // Start is called before the first frame update
    void Start()
    {
        amount = Stats.mineCount;
        maxMine = Stats.maxMineCount;
        ejectForce = Stats.mineDeploySpeed;
        spreadAngle = Stats.mineSpreadAngle;
        initialVelocity = Stats.mineInitialVelocity;
        minePool = new ObjectPool<Mine>(CreateMine,TakeFromMinePool,ReturnToMinePool,null,true,amount,maxMine);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Mine CreateMine()
    {
        Mine m = Instantiate(minePrefab);
        m.pool = minePool;
        return m;
    }

    private void TakeFromMinePool(Mine obj)
    {
        obj.transform.position = transform.position;
        obj.gameObject.SetActive(true);
        obj.enabled = true;
    }

    private void ReturnToMinePool(Mine obj)
    {
        obj.gameObject.SetActive(false);
        obj.enabled = false;
    }

    public void DeployMine()
    {
        Mine m;
        Vector3 pos;
        Vector2 playerDir=-rb.transform.up;
        Vector2 dir;
        for(int i = 0; i < amount; i++)
        {
            m= minePool.Get();
            pos = transform.position;
            dir = RotateVector(playerDir, spreadAngle * (i - amount / 2));
            m.transform.position = pos;
            m.rb.velocity = dir *  ejectForce;
        }
    }

    public Vector2 RotateVector(Vector2 v, float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        float _x = v.x * Mathf.Cos(radian) - v.y * Mathf.Sin(radian);
        float _y = v.x * Mathf.Sin(radian) + v.y * Mathf.Cos(radian);
        return new Vector2(_x, _y);
    }

}
