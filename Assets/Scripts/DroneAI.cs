using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneAI : MonoBehaviour
{
    [SerializeField] public float speed = 1f;
    [SerializeField] public float attackDelay = 3f;
    private float lastShotTime;
    public Transform target;        //Point above the player's head
    public Vector2 targetLoc;
    public Animator animator;
    public GameObject bullet;
    public GameObject destructionPoint;
    public AudioSource audioSource;
    private bool awake;
    private static readonly int Awake = Animator.StringToHash("Awake");
    
    public AudioClip shotAudio;


    private void Start()
    {
        lastShotTime = Time.time;
        animator = gameObject.GetComponent<Animator>();
        awake = animator.GetBool(Awake);
        target = GameObject.FindGameObjectWithTag("DroneTarget").transform;
        targetLoc = GameObject.FindGameObjectWithTag("Player").transform.position;
        audioSource = GetComponentInChildren<AudioSource>();
        targetLoc.y += 9;
    }


    private void Update()
    {
        
        if (Vector2.Distance(transform.position, destructionPoint.transform.position) < 2)
        { Explode(); }
        
        bullet = GetComponentInChildren<BulletScript>().gameObject;
        
        awake = animator.GetBool(Awake);
        targetLoc = GameObject.FindGameObjectWithTag("Player").transform.position;
        targetLoc.y += 9;
        
        if (awake)
        {
            transform.position = Vector2.Lerp(transform.position, targetLoc, Time.deltaTime * speed * 1.5f);
            
            if (lastShotTime + attackDelay < Time.time)
            {
                Shoot();
                lastShotTime = Time.time;
            }
            
        } 
    }


    private void Shoot()
    {
        lastShotTime = Time.time;
        bullet.GetComponent<BulletScript>().Launch();
        audioSource.clip = shotAudio;
        audioSource.volume = 0.3f;
        audioSource.Play();
    }

    private void Explode()
    {
        //GetComponentInChildren<AudioSource>().clip = explosionAudio;
        //GetComponentInChildren<AudioSource>().volume = 0.5f;
        //GetComponentInChildren<AudioSource>().Play();
        animator.SetInteger("Health", 0);
        animator.SetTrigger("Explode");
        //Destroy(gameObject);
    }

}
    
    

