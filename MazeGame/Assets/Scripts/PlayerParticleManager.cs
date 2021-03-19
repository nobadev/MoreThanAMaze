using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParticleManager : MonoBehaviour
{

    [SerializeField] PlayerController player;
    [SerializeField] ParticleSystem[] particles;
    private Vector3 playerFoot;

    // Start is called before the first frame update
    void Start()
    {
        //playerFoot = player.gameObject.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LandParticles() {
        //if(player.airTime)
    }

    private IEnumerator PlayLandParticleA() {
        
        yield return null;
    }

}
