using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UnitParameterSetter : Observer
{
	private Unit[] _units;

	[SerializeField] private Slider acceleration;
	[SerializeField] private Text accelerationText;
	
	[SerializeField] private Slider movementSpeed;
	[SerializeField] private Text movementSpeedText;
	
	[SerializeField] private Slider radius;
	[SerializeField] private Text radiusText;
	
	[SerializeField] private Slider distance;
	[SerializeField] private Text distanceText;

	public Unit[] GetUnits()
	{
		return _units;
	}
	
	protected override void Start()
	{
		base.Start();
		radius.interactable = acceleration.interactable = movementSpeed.interactable = distance.interactable = false;
		_units = FindObjectsOfType<Unit>();
	}

	public override void Notify(GameState state)
	{
		if (state != GameState.Running)
			radius.interactable = acceleration.interactable = movementSpeed.interactable = distance.interactable = false;
		
		radius.interactable = acceleration.interactable = movementSpeed.interactable = distance.interactable = true;
		
		Init();
	}

	private void Init()
	{
		acceleration.onValueChanged.AddListener(delegate 
		{ 
			Array.ForEach(_units, unit => unit.Acceleration = acceleration.value);
			accelerationText.text = acceleration.value.ToString();
		});
	    
		movementSpeed.onValueChanged.AddListener(delegate
		{
			Array.ForEach(_units, unit => unit.MovementSpeed = movementSpeed.value);
			movementSpeedText.text = movementSpeed.value.ToString();
		});
	    
		radius.onValueChanged.AddListener(delegate
		{
			Array.ForEach(_units, unit => unit.Radius = radius.value);
			radiusText.text = radius.value.ToString();
		});
	    
		distance.onValueChanged.AddListener(delegate
		{
			Array.ForEach(_units, unit => unit.Distance = distance.value);
			distanceText.text = distance.value.ToString();
		});
	}
}
