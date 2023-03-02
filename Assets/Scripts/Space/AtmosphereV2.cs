using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtmosphereV2 : MonoBehaviour
{
    public Material atmosphereMaterial;
    public Transform player;
    public Transform sun;
    public float range;
    Vector2 LightDir;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        atmosphereMaterial.SetVector("_Center", transform.position);
        LightDir = (sun.position - transform.position).normalized;
        atmosphereMaterial.SetVector("_LightDir", LightDir);
    }
}
