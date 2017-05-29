using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Controller : MonoBehaviour {
	public static Controller instance;

	protected const string MENU_AXIS = "Menu";
	protected const string FORWARD_AXIS = "Vertical";
	protected const string RIGHT_AXIS = "Horizontal";
	protected const string JUMP_AXIS = "Jump";
	protected const string LOOK_X_AXIS = "X";
	protected const string LOOK_Y_AXIS = "Y";
	protected const string SHOOT_AXIS = "Fire1";
	protected const string ROCKET_AXIS = "Fire2";

	protected const string PP_XSENS_KEY = "xSense";
	protected const string PP_YSENS_KEY = "ySense";

	protected const float HORIZONTAL_SNAP_PERCENT = 1f;
	protected const float HORIZONTAL_SPEED_PERCENT = 1f;
	protected const float MAX_LATERAL_MAGNITUDE = 30f;
	protected const float MAX_ACCELERATION = 10f;
	protected const float GROUND_RAYCAST_DIST = .3f;
	protected const float GRAV_MULT = 2f;

	protected float xSens = 1f;
	protected float ySens = 1f;
	public LayerMask groundLM;
	public LineRenderer velocityLine;
	public Vector3 velocityLineOffset;
	public Vector3 groundCheckOffset = new Vector3(0, .1f, 0);

	protected Rigidbody rb;
	protected Camera cam;
	protected RaycastHit ground;
	protected float jumpCost = 1f;
	protected Entity entity;

	public bool IsGrounded
	{
		get
		{
			return ground.collider != null;
		}
	}

	public float XSens
	{
		get
		{
			return xSens;
		}
		set
		{
			xSens = value;
			PlayerPrefs.SetFloat(PP_XSENS_KEY, XSens);
		}
	}

	public float YSens
	{
		get
		{
			return ySens;
		}
		set
		{
			ySens = value;
			PlayerPrefs.SetFloat(PP_YSENS_KEY, YSens);
		}
	}

	public float Energy
	{
		get
		{
			return entity.Energy;
		}
		protected set
		{
			value = Mathf.Min(value, MaxEnergy);

			entity.Energy = value;
		}
	}

	public float MaxEnergy
	{
		get
		{
			return entity.MaxEnergy;
		}
		protected set
		{
			entity.MaxEnergy = value;
		}
	}

	public float Health
	{
		get
		{
			return entity.Health;
		}
		protected set
		{
			value = Mathf.Min(value, entity.MaxHealth);

			entity.Health = value;
		}
	}

	public float MaxHealth
	{
		get
		{
			return entity.MaxHealth;
		}
		protected set
		{
			entity.MaxHealth = value;
		}
	}

	public float Acceleration
	{
		get
		{
			return entity.Acceleration;
		}
		protected set
		{
			entity.Acceleration = value;
		}
	}

	public float JumpForce
	{
		get
		{
			return entity.JumpForce;
		}
		protected set
		{
			entity.JumpForce = value;
		}
	}

	public Entity Entity
	{
		get
		{
			return entity;
		}
		protected set
		{
			entity = value;
		}
	}

	public Ability[] Abilities
	{
		get
		{
			return entity.Abilities;
		}
	}

	// Run ASAP
	void Awake()
	{
		if(instance != null)
		{
			Destroy(gameObject);
			return;
		}

		instance = this;
	}

	void Start()
	{
		entity = GetComponent<Entity>();
		rb = GetComponent<Rigidbody>();
		cam = GetComponentInChildren<Camera>();

		velocityLine = Instantiate(velocityLine, transform.position, transform.rotation);
		velocityLine.positionCount = 2;

		lockMouse();
		loadSetting();
	}

	void Update ()
	{
		rotate();
		drawNormalizedVelocity();

		if(Input.GetButtonDown(SHOOT_AXIS))
		{
			if(Abilities[0].activate(gameObject))
			{
				Energy -= Abilities[0].cost;
			}
		}

		if(Input.GetButtonDown(ROCKET_AXIS))
		{
			if(Abilities[1].activate(gameObject))
			{
				Energy -= Abilities[1].cost;
			}
		}

		if(Input.GetButtonDown(MENU_AXIS))
		{
			toggleMouse();
		}
	}

	void FixedUpdate()
	{
		if(checkGrounded())
		{
			if(!jump())
			{
				applyFriction();
			}
		}

		move();
		
		capLateralSpeed();

		applyGravity();
	}

	protected void loadSetting()
	{
		if(PlayerPrefs.HasKey(PP_XSENS_KEY))
		{
			XSens = PlayerPrefs.GetFloat(PP_XSENS_KEY);
		}

		if(PlayerPrefs.HasKey(PP_YSENS_KEY))
		{
			YSens = PlayerPrefs.GetFloat(PP_YSENS_KEY);
		}
	}

	public void lockMouse()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		Crosshair.instance.generate();
	}

	public void unlockMouse()
	{
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;

		Crosshair.instance.degenerate();
	}

	public bool toggleMouse()
	{
		if(Cursor.lockState == CursorLockMode.Locked)
		{
			unlockMouse();
			return false;
		}
		else
		{
			lockMouse();
			return true;
		}
	}

	protected void move()
	{
		Vector3 localVel = transform.InverseTransformDirection(rb.velocity);


		// For quake and first itteration acceleration
		/*
		Vector3 input = transform.TransformDirection(new Vector3(Input.GetAxis(RIGHT_AXIS), 0, Input.GetAxis(FORWARD_AXIS)));
		Vector3 velocity = Accelerate(input, rb.velocity, acceleration, MAX_ACCELERATION);
		*/

		// For second itteration acceleration
		Vector3 input = new Vector3(Input.GetAxis(RIGHT_AXIS), 0, Input.GetAxis(FORWARD_AXIS));
		Vector3 velocity = accelerate(input, localVel);

		if(IsGrounded)
		{
			velocity = orientToGround(velocity);
		}

		// Set velocity
		rb.velocity = transform.TransformDirection(velocity);
	}

	// Oreient velocity relative to the normal the player is standing on.
	protected Vector3 orientToGround(Vector3 startVel)
	{
		// orient norm to normal of ground point
		Vector3 norm = transform.InverseTransformDirection(ground.normal);

		Vector3 vel = Vector3.ProjectOnPlane(new Vector3(startVel.x, 0, startVel.z), norm);
		vel.y += startVel.y;

		return vel;
	}

	// Draw a line renderer according to the current lateral velocity
	protected void drawVelocity()
	{
		velocityLine.SetPosition(0, transform.position);
		velocityLine.SetPosition(1, transform.position + new Vector3(rb.velocity.x, 0, rb.velocity.z));
	}

	// Draw a line renderer according to the normalized current lateral velocity
	protected void drawNormalizedVelocity()
	{
		velocityLine.SetPosition(0, velocityLineOffset + transform.position);
		velocityLine.SetPosition(1, velocityLineOffset + transform.position + new Vector3(rb.velocity.x, 0, rb.velocity.z).normalized);
	}

	// First itteration of custom movement
	/*protected Vector3 accelerate(Vector3 dir, Vector3 vel)
	{
		float proj = Vector3.Dot(vel, dir);
		float accelVel = acceleration * Time.fixedDeltaTime;

		if(proj + accelVel > MAX_ACCELERATION)
		{
			accelVel = MAX_ACCELERATION - proj;
		}

		return vel + dir * accelVel;
	}*/

	// Second iteration of custom movement
	// Set velocity to movement vector as long as a forward movement is pressed.
	// OW, if strafe is pressed
	//		Set strafe to clamped value if below it.
	//		OR add strafe value.
	protected Vector3 accelerate(Vector3 input, Vector3 startVel)
	{

		Vector3 vel;
		if(input.z != 0 && IsGrounded)
		{
			vel = input.normalized * Acceleration;
		}
		else if(input.x != 0)
		{
			vel = startVel;

			// Strafe acceleration
			Vector3 accel = (input.normalized * (HORIZONTAL_SNAP_PERCENT * Acceleration)) * Time.fixedDeltaTime;

			// If less than snap amount, snap horizontal speed to correct value
			if(Mathf.Abs(vel.x + accel.x) < HORIZONTAL_SNAP_PERCENT * Acceleration)
			{
				vel.x = (HORIZONTAL_SNAP_PERCENT * Acceleration) * (Mathf.Sign(input.x));
			}

			// OW add strafe acceleration
			else
			{
				vel += accel;
			}
		}
		// If there was no input
		else
		{
			vel = startVel;
		}

		// Add vertical velocity back
		vel.y = startVel.y;

		// Return modified velocity value
		return vel;
	}

	// Classic Quake movement
	/*private Vector3 accelerate(Vector3 accelDir, Vector3 prevVelocity, float accelerate, float max_velocity)
	{
		float projVel = Vector3.Dot(prevVelocity, accelDir); // Vector projection of Current velocity onto accelDir.
		float accelVel = accelerate * Time.fixedDeltaTime; // Accelerated velocity in direction of movment

		// If necessary, truncate the accelerated velocity so the vector projection does not exceed max_velocity
		if(projVel + accelVel > max_velocity)
			accelVel = max_velocity - projVel;

		return prevVelocity + accelDir * accelVel;
	}*/

	protected void rotate()
	{
		float xRot =  (Input.GetAxis(LOOK_X_AXIS) * XSens);
		//transform.Rotate(new Vector3(0, 1, 0), xRot, Space.World);

		float yRot = -(Input.GetAxis(LOOK_Y_AXIS) * YSens);
		//cam.transform.Rotate(new Vector3(1, 0, 0), yRot, Space.Self);

		entity.rotate(xRot, yRot, Space.Self);
	}

	protected bool jump()
	{
		if(Input.GetAxisRaw(JUMP_AXIS) != 0 && Energy > 0)
		//if(Input.GetButtonDown(jumpAxis))
		{
			Vector3 vel = rb.velocity;
			vel.y = JumpForce * (-Physics.gravity.y);
			rb.velocity = vel;

			Energy -= jumpCost;

			//ground = new RaycastHit();	// Fixes jumping off sloped surface adding more Y acceleration when jumping

			return true;
		}

		return false;
	}

	protected void applyGravity()
	{
		if(IsGrounded == false)
		{
			rb.velocity += Physics.gravity * GRAV_MULT * Time.fixedDeltaTime;
		}
	}

	protected void capLateralSpeed()
	{
		Vector2 lateralVel = new Vector2(rb.velocity.x, rb.velocity.z);
		if(lateralVel.magnitude > entity.maxSpeed)
		{
			lateralVel = lateralVel.normalized * entity.maxSpeed;
		}

		rb.velocity = new Vector3(lateralVel.x, rb.velocity.y, lateralVel.y);
	}

	// Applies friction to player. Assumes normal of ground and player are both up
	protected void applyFriction()
	{
		if(IsGrounded)
		{
			Vector2 vel = new Vector2(rb.velocity.x, rb.velocity.z);

			Rigidbody groundRB = ground.collider.GetComponent<Rigidbody>();
			Vector2 groundVel = (groundRB == null) ? Vector2.zero : new Vector2(groundRB.velocity.x, groundRB.velocity.z);

			Vector2 friction = Vector3.zero;
			friction = ground.collider.material.dynamicFriction * (vel - groundVel).normalized;

			// If friction vector is larger than velocity vector, set to 0
			if(friction.magnitude >= vel.magnitude)
			{
				vel = Vector2.zero;
			}
			else
			{
				vel -= friction;
			}
			rb.velocity = new Vector3(vel.x, 0, vel.y);
		}
	}

	// Raycasts to the ground. Sets ground to the highest point
	protected bool checkGrounded()
	{
		RaycastHit hit = new RaycastHit();
		Vector3 point = transform.position + groundCheckOffset + new Vector3(rb.velocity.x, 0, rb.velocity.z).normalized/4;
		if(Physics.Raycast(point, -transform.up, out hit, GROUND_RAYCAST_DIST, groundLM))
		{
			ground = hit;
			return true;
		}

		ground = new RaycastHit();
		return false;
	}
}
