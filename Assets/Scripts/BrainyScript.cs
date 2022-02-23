using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrainyScript : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform target;
    public bool awake;
    public float speed;
    public float attackRange = 1f;
    

    // Update is called once per frame
    void Update()
    {
        awake = gameObject.GetComponent<Animator>().GetBool("Awake");
        if (awake)
        {
            transform.position = Vector2.Lerp(transform.position, target.position, Time.deltaTime * speed / 4);

            if (Vector2.Distance(transform.position, target.position) < attackRange)
            {
                gameObject.GetComponent<Animator>().SetTrigger("Attack1");
            }
        }
        
    }
    
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
    
}
