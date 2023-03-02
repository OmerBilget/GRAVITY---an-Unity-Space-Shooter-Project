using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerEnergy : MonoBehaviour
{
    public float maxEnergy = 100;
    private float energy;
    public float energyConsumptionRate = 1f;
    public float weaponFireConsumptionRate = 1;
    public float weaponPulseConsumptionRate = 4f;
    public float normalFlightConsumptionRate = 1f;
    public float hyperflightConsumptionRate = 1f;
    public float regenRate = 1f;
    public Slider EnergyBar;
    // Start is called before the first frame update
    void Start()
    {
        energy = maxEnergy;
    }

    // Update is called once per frame
  

    public void pulseFire()
    {
        energy -= weaponPulseConsumptionRate;
        if (energy < 0)
        {
            energy = 0;
        }
        updateEnergyBar();
    }

    public void bulletFire()
    {
        energy -= weaponFireConsumptionRate*Time.deltaTime;
        if (energy < 0)
        {
            energy = 0;
        }
        updateEnergyBar();
    }

    public void normalFlightConsume()
    {
        energy -= normalFlightConsumptionRate*Time.deltaTime;
        if (energy < 0)
        {
            energy = 0;
        }
        updateEnergyBar();
    }

    public void hyperFlightConsume()
    {
        energy -= hyperflightConsumptionRate* Time.deltaTime;
        if (energy < 0)
        {
            energy = 0;
        }
        updateEnergyBar();
    }

    public void updateEnergyBar()
    {
        EnergyBar.value = energy / maxEnergy;

    }

    public void updateText()
    {

    }

    public void regenEnergy()
    {
        energy += regenRate;
    }
    public void fuelRunOut()
    {

    }
}
