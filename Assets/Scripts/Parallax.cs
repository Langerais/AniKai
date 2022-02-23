using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    
    private float length, height, startposX, startposY;
    public GameObject camera;
    private Transform cameraTransform;
    [SerializeField]public float parallaxEffectX;
    [SerializeField]public float parallaxEffectY;
    
    void Start()
    {
        startposX = transform.position.x;
        startposY = transform.position.y;
        cameraTransform = camera.transform;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
        height = GetComponent<SpriteRenderer>().bounds.size.y;
        parallaxEffectX *= 0.01f;
        parallaxEffectY *= 0.01f;
    }

    
    void FixedUpdate()
    {
        float temp = (camera.transform.position.x * (1 - parallaxEffectX));
        float distX = (cameraTransform.position.x * parallaxEffectX);
        float distY = (cameraTransform.position.y * parallaxEffectY);

        transform.position = new Vector3(startposX - distX, startposY - distY, transform.position.z);

        if (temp > startposX + length) startposX += length;
        else if (temp < startposX - length) startposX -= length;


    }
}
