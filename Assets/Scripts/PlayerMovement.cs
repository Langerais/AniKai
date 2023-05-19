using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour
{

    private bool godmode = false;   //For debugging purposes
    
    [SerializeField] private float speed = 10f;
    [SerializeField] private float jumpHeight = 10f;
    [SerializeField] private float minJumpHeight = 4f;
    [SerializeField] public float dashForce = 100000f;
    [SerializeField] public float energyRestoreRate = 1;
    
    public AudioClip hitAudio;
    public AudioClip jumpAudio;
    public AudioClip dashAudio;
    public AudioClip pickUpAudio;
    private AudioSource audioSource;
    private bool isPlaying = false;

    private Rigidbody2D rigidBody;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    private GameObject dashFX;  //Dash trail sprite obj
    public GameObject dashP;   //Object with particles
    private ParticleSystem dashPSys;    //Particles system
    public Vector2 dashStartingPoint;
    
    private float inpX = 0;

    public HealthBar healthBar;
    public int health;
    [SerializeField]public int maxHealth = 6;
    [FormerlySerializedAs("RIP")] public bool rip;   //Is alive?

    public int energy;
    public int maxEnergy = 100;
    public EnergyBar energyBar;
    

    public float dashTimerStart;            //Dash started
    private float dashTimerCurrent;                 //Current dash timer (can dash when 0)            
    public static bool isDashing;
    private float dashDir;
    [SerializeField] private float dashDelay = 10f;  //Min time between dashes
    private float lastDashTime;                     //When dashed last time
    
    
    private int hitSide; // -1 = from left, 1 = from right
    private float hitForce; // enemy hit force
    private const float StunDelay = 1.5f;
    public bool isGod = false;  //Cant be damaged
    private float godStart;     
    private float godEnd;


    private Vector3 movement;
    private float jumpHoldTime = 0f;
   // [SerializeField]private float canJumptimer = 0f;
    

    public Transform isGroundedChecker;     //Feet object
    public float checkGroundRadius;         //Dst from feet obj to check
    public LayerMask groundLayer;
    public bool canMove;
    private bool isGrounded;
    private bool jump = false;
    private bool jumpCancel = false;


    private void Start()
    {
        godStart = Time.time;

        canMove = true;

        health = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetHealth(health);
        
        energy = maxEnergy;
        energyBar.SetEnergy(maxEnergy);
        energyBar.SetMaxEnergy(maxEnergy);
        
        lastDashTime = 0;
        dashFX = GameObject.FindWithTag("DashFX");
        isDashing = false;
        
        
        rigidBody = GetComponent<Rigidbody2D>();
        dashPSys = dashP.GetComponent<ParticleSystem>();
        
        jumpHeight = CalculateJumpForce(Physics2D.gravity.magnitude, 25.0f);
        Physics2D.IgnoreLayerCollision(6, 11);
        Physics2D.IgnoreLayerCollision(12, 11);

        audioSource = GetComponentInChildren<AudioSource>();
    }

    private void FixedUpdate() { JumpFixed(); }

    private void Update()
    {
        /////// FIRST THINGS FIRST ///////
        IsDead(); // ?
        //////////////////////////////////

        if (transform.position.y < -9)
        {
            isGod = false;
            //RemoveHp(maxHealth);
        }
        
        if (!rip)
        {
            IsGroundedCheck();
            
            animator.SetFloat("SpeedX", Mathf.Abs(rigidBody.velocity.x));
            animator.SetFloat("SpeedY", rigidBody.velocity.y);
            animator.SetBool("Grounded", isGrounded);

            if (!isGrounded && animator.GetFloat("SpeedY") == 0)
            {
                isGrounded = true;
            }
            
            if (!godmode)
            {
                ResetGod();
            }
            
            if(canMove){ Move(); }
            Dash();
            Jump();

            hitSide = 0;

            if (rigidBody.position.y < -9)  //Instadeath if falls down
            {
                Kill();
            }
            
            if (!audioSource.isPlaying)
            {
                isPlaying = false;
            }
            
            
            
        }
        

        
        /////////////////////////// CHEATING ZONE ///////////////////////////
        

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyUp(KeyCode.X))
        {
            AddEnergy(maxEnergy);
        }

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyUp(KeyCode.C))
        {
            AddHp(maxHealth);
        }
        
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyUp(KeyCode.G))
        {
            if (godmode)
            {
                godmode = false;
                Debug.Log("GODMODE OFF");
            }
            else
            {
                godmode = true;
                isGod = true;
                Debug.Log("GODMODE ON");
            }
        }

        
    }


    private void Dash()
    {
        
        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time > lastDashTime + dashDelay)
        {
            SetTempGod(dashDelay);
            
            audioSource.clip = dashAudio;
            audioSource.volume = 1f;
            audioSource.time = 0.8f;
            audioSource.Play();
            
            var position = rigidBody.position;
            dashStartingPoint = new Vector2(position.x, position.y);
            isDashing = true;
            animator.SetBool("IsDashing", true);
            canMove = false;
            dashTimerCurrent = dashTimerStart;
            lastDashTime = Time.time;
            rigidBody.velocity = Vector2.zero;

            if (transform.localScale.x > 0) //Mirror the sprite on side move
            {
                dashDir = 1;
            }
            else
            {
                dashDir = -1;
            }
            
            
            dashFX.GetComponent<Animator>().SetTrigger("Dash"); //Start dashing animation
            DashParticlesSwitch(true);  //Starts emmiting the particles when dash has started
        }

        if (isDashing)  //Handle dash end
        {
            rigidBody.velocity = transform.right * dashDir * dashForce;
            dashTimerCurrent -= Time.deltaTime;

            if (dashTimerCurrent <= 0)
            {
                isDashing = false;
                canMove = true;
                animator.SetBool("IsDashing", false);
                DashParticlesSwitch(false); //Stopps emmiting the particles when the dash can be used again
            }
        }
        
        animator.SetBool("IsDashing", isDashing);

    }


    private void Move()
    {
        inpX = Input.GetAxisRaw("Horizontal");
        float moveBy = inpX * speed;

        Vector3 charScale = transform.localScale;
        
        if (inpX < 0)  //Right-left sprite
        {
            charScale.x = -10;
        }        
        if (inpX > 0)
        {
            charScale.x = 10;
        }
        transform.localScale = charScale;

        if (isGrounded && animator.GetBool("Attacking1"))   //Stop moving while attacking
        {
            moveBy = 0;
        }
        
        rigidBody.velocity = new Vector2(moveBy, rigidBody.velocity.y);
    }
    
    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded) // Player starts pressing the button
            jump = true;
            
        if (Input.GetButtonUp("Jump") && !isGrounded) // Player stops pressing the button
            jumpCancel = true;
    }

    private void JumpFixed()
    {
        //Start jump
        if (jump)
        {
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpHeight);
            jump = false;
            
            audioSource.clip = jumpAudio;
            audioSource.volume = 1;
            audioSource.time = 0f;
            audioSource.Play();
        }
        // Cancel the jump when the button is no longer pressed
        if (jumpCancel)
        {
            if (rigidBody.velocity.y > minJumpHeight)
                rigidBody.velocity = 
                    new Vector2(rigidBody.velocity.x, minJumpHeight) * Time.fixedDeltaTime;
            
            jumpCancel = false;
        }
    }
    
    
    
    private void OnCollisionStay2D(Collision2D other)
    {
        
        var enemy = other.gameObject.GetComponent<Transform>();

        if (other.gameObject.CompareTag("Enemy") && !other.gameObject.GetComponent<EnemyBasic>().rip)   //If target is an enemy and alive
        {
            var damage = other.gameObject.GetComponent<EnemyBasic>().damage;
            hitForce = other.gameObject.GetComponent<EnemyBasic>().hitForce;

            hitSide = transform.position.x > enemy.position.x ? 1 : -1;

            RemoveHp(damage);
            
        } 
    }


    private void OnCollisionEnter2D(Collision2D other)
    {
        audioSource.clip = pickUpAudio;
        audioSource.volume = 0.3f;

        var gotPickup = false;

        if (other.gameObject.CompareTag("HealthFlask"))
        {
            gotPickup = true;
            AddHp(3);
        }
        else if (other.gameObject.CompareTag("EnergyPill"))
        {
            gotPickup = true;
            AddEnergy(100);
        }

        if (gotPickup && !isPlaying)
        {
            Physics2D.IgnoreCollision(other.gameObject.GetComponent<Collider2D>(), transform.GetComponent<Collider2D>());
            Destroy(other.gameObject);
            audioSource.time = 0f;
            audioSource.Play();
            isPlaying = true;
        }
    }



    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Barrel") && other.GetComponent<Barrel>().isBurning)
        {
            RemoveHp(other.GetComponent<Barrel>().burnDmg);
        }
    }
    


    ///////////////////////////////////// UTILITY STAFF ///////////////////////////////////////////////////////////
    

    private static float CalculateJumpForce(float gravityStrength, float height) //Jump force based on gravity scale
    {
        //h = v^2/2g
        //2gh = v^2
        //sqrt(2gh) = v
        return Mathf.Sqrt(2 * gravityStrength * height);
    }

    private void IsGroundedCheck()
    {
        var collider = 
            Physics2D.OverlapCircle(isGroundedChecker.position,
                checkGroundRadius, groundLayer);

        isGrounded = collider != null;
    }

    private void DashParticlesSwitch(bool on)
    {
        if (on) dashPSys.Play();
        else dashPSys.Stop();
    }


    private void IsDead()
    {
        if (health <= 0)
        {
            rip = true;
            transform.GetComponent<PlayerAttack>().audioSource.enabled = false;
            //camera.GetComponent<CameraScript>().GameOver();
        }
        else
        {
            rip = false;
        }
        animator.SetBool("isDead", rip);
    }

    public void RemoveHp(int d)        //After first hit in the game removes HP 2 times. Just can't find the issue
    {
        if (!isGod)
        {
            SetTempGod(StunDelay);
            
            audioSource.clip = hitAudio;
            audioSource.volume = 0.3f;
            audioSource.time = 0f;
            audioSource.Play();

            
            if (health - d < 0)
            {
                health = 0;
            }
            else
            {
                health -= d;
                animator.SetTrigger("Hit");
            }
            
            ScoreManager.instance.AddScore(d * -10);
            
            rigidBody.velocity = Vector2.zero;
            var impulse = new Vector2(transform.right.x * hitSide * hitForce, 1);
            rigidBody.AddForce(impulse);

        }

        healthBar.SetHealth(health);
    }

    public void Kill()
    {
        isGod = false;
        RemoveHp(maxHealth);
    }

    private void AddHp(int d)
    {
        health = (health + d > maxHealth) ? maxHealth : (health + d);
        healthBar.SetHealth(health);
        Debug.Log(health);
    }

    private bool RemoveEnergy(int e)
    {
        if (energy - e > 0)
        {
            energy -= e;
            energyBar.SetEnergy(energy);
            return true;
        }

        energy = 0;
        energyBar.SetEnergy(energy);
        return false;
    }

    private void AddEnergy(int e)
    {
        energy = (energy + e > maxEnergy) ? maxEnergy : (energy + e);
        energyBar.SetEnergy(energy);
    }
    
    private void SetTempGod(float time)
    {
        godStart = Time.time;
        godEnd = godStart + time;
    }

    private void ResetGod()
    {
        if (Time.time > godEnd)
        {
            isGod = false;
            spriteRenderer.color = new Color (1, 1, 1, 1);
            Physics2D.IgnoreLayerCollision(6, 11, false);
        }
        else
        {
            isGod = true;
            spriteRenderer.color = new Color (1, 1, 1, 0.75f);
            Physics2D.IgnoreLayerCollision(6, 11);
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        if (isGroundedChecker != null) { Gizmos.DrawWireSphere(isGroundedChecker.position, checkGroundRadius); }
    }
    
    
   
    
}
