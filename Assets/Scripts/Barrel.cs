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
    [FormerlySerializedAs("emenyLayer")] public LayerMask enemyLayer;
    [SerializeField]public float burnRange = 1f;
    [SerializeField]public float explosionRange = 4f;
    public int explosionDmg = 2;
    public Transform burningSpot;
    public AudioSource audioSource;

    
    void Update()
    {
        
        
        if (isBurning)
        {
            BurnTarget();
        }
        
        animator.SetInteger("Health", health);
    }

    public void RemoveHp(int d)
    {
        health -= d;
        
        if (health <= 0)
        {
            Explode();
        }
        
        if (health < 4)
        {
            isBurning = true;
            animator.SetBool("Burning", true);
        }

    }

    private void Explode()  
    {
        audioSource.Play();
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(burningSpot.position, explosionRange, enemyLayer);
        animator.SetTrigger("Explosion");
        foreach (Collider2D obj in hitObjects)
        {
            if (obj.CompareTag("Enemy"))
            {
                obj.GetComponent<EnemyBasic>().RemoveHp(explosionDmg);
            } 
            else if (obj.CompareTag("Player"))
            {
                obj.GetComponent<PlayerMovement>().RemoveHp(explosionDmg);
            }
        }
        
        //Destroy(gameObject);
    }
    
    void BurnTarget()
    {
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(burningSpot.position, burnRange, enemyLayer);

        foreach (Collider2D obj in hitObjects)
        {
            if (obj.CompareTag("Enemy"))
            {
                obj.GetComponent<EnemyBasic>().RemoveHp(burnDmg);
            } 
            else if (obj.CompareTag("Player"))
            {
                obj.GetComponent<PlayerMovement>().RemoveHp(burnDmg);
                Debug.Log("Player Burned");
            }
        }
        
    }
    
    
    private void OnDrawGizmosSelected()
    {
        if (burningSpot == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(burningSpot.position, burnRange);
        Gizmos.DrawWireSphere(burningSpot.position, explosionRange);
        
    }
}
