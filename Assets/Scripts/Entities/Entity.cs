using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
	protected const string PP_BITS_KEY = "bits";

	protected const float MAX_VIEW_ANGLE = 90f;
	protected const float MIN_VIEW_ANGLE = -90f;

	protected float energy;
	protected float energyRegenTime = 2f;
	protected float energyRegenRate = 2f;
	protected float lastEnergyUsed = float.MinValue;
	protected float health;
	protected float jumpForce = .5f;
	protected Vector3 targetOffset = Vector3.up;
	public GameObject viewModel;
	public LayerMask visibilityMask;
	public Bar energyBar;
	public Bar healthBar;
	public FloatDisplay bitsDisplay;
	public float acceleration = 7f;
	public float maxSpeed = 14f;
	public float maxEnergy = 10f;
	public float maxHealth = 10f;
	public float bits = 0f;
	public Ability[] abilities;

	public float Energy
	{
		get
		{
			return energy;
		}
		set
		{
			value = Mathf.Min(value, MaxEnergy);
			if(value < energy)
			{
				lastEnergyUsed = Time.time;
			}

			energy = value;
			if(energyBar != null)
			{
				energyBar.Value = Energy;
			}
		}
	}

	public float MaxEnergy
	{
		get
		{
			return maxEnergy;
		}
		set
		{
			maxEnergy = value;
			if(energyBar != null)
			{
				energyBar.Max = MaxEnergy;
			}
		}
	}

	public float Health
	{
		get
		{
			return health;
		}
		set
		{
			value = Mathf.Min(value, maxHealth);

			health = value;
			if(healthBar != null)
			{
				healthBar.Value = Health;
			}
		}
	}

	public float MaxHealth
	{
		get
		{
			return maxHealth;
		}
		set
		{
			maxHealth = value;
			if(healthBar != null)
			{
				healthBar.Max = MaxHealth;
			}
		}
	}

	public float Acceleration
	{
		get
		{
			return acceleration;
		}
		set
		{
			acceleration = value;
		}
	}

	public float JumpForce
	{
		get
		{
			return jumpForce;
		}
		set
		{
			jumpForce = value;
		}
	}

	public Vector3 TarPos
	{
		get
		{
			return transform.position + targetOffset;
		}
		set
		{
			targetOffset = value;
		}
	}

	public float Bits
	{
		get
		{
			return bits;
		}
		set
		{
			bits = value;
			if(bitsDisplay != null)
			{
				bitsDisplay.Value = Bits;
			}

			if(isPlayer())
			{
				PlayerPrefs.SetFloat(PP_BITS_KEY, Bits);
			}
		}
	}

	public Ability[] Abilities
	{
		get
		{
			return abilities;
		}
		protected set
		{
			abilities = value;
		}
	}

	protected void setupEnergy()
	{
		MaxEnergy = MaxEnergy;
		Energy = MaxEnergy;
	}

	protected void setupHealth()
	{
		MaxHealth = MaxHealth;
		Health = MaxHealth;
	}

	protected void setupBits()
	{
		Bits = Bits;
	}

	void Start()
	{
		setupEnergy();
		setupHealth();
		setupBits();
	}

	void Update()
	{
		regenEnergy();
	}

	protected void regenEnergy()
	{
		if(Time.time - lastEnergyUsed > energyRegenTime)
		{
			Energy += Mathf.Min(Time.time - lastEnergyUsed, Time.deltaTime) * energyRegenRate;
		}
	}

	public void damage(float value, Collider hit)
	{
		Armor armor = hit.gameObject.GetComponent<Armor>();
		
		if(armor != null)
		{
			value = armor.damage(value);
		}

		Health -= value;

		if(Health <=0)
		{
			die();
		}
	}

	protected virtual void die()
	{
		if(!isPlayer())
		{
			Controller.instance.Entity.Bits += Bits;
		}
	}

	protected bool isPlayer()
	{
		return Controller.instance.gameObject == gameObject;
	}

	public void rotate(float y, float x, Space space)
	{
		transform.Rotate(Vector3.up, y, space);
		viewModel.transform.Rotate(Vector3.right, x, space);

		Vector3 rot = viewModel.transform.localEulerAngles;
		rot.x = (rot.x > 180) ? -360 + rot.x : rot.x;
		rot.x = Mathf.Min(Mathf.Max(rot.x, MIN_VIEW_ANGLE), MAX_VIEW_ANGLE);

		viewModel.transform.localRotation = Quaternion.Euler(rot.x, 0, 0);
	}

	public void rotate(Vector2 amount, Space space)
	{
		rotate(amount.x, amount.y, space);
	}

	public void lookAt(Vector3 pos)
	{
		Vector3 xzPos = new Vector3(pos.x, transform.position.y, pos.z);
		transform.LookAt(xzPos);

		viewModel.transform.LookAt(pos);
	}

	public bool isVisable(GameObject tar)
	{
		return isVisable(tar.gameObject, tar.transform.position);
	}

	public bool isVisable(Entity tar)
	{
		return isVisable(tar.gameObject, tar.TarPos);
	}

	public bool isVisable(GameObject go, Vector3 tar)
	{
		Debug.DrawLine(viewModel.transform.position, tar, Color.black, .1f);
		RaycastHit hit = new RaycastHit();
		if(Physics.Linecast(viewModel.transform.position, tar, out hit, visibilityMask))
		{
			foreach(Collider c in go.GetComponentsInChildren<Collider>())
			{
				// If c is in tar's colliders, return true.
				if(hit.collider == c)
				{
					return true;
				}
			}

			// Something hit, not target. Return false.
			return false;
		}

		// If no hit, nothing blocking view so default to true.
		return true;
	}
}
