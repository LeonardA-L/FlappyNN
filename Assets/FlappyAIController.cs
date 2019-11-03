using NN;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlappyAIController : MonoBehaviour
{
    public readonly float SCREEN_HEIGHT = 10;
    public readonly float SCREEN_WIDTH = 17.8f;
    public readonly float OBSTACLE_RANGE = 3.33f;

    private FlappyController _controller;
    private Genome _genome;
    public Genome Genome => _genome;
    public float _normalizedHeight = 0;
    public float _normalizedDistance = 0;
    public float _normalizedObsAltitude = 0;

    // Start is called before the first frame update
    void Awake()
    {
        _controller = GetComponent<FlappyController>();
    }

    public void SetGenome(Genome genome)
    {
        _genome = genome;
    }

    private bool Think()
    {
        if (_genome == null)
            return false;
        var outputs = _genome.FeedForward(new List<float> { GetNormalizedAltitude(), GetNormalizedDistanceToObstacle(), GetNormalizedAltitudeOfObstacle() });
        return outputs[0] > 0.6f;
    }

    private float GetNormalizedAltitude()
    {
        //return Mathf.Clamp((transform.position.y + SCREEN_HEIGHT / 2f) / SCREEN_HEIGHT, 0f, 1f);
        return Mathf.Clamp((transform.position.y) / (SCREEN_HEIGHT/2f), -1f, 1f);
    }

    private float GetNormalizedDistanceToObstacle()
    {
        return Mathf.Clamp(ObstacleManager.Instance.GetClosestObstacle(this.transform.position).x / (SCREEN_WIDTH / 2f), 0f, 1f);
    }

    private float GetNormalizedAltitudeOfObstacle()
    {
        var obstacleAltitude = ObstacleManager.Instance.GetClosestObstacle(this.transform.position).y;
        obstacleAltitude /= OBSTACLE_RANGE;
        return Mathf.Clamp(obstacleAltitude, -1f, 1f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _normalizedHeight = GetNormalizedAltitude();
        _normalizedDistance = GetNormalizedDistanceToObstacle();
        _normalizedObsAltitude = GetNormalizedAltitudeOfObstacle();
        if (_controller.IsDead)
            return;
        if(Think())
        {
            _controller.Jump();
        }
    }
}
