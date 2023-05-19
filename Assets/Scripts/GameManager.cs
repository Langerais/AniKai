using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    //public sc
    public GameObject player;
    [SerializeField] public int playerHp;
    public AudioSource audioSource;
    public AudioClip backgroundMusic;
    public AudioClip gameOverMusic;
    public GameObject blackScreen;
    public bool gameIsOver = false;
    private float blackScreenAlpha = 0;
    private float blackScreenFadeStart;
    private float restartDelay = 3f;
    private bool canRestart;
    public Text gameOverText;
    public Text restartGameText;
    public GameObject energyBar;
    public GameObject healthBar;

    private void Start()
    {
        playerHp = player.GetComponent<PlayerMovement>().health;
        audioSource.clip = backgroundMusic;
        audioSource.Play();
        canRestart = false;
    }
    
    private void Update()
    {
        playerHp = player.GetComponent<PlayerMovement>().health;
        
        if (playerHp <= 0 && !gameIsOver) { GameOver(); }

        if (Time.time > blackScreenFadeStart + restartDelay && gameIsOver)
        {
            canRestart = true;
            restartGameText.color = new Color(1, 1, 1, blackScreenAlpha);
        }
        
        if (Input.GetKeyDown(KeyCode.Escape)) { SceneManager.LoadScene("Starting Screen"); }
        
        if (Input.anyKey && canRestart)
        {
            canRestart = false;
            RestartGame();
        }
        
        BlackScreenReveal();

        blackScreen.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, blackScreenAlpha);
        gameOverText.color = new Color(1, 1, 1, blackScreenAlpha - 0.002f);
    }


    private void GameOver()
    {
        if (gameIsOver) return;
        
        BlackScreenReveal();
        gameIsOver = true;
        blackScreenFadeStart = Time.time;
        audioSource.Pause();
        audioSource.clip = gameOverMusic;
        audioSource.loop = false;
        audioSource.PlayDelayed(0.5f);
        Destroy(energyBar);
        Destroy(healthBar);

    }

    private void RestartGame()
    {
        restartGameText.enabled = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void BlackScreenReveal()
    {
        if (gameIsOver && blackScreenAlpha < 1) { blackScreenAlpha += 0.005f; }
    }
}
