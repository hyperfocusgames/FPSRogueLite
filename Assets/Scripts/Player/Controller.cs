using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Controller : MonoBehaviour {
	protected const string forwardAxis = "Vertical";
	protected const string rightAxis = "Horizontal";
	protected const string jumpAxis = "Jump";
	protected const float horizontalSnapPercent = .5f;
	protected const float horizontalSpeedPercent = 1f;
	protected const float maxLateralMagnitude = 50f;

	public float speed = 5f;
	public float jumpForce = 1f;
	public float xSens = 1f;
	public float ySens = 1f;
	public LayerMask jumpLM;

	protected Rigidbody rb;
	protected Camera cam;
	protected bool grounded = true;
	void Awake ()
	{
		rb = GetComponent<Rigidbody>();
		cam = GetComponentInChildren<Camera>();

		lockMouse();
	}

	void Update ()
	{
		move();
		rotate();
		jump();

		capLateralSpeed();
	}

	public void lockMouse()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	public void unlockMouse()
	{
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	protected void move()
	{
		Vector3 localVel = transform.InverseTransformDirection(rb.velocity);
		float forwardInput = Input.GetAxis(forwardAxis);
		if(forwardInput != 0)
		{
			localVel.z = speed * forwardInput;
		}

		float rightInput = Input.GetAxis(rightAxis);
		if(rightInput != 0)
		{
			float rightVal = horizontalSpeedPercent * speed * rightInput;
			localVel.x = rightVal;
		}

		rb.velocity = transform.TransformDirection(localVel);
	}

	protected void rotate()
	{
		float yRot = -(Input.GetAxis("Y") * ySens);
		cam.transform.Rotate(new Vector3(1, 0, 0), yRot, Space.Self);

		float xRot =  (Input.GetAxis("X") * xSens);
		transform.Rotate(new Vector3(0, 1, 0), xRot, Space.World);
	}

	protected void jump()
	{
		if(Input.GetAxisRaw(jumpAxis) != 0)
		//if(Input.GetButtonDown(jumpAxis))
		{
			if(Physics.Raycast(transform.position, -transform.up, .3f, jumpLM))
			{
				Debug.DrawLine(transform.position, transform.position - (transform.up * .5f), Color.red, 1f);

				Vector3 vel = rb.velocity;
				vel.y = jumpForce * (-Physics.gravity.y);
				rb.velocity = vel;
			}
		}
	}

	protected void capLateralSpeed()
	{
		Vector2 lateralVel = new Vector2(rb.velocity.x, rb.velocity.z);
		if(lateralVel.magnitude > maxLateralMagnitude)
		{
			lateralVel = lateralVel.normalized * maxLateralMagnitude;
		}

		rb.velocity = new Vector3(lateralVel.x, rb.velocity.y, lateralVel.y);
	}
}
