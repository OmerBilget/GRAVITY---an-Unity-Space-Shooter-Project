using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : SpaceObject
{
    public GameObject centerOfOrbit;
    [SerializeField]
    public double orbitalSpeed=0.001f;
    public double angle = 0;
    public float orbitRadius=100.0f;
    public float rotateSpeed = 0.01f;
    public float rotateAngle = 0;
    public bool tidalLocked = false;
    public int PlanetarySystemIndex = 0; 
    public CircleCollider2D col;
    Rigidbody2D rb;
    private float PIx2 = Mathf.PI * 2;
    // Start iscalled before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (centerOfOrbit != null)
        {
            orbitRadius = (centerOfOrbit.transform.position - transform.position).magnitude;
        }
        radius = col.radius;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        //move
        Vector2 targetPosition = (Vector2)centerOfOrbit.transform.position + (rotate(Vector2.right,(float)angle)*orbitRadius);
        angle += orbitalSpeed*Time.deltaTime;
        angle %= PIx2;
        rb.MovePosition(targetPosition);
        /*
        //rotate 
        if (tidalLocked)
        {
            float angle = Vector2.Angle(transform.position, centerOfOrbit.transform.position);
            rb.SetRotation(angle);
               
        }
        else
        {
            rb.rotation += rotateSpeed * Time.deltaTime;
        }
        */

    }
    public static Vector2 rotate(Vector2 v, float delta)
    {
        return new Vector2(
            v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
            v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
        );
    }

}
