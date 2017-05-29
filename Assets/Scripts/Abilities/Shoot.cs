using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : Ability
{
	protected const float MAX_DISTANCE = 100f;
	protected Gun gun;
	public float damage = 1f;
	public LayerMask lm;
	protected virtual bool setup(GameObject go)
	{
		gun = findGun(go);
		if(gun != null)
		{
			return true;
		}

		return false;
	}

	protected Gun findGun(GameObject go)
	{
		return go.GetComponentInChildren<Gun>();
	}
	
	public override bool activate(GameObject go)
    {
		if(gun == null)
		{
			if(!setup(go))
			{
				return false;
			}
		}

		return base.activate(go);
	}
}
