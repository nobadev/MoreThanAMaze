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

    [SerializeField] private int movementSpeed;
    [SerializeField] private int jumpImpulse;
    [SerializeField] private int sprintSpeed;
    [SerializeField] private int dashImpulse;
    private bool isGrounded;
    public float mouseSensitivity = 100f;
    private float mouseVertical = 0f;
    private float mouseHorizontal;
    public Camera cam;
    private Vector3 moveDirection;
    public Transform playerController;
    private Rigidbody rb;


    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>(); //assigns the rigidbody component of the Player to rb 
        cam = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update() {
        //multiplied by sensitivity - allows user to adjust cam sensitivity
        mouseHorizontal = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        mouseVertical -= Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime; 
        
        //clamps camera - stops it from moving past 90 degrees above and below
        mouseVertical = Mathf.Clamp(mouseVertical, -90f, 90f);

        cam.transform.localRotation = Quaternion.Euler(mouseVertical, 0f, 0f); //vertical rotation
        playerController.transform.Rotate(0, mouseHorizontal, 0); //horizontal rotation

        Debug.Log("isgrounded: " + isGrounded);
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
        if(Input.GetKeyDown(KeyCode.LeftShift)) {
           // rb.AddForce(, 0, 0, ForceMode.Impulse);
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
