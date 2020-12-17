using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITest : MonoBehaviour
{

    public Image HealthBar;
    [SerializeField] float maxHealth = 100f;
    [SerializeField] float currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        HealthBar = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)) {
            StartCoroutine("HPRestore");
        }

        HealthBar.fillAmount = currentHealth / maxHealth;
    }

    
    
    private IEnumerator HPRestore() {
        currentHealth -= 50f;
        yield return null;
    }
}
