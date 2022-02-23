using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerAttack : MonoBehaviour
{
    private Animator animator;
    [SerializeField] [Range(0f, 5f)] private float maxDurationAttack1 = 1f;
    [SerializeField] [Range(0f, 5f)] private float maxDurationAttack2 = 1f;
    [SerializeField] [Range(0f, 1f)] private float postAttackMoveDelay = 0.9f;
    [SerializeField] [Range(0f, 5f)] private float attack1Range = 5f;
    public Transform attackSpot;
    [FormerlySerializedAs("emenyLayer")] public LayerMask enemyLayer;
    [SerializeField]public int attackDmg = 1;
    public AudioClip attackSound;
    public AudioSource audioSource;
    private bool RIP;
    
    public int energy;
    public int maxEnergy = 100;
    public EnergyBar energyBar;
    public float energyConsumptionFreq = 0.1f;
    public float energyConsumptionStart;
    
    private float attack1Time = 0;
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        maxEnergy = gameObject.GetComponent<PlayerMovement>().maxEnergy;
        energy = maxEnergy;
        //energyConsumptionStart = Time.time;
        //animation["Attack2"].wrapMode = WrapMode.Once;
    }

    // Update is called once per frame
    void Update()
    {
        RIP = gameObject.GetComponent<PlayerMovement>().health <= 0;
        energy = gameObject.GetComponent<PlayerMovement>().energy;

        if (RIP)
        {
            try
            {
                audioSource.enabled = false;
            }
            catch(Exception e){}
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            Attack(0);
        }
        

        if (Time.time >= attack1Time + (maxDurationAttack1 * postAttackMoveDelay))
        {
            animator.SetBool("Attacking1", false);
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (energy > 0)
            {
                Time.timeScale = 0.5f;
                gameObject.GetComponent<Rigidbody2D>().gravityScale = 3.5f;
                energyConsumptionStart = Time.time;
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            Time.timeScale = 1f;    
            gameObject.GetComponent<Rigidbody2D>().gravityScale = 5f;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }

        SlowTimeEnergyReduction(); 
        
    }

    void AttackSoundPlay()
    {
        if (audioSource != null)
        {
            audioSource.clip = attackSound;
            audioSource.Play();
        }
        
    }
    
    void Interact()
    {
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(attackSpot.position, attack1Range, enemyLayer);

        foreach (Collider2D obj in hitObjects)
        {
            if (obj.CompareTag("Chest"))
            {
                obj.GetComponent<Chest>().Open();
            }
        }
    }

    void Attack(int type)
    {
        switch (type)
        {
            case 0: //Attack 1
                if (Time.time > attack1Time + maxDurationAttack1)
                {
                    AttackSoundPlay();
                    attack1Time = Time.time;
                    animator.SetTrigger("Attack1");
                    animator.SetBool("Attacking1", true);

                    Collider2D[] hitObjects = Physics2D.OverlapCircleAll(attackSpot.position, attack1Range, enemyLayer);

                    foreach (Collider2D obj in hitObjects)
                    {

                        if (obj.transform.position.y < attackSpot.position.y + 1.8f &&
                            obj.transform.position.y > attackSpot.position.y - 2f)
                        {

                            if (obj.CompareTag("Enemy"))
                            {
                                obj.GetComponent<EnemyBasic>().RemoveHp(attackDmg);
                            } else if (obj.CompareTag("Barrel"))
                            {
                                obj.GetComponent<Barrel>().RemoveHp(1);
                            }
                        }

                    }

                }
                break;

            default:
                Debug.Log("Invalid Attack Code");
                break;
        }

    }

    void SlowTimeEnergyReduction()
    {
        if (Time.timeScale < 1f)
        {
            if (energyConsumptionStart + energyConsumptionFreq < Time.time)
            {
                if (!RemoveEnergy(5))
                {
                    Time.timeScale = 1f;
                }
                energyConsumptionStart = Time.time;
            }
        }

    }
    
    bool RemoveEnergy(int e)
    {
        if (energy - e > 0)
        {
            energy -= e;
            energyBar.SetEnergy(energy);
            GetComponent<PlayerMovement>().energy = energy;
            return true;
        }

        energy = 0;
        energyBar.SetEnergy(energy);
        GetComponent<PlayerMovement>().energy = energy;
        return false;
    }

    void SlowTime(float scale)
    {
        if (Time.timeScale == 1.0f)
            Time.timeScale = scale;
        else
            Time.timeScale = 1.0f;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackSpot == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(attackSpot.position, attack1Range);
    }
}
