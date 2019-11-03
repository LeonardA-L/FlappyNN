using NN;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private struct SContestants
    {
        public float _score;
        public Genome _genome;
    }

    public float _speed = 1;
    public float _speedRuntime = 1;
    public FlappyController _flappy;
    public Transform _flappyParent;

    public float Speed => _speedRuntime;

    public int _population = 10;
    public int _championCount = 3;
    public int _randomCount = 3;
    public float _chanceOfCrossover = 0.75f;
    public float _mutationForce = 0.3f;
    public static float MutationForce = 0;

    public List<FlappyController> _players = null;

    private List<SContestants> _contestants;
    private List<FlappyController> _alives;

    public TextMeshProUGUI _genTxt = null;
    public TextMeshProUGUI _scoreTxt = null;
    private int _genCount = 0;
    private float _maxDistance = 0;
    private float _currentDistance = 0;

    // Start is called before the first frame update
    void Start()
    {
        _speedRuntime = _speed;
        MutationForce = _mutationForce;
        _contestants = new List<SContestants>();
        _alives = new List<FlappyController>();
        Generate(new List<Genome>());
    }

    // Update is called once per frame
    void Update()
    {
        _genTxt.text = $"Gen: {_genCount}\nAlive: {_alives.Count}\nMax Score: {_maxDistance.ToString("0.00")}";
        _scoreTxt.text = _currentDistance.ToString("0.00");
        _currentDistance += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.D))
        {
            foreach (var flappy in _alives)
            {
                flappy.Die();
            }
        }
    }

    private void ClearField()
    {
        _alives.Clear();
        foreach (Transform child in _flappyParent)
        {
            Destroy(child.gameObject);
        }
    }

    void Generate(List<Genome> champs)
    {
        _genCount++;
        ClearField();
        for (int i = 0; i < _population; i++)
        {
            var flappy = Instantiate(_flappy, _flappyParent);
            flappy.transform.localPosition = Vector3.zero;
            _alives.Add(flappy);
            var aIController = flappy.GetComponent<FlappyAIController>();
            Genome genome = new Genome();
            if (i < champs.Count)
            {
                genome.CloneFrom(champs[i]);
                flappy.SetChampion();
            }
            else if (i < champs.Count + _randomCount) { }
            else if(_contestants.Count > 0)
            {
                MakeBaby(ref genome);
            }
            aIController.SetGenome(genome);
        }
        _contestants.Clear();
    }

    private void MakeBaby(ref Genome genome)
    {
        float rand = UnityEngine.Random.value;
        if (rand < (1 - _chanceOfCrossover))
        {
            var player = SelectPlayer();
            genome.CloneFrom(player._genome);
        }
        else
        {
            // rest of the time do crossover
            var parent1 = SelectPlayer();
            var parent2 = SelectPlayer();

            //the crossover function expects the highest fitness parent to be the object and the lowest as the argument
            if (parent1._score < parent2._score)
            {

                genome.Crossover(parent1._genome, parent2._genome);
            }
            else
            {
                genome.Crossover(parent2._genome, parent1._genome);
            }

        }
        genome.Mutate();
    }

    public void RegisterDeath(FlappyController flappy)
    {
        var aIController = flappy.GetComponent<FlappyAIController>();
        if (aIController == null)
            return;
        _contestants.Add(new SContestants()
        {
            _score = CalculateFitness(flappy),
            _genome = aIController.Genome
        });

        _alives.Remove(flappy);

        if(_alives.Count == 0)
        {
            var tmpContestants = new List<SContestants>(_contestants);
            _contestants.Clear();
            _contestants = new List<SContestants>(tmpContestants.OrderBy((x) => x._score));
            //_contestants = new List<SContestants>(_contestants.OrderBy((x) => x._score));
            Debug.Log($"MaxScore: {_contestants.Last()._score}");
            if(_contestants.Last()._score > _maxDistance)
            {
                _maxDistance = _contestants.Last()._score;
            }
            var champions = _contestants.Skip(Mathf.Max(0, _contestants.Count() - _championCount)).Select(c=>c._genome).ToList();
            //var champions = _contestants.Take(_championCount).Select(c=>c._genome).ToList();
            Reset();
            Generate(champions);
        }
    }

    private float CalculateFitness(FlappyController flappy)
    {
        return flappy.Score;
    }

    private SContestants SelectPlayer()
    {
        float fitnessSum = 0;
        for (var i = 0; i < _contestants.Count; i++)
        {
            fitnessSum += _contestants[i]._score;
        }

        var rand = UnityEngine.Random.value * fitnessSum;
        float runningSum = 0;

        for (var i = 0; i < _contestants.Count; i++)
        {
            runningSum += _contestants[i]._score;
            if (runningSum > rand)
            {
                return _contestants[i];
            }
        }
        return _contestants[0];
    }

    public void Reset()
    {
        ObstacleManager.Instance.Reset();
        _speedRuntime = _speed;
        foreach (var player in _players)
        {
            player.transform.position = Vector3.zero;
            player.gameObject.SetActive(true);
            player.ResetVelocity();
        }
    }
}
