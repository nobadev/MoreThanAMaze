using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPController : MonoBehaviour
{

    bool isGrounded = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.W)) {
            transform.Translate(new Vector3(0, 0, 2) * Time.deltaTime);
        }
        if(Input.GetKey(KeyCode.S)) {
            transform.Translate(new Vector3(0, 0, -2) * Time.deltaTime);
        }
        if(Input.GetKey(KeyCode.A)) {
            transform.Translate(new Vector3(-2, 0, 0) * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D)) {
            transform.Translate(new Vector3(2, 0, 0) * Time.deltaTime);
        }
        if(Input.GetKey(KeyCode.Space) && isGrounded == true) {
            GetComponent<Rigidbody>().AddForce(new Vector3(0, 5, 0), ForceMode.Impulse);
        }
    }

    void JumpCooldown(Collider targetCollision) { //coroutine function to destroy rockets after specified time
        if (targetCollision.gameObject.layer.Equals("Ground")) {
            isGrounded = true;
        }
        else {
            isGrounded = false;
        }
    }
}
