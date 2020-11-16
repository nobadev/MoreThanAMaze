using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    //Inspector fields
    [SerializeField] int movementSpeed;
    [SerializeField] float mouseSensitivity;
    [SerializeField] Transform playerCam;
    [SerializeField] float gravity;
    [SerializeField] float pushPower = 2.0f;
    [SerializeField] float jumpImpulse;
    [SerializeField] float dashImpulse;
    [SerializeField] float dashCooldown;
    [SerializeField] float airControl;
    [SerializeField] int maxDoubleJumps;
    [SerializeField] uint dashLimit = 100;
    private AudioSource playerAudioSource;
    private AudioManager audioManager;
    [SerializeField] private float timeBetweenStep;
    public bool isGrounded;
    public bool isFalling;
    public bool isDashing;
    private int footstepRandomIndex;
    float timeSinceStep = 0;
    //Movement
    Vector2 movementDirection;
    Vector3 velocity;
    Vector3 airControlVelocity;
    float downSpeed = 0.0f;
    [SerializeField] float jumpVelocity;
    float airSpeed = 4.0f;
    int doubleJumpCounter;
    float dashTime = 0.25f;

    //Camera
    Vector2 mouseVector;
    float yRotation = 0.0f;

    //Player States
    [SerializeField] bool isJumping;
    [SerializeField] bool isWalking;
    
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
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update() {
        Debug.Log(dashTime);
        GetPlayerInput();
        CameraMovement();


        if(charController.isGrounded) {
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

    //Gets player input - called in Update()
    private void GetPlayerInput() {
        movementDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        mouseVector = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        if(Input.GetKeyDown(KeyCode.Space)) {
            isJumping = true;
            JumpState();
        }

        if(Input.GetKeyDown(KeyCode.LeftShift)) {
            isDashing = true;
            StartCoroutine("DashState");
        }
    }

    //Player movement
    private void PlayerMovement() {
        
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
        FallState();
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
    private IEnumerator DashState() {

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

    /*
    private IEnumerator DashCounter() {
        
    }
    */

    private void FallState() {
        if(isFalling) {
            charController.slopeLimit = 90.0f;
        }
    }

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

    private void PlayFootsteps() {
        StartCoroutine("PlayFootstepSound", timeBetweenStep);
    }

    private IEnumerator PlayFootstepSound() {

        timeSinceStep += Time.fixedDeltaTime;

        if (charController.isGrounded && !playerAudioSource.isPlaying && timeSinceStep >= timeBetweenStep) { //only plays if character is grounded, and if no other audioclip is playing
            footstepRandomIndex = Random.Range(0, 3);
            playerAudioSource.clip = audioManager.sound.footstep[footstepRandomIndex]; //takes audio clip from audiomanager gameobject
            playerAudioSource.pitch = Random.Range(0.95f, 1.05f);
            playerAudioSource.Play();
            timeSinceStep = 0;
            yield return new WaitForSeconds(1);
        }
    }
}

