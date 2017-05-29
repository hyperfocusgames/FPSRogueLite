using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour
{
	protected Slider s;

	void Awake()
	{
		s = GetComponentInChildren<Slider>();
	}

	public float Value
	{
		get
		{
			return s.value;
		}
		set
		{
			s.value = value;
		}
	}

	public float Max
	{
		get
		{
			return s.maxValue;
		}
		set
		{
			s.maxValue = value;
		}
	}

	public float Min
	{
		get
		{
			return s.minValue;
		}
		set
		{
			s.minValue = value;
		}
	}
}
