using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlappyController : MonoBehaviour
{
    private Rigidbody2D _body;
    private bool _dead;
    private float _t = 0;
    public float _debounce = 0.2f;
    public float _score = 0;
    public float Score => _score;
    public Vector2 _upforce;

    // Start is called before the first frame update
    void Awake()
    {
        _body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        _t -= Time.deltaTime;

        if(_dead)
            return;
        _score += Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Obstacle")
        {
            _dead = true;
            GameManager.Instance.RegisterDeath(this);
            gameObject.SetActive(false);
        }
    }

    internal void Jump()
    {
        if(_t > 0)
        {
            return;
        }
        _body.velocity = _upforce;
        _t = _debounce;
    }
}
