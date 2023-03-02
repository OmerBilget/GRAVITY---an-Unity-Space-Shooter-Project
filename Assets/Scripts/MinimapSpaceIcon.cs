using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MinimapSpaceIcon : MonoBehaviour
{
    // Start is called before the first frame update
    public RectTransform sunIcon;
    public RectTransform p1Icon;
    public RectTransform p2Icon;
    public RectTransform p3Icon;
    public RectTransform p4Icon;
    public RectTransform p5Icon;
    public RectTransform p6Icon;
    public RectTransform p7Icon;
    public Transform camera;
    public SolarSystem sol;
    public float radius;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 dist = (sol.transform.position- camera.transform.position).normalized * radius;
        if (dist.magnitude < radius)
        {
            dist = dist.normalized * radius;
        }
        sunIcon.anchoredPosition = dist;


        dist = (sol.planets[0].transform.position - camera.transform.position).normalized * radius;
        if (dist.magnitude < radius)
        {
            dist = dist.normalized * radius;
        }
        p1Icon.anchoredPosition = dist;

        dist = (sol.planets[1].transform.position - camera.transform.position).normalized * radius;
        if (dist.magnitude < radius)
        {
            dist = dist.normalized * radius;
        }
        p2Icon.anchoredPosition = dist;

        dist = (sol.planets[2].transform.position - camera.transform.position).normalized * radius;
        if (dist.magnitude < radius)
        {
            dist = dist.normalized * radius;
        }
        p3Icon.anchoredPosition = dist;

        dist = (sol.planets[3].transform.position - camera.transform.position).normalized * radius;
        if (dist.magnitude < radius)
        {
            dist = dist.normalized * radius;
        }
        p4Icon.anchoredPosition = dist;


        dist = (sol.planets[4].transform.position - camera.transform.position).normalized * radius;
        if (dist.magnitude < radius)
        {
            dist = dist.normalized * radius;
        }
        p5Icon.anchoredPosition = dist;


        dist = (sol.planets[5].transform.position - camera.transform.position).normalized * radius;
        if (dist.magnitude < radius)
        {
            dist = dist.normalized * radius;
        }
        p6Icon.anchoredPosition = dist;


        dist = (sol.planets[6].transform.position - camera.transform.position).normalized * radius;
        if (dist.magnitude < radius)
        {
            dist = dist.normalized * radius;
        }
        p7Icon.anchoredPosition = dist;

    }
}
