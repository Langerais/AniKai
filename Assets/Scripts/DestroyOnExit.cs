using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnExit : StateMachineBehaviour
{
    public AudioClip explosionAudio;
    private GameObject drone;
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        drone = GameObject.FindGameObjectWithTag("Drone");

        drone.GetComponentInChildren<AudioSource>().clip = explosionAudio;
        drone.GetComponentInChildren<AudioSource>().volume = 0.5f;
        drone.GetComponentInChildren<AudioSource>().Play();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        // Add any additional actions before destroying the object
        // For example, you can play an audio clip or trigger a particle effect
        Debug.Log("DESTROY");
        Destroy(animator.gameObject, stateInfo.length);
    }
}
