using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScore : MonoBehaviour
{

    int orbsCollected;
    float timeBonus;
    [SerializeField] int orbWorth;
    float endOfLevelScore;
    float finalScore;




    void OnLevelComplete() {

        for(int i = 0; i < orbsCollected; i++) {
            endOfLevelScore += orbWorth * 4;
        }



        finalScore += endOfLevelScore;
        orbsCollected = 0;
        endOfLevelScore = 0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
