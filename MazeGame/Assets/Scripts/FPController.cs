using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class FPController : MonoBehaviour
{
    /* TO DO
     *  jumpimpulse needs to be affected by gravity
     *  adding float - affected by gravity
     */

    [SerializeField] private int movementSpeed;
    [SerializeField] private int jumpImpulse;
    [SerializeField] private int sprintSpeed;
    private Rigidbody rb;
    private bool isGrounded;
    public float mouseSensitivity = 100f;
    public Transform playerController;
    float xRotation = 0f;

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>(); //assigns the rigidbody component of the Player to rb 
    }

    // Update is called once per frame
    void Update() {
        float mouseHorizontal = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseVertical = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseVertical;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerController.Rotate(Vector3.up * mouseHorizontal);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded == true) {
            Jump();
        }
    }
    private void FixedUpdate() {
        Movement();
    }

    private void Movement() {
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
    }

    private void Jump() {
        rb.AddForce(0, jumpImpulse, 0, ForceMode.Impulse);
    }

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
}
