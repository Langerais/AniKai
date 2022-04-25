using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyBar : MonoBehaviour
{


    public Slider slider;

    [SerializeField] public float restoreRate = 1;
    
//public Animator spinnerAnimator;


    public void SetMaxEnergy(int energy)
    {
        slider.maxValue = energy;
    }
    
    public void SetEnergy(int energy)
    {
        slider.value = energy;
        //spinnerAnimator.SetInteger("Energy", energy);
    }
    
    
}
