using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] public float speed = 20f;
    [SerializeField] public float hitRange = 1f;
    [SerializeField] public int damage = 1;
    public Transform player;
    public Transform shotSpot;  //Starting point
    private bool launched;
    private bool hit;
    
    void Start()
    {
        hit = false;
        launched = false;
    }
    
    void Update()
    {
        
        transform.GetComponent<Animator>().SetBool("Launched", launched);

        if (Vector2.Distance(transform.position, player.position) < hitRange && !hit && launched)
        {
            hit = true;
            gameObject.GetComponent<Animator>().SetTrigger("Hit");
            player.gameObject.GetComponent<PlayerMovement>().RemoveHp(damage);
            transform.Translate(0,0,0);
            Debug.Log("HIT");
        }
        
        if (!hit && launched)
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        }
    }

    public void Reload()
    {
        transform.position = shotSpot.position;
        launched = false;
        hit = false;
        transform.GetComponent<Animator>().SetBool("Launched", false);
    }
    
    public void Launch()
    {
        transform.position = transform.parent.position;
        launched = true;
        hit = false;
        transform.GetComponent<Animator>().SetBool("Launched", true);
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, hitRange);
    }
}
