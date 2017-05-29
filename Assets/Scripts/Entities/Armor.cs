using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor : MonoBehaviour
{
	public float resistance = 0f;

	public float damage(float value)
	{
		return value - (value * resistance);
	}
}
