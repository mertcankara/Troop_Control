using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Subject : MonoBehaviour
{
    protected List<Observer> Observers;

    protected void Awake()
    {
        Observers = new List<Observer>();
    }

    public void Subscribe(Observer observer)
    {
        Observers.Add(observer);
    }

    public void Unsubscribe(Observer observer)
    {
        Observers.Remove(observer);
    }
}       