using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleDrawer : MonoBehaviour
{

    public SolarSystem Sol;
    public LineRenderer[] rendererPlanet;
    Vector3[] positions = new Vector3[500];
    Color[] colors = new Color[20];
    public LineRenderer L;
    // Start is called before the first frame update
    void Start()
    {
       
           for(int i = 0; i< Sol.planets.Count; i++)
        {
            drawPlanetCircle(transform.position, Sol.planets[i].orbitRadius, 150, 0.1f, rendererPlanet[i]);
        }
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void drawPlanetCircle(Vector2 origin ,float radius,int step,float z,LineRenderer lineRenderer)
    {
        float angle = 0;
        float x, y;
        Vector2 v = Vector2.up * radius;
        float stepAngle =2*Mathf.PI / step;
        float inc;
        for(int i = 0; i < step; i++)
        {
            inc = stepAngle * i;
            x = v.x * Mathf.Cos(inc) - v.y * Mathf.Sin(inc);
            y = v.x * Mathf.Sin(inc) + v.y * Mathf.Cos(inc);
            positions[i].x = x;
            positions[i].y = y;
            positions[i].z = z;
        }
        lineRenderer.positionCount = step;
        lineRenderer.SetPositions(positions);
        
    }
}
