using UnityEngine;
using System.Collections;
 
[RequireComponent (typeof (CharacterController))]
public class FPSWalkerEnhanced: MonoBehaviour {
 
    public float walkSpeed = 6.0f;
    public float runSpeed = 11.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
 
    // Units that player can fall before a falling damage function is run. To disable, type "infinity" in the inspector
    public float fallingDamageThreshold = 10.0f;
 
    // Small amounts of this results in bumping when walking down slopes, but large amounts results in falling too fast
    public float antiBumpFactor = .75f;
	public GameObject maincam;
	public float height_normal = 2.0f;
	public float height_crouch = 1.0f;
	public AudioClip jump_snd;
	public bool cancontrol = true;
 
    private Vector3 moveDirection = Vector3.zero;
    private bool grounded = false;
    private CharacterController controller;
    private Transform myTransform;
    private float speed;
    private RaycastHit hit;
    private float fallStartLevel;
    private bool falling;
	private float height_diff;
	
	public bool Grounded
	{
		get
		{
			return this.grounded;
		}
	}
	
	public void ResetFall()
	{
		falling = false;
	}
	
	public void ResetCrouch()
	{
		if(controller.height == height_normal) return;
		maincam.transform.position = new Vector3(
		maincam.transform.position.x,
		maincam.transform.position.y+height_diff/2,
		maincam.transform.position.z);
		controller.transform.position = new Vector3(
		controller.transform.position.x,
		controller.transform.position.y+height_diff/2,
		controller.transform.position.z);
		controller.height = height_normal;
	}
	
    void Start()
	{
        controller = GetComponent<CharacterController>();
		height_diff = height_normal-height_crouch;
        myTransform = transform;
        speed = walkSpeed;
    }
 
    void FixedUpdate()
	{
		if(cancontrol)
		{
	        float inputX = Input.GetAxis("Horizontal");
	        float inputY = Input.GetAxis("Vertical");
			
			if(Input.GetButton("Crouch"))
			{
				if(controller.height != height_crouch)
				{
					maincam.transform.position = new Vector3(
					maincam.transform.position.x,
					maincam.transform.position.y-height_diff/2,
					maincam.transform.position.z);
					controller.height = height_crouch;
					if(speed == runSpeed)
						speed = walkSpeed;
				}
			}
			else
			{
				if(controller.height != height_normal)
				{
					if(!Physics.SphereCast(new Ray(transform.position + new Vector3(0,0.3f,0), transform.up),0.3f,1.25f))
					{
						maincam.transform.position = new Vector3(
						maincam.transform.position.x,
						maincam.transform.position.y+height_diff/2,
						maincam.transform.position.z);
						controller.transform.position = new Vector3(
						controller.transform.position.x,
						controller.transform.position.y+height_diff,
						controller.transform.position.z);
						controller.height = height_normal;
					}
				}
				speed = Input.GetButton("Run") && MainScript.stamina >= 20 ? runSpeed : walkSpeed;
			}
	        if (grounded)
			{
	            // If we were falling, and we fell a vertical distance greater than the threshold, run a falling damage routine
	            if (falling)
				{
	                falling = false;
	                if (myTransform.position.y < fallStartLevel - fallingDamageThreshold)
	                    FallingDamageAlert(fallStartLevel - myTransform.position.y);
	            }
				
				moveDirection = new Vector3(inputX, -antiBumpFactor, inputY);
                moveDirection = myTransform.TransformDirection(moveDirection) * speed;
				
	            if (Input.GetButton("Jump") && MainScript.stamina >= 100)
				{
					audio.PlayOneShot(jump_snd);
	                moveDirection.y = jumpSpeed;
					networkView.RPC("SendJumpSND",RPCMode.Others);
					MainScript.stamina -= 100;
					if(MainScript.stamina < 0) MainScript.stamina = 0;
	            }
	        }
	        else
			{
	            // If we stepped over a cliff or something, set the height at which we started falling
	            if (!falling)
				{
	                falling = true;
	                fallStartLevel = myTransform.position.y;
	            }
                moveDirection.x = inputX * speed;
                moveDirection.z = inputY * speed;
                moveDirection = myTransform.TransformDirection(moveDirection);
	        }
		}
		else moveDirection.x = moveDirection.z = 0;
        // Apply gravity
        moveDirection.y -= gravity * Time.deltaTime;
 
        // Move the controller, and set grounded true or false depending on whether we're standing on something
        grounded = (controller.Move(moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0;
    }
 
    // If falling damage occured, this is the place to do something about it. You can make the player
    // have hitpoints and remove some of them based on the distance fallen, add sound effects, etc.
    void FallingDamageAlert (float fallDistance) {
		GetComponent<MainScript>().SendDamage(null, (int)fallDistance*2, false);
		audio.PlayOneShot(GetComponent<MainScript>().death_snd);
		networkView.RPC("SendFallSND",RPCMode.Others);
    }
	
	[RPC]
	void SendJumpSND()
	{
		audio.PlayOneShot(jump_snd);
	}
	
	[RPC]
	void SendFallSND()
	{
		audio.PlayOneShot(GetComponent<MainScript>().death_snd);
	}
}