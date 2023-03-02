using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class FlockingEnemy : Enemy
{
    private Vector2 currentVelocity;
    //power of avoiding obstacle 
    protected float evasionPower;
    //steering power towards target
    protected float flockingPower;
    protected float steeringPower;
    protected float accelerationPower;
    protected float maxVelocity;
    protected float driveFactor;
    protected float smoothTime;
    protected float approachRadius;
    protected float malfunctionLimit;
    protected float malfunctionCounter;
    protected float attackRadius;
    protected float defaultRotateSpeed;
    protected float smoothDampAmount=20.0f;
    protected float mass;

    public float difficultyMultiplier=1;
    public float waveMultiplier=1;
    //nearby flock range
    protected float nRange;

    protected Collider2D agentCollider;
    protected Rigidbody2D rb;

    protected Vector2 velocity;
    protected Vector2 headingDirection;
    protected Vector2 tmp;
    public int[] weights = new int[] { 4, 7, 1 };
    protected Vector2 flockVelocity;
    public Collider2D AgentCollider { get { return agentCollider; } }
    public EnemyPool pool;
    Vector2 tmpvelocity;

    protected List<Transform> context = new List<Transform>();
    protected List<Transform> nList;
    protected Collider2D[] contextCollider = new Collider2D[10];

    protected Planet ClosestPlanet;
    protected float closestPlanetUpdateInterval = 3.0f;
    protected float planetUpdateTimer=0;


    protected int layerMask = (1 << 6);
    protected float timer = 0;
    protected float maxTimer=3;
    public Boss Boss;
    public Stats Stats;
    protected virtual void Start()
    {
        agentCollider = GetComponent<Collider2D>();
      
        rb = GetComponent<Rigidbody2D>();
        velocity = Vector2.zero;
        headingDirection = Vector2.up;
        
        accelerationPower = 0.0f;
        approachRadius = 1f;
        steeringPower = 9f;

        maxVelocity = 8.0f;
        evasionPower = 0.1f;
        driveFactor = 10.0f;
        nRange = 0.15f;
        ClosestPlanet = sol.getClosestPlanet(transform.position);
       
        
    }

    

    protected virtual void  Update()
    {

       
        //Vector2 tmpvelocity = getFlockMove(nRange) + GetObstacleAvoidVector() +SteeringToTarget(velocity,target);
        if (timer <= 0)
        {
            timer = maxTimer;
            flockVelocity = getFlockMove(nRange);
        }
        else
        {
            timer -= Time.deltaTime;
        }
        tmpvelocity = flockVelocity;
        tmpvelocity +=SteeringToTargetApproach(tmpvelocity, target);
        tmpvelocity += GetObstacleAvoidVector();
        tmpvelocity *= driveFactor;
        if (tmpvelocity.magnitude * driveFactor > maxVelocity)
        {
            tmpvelocity = tmpvelocity.normalized * maxVelocity;
        }
     
        Move(tmpvelocity );

    }

   




    




    protected void Move(Vector2 newTargetVelocity)
    {


   
        velocity = Vector2.SmoothDamp(velocity, newTargetVelocity, ref tmp, smoothDampAmount * Time.deltaTime);
        /*
        if (velocity.magnitude > maxVelocity)
        {
            velocity = velocity.normalized * maxVelocity;
        }
        */
        rb.velocity = velocity;
    }

    protected void rotateTowardsTarget(Vector3 target,float rotateSpeed)
    {
        headingDirection = Vector3.RotateTowards(headingDirection, target, rotateSpeed, 1.0f);
        transform.up = headingDirection;
    }

    protected void InstantMove(Vector2 newTargetVelocity)
    {
        velocity += newTargetVelocity;
        rb.velocity = velocity;
    }

     protected Vector2 getFlockMove(float range)
    {
        nList = GetNearbyObject(this, range);
        Vector2 flockVector = (AlignmentMove(this, nList)*weights[0] + AvoidanceMove(this, nList)*weights[1])/(weights[0]+weights[1]);
        return flockVector*flockingPower;
    }

    //get obstacle avoid vector
    protected Vector2 GetObstacleAvoidVector()
    {
        Vector2 avoidanceVector = Vector2.zero;
        List<Planet> planets = sol.planetarySystemList[ClosestPlanet.PlanetarySystemIndex];
        int count = planets.Count;
        Vector2 dist;
        for (int i = 0; i <count; i++)
        {
            dist = (rb.transform.position - planets[i].transform.position);
            if (dist.sqrMagnitude < planets[i].avoidRadius * planets[i].avoidRadius)
            {
                avoidanceVector += (Vector2)dist.normalized * evasionPower;
            }
        }
        if (transform.position.sqrMagnitude < sol.sunAvoidRadius * sol.sunAvoidRadius)
        {
            avoidanceVector += (Vector2)transform.position.normalized * evasionPower;
        }
        
        dist = (rb.transform.position - Boss.transform.position);
        if (dist.sqrMagnitude < 500)
        {
            avoidanceVector += (Vector2)dist.normalized * evasionPower;
        }
        
        return avoidanceVector;
    }

    protected List<Transform> GetNearbyObject(FlockingEnemy agent, float radius)
    {
        context.Clear();

        int num=Physics2D.OverlapCircleNonAlloc(agent.transform.position, radius,contextCollider, layerMask); //flocking Enemy

        for(int i = 0; i < num; i++)
        {
            if (contextCollider[i] != agent.AgentCollider)
            {
                context.Add(contextCollider[i].transform);
            }
        }
        /*
        foreach (Collider2D c in contextCollider)
        {
            if (c != agent.AgentCollider)
{
                context.Add(c.transform);
            }
        }
        */
        return context;
    }

    protected Vector2 SteeringToTarget(Vector2 currentVelocity, Transform target)
    {
        Vector2 steeringVector = Vector2.zero;
        if (target == null)
        {
            Debug.Log("error");
            return steeringVector;

        }
       
        Vector2 distanceVector = target.position -transform.position;
        Vector2 desiredVelocity = distanceVector.normalized * currentVelocity.magnitude;
        steeringVector = desiredVelocity - currentVelocity;
        steeringVector = (steeringVector * steeringPower) / 100.0f;
        return steeringVector;
    }
    protected Vector2 SteeringToTargetApproach(Vector2 currentVelocity, Transform target)
    {
        Vector2 steeringVector = Vector2.zero;
        if (target == null)
        {
          
            return steeringVector;

        }

        Vector2 distanceVector = target.position - transform.position;
        float distance = distanceVector.sqrMagnitude;
        float approach = approachRadius * approachRadius;
        Vector2 desiredVelocity;
        if (distance<approach)
        {
            desiredVelocity = -distanceVector.normalized * currentVelocity.magnitude;
            steeringVector = desiredVelocity - currentVelocity;
            steeringVector = (steeringVector * steeringPower*4) / 100.0f;
            
        }
        else
        {
            desiredVelocity = distanceVector.normalized * currentVelocity.magnitude;
            steeringVector = desiredVelocity - currentVelocity;
            steeringVector = (steeringVector * steeringPower) / 100.0f;
           
        }
        return steeringVector;

    }
    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    protected Vector2 AccelerateTowardTarget(Transform target)
    {
        if (target == null)
        {
            return Vector2.zero;
        }
        float avaibleSpeed = maxVelocity - velocity.magnitude;

        Vector2 result = (Vector2)(target.position - transform.position).normalized * (accelerationPower * (avaibleSpeed / maxVelocity));
        //Debug.Log(accelerationPower);
        return result;
    }

    protected Vector2 Accelerate()
    {

        float avaibleSpeed = maxVelocity - velocity.sqrMagnitude;
        return (avaibleSpeed / maxVelocity) * accelerationPower * velocity;
    }


    protected Vector2 AvoidanceMove(FlockingEnemy agent, List<Transform> context)
    {
        //no neighbor 
        if (context.Count == 0)
        {
            return Vector2.zero;
        }

        Vector2 avoidanceVelocity = Vector2.zero;
        int avoidnum = 0;
        foreach (Transform item in context)
        {

            avoidnum += 1;
            avoidanceVelocity += (Vector2)(agent.transform.position - item.position);
        }
        if (avoidnum > 0)
        {
            avoidanceVelocity /= avoidnum;
        }
        //avoidanceVelocity = Vector2.SmoothDamp(agent.transform.up, avoidanceVelocity, ref currentVelocity, smoothTime);
        return avoidanceVelocity;
    }

    protected Vector2 AlignmentMove(FlockingEnemy agent, List<Transform> context)
    {
        //no neighbor maintain current alignment
        if (context.Count == 0)
        {
            return agent.transform.up;
        }

        Vector2 alignmentVelocity = Vector2.zero;
        foreach (Transform item in context)
        {
            alignmentVelocity += (Vector2)item.transform.up;
        }
        alignmentVelocity /= context.Count;

        //create offset from position

        return alignmentVelocity;
    }

    protected Vector2 CohesionMove(FlockingEnemy agent, List<Transform> context)
    {
        if (context.Count == 0)
        {
            return Vector2.zero;
        }

        Vector2 cohesionVelocity = Vector2.zero;
        foreach (Transform item in context)
        {
            cohesionVelocity += (Vector2)item.position;
        }
        cohesionVelocity /= context.Count;

        //create offset from position

        cohesionVelocity -= (Vector2)agent.transform.position;
        cohesionVelocity = Vector2.SmoothDamp(agent.transform.up, cohesionVelocity, ref currentVelocity, smoothTime);
        return cohesionVelocity;
    }

    
}


