using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : MonoBehaviour
{
	protected float lastActivate = float.MinValue;

	public float cooldownTime = 0f;
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

	protected bool ready()
	{
		if(Time.time - lastActivate > cooldownTime)
		{
			return true;
		}

		return false;
	}
}
