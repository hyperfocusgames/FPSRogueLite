using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
	public Vector3 tipOffset;
	public LineRenderer trail;
	public Light flash;
	public Vector3 flashOffset;
	public float volume;

	protected AudioSource source;

	void Awake()
	{
		source = GetComponent<AudioSource>();
	}

	public Vector3 Tip
	{
		get
		{
			return transform.TransformPoint(tipOffset);
		}
		set
		{
			tipOffset = transform.InverseTransformPoint(value);
		}
	}

	public void playEffect(Vector3 target, float time)
	{
		Destroy(Instantiate(flash, transform.TransformPoint(flashOffset + tipOffset), transform.rotation).gameObject, time);
		
		LineRenderer lr = Instantiate(trail, Tip, transform.rotation);
		lr.positionCount = 2;
		lr.SetPosition(0, Tip);
		lr.SetPosition(1, target);
		Destroy(lr, time);

		source.Play();
	}
}
