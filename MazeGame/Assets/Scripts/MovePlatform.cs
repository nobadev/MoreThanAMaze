using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlatform : MonoBehaviour
{
    public GameObject Player;

    private void OnTriggerEnter(Collider collision) {
        if(collision.gameObject == Player) {
            Player.transform.parent = transform;
        }
    }

    private void OnTriggerExit(Collider collision) {
        if(collision.gameObject == Player) {
            Player.transform.parent = null;
        }
    }
}
