using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.Serialization;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerAttack : MonoBehaviour
{
    private Animator animator;
    [SerializeField] [Range(0f, 5f)] private float maxDurationAttack1 = 1f;
    [SerializeField] [Range(0f, 5f)] private float maxDurationDashAttack = 1f;
    [SerializeField] [Range(0f, 5f)] private float maxDurationAttack2 = 1f;
    [SerializeField] [Range(0f, 1f)] private float postAttackMoveDelay = 0.9f;
    [SerializeField] [Range(0f, 5f)] private float attack1Range = 5f;
    [SerializeField] [Range(0f, 10f)] private float dashAttackRange = 7f;
    public Vector2 dashStartingPoint;
    public Transform attackSpot;
    private Vector2 attackSpotInit1;
    private Vector2 dashAttackSpotInit1;
    private Transform dashAttackSpot;
    
    [FormerlySerializedAs("emenyLayer")] public LayerMask enemyLayer;
    [SerializeField]public int attackDmg = 1;
    public AudioClip attackSound;
    public AudioSource audioSource;
    private bool rip;
    private bool dashing;
    
    public int energy;
    public int maxEnergy = 100;
    public EnergyBar energyBar;
    public float energyConsumptionFreq = 0.1f;
    public float energyConsumptionStart;
    public float energyConsumptionEnd;
    public float energyRegenLast;
    [SerializeField] public int energyRegenRate = 3; // Units Per Second
    
    private float attack1Time = 0;
    private static readonly int IsDashing = Animator.StringToHash("isDashing");

    private void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        maxEnergy = gameObject.GetComponent<PlayerMovement>().maxEnergy;
        dashStartingPoint = transform.position;
        dashing = animator.GetBool("isDashing");
        dashAttackSpot = GameObject.Find("DashAttackSpot").transform;
        energy = maxEnergy;
        energyConsumptionEnd = Time.time;
        energyRegenLast = Time.time;
        attackSpot = GameObject.Find("AttackSpot").transform;
        
        attackSpotInit1 = attackSpot.position;
        DashAtackSpotAdjust();
    }

    private void Update()
    {
        rip = gameObject.GetComponent<PlayerMovement>().health <= 0;
        energy = gameObject.GetComponent<PlayerMovement>().energy;
        DashAtackSpotAdjust();
        var dashingNew = animator.GetBool("IsDashing");
        var dash = PlayerMovement.isDashing;
        
        if (dashingNew && !dashing)
        {
            dashStartingPoint = transform.position;
            Debug.Log("UPDATED");
        }
        
        dashing = dashingNew;
        

        dashAttackSpot.position = new Vector2(
            (dashStartingPoint.x + transform.position.x) / 2,
            (dashStartingPoint.y + transform.position.y) / 2);
        
        
        if (rip)
        {
            try
            {
                audioSource.enabled = false;
            }
            catch (Exception e)
            {
                // ignored
            }
        }
        
        if (Input.GetMouseButtonDown(0)) { Attack(0); }
        
        if (Time.time >= attack1Time + (maxDurationAttack1 * postAttackMoveDelay)) { animator.SetBool("Attacking1", false); }

        if (Input.GetMouseButtonDown(1) && energy > 0) { SlowTime(true); }

        if (Input.GetMouseButtonUp(1)) { SlowTime(false); }

        if (Input.GetKeyDown(KeyCode.E)) { Interact(); }

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

    void DashAtackSpotAdjust()
    {
        dashAttackSpot.position = new Vector2(
            (dashStartingPoint.x+transform.position.x)/2,
            (dashStartingPoint.y+transform.position.y)/2);
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

                    //Collider2D[] hitObjects = Physics2D.OverlapCircleAll(attackSpot.position, attack1Range, enemyLayer);
                    Collider2D[] hitObjects = Physics2D.OverlapCapsuleAll(attackSpot.position,  new Vector2(attack1Range,3.8f), CapsuleDirection2D.Horizontal,0f);

                    foreach (Collider2D obj in hitObjects)
                    {
                        if (obj.CompareTag("Enemy"))
                            {
                                obj.GetComponent<EnemyBasic>().RemoveHp(attackDmg);
                            } else if (obj.CompareTag("Barrel"))
                            {
                                obj.GetComponent<Barrel>().RemoveHp(1);
                            }
                            
                    }

                } else if (canAttack)
                {
                    Debug.Log("DASH ATTACK");
                    //DashAttack();  //TODO
                }
                break;

            default:
                Debug.Log("Invalid Attack Code");
                break;
        }

    }

    private void DashAttack()
    {
        
        var position = dashAttackSpot.position;
        var targetDir = position - gameObject.transform.position;
        var forward = transform.up;
        var angle = Vector3.Angle(targetDir, forward);
        
        Collider2D[] hitObjects = Physics2D.OverlapCapsuleAll(position,  new Vector2(dashAttackRange,3.8f), CapsuleDirection2D.Horizontal,angle);
        
        foreach (var obj in hitObjects)
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

    private void SlowTimeEnergyReduction()
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

    private bool RemoveEnergy(int e)
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

    private void SlowTime(bool slow)
    {

        if (slow && energy > 0)
        {
            Time.timeScale = 0.5f;
            gameObject.GetComponent<Rigidbody2D>().gravityScale = 3.5f;
            energyConsumptionStart = Time.time;
        }
        else
        {
            Time.timeScale = 1f;    
            gameObject.GetComponent<Rigidbody2D>().gravityScale = 5f;
            energyConsumptionEnd = Time.time;
        }


    }

    private void EnergyRegen()
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
        if (attackSpot != null) { Gizmos.DrawWireSphere(attackSpot.position, attack1Range); }
    }
}
