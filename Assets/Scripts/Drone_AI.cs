using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone_AI : MonoBehaviour
{
    [SerializeField] public float speed = 1f;
    [SerializeField] public float attackDelay = 3f;
    private float lastShotTime;
    public Transform target;        //Point above the player's head
    public Animator animator;
    public GameObject bullet;
    private bool awake;
    void Start()
    {
        lastShotTime = Time.time;
        animator = gameObject.GetComponent<Animator>();
        awake = animator.GetBool("Awake");
    }


    void Update()
    {

        bullet = GetComponentInChildren<BulletScript>().gameObject;
        
        awake = animator.GetBool("Awake");
        
        if (awake)
        {
            transform.position = Vector2.Lerp(transform.position, target.position, Time.deltaTime * speed / 50);


            if (lastShotTime + attackDelay < Time.time)
            {
                Shoot();
                lastShotTime = Time.time;
                
            }
            
        } 
    }
    

    void Shoot()
    {
        lastShotTime = Time.time;
        bullet.GetComponent<BulletScript>().Launch();
    }

}
    
    

