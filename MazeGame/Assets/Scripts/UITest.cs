using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITest : MonoBehaviour
{

    public Image HealthBar;
    [SerializeField] float maxHealth = 100f;
    [SerializeField] float currentHealth;
    [SerializeField] float healthChargeRate;
    bool isHPRestoreRunning;

    // Start is called before the first frame update
    void Start()
    {
        HealthBar = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)) {
            currentHealth -= 25f;
        }
        HealthBar.fillAmount = currentHealth / maxHealth;
        StatCheck();
    }

    private void StatCheck() {
        if(currentHealth < maxHealth) {
            StartCoroutine("HPRestore");
        }
    }
    
    private IEnumerator HPRestore() {
        if(isHPRestoreRunning) {
            yield break;
        }

        isHPRestoreRunning = true;
        yield return new WaitForSeconds(2f);
        while(currentHealth < maxHealth) {
            currentHealth += healthChargeRate;
        }

        isHPRestoreRunning = false;
    }
}
