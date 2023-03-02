using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLaser : MonoBehaviour
{

    public int count;
    LineRenderer lineRenderer;
    public Transform origin;
    RaycastHit2D hit;
    Vector3[] points = new Vector3[100];
    public float maxLen = 100;
    public float reflectCount = 25;
    Ray ray = new Ray();
    Vector3 hitPoint;
    bool laseractive = false;
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
    }

    // Update is called once per frame
    void Update()
    {
      

  

        if (Input.GetMouseButtonDown(1))
        {
            enableLaser();
            laseractive = true;
        }

        if (Input.GetMouseButtonUp(1))
        {
            disableLaser();
            laseractive = false;
        }
    }

    void enableLaser()
    {
        lineRenderer.enabled = true;
    }

    void disableLaser()
    {
        lineRenderer.enabled = false;
    }

    private void FixedUpdate()
    {
        if (laseractive)
        {
            updateLaser();
        }
    }
    void updateLaser()
    {
            points[0] = origin.position;
            hit = Physics2D.Raycast(origin.position, origin.up, maxLen, ~(1 << 7));
            if (hit)
            {
                points[1] = hit.point;
            }
            else
            {
                points[1] = origin.position+origin.up * maxLen;
              
            }
            
     
        lineRenderer.SetPositions(points);
    }
    
    Vector3 getNextPosition(Transform origin,Vector2 direction)
    {
        Vector3 nextPoint;
        hit = Physics2D.Raycast(origin.position, direction, maxLen, ~(1 << 7));
        if (hit)
        {
            nextPoint = hit.point;
        }
        else
        {
            nextPoint = origin.position + origin.up * maxLen;
        }
        return nextPoint;
    }

    Vector2 getReflectionDirection(Vector2 direction,Vector2 axis)
    {
        axis.Normalize();
        return direction - 2 * Vector2.Dot(direction, axis) * axis;
    }
}
