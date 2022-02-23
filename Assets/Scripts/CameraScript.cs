using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public float interpVelocity;
    public float delay = 0.2f;
    [SerializeField]public float speed = 15f;
    public GameObject target;
    public Vector3 offset;
    Vector3 targetPos;
    
    public AudioSource audioSource;
    public AudioClip gameOverMusic;
    public AudioClip soundrtack;
    
    void Start()
    {
        targetPos = transform.position;
        offset = new Vector3(0, 1, 0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (target && target.GetComponent<PlayerMovement>().health > 0)
        {
            Vector3 posNoZ = transform.position;
            posNoZ.z = target.transform.position.z;

            Vector3 targetDirection = (target.transform.position - posNoZ);

            interpVelocity = targetDirection.magnitude * speed;
            

            targetPos = transform.position + (targetDirection.normalized * interpVelocity * Time.deltaTime); 

            transform.position = Vector3.Lerp( transform.position, targetPos + offset, delay);

        }
    }

    public void GameOver()
    {
        //audioSource.clip = gameOverMusic;
        //audioSource.Play();
    }

}
