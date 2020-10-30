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
    [SerializeField] float airControl;
    [SerializeField] private AnimationCurve jumpFallOff;
    [SerializeField] int maxDoubleJumps;
    public bool isGrounded;
    public bool isFalling;

    //Movement
    Vector2 movementDirection;
    Vector3 velocity;
    Vector3 airControlVelocity;
    float downSpeed = 0.0f;
    [SerializeField] float jumpVelocity;
    float airSpeed = 4.0f;
    int doubleJumpCounter;

    //Camera
    Vector2 mouseVector;
    float yRotation = 0.0f;

    //Player States
    [SerializeField] bool isJumping;
    
    //References
    CharacterController charController;

    /* To-Do
     * do the jump stuff w/ air control
     * interaction with rigidbodies - DONE
     * good slope detection
     * slide off of steep slopes
     * dash
     */

    // Start is called before the first frame update
    void Start() {
        charController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update() {
        Debug.Log(doubleJumpCounter);
        GetPlayerInput();
        CameraMovement();

        if(charController.isGrounded) {
            doubleJumpCounter = 0;
        }
    }

    private void FixedUpdate() {
        isGrounded = charController.isGrounded;
        isFalling = !charController.isGrounded;
        PlayerMovement();
        FallState();
    }

    private void GetPlayerInput() {
        movementDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        mouseVector = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        if(Input.GetKeyDown(KeyCode.Space)) {
            isJumping = true;
            JumpState();
        }
    }

    private void PlayerMovement() {
        
        movementDirection.Normalize();

        downSpeed += gravity * Time.deltaTime;
        velocity = (transform.right * movementDirection.x + transform.forward * movementDirection.y) * movementSpeed + Vector3.up * downSpeed;
        charController.Move(velocity * Time.deltaTime);

        //checks if player is grounded and resets its y velocity, otherwise it continues to stack
        if (charController.isGrounded) {
            downSpeed = 0.0f;
            charController.slopeLimit = 45.0f;
        }
    }

    private void CameraMovement() {
        yRotation -= mouseVector.y * mouseSensitivity;
        yRotation = Mathf.Clamp(yRotation, -90.0f, 90.0f);
        playerCam.localEulerAngles = Vector3.right * yRotation;
        transform.Rotate(Vector3.up * mouseVector.x * mouseSensitivity);
    }

    private void JumpState() {
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
}

