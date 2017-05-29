using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootProjectile : Shoot
{
	public GameObject projectile;

	public override bool activate(GameObject go)
	{
		if(base.activate(go))
		{
			Instantiate(projectile, gun.Tip, transform.rotation);
		}

		return false;
	}

}
