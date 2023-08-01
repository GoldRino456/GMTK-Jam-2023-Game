using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

using static States;

public class SniperScript : MonoBehaviour
{
    //References
    public CanvasScript canvas;
    private Rigidbody2D rb;
    public GameObject scopeRadius;
    public GameObject reticleRadius;
    private GameObject targetBeingChased;
    private AudioSource audioSource;

    //Audio
    public AudioClip sniperShot;
    public AudioClip reloadSFX;

    //Movement Variables
    [SerializeField] private float movementSpeed = 10f;
    [SerializeField] private float minTravelTime = 1f;
    [SerializeField] private float maxTravelTime = 5f;
    [SerializeField] private float minWaitTime = 0.5f;
    [SerializeField] private float maxWaitTime = 2f;
    [SerializeField] private float maxVelocity = 7f;
    [SerializeField] private Vector3 directionVector = Vector3.zero;

    //Weapon Variables
    [SerializeField] private float fireRate = 1.2f;
    [SerializeField] private float reloadTime = 0.7f;
    [SerializeField] private float triggerHesitationTime = 0.1f;
    [SerializeField] private float accuracy = 0.3f;
    [SerializeField] private int maxNumBullets = 5;
    [SerializeField] private int numBullets = 5;

    
    
    

    //State
    [SerializeField] private SniperStates currentState = States.SniperStates.hunting; //By default the AI should begin by hunting.
    private Directions movementDirection; //Used to determine travel direction for the sniper scope
    private bool isTraveling = false; //Is the sniper scope moving? True = Moving, False = Holding Position
    private bool movementCoroutineFlag; //Used to determine when the coroutine starts and finishes so it can be called again.
    private bool isReloadNeeded = false;
    private bool isReloading = false;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = rb.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isReloadNeeded && currentState != SniperStates.reloading)
        {
            currentState = SniperStates.reloading;
        }

        if(currentState == SniperStates.reloading)
        {
            ReloadingState();
        }

        if(currentState == SniperStates.reset)
        {
            StopCoroutine(MoveScope());
            StartCoroutine(HaltMovement());
        }

        if(currentState == SniperStates.hunting)
        {
            HuntingState();
        }

        if(currentState == SniperStates.chasing)
        {
            ChasingState();
        }

    }

    private void HuntingState()
    {
        if (!movementCoroutineFlag)
        {
            StartCoroutine(MoveScope());
        }

        if (isTraveling)
        {
            ProcessMovementInDirection();
        }

        if (!isTraveling)
        {
            rb.velocity = Vector3.zero; //Hard Stop
        }
    }

    private void ChasingState()
    {
        //Change Speed and/or factor in reaction time here?
        ProcessMovementInDirection(); //GO GO GO GO GO!
    }

    private void ReloadingState()
    {
        if(!isReloading)
        {
            directionVector = Vector3.zero;
            ProcessMovementInDirection();
            StartCoroutine(ReloadBullets());
        }

        
    }

    private void ChooseRandomDirection()
    {
        movementDirection = (Directions)UnityEngine.Random.Range(0,8); //Only ever 8 possible cardinal directions, so this will not change. See state in States.cs
        UpdateDirectionVector();
    }  

    private void ProcessMovementInDirection()
    {
        rb.velocity = directionVector * movementSpeed;
        rb.velocity = Vector2.ClampMagnitude(directionVector * movementSpeed, maxVelocity);
    }

    private void UpdateDirectionVector()
    {
        switch (movementDirection)
        {
            case Directions.north:
                directionVector = Vector3.up;
                break;

            case Directions.south:
                directionVector = Vector3.down;
                break;

            case Directions.east:
                directionVector = Vector3.right;
                break;

            case Directions.west:
                directionVector = Vector3.left;
                break;

            case Directions.northwest:
                directionVector = Vector3.up + Vector3.left;
                break;

            case Directions.northeast:
                directionVector = Vector3.up + Vector3.right;
                break;

            case Directions.southwest:
                directionVector = Vector3.down + Vector3.left;
                break;

            case Directions.southeast:
                directionVector = Vector3.down + Vector3.right;
                break;

            default:
                Debug.Log("ERR: Movement Direction Invalid! - SniperScript.cs - ProcessMovementInDirection()");
                Debug.LogException(new ArgumentException());
                break;
        }
    }

    //Referenced this video by Unitips on YouTube for this part: https://www.youtube.com/watch?v=r6zhaguEzs8
    //Thank you for a really helpful tutorial!
    IEnumerator MoveScope()
    {
        float moveTime = UnityEngine.Random.Range(minTravelTime, maxTravelTime);
        float waitTime = UnityEngine.Random.Range(minWaitTime, maxWaitTime);

        movementCoroutineFlag = true; //Coroutine is RUNNING!

        //Change Direction or Choose on First Running
        ChooseRandomDirection();

        //Always start coroutine with movement
        isTraveling = true;
        yield return new WaitForSeconds(moveTime);

        //Hold scope on area
        isTraveling = false;
        yield return new WaitForSeconds(waitTime);

        //Flag that Coroutine has finished.
        movementCoroutineFlag = false;
    }

    IEnumerator HaltMovement()
    {
        movementCoroutineFlag = false; //Reset Movement Coroutine Flag
        directionVector = Vector3.zero; //Reset Direction Vector
        ProcessMovementInDirection(); //Ensure Scope Halts
        yield return new WaitForSeconds(minWaitTime);

        if(targetBeingChased == null) //Didn't start a chase during halted movement
        {
            SetCurrentState(SniperStates.hunting); //Return to hunting state
        }    
    }

    IEnumerator ReloadBullets()
    {
        isReloading = true;

        for(int i = numBullets; i < maxNumBullets; i++) 
        {
            //Play sound either way
            audioSource.clip = reloadSFX;
            audioSource.Play();

            numBullets++;
            canvas.ShowPreviousBullet();
            yield return new WaitForSeconds(reloadTime);
        }

        isReloadNeeded = false;
        isReloading = false;
        SetCurrentState(SniperStates.hunting);
    }

    /*private bool CheckTargetChangedDirections(Vector3 currentDirection, Vector3 newDirection)
    {
        //Check for change in sign on X axis
        if((currentDirection.x >= 0 && newDirection.x < 0) || currentDirection.x <= 0 && newDirection.x > 0)
        {
            return true;
        }


        //Check for change in sign on Y axis
        if ((currentDirection.y >= 0 && newDirection.y < 0) || currentDirection.y <= 0 && newDirection.y > 0)
        {
            return true;
        }

        return false;
    }*/

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ChooseRandomDirection(); //If scope reaches a wall, roll a new direction.
    }

    public SniperStates GetCurrentState()
    {
        return currentState;
    }

    public void SetCurrentState(SniperStates newState)
    {
        currentState = newState;
    }

    public void SetChasedTarget(GameObject target)
    {
        targetBeingChased = target;
    }

    public void SetDirectionVector(Vector3 direction)
    {
        directionVector = direction;
    }

    public GameObject GetChasedTarget() 
    {
        return targetBeingChased;
    }

    public void NullifyChasedTarget()
    {
        targetBeingChased = null;
    }

    public void TakeTheShot()
    {
        //Play sound either way
        audioSource.clip = sniperShot;
        audioSource.Play();

        float randNum = UnityEngine.Random.Range(0f, 1f);
        Debug.Log("Taking the shot!");

        if(randNum < accuracy) //Accuracy % chance of a successful hit!
        {
            targetBeingChased.GetComponentInParent<TargetScript>().GotHit(); //Got em' coach!
        }

        numBullets--;
        canvas.HideNextBullet();

        if(numBullets < 1)
        {
            isReloadNeeded = true;
        }
    }

    public void MissedShot()
    {
        //Play sound
        audioSource.clip = sniperShot;
        audioSource.Play();

        numBullets--;
        canvas.HideNextBullet();
        
        if (numBullets < 1)
        {
            isReloadNeeded = true;
        }
    }
    
    public float GetHesitationTime()
    {
        return triggerHesitationTime;
    }

    public float GetFireRate() { return fireRate; }
}
