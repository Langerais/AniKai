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
    public float energyConsumptionEnd;
    public float energyRegenLast;
    [SerializeField] public int energyRegenRate = 3; // Units Per Second
    
    private float attack1Time = 0;
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        maxEnergy = gameObject.GetComponent<PlayerMovement>().maxEnergy;
        energy = maxEnergy;
        energyConsumptionEnd = Time.time;
        energyRegenLast = Time.time;
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
                SlowTime(true);
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            SlowTime(false);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }

        SlowTimeEnergyReduction();
        EnergyRegen();
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
        bool dashing = animator.GetBool("isDashing");
        bool canAttack = Time.time > attack1Time + maxDurationAttack1;
        
        switch (type)
        {
            case 0: //Attack 1
                if (canAttack && !dashing)
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

                } else if (canAttack)
                {
                    
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

    void SlowTime(bool slow)
    {

        if (slow)
        {
            if (energy > 0)
            {
                Time.timeScale = 0.5f;
                gameObject.GetComponent<Rigidbody2D>().gravityScale = 3.5f;
                energyConsumptionStart = Time.time;
            }
        }
        else
        {
            Time.timeScale = 1f;    
            gameObject.GetComponent<Rigidbody2D>().gravityScale = 5f;
            energyConsumptionEnd = Time.time;
        }


    }

    void EnergyRegen()
    {
        if ( Math.Abs(Time.timeScale - 1f) == 0     //If time NOT slowed
             && energyConsumptionEnd + 2 < Time.time //Delay after last consumption
             && energyRegenLast + 1 < Time.time)   //Energy Regen Speed
        {
            if (energy + energyRegenRate <= 100)
            {
                energy += energyRegenRate;
                energyBar.SetEnergy(energy);
                GetComponent<PlayerMovement>().energy = energy;
            }
            energyRegenLast = Time.time;
        }
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
