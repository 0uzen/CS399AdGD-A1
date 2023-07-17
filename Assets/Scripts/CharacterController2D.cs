using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterController2D : MonoBehaviour
{
	[Header("For Movement")]
	//[SerializeField] float moveSpeed = 10f;
	//[SerializeField] float airMoveSpeed = 10f;
	//private float XDirectionalInput;
	private bool pFacingRight = true;  // For determining which way the player is currently facing.
	//private bool isMoving;

	[Header("Jumping")]
	[SerializeField] private float pJumpForce = 400f;							// Amount of force added when the player jumps.
	[Range(0, 1)] [SerializeField] private float pCrouchSpeed = .36f;			// Amount of maxSpeed applied to crouching movement. 1 = 100%
	[Range(0, .3f)] [SerializeField] private float pMovementSmoothing = .05f;	// How much to smooth out the movement
	[SerializeField] private bool pAirControl = true;							// Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask pWhatIsGround;							// A mask determining what is ground to the character
	[SerializeField] private Transform pGroundCheck;							// A position marking where to check if the player is grounded.
	[SerializeField] private Transform pCeilingCheck;							// A position marking where to check for ceilings
	[SerializeField] private Collider2D pCrouchDisableCollider;             // A collider that will be disabled when crouching
	//[SerializeField] Vector2 groundCheckSize;
	const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	private bool pGrounded;            // Whether or not the player is grounded.
	const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
	private Rigidbody2D pRb2D;
	private Vector3 pVelocity = Vector3.zero;
	bool jump;
	bool enableAirJump = false;

	[Header("Events")]
	[Space]

	[Header("Wallsliding")]
	[SerializeField] float wallSlideSpeed = 0;
	[SerializeField] LayerMask wallLayer;
	[SerializeField] Transform wallCheckPoint;
	[SerializeField] Vector2 wallCheckSize;
	private bool isTouchingWall;
	private bool isWallSliding;

	[Header("Wall Jumping")]
	[SerializeField] float wallJumpForce = 18f;
	[SerializeField] float wallJumpDirection = -1f;
	[SerializeField] Vector2 wallJumpAngle;

	public UnityEvent OnLandEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	public BoolEvent OnCrouchEvent;
	private bool pCrouching = false;

    //[Header("UI")]
    public int Lives
    {
        get;
        set;
    } = 3;

    string FruitTag = "Fruit";
	public int ItemsCount
	{
		get;
		set;
	} = 0;

    private void Start()
    {
		//Load lives from PlayerPrefs, a default value is set here if entry does not exist
		Lives = PlayerPrefs.GetInt("Lives", 3);
		//Load items from PlayerPrefs, a default value is set here if entry does not exist
		ItemsCount = PlayerPrefs.GetInt("ItemsCount", 0);

		wallJumpAngle.Normalize();
    }

    private void Awake()
	{
		pRb2D = GetComponent<Rigidbody2D>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();
	}

	private void FixedUpdate()
	{
        bool wasGrounded = pGrounded;
        pGrounded = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(pGroundCheck.position, k_GroundedRadius, pWhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                pGrounded = true;
                if (!wasGrounded)
                    OnLandEvent.Invoke();
            }
        }

		//Jump();
		WallSlide();
		WallJump();
	}

	private void Update()
	{
		CheckWorld();
		Jump();
	}

    public void Move(float move, bool crouch, bool jump)
	{

		// If crouching, check to see if the character can stand up
		if (!crouch)
		{
			// If the character has a ceiling preventing them from standing up, keep them crouching
			if (Physics2D.OverlapCircle(pCeilingCheck.position, k_CeilingRadius, pWhatIsGround))
			{
				crouch = true;
			}
		}
		//only control the player if grounded or airControl is turned on
		if (pGrounded || pAirControl)
		{

			// If crouching
			if (crouch)
			{
				if (!pCrouching)
				{
					pCrouching = true;
					OnCrouchEvent.Invoke(true);
				}

				// Reduce the speed by the crouchSpeed multiplier
				move *= pCrouchSpeed;

				// Disable one of the colliders when crouching
				if (pCrouchDisableCollider != null)
					pCrouchDisableCollider.enabled = false;
			} else
			{
				// Enable the collider when not crouching
				if (pCrouchDisableCollider != null)
					pCrouchDisableCollider.enabled = true;

				if (pCrouching)
				{
					pCrouching = false;
					OnCrouchEvent.Invoke(false);
				}
			}

			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(move * 10f, pRb2D.velocity.y);
			// And then smoothing it out and applying it to the character
			pRb2D.velocity = Vector3.SmoothDamp(pRb2D.velocity, targetVelocity, ref pVelocity, pMovementSmoothing);

			// If the input is moving the player right and the player is facing left...
			if (move > 0 && !pFacingRight)
			{
				// ... flip the player.
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (move < 0 && pFacingRight)
			{
				// ... flip the player.
				Flip();
			}
		}
		// If the player should jump...
		if (pGrounded && jump)
		{
			// Add a vertical force to the player.
			pGrounded = false;
			pRb2D.AddForce(new Vector2(0f, pJumpForce));
		}
	}

    private void Jump()
    {
        jump = Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump");

        if (jump)
        {
            if (pGrounded)
            {
                pRb2D.AddForce(Vector2.up * 600, ForceMode2D.Force);
                pGrounded = false;
                enableAirJump = true;
            }
            else if (enableAirJump)
            {
                pRb2D.AddForce(Vector2.up * 600, ForceMode2D.Force);
                enableAirJump = false;
            }
        }
    }

    private void CheckWorld()
    {
		//pGrounded = Physics2D.OverlapBox(pGroundCheck.position, groundCheckSize, 0, pWhatIsGround);
		isTouchingWall = Physics2D.OverlapBox(wallCheckPoint.position, wallCheckSize, 0, wallLayer);

		if (Lives == 0)
		{
			RestartLevel();
		}
	}

	private void WallSlide()
    {
		if (isTouchingWall && !pGrounded && pRb2D.velocity.y < 0)
        {
			isWallSliding = true;
        }
        else
        {
			isWallSliding = false;
        }
		//Wall Slide
		if (isWallSliding)
        {
			pRb2D.velocity = new Vector2(pRb2D.velocity.x, -wallSlideSpeed);
        }
    }

	private void WallJump()
    {
		if ((isWallSliding) && jump)
        {
			pRb2D.AddForce(new Vector2(wallJumpForce * wallJumpDirection * wallJumpAngle.x, wallJumpForce * wallJumpAngle.y), ForceMode2D.Impulse);
			Flip();
			jump = false;
        }
    }

	private void Flip()
	{
		wallJumpDirection *= -1;

		// Switch the way the player is labelled as facing.
		pFacingRight = !pFacingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	string SpikeTag = "Spikes";
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == pWhatIsGround)
        {
			pGrounded = true;
			Debug.Log("OnCollisionEnter2D:" + collision.gameObject.layer);
        }

		if (collision.gameObject.tag == SpikeTag)
        {
			if (pFacingRight)
            {
				pRb2D.AddForce(Vector2.left * 5000, ForceMode2D.Force);
            }
            else
            {
				pRb2D.AddForce(Vector2.right * 5000, ForceMode2D.Force);
            }

			Lives--;

			//Update player lives
			PlayerPrefs.SetInt("Lives", Lives);
        }

		if (collision.gameObject.tag == FruitTag)
        {
			ItemsCount++;

			Destroy(collision.gameObject);

			//Update player items count
			PlayerPrefs.SetInt("ItemsCount", ItemsCount);

			if (ItemsCount == 10)
            {
				Lives++;

				//Update player lives
				PlayerPrefs.SetInt("Lives", Lives);
			}
        }
    }

	private void RestartLevel()
    {
		//Reset player lives
		PlayerPrefs.SetInt("Lives", 3);
		//Update player items
		PlayerPrefs.SetInt("ItemsCount", 0);

		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

	private void RestartGame()
    {
		//Reset player lives
		PlayerPrefs.SetInt("Lives", 3);
		//Update player items
		PlayerPrefs.SetInt("ItemsCount", 0);

		SceneManager.LoadScene("Level1");
    }

    private void OnDrawGizmosSelected()
    {
		Gizmos.color = Color.blue;
		Gizmos.DrawCube(wallCheckPoint.position, wallCheckSize);
    }
}
