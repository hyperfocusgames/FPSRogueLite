using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Controller : MonoBehaviour {
	protected const string FORWARD_AXIS = "Vertical";
	protected const string RIGHT_AXIS = "Horizontal";
	protected const string JUMP_AXIS = "Jump";
	protected const float HORIZONTAL_SNAP_PERCENT = .5f;
	protected const float HORIZONTAL_SPEED_PERCENT = 1f;
	protected const float MAX_LATERAL_MAGNITUDE = 50f;
	protected const float MAX_ACCELERATION = 50f;

	public float acceleration = 7f;
	public float jumpForce = .5f;
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
		rotate();
		jump();

		//capLateralSpeed();
	}

	void FixedUpdate()
	{
		move();
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
		Vector3 input = transform.TransformDirection(new Vector3(Input.GetAxis(RIGHT_AXIS), 0, Input.GetAxis(FORWARD_AXIS)));

		rb.velocity = Accelerate(input, rb.velocity, acceleration, MAX_ACCELERATION);
	}

	protected Vector3 accelerate(Vector3 dir, Vector3 vel)
	{
		float proj = Vector3.Dot(vel, dir);
		float accelVel = acceleration * Time.fixedDeltaTime;

		if(proj + accelVel > MAX_ACCELERATION)
		{
			accelVel = MAX_ACCELERATION - proj;
		}

		return vel + dir * accelVel;
	}

	private Vector3 Accelerate(Vector3 accelDir, Vector3 prevVelocity, float accelerate, float max_velocity)
	{
		float projVel = Vector3.Dot(prevVelocity, accelDir); // Vector projection of Current velocity onto accelDir.
		float accelVel = accelerate * Time.fixedDeltaTime; // Accelerated velocity in direction of movment

		// If necessary, truncate the accelerated velocity so the vector projection does not exceed max_velocity
		if(projVel + accelVel > max_velocity)
			accelVel = max_velocity - projVel;

		return prevVelocity + accelDir * accelVel;
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
		if(Input.GetAxisRaw(JUMP_AXIS) != 0)
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
		if(lateralVel.magnitude > MAX_LATERAL_MAGNITUDE)
		{
			lateralVel = lateralVel.normalized * MAX_LATERAL_MAGNITUDE;
		}

		rb.velocity = new Vector3(lateralVel.x, rb.velocity.y, lateralVel.y);
	}
}
