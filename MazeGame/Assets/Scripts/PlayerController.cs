using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
    //Inspector fields
    [SerializeField] public float movementSpeed;
    [SerializeField] float mouseSensitivity;
    [SerializeField] Transform playerCam;
    [SerializeField] float gravity;
    [SerializeField] float pushPower = 2.0f;
    [SerializeField] float jumpImpulse;
    [SerializeField] float dashImpulse;
    [SerializeField] float airControl;
    [SerializeField] uint dashLimit = 100;
    [SerializeField] float jumpVelocity;
    [SerializeField] int maxDoubleJumps;
    [SerializeField] private float timeBetweenStep;

    int OrbsCollected;

    //Movement
    [SerializeField] public Vector2 movementDirection;
    Vector3 velocity;
    Vector3 airControlVelocity;
    float downSpeed = 0.0f;
    float airSpeed = 4.0f;
    int doubleJumpCounter;
    float dashTime = 0.25f;
    [SerializeField] public float airTime = 0.0f;

    //Dash
    [SerializeField] Text dashText;
    [SerializeField] float dashChargeRate;
    [SerializeField] float dashCharge = 100f;

    //Slopes
    Vector3 forward;
    float maxGroundAngle;
    float currentGroundAngle;
    RaycastHit hitSlope;
    float height;
    [SerializeField] float heightPadding;
    float slopeAngle;
    [SerializeField] float raycastFireDistance;
    
    //Camera
    Vector2 mouseVector;
    float yRotation = 0.0f;

    //Player States
    [SerializeField] bool isJumping;
    [SerializeField] bool isWalking;
    public bool isGrounded;
    public bool isFalling;
    public bool isDashing;
    
    //Audio
    private AudioManager audioManager;
    private AudioSource playerAudioSource;

    //Footstep things
    private int footstepRandomIndex;
    float timeSinceStep = 0;

    [SerializeField] private ParticleSystem[] playerParticles;

    //References
    CharacterController charController;

    /* To-Do
     * do the jump stuff w/ air control - nearly done no air control
     * interaction with rigidbodies - DONE
     * good slope detection
     * slide off of steep slopes
     * dash - nearly done add dash limit
     * MAKE FOOTSTEPS WORK AT INTERVALS
     * code cleanup
     * 
     */


    /* BUGS
     * while dashing, letting go of a directional key midway through a dash..
     * ..makes it dash forwards - caused by dashing forwards if no input is detected
     * 
     * charactercontroller gets "warped"  on platform - caused by being parented by..
     * ..the platform
     * 
     */

    private void Awake() {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }

    // Start is called before the first frame update
    void Start() {
        charController = GetComponent<CharacterController>();
        playerAudioSource = GetComponent<AudioSource>();
        height = 0.5f;
        /*
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        */
    }

    // Update is called once per frame
    void Update() {
        DebugConsole();
        GetPlayerInput();
        CameraMovement();
        DashCheck();

        if (charController.isGrounded) {
            doubleJumpCounter = 0;
        }
    }

    // called every physics step
    private void FixedUpdate() {
        isGrounded = charController.isGrounded;
        isFalling = !charController.isGrounded;
        PlayerMovement();
        FallState();
        WalkState();
    }

    private void DashCheck() {
        if (dashCharge < 100 && !isDashing) {
            StartCoroutine("DashCounter");
        }

        if (dashCharge > 100) {
            dashCharge = 100;
        }
    }

    private void DebugConsole() {
        //Debug.Log(timeSinceStep);
        //Debug.DrawLine(transform.position, -transform.right, Color.blue);
        //Debug.Log(onSlope());

        //dashText.text = "DashC: " + dashCharge;

        //Debug.Log(OrbsCollected);
    }

    //Gets player input - called in Update()
    private void GetPlayerInput() {
        //movementDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        mouseVector = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        if(Input.GetKeyDown(KeyCode.Space)) {
            isJumping = true;
            JumpState();
        }

        if(Input.GetKeyDown(KeyCode.LeftShift)) {
            if(dashCharge >= 50) {
                isDashing = true;
                StartCoroutine("DashState");
            }
        }
    }

    public void ButtonAction() {
        Debug.Log("This button is being clicked");
    }

    public void GetMobileInput(int movementKey) {
        switch (movementKey) {
            case 0:
                movementDirection = new Vector2(0, 1);
                break;
            case 1:
                movementDirection = new Vector2(0, -1);
                break;
            case 2:
                movementDirection = new Vector2(1, 0);
                break;
            case 3:
                movementDirection = new Vector2(-1, 0);
                break;
            case 4:
                movementDirection = new Vector2(0, 0);
                break;
            default:
                break;
        }
    }

    //Player movement
    public void PlayerMovement() {
        
        movementDirection.Normalize();
        downSpeed += gravity * Time.deltaTime;
        velocity = (transform.right * movementDirection.x + transform.forward * movementDirection.y) * movementSpeed;
        if(isDashing) {
            downSpeed = 0.0f;
        }
        else {
            velocity += Vector3.up * downSpeed;
        }
        charController.Move(velocity * Time.deltaTime);

        //checks if player is grounded and resets its y velocity, otherwise it continues to stack
        if (charController.isGrounded) {
            downSpeed = 0.0f;
            charController.slopeLimit = 45.0f;
        }
        if(isWalking) {
            PlayFootsteps();
        }

        if ((movementDirection.y != 0 || movementDirection.x != 0) && onSlope()) {
           // velocity = Vector3.ProjectOnPlane(hitSlope., hitSlope.normal) * Time.deltaTime;
        }
    }

    //Camera
    private void CameraMovement() {
        yRotation -= mouseVector.y * mouseSensitivity;
        yRotation = Mathf.Clamp(yRotation, -90.0f, 90.0f);
        playerCam.localEulerAngles = Vector3.right * yRotation;
        transform.Rotate(Vector3.up * mouseVector.x * mouseSensitivity);
    }

    private void WalkState() {
        if(charController.velocity.magnitude > 2f) {
            isWalking = true;
        }
        else {
            isWalking = false;
        }
    }

    //jump movement
    private void JumpState() { //convert to coroutine maybe?
        if(charController.isGrounded) {

            jumpVelocity = Mathf.Sqrt(-2 * gravity * jumpImpulse);
            downSpeed = jumpVelocity;
        }


        //Double Jump
        if(!charController.isGrounded && doubleJumpCounter < maxDoubleJumps) {
            jumpVelocity = Mathf.Sqrt(-2 * gravity * jumpImpulse);
            downSpeed = jumpVelocity;
            doubleJumpCounter++;
        }

        if (isFalling) {
            if(doubleJumpCounter != maxDoubleJumps) {
                jumpVelocity = Mathf.Sqrt(-2 * gravity * jumpImpulse);
                downSpeed = jumpVelocity;
                doubleJumpCounter++;
            }
        }

        isJumping = false;  
    }

    //dash movement

    private bool onSlope() {
        if (isJumping) {
            return false;
        }

        Debug.DrawRay(transform.position, Vector3.down, Color.red);
        if (Physics.Raycast(transform.position, Vector3.down, out hitSlope, raycastFireDistance + 0.1f)) {
            slopeAngle = (Vector3.Angle(hitSlope.normal, transform.forward) - 90);
            if (hitSlope.normal != Vector3.up) {
                return true;
            }
        }
        return false;
    }

    private IEnumerator DashState() {

        dashCharge -= 50;
        float startTime = Time.time;

        while(Time.time < startTime + dashTime) {
            if(movementDirection.x != 0 || movementDirection.y != 0) {
                dashLimit -= 50;
                charController.Move((transform.forward * movementDirection.y + transform.right * movementDirection.x) * dashImpulse * Time.deltaTime);
                yield return null;
            }
            else {
                charController.Move(transform.forward * dashImpulse * Time.deltaTime);
                yield return null;
            }
        }
        isDashing = false;

    }
        /* When dashing..
         * - minus one charge
         * - start generating a new charge over time specified by dashchargeperrate (dashchargetime += Time.fixeddeltatime * dashchargeperrate)
         * - if no charges - return false - if false - dont dash
         */ 
    private IEnumerator DashCounter() {
        
        dashCharge += dashChargeRate;
        yield return null;
    }
        

    private void FallState() {
        if(isFalling) {
            charController.slopeLimit = 90.0f;
            airTime += Time.deltaTime;
            //slopeAngle = 0f;
        }
        else {
            airTime = 0f;
        }
    }

    //COLLISION
    //adds "fake physics" collision with a charcontroller
    void OnControllerColliderHit(ControllerColliderHit hit) {
        Rigidbody body = hit.collider.attachedRigidbody;

        // no rigidbody
        if (body == null || body.isKinematic) {
            return;
        }

        // We dont want to push objects below us
        if (hit.moveDirection.y < -0.3) {
            return;
        }

        // Calculate push direction from move direction,
        // we only push objects to the sides never up and down
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

        // If you know how fast your character is trying to move,
        // then you can also multiply the push velocity by that.

        // Apply the push
        body.velocity = pushDir * pushPower;
    }

    //AUDIO
    private void PlayFootsteps() {
        StartCoroutine("PlayFootstepSound", timeBetweenStep);
    }

    private IEnumerator PlayFootstepSound() {

        timeSinceStep += Time.fixedDeltaTime;
        //only plays if character is grounded, and if no other audioclip is playing
        if (charController.isGrounded && !playerAudioSource.isPlaying && timeSinceStep >= timeBetweenStep) {
            footstepRandomIndex = Random.Range(0, 3);
            playerAudioSource.clip = audioManager.sound.footstep[footstepRandomIndex]; //takes audio clip from audiomanager gameobject
            playerAudioSource.pitch = Random.Range(0.95f, 1.05f);
            playerAudioSource.Play();
            timeSinceStep = 0;
            yield return new WaitForSeconds(1);
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.tag == "Orb") {
            Destroy(collision.gameObject);
            OrbsCollected += 1;
            //Debug.Log(OrbsCollected);
        }
    }
}

