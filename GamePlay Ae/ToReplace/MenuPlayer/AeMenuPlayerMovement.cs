using UnityEngine;
using System.Collections;

public class AeMenuPlayerMovement : MonoBehaviour 
{
	public float horizontalAxis = 0;
	public float VerticalAxis = 0;
	public float baseWalkSpeed = 6.0f;
	public float walkSpeed = 6.0f;
	
	
	public float SpeedMultiplicator = 1.0f;
	public float baseRunSpeed = 6.0f;
	public float runSpeed = 11.0f;
	public bool limitDiagonalSpeed = true;
	public Animator AnimatorGameObject;
	private Vector3 lastPosition;
	
	public float jumpSpeed = 8.0f;
	public float gravity = 20.0f;
	
	public bool slideWhenOverSlopeLimit = false;
	
	// If checked and the player is on an object tagged "Slide", he will slide down it regardless of the slope limit
	public bool slideOnTaggedObjects = false;
	
	public float slideSpeed = 12.0f;
	
	// If checked, then the player can change direction while in the air
	public bool airControl = false;
	
	// Small amounts of this results in bumping when walking down slopes, but large amounts results in falling too fast
	public float antiBumpFactor = .75f;
	
	// Player must be grounded for at least this many physics frames before being able to jump again; set to 0 to allow bunny hopping
	public int antiBunnyHopFactor = 1;
	
	
	private Vector3 moveDirection = Vector3.zero;
	public bool grounded = false;
	public bool running = false;
	public CharacterController controller;
	private Transform myTransform;
	public float speed;
	private RaycastHit hit;
	private bool falling;
	private float slideLimit;
	private float rayDistance;
	private Vector3 contactPoint;
	private bool playerControl = false;
	private int jumpTimer;
	
	private float SpeedAnimator;
	private float DirectionAnimator;
	private bool JumpAnimator;
	private bool SlideAnimator;
	
	public float inputX = 0.0f;
	public float inputY = 0.0f;
	
	
	public bool IsOnPause = false;
	
	void Awake()
	{
		myTransform = transform;
	}
	
	void Start() 
	{
		runSpeed = baseRunSpeed;
		walkSpeed = baseWalkSpeed;
		controller = GetComponent<CharacterController>();
		speed = walkSpeed;
		rayDistance = controller.height * .5f + controller.radius;
		slideLimit = controller.slopeLimit - .1f;
		jumpTimer = antiBunnyHopFactor;
	}
	void FixedUpdate() 
	{
		inputX = Input.GetAxis("Horizontal");
		inputY = Input.GetAxis("Vertical");


		float inputModifyFactor = (inputX != 0.0f && inputY != 0.0f && limitDiagonalSpeed)? .7071f : 1.0f;
		
		if (grounded) 
		{
			bool sliding = false;
			// See if surface immediately below should be slid down. We use this normally rather than a ControllerColliderHit point,
			// because that interferes with step climbing amongst other annoyances
			if (Physics.Raycast(myTransform.position, -Vector3.up, out hit, rayDistance)) 
			{
				if (Vector3.Angle(hit.normal, Vector3.up) > slideLimit)
					sliding = true;
			}
			// However, just raycasting straight down from the center can fail when on steep slopes
			// So if the above raycast didn't catch anything, raycast down from the stored ControllerColliderHit point instead
			else 
			{
				Physics.Raycast(contactPoint + Vector3.up, -Vector3.up, out hit);
				if (Vector3.Angle(hit.normal, Vector3.up) > slideLimit)
					sliding = true;
			}

			if(Input.GetKey(KeyCode.LeftShift) && inputY > 0.0f)
			{
				speed = runSpeed;
				running = true;
			}
			else
			{
				speed = walkSpeed;
				running = false;
			}
			
			// If sliding (and it's allowed), or if we're on an object tagged "Slide", get a vector pointing down the slope we're on
			if ( (sliding && slideWhenOverSlopeLimit) || (slideOnTaggedObjects && hit.collider.tag == "Slide") ) 
			{
				Vector3 hitNormal = hit.normal;
				moveDirection = new Vector3(hitNormal.x, -hitNormal.y, hitNormal.z);
				Vector3.OrthoNormalize (ref hitNormal, ref moveDirection);
				moveDirection *= slideSpeed;
				playerControl = false;
			}
			else 
			{
				moveDirection = new Vector3(inputX * inputModifyFactor, -antiBumpFactor, inputY * inputModifyFactor);
				moveDirection = myTransform.TransformDirection(moveDirection) * speed;
				playerControl = true;
			}

			if (!Input.GetButton("Jump"))
			{
				jumpTimer++;
			}
			
			else if (jumpTimer >= antiBunnyHopFactor) 
			{
				moveDirection.y = jumpSpeed;
				jumpTimer = 0;
			}
		}
		else 
		{
			// If we stepped over a cliff or something, set the height at which we started falling
			if (!falling) 
			{
				falling = true;
			}

			if (airControl && playerControl) 
			{
				moveDirection.x = inputX * speed * inputModifyFactor;
				moveDirection.z = inputY * speed * inputModifyFactor;
				moveDirection = myTransform.TransformDirection(moveDirection);
			}
		}
		
		// Apply gravity
		moveDirection.y -= gravity * Time.deltaTime;

		grounded = (controller.Move(moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0;	}
	
	void OnControllerColliderHit (ControllerColliderHit hit) 
	{
		contactPoint = hit.point;
	}
	
	void FallingDamageAlert (float fallDistance) 
	{
		print ("Ouch! Fell " + fallDistance + " units!");   
	}
}
