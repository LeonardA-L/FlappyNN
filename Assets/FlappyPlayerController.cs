using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlappyPlayerController : MonoBehaviour
{
    private FlappyController _controller;

    // Start is called before the first frame update
    void Awake()
    {
        _controller = GetComponent<FlappyController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _controller.Jump();
        }
    }
}
