using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlappyController : MonoBehaviour
{
    private Rigidbody2D _body;
    private bool _dead;
    public bool IsDead => _dead;
    private float _t = 0;
    public float _debounce = 0.2f;
    public float _score = 0;
    public float Score => _score;
    public Vector2 _upforce;
    public SpriteRenderer _sprite = null;

    // Start is called before the first frame update
    void Awake()
    {
        _body = GetComponent<Rigidbody2D>();
    }

    public void SetChampion()
    {
        _sprite.color = Color.red;
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
            Die();
        }
    }

    public void Die()
    {

        _dead = true;
        if (GetComponent<FlappyAIController>() != null) {
            GameManager.Instance.RegisterDeath(this);
        }
        gameObject.SetActive(false);
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

    internal void ResetVelocity()
    {
        _body.velocity = Vector3.zero;
    }
}
