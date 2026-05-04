using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverAnimation : MonoBehaviour
{
    public float amplitude = 0.5f; // How high/low it goes
    public float speed = 1f;       // How fast it bobs
    
    private Vector3 startPos;

    void Start()
    {
        // Store the starting position to hover around it
        startPos = transform.position;
    }

    void Update()
    {
        // Calculate new Y position using a sine wave
        float newY = startPos.y + Mathf.Sin(Time.time * speed) * amplitude;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }
}
