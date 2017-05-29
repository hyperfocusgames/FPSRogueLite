using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{
    protected NavMeshAgent agent;
    protected Entity entity;
	protected GameObject aggro;

    void OnEnable()
    {
        setup();
    }

    void Update()
    {
		lookForPlayer();
		active();
    }

	public GameObject Aggro
	{
		get
		{
			return aggro;
		}
		set
		{
			aggro = value;
		}
	}

	public bool IsAggroed
	{
		get
		{
			return Aggro != null;
		}
	}

	public Entity AggroEntity
	{
		get
		{
			return Aggro.GetComponent<Entity>();
		}
	}

    void setup()
    {
        agent = GetComponent<NavMeshAgent>();
        entity = GetComponent<Entity>();

        agent.acceleration = entity.Acceleration;
        agent.speed = entity.Acceleration;
    }

	protected void lookAtTarget()
	{
		Entity d = AggroEntity;
		entity.lookAt(Aggro.transform.position + ((d != null) ? d.TarPos : Vector3.zero));
	}

	protected void moveTowardTarget()
	{
        agent.destination = Aggro.transform.position;
	}

	protected void active()
	{
		if(IsAggroed)
		{
			moveTowardTarget();
       		lookAtTarget();
		}
		else
		{
			idle();
		}
	}

	protected void idle()
	{
		stop();
	}

	protected void stop()
	{
		agent.destination = transform.position;
	}

	protected void lookForPlayer()
	{
		Controller player = Controller.instance;
		if(entity.isVisable(player.Entity))
		{
			aggro = player.gameObject;
		}
		else
		{
			aggro = null;
		}
	}
}
