using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{

    [SerializeField] public int health;
    [SerializeField] public bool itsATrap;
    private int score;
    public Animator animator;
    public Transform attackPoint;
    public LayerMask enemyLayer;
    [SerializeField] public float attackRange;

    public AudioSource audioSource;
    //public Collider2D[] hitObjects;
    
    // Start is called before the first frame update
    void Start()
    {
        animator.SetBool("ItsATrap", itsATrap);
    }

    // Update is called once per frame
    void Update()
    {
        //hitObjects = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);
    }

    public void Open()
    {
        animator.SetTrigger("Open");
    }

    public void Eat()
    {
        if (itsATrap)
        {
            audioSource.Play();
            Collider2D[] hitObjects = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);
            foreach (Collider2D obj in hitObjects)
            {
                if (obj.CompareTag("Enemy"))
                {
                    obj.GetComponent<EnemyBasic>().Kill();
                } 
                else if (obj.CompareTag("Player"))
                {
                    obj.GetComponent<PlayerMovement>().Kill();
                }
            }
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        
        
    }
    
}
