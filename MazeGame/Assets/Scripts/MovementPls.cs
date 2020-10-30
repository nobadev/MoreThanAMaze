using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementPls : MonoBehaviour
{
    [SerializeField] int movementSpeed;
    [SerializeField] float gravity = -13.0f;
    [SerializeField] Transform playerCam;
    [SerializeField] int mouseSensitivity;
    float yRotation = 0.0f;
    Rigidbody rb;
    Vector3 velocityY;
    bool isGrounded = true;
    float downSpeed;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        CameraMovement();
    }
    private void FixedUpdate() {
        PlayerMovement();
    }

    private void PlayerMovement() {
        Vector2 movementDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        movementDir.Normalize();

        //checks if grounded - resets velocity.y to 0 otherwise it stacks up
        if (isGrounded) {
            downSpeed = 0.0f;
        }

        downSpeed += gravity * Time.deltaTime;

        Vector3 velocity = (movementDir.y * transform.forward + movementDir.x * transform.right) * movementSpeed + Vector3.up * downSpeed;
        //velocityY = new Vector3(0, rb.velocity.y, 0);
        rb.velocity = velocity * Time.deltaTime;
        //rb.velocity += velocityY;
    }

    private void CameraMovement() {
        Vector2 mouseVector = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        yRotation -= mouseVector.y * mouseSensitivity;
        yRotation = Mathf.Clamp(yRotation, -90.0f, 90.0f);
        playerCam.localEulerAngles = Vector3.right * yRotation;
        transform.Rotate(Vector3.up * mouseVector.x * mouseSensitivity);
    }
}
