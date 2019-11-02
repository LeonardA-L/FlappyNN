using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallController : MonoBehaviour
{
    public float _xmin = 0;
    public float _xmax = 0;
    private float _y = 0;

    // Start is called before the first frame update
    void Start()
    {
        _y = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(-1, 0, 0) * GameManager.Instance.Speed * Time.deltaTime;
        if (transform.position.x < _xmin)
        {
            transform.position = new Vector3(_xmax, _y, 0);
        }
    }
}
