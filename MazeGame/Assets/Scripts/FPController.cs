using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class FPController : MonoBehaviour
{
    /* TO DO
     * new movement controller
     *  jumpimpulse needs to be affected by gravity
     *  adding float - affected by gravity
     *  dash movement
     *  double jumping - check for isgrounded && doublejumpcharge - resets on groundtouch
     */

    [SerializeField] int movementSpeed;
    [SerializeField] int jumpImpulse;
    [SerializeField] int sprintSpeed;
    [SerializeField] int dashImpulse;
    [SerializeField] private Transform playerCam;
    [SerializeField] float gravity = -13.0f;
    [SerializeField] private float slopeForce;
    [SerializeField] private float raycastFireDistance;
    float groundClamp = -0.75f;
    float downSpeed;
    Vector3 velocityY;
    private Rigidbody rb;
    private float capCollider;
    public float mouseSensitivity;
    float yRotation = 0.0f;
    private bool isJumping;
    Vector3 velocity;
    Vector2 movementDir;
    float groundAngle;
    float slopeLimit;
    float slopeAngle;

    RaycastHit hit;
    RaycastHit hitSlope;

    // Start is called before the first frame update
    void Start() {
        raycastFireDistance = GetComponent<CapsuleCollider>().bounds.extents.y;
        capCollider = GetComponent<CapsuleCollider>().height;
        rb = GetComponent<Rigidbody>(); //assigns the rigidbody component of the Player to rb 
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame

    void Update() {
        CameraMovement();

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded() == true) {
            Jump();
        }

        GetGroundAngle();
        //Debug.Log(isGrounded());
        Debug.Log(groundAngle);
        //Debug.Log(rb.velocity.y);
        //Debug.Log(hit.normal);
        //Debug.Log(onSlope());
    }
    private void FixedUpdate() {
        Movement();
    }

    private void CameraMovement() {
        Vector2 mouseVector = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        yRotation -= mouseVector.y * mouseSensitivity;
        yRotation = Mathf.Clamp(yRotation, -90.0f, 90.0f);
        playerCam.localEulerAngles = Vector3.right * yRotation;
        transform.Rotate(Vector3.up * mouseVector.x * mouseSensitivity);
    }

/*  
    private void CameraMovement() {
        //multiplied by sensitivity - allows user to adjust cam sensitivity
        mouseHorizontal = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        mouseVertical -= Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime; 
        
        //clamps camera - stops it from moving past 90 degrees above and below
        mouseVertical = Mathf.Clamp(mouseVertical, -90f, 90f);

        playerController.transform.Rotate(0, mouseHorizontal, 0); //horizontal rotation
        cam.transform.localRotation = Quaternion.Euler(mouseVertical, 0f, 0f); //vertical rotation

    }
*/
    private void Movement() {

//if(floor.transform == null) {
        //    return transform.forward;
//}

        movementDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        movementDir.Normalize();

        //checks if grounded - resets velocity.y to 0 otherwise it stacks up
        if(isGrounded()) {
            downSpeed = 0.0f;
        }

        downSpeed += gravity * Time.deltaTime;

        Vector3 velocity = (movementDir.x * transform.right + movementDir.y * transform.forward) * movementSpeed + Vector3.up * downSpeed;

        rb.velocity = velocity * Time.deltaTime;
        velocityY = new Vector3(0, rb.velocity.y, 0);
        rb.velocity += velocityY;

        /*
        if((movementDir.y != 0 || movementDir.x != 0) && onSlope()) {
            rb.velocity = Vector3.ProjectOnPlane(new Vector3(velocity.x, velocityY.y, velocity.z), hitSlope.normal) * Time.deltaTime;
        }
        */
       // if ((movementDir.y != 0 || movementDir.x != 0) && isGrounded())
        //    rb.AddForce(hit.normal * -hit.distance, ForceMode.VelocityChange);

        // rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.y);

        /*
        if(Input.GetKey(KeyCode.W)) {
            transform.Translate(new Vector3(0, 0, movementSpeed) * Time.deltaTime);
        }
        if(Input.GetKey(KeyCode.S)) {
            transform.Translate(new Vector3(0, 0, -movementSpeed) * Time.deltaTime);
        }
        if(Input.GetKey(KeyCode.A)) {
            transform.Translate(new Vector3(-movementSpeed, 0, 0) * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D)) {
        transform.Translate(new Vector3(movementSpeed, 0, 0) * Time.deltaTime);
        }
        if(Input.GetKeyDown(KeyCode.LeftShift)) {
           // rb.AddForce(, 0, 0, ForceMode.Impulse);
        }
        */
    }

    //checks if grounded
    private bool isGrounded() {
        Debug.DrawRay(transform.position, Vector3.down, Color.red);
        if (Physics.Raycast(transform.position, Vector3.down, out hit, raycastFireDistance + 0.1f))
            return true;
        return false;
    }

    /*
    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, GetComponent<CapsuleCollider>().radius);
    }
    */

    /*
    private bool onSlope() {
        if(isJumping) {
            return false;
        }

        Debug.DrawRay(transform.position, Vector3.down, Color.red);
        if (Physics.Raycast(transform.position, Vector3.down, out hitSlope, raycastFireDistance + 0.1f)) {
            slopeAngle = (Vector3.Angle(hit.normal, transform.forward) - 90);
            if(hitSlope.normal != Vector3.up) {
                return true;
            }
        }
        return false;
        
        //Debug.DrawLine(transform.position, rbBottom, Color.blue);
        if(Physics.Raycast(transform.position, Vector3.down, out hit, rbBottom * raycastFireDistance)) {
            return true;
            
        }
        return false;
        
    }
    */
    private float GetGroundAngle() {
        if(!isGrounded()) {
            return groundAngle = 90f;
        }

        groundAngle = Vector3.Angle(Vector3.up, hit.normal);

        return groundAngle;
    }

    private void Jump() {
        //isJumping = true;
        rb.AddForce(0, jumpImpulse, 0, ForceMode.Impulse);
    }

    /*
    private void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.tag == "Ground") {
            isGrounded = true;
        }
    }
    private void OnCollisionExit(Collision collision) {
        if(collision.gameObject.tag == "Ground") {
            isGrounded = false;
        }
    }
    */
}
