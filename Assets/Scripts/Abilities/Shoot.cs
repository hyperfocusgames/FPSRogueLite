using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : Ability
{
	protected const float MAX_DISTANCE = 100f;
	protected Gun gun;
	protected LineRenderer lr;
	public LayerMask lm;

	protected bool setup(GameObject go)
	{
		gun = findGun(go);
		if(gun != null)
		{
			lr = gun.GetComponentInChildren<LineRenderer>();
			if(lr != null)
			{
				lr.positionCount = 2;
			}

			if(gun != null)
			{
				return true;
			}
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

		if(base.activate(go))
		{
			Vector3 tar;
			RaycastHit hit = new RaycastHit();
			if(Physics.Raycast(transform.position, transform.forward, out hit, MAX_DISTANCE, lm))
			{
				tar = hit.point;
			}
			else
			{
				tar = transform.position + transform.forward * MAX_DISTANCE;
			}

			gun.playEffect(tar, effectTime);
			return true;
		}

		return false;
    }
}
