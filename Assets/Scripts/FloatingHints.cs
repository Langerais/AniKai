using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingHints : MonoBehaviour
{

    private Vector2 startPos;
    private Vector2 endPos;

    
    void Start()
    {
        startPos = transform.position;
        endPos = new Vector2(startPos.x, startPos.y - 0.25f);
    }
    
    void Update()
    {
        transform.position = Vector3.Lerp(startPos, endPos,
            Mathf.PingPong(Time.time, 1));
    }
    
    
}
