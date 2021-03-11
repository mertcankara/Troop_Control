using System;
using UnityEngine;

public abstract class Observer : MonoBehaviour
{
    public virtual void Notify(GameState state) { subject.Unsubscribe(this); }
    [SerializeField] protected Subject subject;

    protected virtual void Start()
    {
        subject.Subscribe(this);
    }
}