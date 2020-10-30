using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    Vector2 movementDir;
    public int movementSpeed;
    [SerializeField] private Transform playerCam;
    public float mouseSensitivity;
    float yRotation = 0.0f;
    Rigidbody rb;
    Vector3 velocity;
    Vector2 mouseVector;
    public int jumpForce;
    RaycastHit hit;
    [SerializeField]float distanceToGround;
    Vector3 forward;
    float groundAngle;
    [SerializeField] float maxGroundAngle = 120f;
    float capHeight = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        GetPlayerInputs();
        Debug.Log(groundAngle);
        //Debug.Log(isGrounded());
    }

    private void FixedUpdate() {
        CameraMovement();
        CalculateGroundAngle();
        CalculateForward();
        PlayerMovement();
        //SetDrag();
    }

    private void GetPlayerInputs() {
        movementDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        mouseVector = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        if(Input.GetKeyDown(KeyCode.Space) && isGrounded()) {
            Jump();
        }
    }

    private void PlayerMovement() {
        if (groundAngle >= maxGroundAngle) return;
        isGrounded();

        velocity = ((movementDir.y * forward) + (movementDir.x * transform.right)).normalized * movementSpeed * Time.fixedDeltaTime;
        rb.MovePosition(transform.position + velocity);

        //Vector3.Pro
    }

    //Camera
    private void CameraMovement() {
        yRotation -= mouseVector.y * mouseSensitivity;
        yRotation = Mathf.Clamp(yRotation, -90.0f, 90.0f);
        playerCam.localEulerAngles = Vector3.right * yRotation;
        transform.Rotate(Vector3.up * mouseVector.x * mouseSensitivity);
    }

    //Calculates forward - we calculate the 
    void CalculateForward() {
        if (!isGrounded()) {
            forward = transform.forward;
            return;
        }

        forward = Vector3.Cross(transform.right, hit.normal);
    }

    void CalculateGroundAngle() {
        if(!isGrounded()) {
            groundAngle = 90;
            return;
        }

        groundAngle = Vector3.Angle(hit.normal, transform.forward);
    }
    
    bool OnSlope() {
        if (groundAngle != 90 && groundAngle <= 120) {
            return true;
        }
        return false;
    }

    /*void SetDrag() {
        if(OnSlope()) {
            rb.drag = 50;
        }
        else {
            rb.drag = 0;
            return;
        }
    }*/

    private bool isGrounded() {

        
        Debug.DrawLine(transform.position, transform.position + forward * capHeight * 2, Color.blue);
        if (Physics.Raycast(transform.position, Vector3.down, out hit, capHeight + 1.5f)) {
            return true;
        }
        return false;
    }
    
    private void Jump() {
        rb.AddForce(0, jumpForce, 0, ForceMode.VelocityChange);
    }
}