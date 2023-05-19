using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Serialization;

public class EnemyBasic : MonoBehaviour
{
    
    [SerializeField] public float awarenessLostRange = 30f;
    [SerializeField] public bool canLooseAwareness = true;
    public AudioSource audioSource;
    public int damage = 1;
    public int hitForce = 2500;     //How much force will be applied to Player on hit
    
    private Rigidbody2D rigidBody;
    public Animator animator;
    public Transform awarenessSpot;
    public GameObject player;
    private float awarenessLostDelay = 1.5f;
    private float awarenessLostTime;

    [SerializeField]public int score = 100;
    
    public int health;
    [SerializeField]public int maxHealth = 2;
    [FormerlySerializedAs("RIP")] public bool rip;   //Is alive?


    private void Start()
    {
        health = maxHealth;
        awarenessLostTime = Time.time;
        player = GameObject.FindWithTag("Player");
        rigidBody = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (rip) return;
        
        animator.SetInteger("Health", health);
        animator.SetFloat("SpeedX", rigidBody.velocity.x);
        animator.SetFloat("SpeedY", rigidBody.velocity.y);
            
        IsDead();
        AwarenessCheck();

    }

    private void AwarenessCheck()
    {

        if (Vector2.Distance(player.transform.position, transform.position) > awarenessLostRange ||
            Mathf.Abs(player.transform.position.y - transform.position.y) > awarenessLostRange/2)       //If Player is in range
        {
            if (awarenessLostTime + awarenessLostDelay < Time.time && canLooseAwareness) { WakeUp(false); }
        }
        else       //If Player is NOT in range
        {
            WakeUp(true);
            awarenessLostTime = Time.time;
        }
        
    }
    
    private void OnDrawGizmosSelected()
    {
        if (awarenessSpot != null) { Gizmos.DrawWireSphere(awarenessSpot.position, awarenessLostRange); }
    }


    private void IsDead()
    {
        if (health <= 0)
        {
            rip = true;

            try { audioSource.Play(); }
            catch (Exception e)
            {
                // ignored
            }

            animator.SetTrigger("Death");
            gameObject.layer = 14;
            ScoreManager.instance.AddScore(score);
        }
        else
        {
            rip = false;
        }

    }

    public void RemoveHp(int d)
    {
        if (health <= 0) return;

        health = (health - d < 0) ? 0 : health -= d;

        animator.SetTrigger("Hit");
        animator.SetBool("IsStunned", true);

        gameObject.GetComponent<BrainyScript>().KickBack();
        
        IsDead();
        animator.SetInteger("Health", health);
        Debug.Log(health);
    }
    
    private Vector2 kickDirection;

// ...

    public void Kill() { RemoveHp(maxHealth); }

    private void AddHp(int d)
    {
        if (health + d > maxHealth)
        {
            health = maxHealth;
        }
        else
        {
            health += d;
        }
        Debug.Log(health);
    }

    private void WakeUp(bool awake)
    {
        animator.SetBool("Awake", awake);
        //if(!awake) animator.SetTrigger("Sleep");
    }
}
