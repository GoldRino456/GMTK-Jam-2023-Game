using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalScript : MonoBehaviour
{
    //References
    private LayerMask targetLayer; //Goal should only trigger for Targets
    private GameManagerScript gameManager;

    private void Start()
    {
        targetLayer = LayerMask.NameToLayer("Targets");
        gameManager = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<GameManagerScript>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == targetLayer)
        {
            collision.gameObject.GetComponent<TargetScript>().GotAway();
        }
    }
}
