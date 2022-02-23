using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class TitleScreenManager : MonoBehaviour
{
    public Transform titleText;
    public Transform titleEndSpot;
    private Vector2 startPos;
    private float timePassed = 0;
    public Text pressAnyKey;
    private float anyKeyAppearDelay = 1.5f;
    private float anyKeyTransparency;
    private float anyKeyBlinkTime;
    private float anyKeyBlinkDelay = 0.5f;
    private bool anyKeyAppeared;
    public Transform background;
    private Vector2 backgroundStartPos;
    private Vector2 backgroundEndPos;
    public Transform backgroundEndPoint;
    
    void Start()
    {
        anyKeyTransparency = 0;
        startPos = titleText.position;
        anyKeyAppeared = false;
        anyKeyBlinkTime = Time.time;
        backgroundStartPos = background.position;
        backgroundEndPos.x = backgroundEndPoint.position.x;
        backgroundEndPos.y = backgroundStartPos.y;
    }
    
    void Update()
    {
        timePassed += Time.deltaTime;
        
        TitleAnimate();
        AnyKeyTextAnimate();
        
        pressAnyKey.color = new Color(1, 1, 1, anyKeyTransparency);


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        } 
        else if (anyKeyAppeared && Input.anyKey)
        {
            StartTheGame();
        }
        

    }

    void TitleAnimate()
    {
        titleText.position = Vector3.Lerp(startPos, titleEndSpot.position,
            Mathf.PingPong(timePassed, 1));
        background.position = Vector3.Lerp(backgroundStartPos, backgroundEndPos,
            Mathf.PingPong(timePassed / 60, 1f));
    }

    void AnyKeyTextAnimate()
    {
        if (anyKeyTransparency >= 1)
        {
            anyKeyAppeared = true;
        }
        
        if (timePassed > anyKeyAppearDelay && !anyKeyAppeared)
        {
            anyKeyTransparency += 0.025f;
        }

        if (anyKeyAppeared && anyKeyBlinkTime + anyKeyBlinkDelay < Time.time)
        {
            anyKeyBlinkTime = Time.time;
            
            if (anyKeyTransparency != 1)
            {
                anyKeyTransparency = 1;
            }
            else
            {
                anyKeyTransparency = 0;
            }
        }
        
    }

    void StartTheGame()
    {
        SceneManager.LoadScene("Game");
    }
    
    
    
}
