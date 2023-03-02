using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarSystem : MonoBehaviour
{
    public float sunRadius=57.0f;
    public float sunMass=100000;
    public float sunAvoidRadius=75.0f;
    public List<Planet> allSpaceObjects = new List<Planet>(); //list of planets and moons
    public int[] planetarySystemIndex; // all moons and a planet of system have same ýndex
    public List<Planet> planets= new List<Planet>(); // list of only planets
    public List<List<Planet>> planetarySystemList;
    Vector2 dist;

    [Range(0.0f, 100000.0f)]
    public float G = 1.0f;
    // Start is called before the first frame update
    private void Start()
    {

        planetarySystemList = new List<List<Planet>>(planets.Count);

        //set up list of planets and moons categorize systems
        for(int i = 0; i < planets.Count; i++)
        {
            planetarySystemList.Add(new List<Planet>());
        }
        for(int i = 0; i < allSpaceObjects.Count; i++)
        {
            planetarySystemList[allSpaceObjects[i].PlanetarySystemIndex].Add(allSpaceObjects[i]);
        }
    }

    //only conside planet
    public Vector2 getGravity(Vector2 position,float mass)
    {
        
        Vector2 gravForce = Vector2.zero;
        
        for (int i = 0; i < planets.Count; i++) { 
            dist = (Vector2)planets[i].transform.position - position;
            gravForce += dist.normalized*(G * planets[i].mass * mass / dist.sqrMagnitude);
        }
        gravForce += (-position.normalized) * (G * sunMass * mass) / position.sqrMagnitude;
        return gravForce;
    }

    //consider planets and moons
    public Vector2 getGravityAll(Vector2 position, float mass)
    {
        Vector2 gravForce = Vector2.zero;
        int count = allSpaceObjects.Count;
        for (int i = 0; i < allSpaceObjects.Count; i++)
        {
            dist = (Vector2)allSpaceObjects[i].transform.position - position;
            gravForce += dist.normalized * (G * allSpaceObjects[i].mass * mass / dist.sqrMagnitude);
        }
        gravForce += (-position.normalized) * (G * sunMass * mass) / position.sqrMagnitude;
        return gravForce;
    }

    public Planet getClosestPlanet(Vector2 position)
    {
        float mindist = 100000000;
        float sqrMagnitude;
        int minIndex = -1;
        int count = planets.Count;
        for (int i = 0; i < count; i++)
        {
            dist = (Vector2)planets[i].transform.position - position;
            sqrMagnitude = dist.sqrMagnitude;
            if (sqrMagnitude < mindist)
            {
                minIndex = i;
                mindist = sqrMagnitude;
            }
        }

        return planets[minIndex];
    }

    public Vector2 getGravity(Planet closest,Vector2 position, float mass)
    {
        Vector2 gravForce = Vector2.zero;
        int planetarySystemIndex = closest.PlanetarySystemIndex;
        int count = planetarySystemList[planetarySystemIndex].Count;
        for (int i = 0; i <count; i++)
        {
            dist = (Vector2)planetarySystemList[planetarySystemIndex][i].transform.position - position;
            gravForce += dist.normalized * (G * planetarySystemList[planetarySystemIndex][i].mass * mass / dist.sqrMagnitude);
        }
        gravForce += (-position.normalized) * (G * sunMass * mass) / position.sqrMagnitude;
        return gravForce;
    }

}
