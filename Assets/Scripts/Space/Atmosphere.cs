using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Atmosphere : MonoBehaviour
{
    public GameObject sun;
    public Planet planet;
    public Material mat;
    public float a= 30;
    Vector3 tmp;
    Vector3 dir;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        dir = sun.transform.position - planet.transform.position;
        dir.Normalize();
        dir += new Vector3(a, 0, 0);
        mat.SetVector("_Vector3", dir);
    }
}
