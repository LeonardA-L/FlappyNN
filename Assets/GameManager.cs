using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private struct SContestants
    {
        public float _score;
        public FlappyController _flappy;
    }

    public float _speed = 1;
    public FlappyController _flappy;
    public int _population = 10;
    public Transform _flappyParent;

    public float Speed => _speed;

    private List<SContestants> _contestants;

    // Start is called before the first frame update
    void Start()
    {
        _contestants = new List<SContestants>();
        Generate();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Generate()
    {
        _contestants.Clear();
        foreach (Transform child in _flappyParent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < _population; i++)
        {
            var flappy = Instantiate(_flappy, _flappyParent);
            flappy.transform.localPosition = Vector3.zero;
        }
    }

    public void RegisterDeath(FlappyController flappy)
    {
        _contestants.Add(new SContestants()
        {
            _score = CalculateFitness(flappy),
            _flappy = flappy
        });

        if(_contestants.Count == _population)
        {
            _contestants.OrderBy((x) => x._score);
            Debug.Log($"MaxScore: {_contestants.Last()._score}");
        }
    }

    private float CalculateFitness(FlappyController flappy)
    {
        return flappy.Score;
    }
}
