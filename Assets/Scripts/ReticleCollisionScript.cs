using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static States;

public class ReticleCollisionScript : MonoBehaviour
{
    //References
    private LayerMask hitBoxLayer;
    private SniperScript sniper;

    //State
    [SerializeField] private bool isInSights = false;
    [SerializeField] private bool notFiring = true;
    private bool isWaitingForMultiShot = false;

    // Start is called before the first frame update
    void Start()
    {
        hitBoxLayer = LayerMask.NameToLayer("Target Hit Box");
        sniper = GetComponentInParent<SniperScript>();
    }

    private void OnTriggerEnter2D(Collider2D collision) //Enters scope radius
    {
        Debug.Log("Collison with Reticle" +  collision.gameObject);

        if (collision.gameObject.layer == hitBoxLayer && //On the hit box layer?
            sniper.GetCurrentState() == SniperStates.chasing && //Are we chasing something?
            notFiring) //Are we already shooting? (don't run corountine twice)
        {
            isInSights = true; //Gotcha!
            Debug.Log("Target is in Danger Zone!");
            StartCoroutine(TriggerHesistation());
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == hitBoxLayer && //On the hit box layer?
            sniper.GetCurrentState() == SniperStates.chasing && //Are we chasing something?
            notFiring) //Are we already shooting? (don't run corountine twice)
        {
            isInSights = true;
            Debug.Log("Collison with Reticle" + collision.gameObject);
            if (!isWaitingForMultiShot) //Only run once!
            {
                StartCoroutine(MultipleShotDelay());
            }
            
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //if (collision.gameObject.layer == hitBoxLayer && //On the hit box layer?
       //     sniper.GetCurrentState() == SniperStates.chasing && //Are we chasing something?
       //     collision.gameObject.Equals(sniper.GetChasedTarget())) //Is it what we're chasing?
       // {
            Debug.Log("Out of Sights!");
            isInSights = false; //He's not in range. Shot won't connect.
       // }
    }

    IEnumerator TriggerHesistation()
    {
        Debug.Log("Hesitation!");
        notFiring = false; //Now we're shooting, cowboy!

        yield return new WaitForSeconds(sniper.GetHesitationTime());

        if(isInSights)
        {
            sniper.TakeTheShot(); //TACCOM Requesting Sniper Overwatch!
        }
        else
        {
            sniper.MissedShot();
            //Shot, but a total miss! Lose a bullet bro!
        }

        notFiring = true;
        isWaitingForMultiShot = false;

    }

    IEnumerator MultipleShotDelay()
    {
        isWaitingForMultiShot = true;
        yield return new WaitForSeconds(sniper.GetFireRate());
        StartCoroutine(TriggerHesistation());
    }
}
