using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    private CharacterController charController;
    public Sounds sound;
    // Start is called before the first frame update
    void Awake() {
        /*
        foreach(Sounds s in sound) {
            s.source = gameObject.AddComponent<AudioSource>();
            
           
        }
        */
    }

    // Update is called once per frame
    void Update() {
        /*
        if (charController.isGrounded && charController.velocity.magnitude > 2f && !playerAudio.isPlaying) {
            playerAudio.pitch = Random.Range(0.95f, 1.10f);
            playerAudio.Play();

        }
        */
    }
}
