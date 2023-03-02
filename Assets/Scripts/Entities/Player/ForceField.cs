using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceField : MonoBehaviour
{

    public PointEffector2D forceField;
    public Collider2D circleCollider;

    public float maxforceTime;
    bool activated = false;
    public float maxcooldownTime;
    public float delayTime;
    bool forceOn = false;
    float timerCooldown;
    float timerActive;
    public ParticleSystem particleInstance;
    void Start()
    {
       
        DisableField();
    }

    public void activatePulse()
    {
        if (timerCooldown<=0 && activated==false)
        {
           
            particleInstance.Play();
            activated = true;
            timerActive = maxforceTime;
            forceOn = false;
        }
    }
    
    private void Update()
    {
       
        if (activated)
        {
            timerActive -= Time.deltaTime;
            if (timerActive<=delayTime  && forceOn==false)
            {
                forceOn = true;
                ActivateField();
            }
            
            if (timerActive <= 0)
            {
                activated = false;
                DisableField();
                timerCooldown = maxcooldownTime;
            }
        }
        else
        {
            if (timerCooldown >= 0)
            {
                timerCooldown -= Time.deltaTime;
            }
        }
       
        
    }
   
    public void ActivateField()
    {
        forceField.enabled = true;
        circleCollider.enabled = true;
    }

    public void DisableField()
    {
        forceField.enabled = false;
        circleCollider.enabled = false;
    }

    private void OnParticleSystemStopped()
    {
        
    }
}
