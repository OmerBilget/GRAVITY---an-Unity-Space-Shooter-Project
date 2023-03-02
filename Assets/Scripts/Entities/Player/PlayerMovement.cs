using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update
    Rigidbody2D rb;
    Vector2 heading = Vector2.up;
    public float acceleration = 0.1f;
    public float reverseAcceleration = 0.02f;
    public float maxSpeed;
    private bool hyperFlight = false;
    public float HFacceleration;
    public float HFmaxSpeed;
 
    public float maxHFtime = 100;
    public float HFdrainRate = 0.1f;
    public float HFrotateSpeed;
    public float mass = 1;
    public bool landed = false;
    Queue<Vector2> averageSpeedArray = new Queue<Vector2>(10);
  
    
    // 0 normal flight
    // 1 hyperflight

   

    Vector2 targetHeading;
    Vector2 dist;
    [Range(0.0f,3.0f)]
    public float rotateAngle=0.1f;

    private bool[] inputThrottle = new bool[] { false, false };
    private bool keyPressed;
    [SerializeField]
    public SolarSystem sol;
    public ParticleSystem particle;
    bool thrusting = false;
    public Planet LandedPlanet;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        List<Vector3> pos = new List<Vector3>();
        
        for(int i = 0; i < 10; i++)
        {
            averageSpeedArray.Enqueue(Vector2.zero);
        }
        heading = Vector2.up;
        targetHeading = Vector2.up;
        keyPressed = false;
      
    }
    public void EnableThrust()
    {
        inputThrottle[0] = true;
        keyPressed = true;
        if (thrusting == false)
        {
            thrusting = true;
            particle.Play();
        }
       
    }

    public void DisableThrust()
    {
     
        inputThrottle[0] = false;
        keyPressed = false;
        
    }
    


  
    public void setTargetHeading(Vector2 heading)
    {
        targetHeading = heading;
    }
    public void EnableHyperFlight()
    {
        hyperFlight = true;
        if (thrusting == false)
        {
            thrusting = true;
            particle.Play();
        }
    }

    public void DisableHyperflight()
    {
        hyperFlight = false;
    }
    
    // Update is called once per frame
   
    private void updateAvgSpeedArray(Vector2 vel)
    {
        averageSpeedArray.Enqueue(vel);
        if (averageSpeedArray.Count > 10)
        {
            averageSpeedArray.Dequeue();
        }
    }
    public float getAvgSpeed()
    {
        Vector2[] avgArray = averageSpeedArray.ToArray();
        float avgSpeed = 0;
        for(int i = 0; i < 10; i++)
        {
            avgSpeed += avgArray[i].magnitude;
        }
        avgSpeed /= 10;
        return avgSpeed;
    }
    private void FixedUpdate()
    {
        

        if (hyperFlight)
        {
            rb.AddForce(60 * HFacceleration * Time.deltaTime * heading);
            if (rb.velocity.magnitude > HFmaxSpeed)
            {
                rb.velocity = rb.velocity.normalized * HFmaxSpeed;
            }
            
                heading = Vector3.RotateTowards(heading, targetHeading, HFrotateSpeed * Time.deltaTime * 60, 1.0f).normalized;
                rb.transform.up = heading;
         
              
            
       
         
        }
        else
        {
            if (inputThrottle[0])
            {

                rb.AddForce(60 * acceleration * Time.deltaTime * heading);
          

            }
            else
            {
                if (thrusting == true)
                {
                    thrusting = false;
                    particle.Stop();
                }
            }

            float speed = rb.velocity.magnitude;

            rb.AddForce(sol.getGravity(transform.position, mass) * Time.deltaTime * 60);


            if (speed > maxSpeed)
            {
                rb.AddForce(-rb.velocity.normalized * (speed / 2));
            }

          
                heading = Vector3.RotateTowards(heading, targetHeading, rotateAngle * Time.deltaTime * 60, 1.0f).normalized;
                rb.transform.up = heading;
            
            
                
              


            
            

        }

        updateAvgSpeedArray(rb.velocity);
        
    }

    public Vector2 RotateVector(Vector2 v, float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        float _x = v.x * Mathf.Cos(radian) - v.y * Mathf.Sin(radian);
        float _y = v.x * Mathf.Sin(radian) + v.y * Mathf.Cos(radian);
        return new Vector2(_x, _y);
    }
}
