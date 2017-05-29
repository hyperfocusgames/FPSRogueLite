using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacePlayer : MonoBehaviour
{
	void Update()
	{
		Vector3 tar = Controller.instance.transform.position;
		tar.y = transform.position.y;
		transform.LookAt(tar);
	}
}
