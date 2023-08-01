using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static States;

public class ScopeCollisionScript : MonoBehaviour
{
    //References
    private SniperScript sniper;
    private GameObject targetBeingChased;

    //Layer Masks
    private LayerMask detectionLayer;

    //State
    private bool inHuntingDelay = false;
    private bool foundTarget = false;

    //Variables
    [SerializeField] private float delayTime = 2f;

    private void Start()
    {
        detectionLayer = LayerMask.NameToLayer("Target Detection");
        sniper = GetComponentInParent<SniperScript>();
    }

    private void OnTriggerEnter2D(Collider2D collision) //Enters scope radius
    {
        if(collision.gameObject.layer == detectionLayer && sniper.GetCurrentState() != SniperStates.chasing) 
        {
            sniper.SetCurrentState(SniperStates.chasing);
            sniper.SetChasedTarget(collision.gameObject);
            targetBeingChased = collision.gameObject;
            foundTarget = true;
        }
        else if(collision.gameObject.layer == detectionLayer && 
            sniper.GetCurrentState() == SniperStates.chasing &&
            collision.gameObject.Equals(targetBeingChased)) //In a chase, found same target on that layer
        {
            foundTarget = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision) //Remains in scope radius
    {
        Vector3 potentialNewDirection = collision.transform.position - GetComponentInParent<Transform>().position;
        Debug.Log("Collison with Scope" + collision.gameObject);
        if (collision.gameObject.layer == detectionLayer && collision.gameObject.Equals(targetBeingChased)) //Prevents getting stuck when more than one target in view, and can be used to update the direction vector!
        {
            sniper.SetDirectionVector(potentialNewDirection); //Direction Vector = Point B (end) - Point A (start)
        }
    }

    private void OnTriggerExit2D(Collider2D collision) //Exits scope radius
    {
        if(collision.gameObject.layer == detectionLayer && 
            sniper.GetCurrentState() == SniperStates.chasing && 
            !inHuntingDelay)
        {
            foundTarget = false;
            StartCoroutine(ReturnToHuntingStateDelay()); //Activates only if same target cannot be found in given span of time
            
        }
    }

    IEnumerator ReturnToHuntingStateDelay()
    {
        inHuntingDelay = true;

        yield return new WaitForSeconds(delayTime); //How long the sniper will stay in current direction before returning to hunt state
    
        if(!foundTarget)
        {
            sniper.NullifyChasedTarget();
            targetBeingChased = null;
            sniper.SetCurrentState(SniperStates.reset);
        }

        inHuntingDelay = false;
    }
}
