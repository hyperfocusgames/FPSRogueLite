using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : MonoBehaviour
{
	protected float lastActivate = float.MinValue;

	public float cooldownTime = 0f;
	public float cost = 0f;
	public GameObject effect;
	public float effectTime;
	public virtual bool activate(GameObject go)
	{
		if(ready())
		{
			if(effect != null)
			{
				// Create effect and destroy it when complete
				Destroy(Instantiate(effect, transform.position, transform.rotation, transform), effectTime);
			}

			lastActivate = Time.time;
			return true;
		}

		return false;
	}

	public float Cost
	{
		get
		{
			return cost;
		}
		protected set
		{
			cost = value;
		}
	}

	protected bool ready()
	{
		if(Time.time - lastActivate > cooldownTime && (Cost <= 0 || Controller.instance.Energy > 0))
		{
			return true;
		}

		return false;
	}
}
