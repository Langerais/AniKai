using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    
    //Currently does not work (WIP)
    
    private float climbSpeed = 6;
    private float climbDelay = 1;
    private float lastClimb;
    private Rigidbody2D playerRb;
    private GameObject player;
    private Transform playerTransform;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player_1");
        playerTransform = player.GetComponent<Transform>();
        lastClimb = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void OnTriggerStay2D (Collider2D other)
    {
        playerRb = other.GetComponent<Rigidbody2D>();
        
        if (other.CompareTag("Player") && Input.GetKey (KeyCode.W)) {
            playerRb.velocity = new Vector2 (0, 0);
            playerTransform.Translate(new Vector3(0,climbSpeed));
            other.GetComponent<Animator>().SetBool("Climbing", true);
            playerRb.gravityScale = 0;
        } else if (other.CompareTag("Player") && Input.GetKey (KeyCode.S)) {
            playerRb.velocity = new Vector2 (0, 0);
            playerTransform.Translate(new Vector3(0,-climbSpeed));
            other.GetComponent<Animator>().SetBool("Climbing", true);
            playerRb.gravityScale = 0;
        }
        else if(other.GetComponent<Animator>().GetBool("Climbing"))
        {
            other.GetComponent<Rigidbody2D> ().velocity = new Vector2 (0, 0);
            playerRb.gravityScale = 0;
        }


    }

    void OnTriggerExit(Collider other)
    {
        playerRb = other.GetComponent<Rigidbody2D>();
        other.GetComponent<Animator>().SetBool("Climbing", false);
        playerRb.gravityScale = 5;
        lastClimb = Time.time;
        
    }

    bool ClimbTimer()
    {
        if (Time.time > lastClimb + climbDelay)
        {
            return true;
        }

        return false;
    }
}
