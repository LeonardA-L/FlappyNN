using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : Singleton<ObstacleManager>
{
    public ObstacleController _obstaclePrefab = null;
    public Transform _obstacleWrapper = null;

    public float _interval = 3;
    public float _intervalRuntime = 3;
    public int _rampCount = 10;
    public float _rampImpact = 0.1f;
    public float _xStart = 10f;
    public float _yRange = 1f;
    public float _yRangeRuntime = 1f;
    public int _yRangeRampCount = 5;
    public float _yRangeRampImpact = 0.2f;
    public float _obstacleYRange = 3.33f;

    public float _t = 0;

    public int _count = 0;

    private List<ObstacleController> _obstacles = null;

    // Start is called before the first frame update
    void Start()
    {
        _intervalRuntime = _interval;
        _obstacles = new List<ObstacleController>();
    }

    // Update is called once per frame
    void Update()
    {
        _t += Time.deltaTime;
        if (_t > _intervalRuntime)
        {
            Spawn();
            _t = 0;
            _count++;
            if(_count % _rampCount == 0)
            {
                _intervalRuntime -= _rampImpact;
            }
            if (_count % _yRangeRampCount == 0)
            {
                _yRangeRuntime += _yRangeRampImpact;
                _yRangeRuntime = Mathf.Clamp(_yRangeRuntime, -_obstacleYRange, _obstacleYRange);
            }
        }
    }

    private void Spawn()
    {
        var instance = Instantiate(_obstaclePrefab, _obstacleWrapper);
        instance.transform.position = new Vector3(_xStart, Random.Range(-_yRangeRuntime, _yRangeRuntime), 0);
        _obstacles.Add(instance);
    }
    private void ClearField()
    {
        _obstacles.Clear();
        foreach (Transform child in _obstacleWrapper)
        {
            Destroy(child.gameObject);
        }
    }

    public void Remove(ObstacleController obstacle)
    {
        _obstacles.Remove(obstacle);
    }

    public void Reset()
    {
        ClearField();
        _t = 0;
        _intervalRuntime = _interval;
        _yRangeRuntime = _yRange;
    }

    public Vector3 GetClosestObstacle(Vector3 position)
    {
        if(_obstacles.Count == 0)
        {
            return new Vector3(1000f, 0, 0);
        }
        float minDistance = 10000f;
        Vector3 minObstacle = _obstacles[0].transform.position;
        foreach (var obstacle in _obstacles)
        {
            float distance = obstacle.transform.position.x - position.x;
            if (distance > 0 && distance < minDistance)
            {
                minDistance = distance;
                minObstacle = obstacle.transform.position;
            }
        }
        return minObstacle;
    }
}
