using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{

    [SerializeField] private PlayerController player;
    [SerializeField] float time;
    [SerializeField] bool isLevelLoadTrigger;
    [SerializeField] ParticleSystem[] particles;
    [SerializeField] int sadsa;
    Vector3 triggerTransform;

    // Start is called before the first frame update
    void Start()
    {
        triggerTransform = GetComponent<BoxCollider>().transform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other) {
        if(!isLevelLoadTrigger) {
            StartCoroutine("LoadEndUI");
        }
    }

    private IEnumerator LoadEndUI() {
        isLevelLoadTrigger = true;
        time += Time.deltaTime;
        player.movementSpeed = 0.50f;
        Instantiate(particles[0], triggerTransform, Quaternion.identity);
        Instantiate(particles[1], triggerTransform, Quaternion.identity);
        yield return new WaitForSeconds(7f);
        time = 0f;
        player.movementSpeed = 8;
        isLevelLoadTrigger = false;

        //LOAD UI


        yield return new WaitForSeconds(7f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    }
}
