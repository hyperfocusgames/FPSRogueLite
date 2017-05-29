using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootRaycast : Shoot
{
	protected LineRenderer lr;

	protected override bool setup(GameObject go)
	{
		if(base.setup(go))
		{
			lr = gun.GetComponentInChildren<LineRenderer>();
			if(lr != null)
			{
				lr.positionCount = 2;
			}
			return true;
		}

		return false;
	}

    public override bool activate(GameObject go)
    {
		if(base.activate(go))
		{
			Vector3 tar;
			RaycastHit hit = new RaycastHit();
			if(Physics.Raycast(transform.position, transform.forward, out hit, MAX_DISTANCE, lm))
			{
				tar = hit.point;

				Entity tarEntity = hit.collider.gameObject.GetComponent<Entity>();
				if(tarEntity != null)
				{
					tarEntity.damage(damage, hit.collider);
				}
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
