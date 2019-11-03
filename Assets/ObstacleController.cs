using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    public float _xmin = 0;
    // Start is called before the first frame update
    // Start is called before the first frame update
    void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(-1, 0, 0) * GameManager.Instance.Speed * Time.deltaTime;
        if (transform.position.x < _xmin)
        {
            ObstacleManager.Instance.Remove(this);
            Destroy(gameObject);
        }
    }
}
