using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{


    public Slider slider;
    public Animator heartAnimator;

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
    }
    
    public void SetHealth(int health)
    {
        slider.value = health;
        heartAnimator.SetInteger("Health", health);
    }
    
    
}
