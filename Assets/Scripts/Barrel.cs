using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Barrel : MonoBehaviour
{

    [SerializeField]public int health = 5;
    public Animator animator;
    public bool isBurning = false;
    public int burnDmg = 1;
    [SerializeField] public LayerMask enemyLayer;
    [SerializeField] public float burnRange = 1f;
    [SerializeField] public float explosionRange = 4f;
    public int explosionDmg = 2;
    public Transform burningSpot;
    public AudioSource audioSource;


    private void Update()
    {
        if (isBurning) { BurnTarget(); }
        animator.SetInteger("Health", health);
    }

    public void RemoveHp(int d)
    {
        health -= d;
        
        if (health <= 0) { Explode(); }
        
        if (health < 4)
        {
            isBurning = true;
            animator.SetBool("Burning", true); 
        }

    }

    private void Explode()  
    {
        audioSource.Play();
        animator.SetTrigger("Explosion");
        
        foreach (var obj in Physics2D.OverlapCircleAll(burningSpot.position, explosionRange, enemyLayer))
        {
            if (obj.CompareTag("Enemy")) { obj.GetComponent<EnemyBasic>().RemoveHp(explosionDmg); } 
            
            else if (obj.CompareTag("Player")) { obj.GetComponent<PlayerMovement>().RemoveHp(explosionDmg); }
        }
        
        //Destroy(gameObject);
    }

    private void BurnTarget()
    {
        foreach (var obj in Physics2D.OverlapCircleAll(burningSpot.position, burnRange, enemyLayer))
        {
            if (obj.CompareTag("Enemy")) { obj.GetComponent<EnemyBasic>().RemoveHp(burnDmg); } 
            
            else if (obj.CompareTag("Player")) { obj.GetComponent<PlayerMovement>().RemoveHp(burnDmg); }
        }
        
    }
    
    
    private void OnDrawGizmosSelected()
    {
        if (burningSpot != null)
        {
            var position = burningSpot.position;
            Gizmos.DrawWireSphere(position, burnRange);
            Gizmos.DrawWireSphere(position, explosionRange);
        }

        
    }
}
