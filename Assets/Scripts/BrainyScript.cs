using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrainyScript : MonoBehaviour
{

    public Transform target;
    public bool awake;
    public float speed;
    public float attackRange = 1f;
    public bool isStunned;

    public float stunTime = 1f;

    public float lastStun;
    //public int hp;
    public AudioClip hitSound;
    public AudioSource audioSource;


    private void Start()
    {
        isStunned = false;
        //hp = gameObject.GetComponent<Animator>().GetInteger("Health");
        target = GameObject.FindGameObjectWithTag("Player").transform;
        lastStun = Time.time;
    }

    // Update is called once per frame
    private void Update()
    {
        isStunned = gameObject.GetComponent<Animator>().GetBool("IsStunned");
        
        awake = gameObject.GetComponent<Animator>().GetBool("Awake");
        if (awake)
        {
            if (!isStunned)
            {
                transform.position = Vector2.Lerp(transform.position, target.position, Time.deltaTime * speed / 4);
                
                Vector3 charScale = transform.localScale;
        
                if (target.position.x < transform.position.x)  //Right-left sprite
                {
                    charScale.x = -1;
                }        
                if (target.position.x > transform.position.x)
                {
                    charScale.x = 1;
                }
                transform.localScale = charScale;

                if (Vector2.Distance(transform.position, target.position) < attackRange)
                {
                    gameObject.GetComponent<Animator>().SetTrigger("Attack1");
                }
            }
            else
            {
                gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                if (Time.time >= lastStun + stunTime)
                {
                    gameObject.GetComponent<Animator>().SetBool("IsStunned", false);
                }
            }

        }
        
    }

    public void KickBack()
    {
        gameObject.GetComponent<Animator>().SetBool("IsStunned", true);
        lastStun = Time.time;
        
        var position = transform.position;
        var hitDir = (target.position.x < position.x) ? 1f : -1f;
        
       // var impulse =  new Vector2(hitDir, 0.5f);
        var hitVector = new Vector2((position.x + hitDir * 125), (position.y + 10f));
        transform.position = Vector2.Lerp(position, hitVector, Time.deltaTime*1.5f);
        HitSoundPlay();

    }
    
    private void OnDrawGizmosSelected() { Gizmos.DrawWireSphere(transform.position, attackRange); }
    
    void HitSoundPlay()
    {
        if (audioSource == null) return;
        
        audioSource.clip = hitSound;
        audioSource.Play();
    }
    
}
