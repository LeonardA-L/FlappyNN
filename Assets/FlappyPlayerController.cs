using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlappyPlayerController : MonoBehaviour
{
    public KeyCode _key;
    private FlappyController _controller;

    // Start is called before the first frame update
    void Awake()
    {
        _controller = GetComponent<FlappyController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<FlappyAIController>() != null)
        {
            return;
        }
        
        if (Input.GetKeyDown(_key))
        {
            _controller.Jump();
        }
    }
}
