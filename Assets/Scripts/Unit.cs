using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Unit : Observer
{
    private enum TargetMode
    {
        Vector,
        Transform
    }

    [SerializeField] protected NavMeshAgent agent;
    
    private Transform _targetTransform;
    private Vector3 _targetVector;
    private TargetMode _targetMode;
    [SerializeField] private GameObject ring;
    private GameController _gameController;

    protected float DefaultAcceleration;
    protected float DefaultMovementSpeed;
    protected float DefaultRadius;

    public void SetTarget(Transform target)
    {
        _targetTransform = target.transform;
        _targetMode = TargetMode.Transform;
    }

    public void SetTarget(Vector3 target)
    {
        _targetVector = target;
        _targetMode = TargetMode.Vector;
    }

    protected override void Start()
    {
        base.Start();
        DefaultAcceleration = 8f;
        DefaultMovementSpeed = 10f;
        DefaultRadius = 1f;
        _gameController = (GameController) subject;
        _targetTransform = transform;
        _targetMode = TargetMode.Transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (!agent.enabled) return;

        switch (_targetMode)
        {
            case TargetMode.Transform:
                agent.SetDestination(_targetTransform.position);
                break;
            case TargetMode.Vector:
                agent.SetDestination(_targetVector);
                break;
        }
    }

    public void SetSelected(bool isSelected)
    {
        ring.SetActive(isSelected);
    }

    public override void Notify(GameState state)
    {
        if (state != GameState.Running)
        {
            agent.enabled = false;
            return;
        }
        agent.enabled = true;
    }

    public float MovementSpeed
    {
        get => agent.speed;
        set => agent.speed = DefaultMovementSpeed * value;
    }

    public float Radius
    {
        get => agent.radius;
        set => agent.radius = DefaultRadius * value;
    }

    public float Acceleration
    {
        get => agent.acceleration;
        set => agent.acceleration = DefaultAcceleration * value;
    }
    
    public float Distance
    {
        get => agent.stoppingDistance;
        set => agent.stoppingDistance = value;
    }

    protected void SetDefaults(float movementSpeed, float acceleration, float radius)
    {
        DefaultMovementSpeed = movementSpeed;
        DefaultAcceleration = acceleration;
        DefaultRadius = radius;

        MovementSpeed = 0.5f;
        Acceleration = 0.5f;
        Radius = 0.5f;
    }
}
    
