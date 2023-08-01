using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

using static States;

public class TargetScript : MonoBehaviour
{
    /******************************************************************
    * 
    * References & Variables
    * 
    *******************************************************************/

    //References
    public Rigidbody2D rb;
    public SpriteRenderer sr;
    public GameManagerScript gameManager;
    private HideObjectScript hidingPlace;

    //Command Variables
    
    [SerializeField] private CommandType currentTask = CommandType.stop; //What is the target currently doing
    [SerializeField] private float responseTimeDelay = 0.1f; //Delay between orders from player and target taking action

    //Fear Variables
    [SerializeField] private float fearLevel = 10.0f; //Out of 100%
    [SerializeField] private FearStates fearState = FearStates.normal; //How freaked out is the target

    //Movement Variables
    [SerializeField] private float walkSpeed = 1f;
    [SerializeField] private float runSpeed = 5f;

    //State
    [SerializeField] private bool inContactWithPlayer = false; //Does the player have this target selected
    [SerializeField] private bool canHide = false; //Should only be allowed when touching a hiding space
    [SerializeField] private bool inHiding = false; //True when object is hidden by a hide space object (Command Type will also be hide)
    [SerializeField] private Color assignedColor;
    [SerializeField] private string assignedName;


    /******************************************************************
    * 
    * Unity Functions
    * 
    *******************************************************************/


    //Awake is called when the object is being loaded
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        gameManager = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<GameManagerScript>();
    }

    /******************************************************************
    * 
    * Movement & Commands
    * 
    *******************************************************************/

    public void IssueCommand(CommandType type)
    {
        if (type == CommandType.walk || type == CommandType.run)
        {
            Debug.LogException(new ArgumentException()); //Cannot Walk or Run with variant of this function
        }

        if(type == CommandType.stop)
        {
            rb.velocity = Vector3.zero;

            if(inHiding)
            {
                gameObject.SetActive(true);
                inHiding = false;
                hidingPlace.SetIsNotOccupied();
            }

        }

        if(type == CommandType.use) 
        {
            //If touching Hiding Spot, do so
            if(canHide && hidingPlace.GetIsOccupied() != true)
            {
                gameObject.SetActive(false); //Whole thing needs to disappear
                inHiding = true;
                hidingPlace.SetIsOccupied();
            }
        }
    }

    public void IssueCommand(CommandType type, int direction) //Specifically for movement, running or walking
    {
        if(type == CommandType.stop || type == CommandType.use) 
        {
            Debug.LogException(new ArgumentException()); //Cannot Stop or Hide with variant of this function
        }

        //TODO: Implement Delay Here

        MoveInDirection(type, direction);
    }

    private void MoveInDirection(CommandType type, int direction) //Direction should be 1 or -1 for LR in X Axis respectively, type should be run or walk
    {
        //If Walk, else Run
        if (type == CommandType.walk)
        {
            rb.velocity = (new Vector3(direction, 0f, 0f)) * walkSpeed; //Vector of Direction * Speed
        }
        else
        {
            rb.velocity = (new Vector3(direction, 0f, 0f)) * runSpeed; //Vector of Direction * Speed
        }
    }

    /******************************************************************
    * 
    * State & Color
    * 
    *******************************************************************/

    public void SetColor(Color color)
    {
        assignedColor = color; //Uniquely identifable shade of Target Object!
        sr.color = assignedColor;
    }

    public Color GetColor()
    {
        return assignedColor;
    }

    public void SetName(string name)
    {
        assignedName = name;
    }

    public string GetName() 
    {
        return assignedName;
    }

    public void SetInContact()
    {
        inContactWithPlayer = true;
    }

    public void SetOutOfContact()
    {
        inContactWithPlayer = false;
    }

    /******************************************************************
    * 
    * Collisions
    * 
    *******************************************************************/

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.CompareTag("Hiding Spot") && col.gameObject.GetComponent<HideObjectScript>().GetIsOccupied() == false)
        {
            hidingPlace = col.gameObject.GetComponent<HideObjectScript>();
            canHide = true;

        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Hiding Spot"))
        {
            canHide = false;
            hidingPlace = null;
        }
    }

    /******************************************************************
    * 
    * Injury & Destruction
    * 
    *******************************************************************/

    public void GotHit()
    {
        //Can implement some other thing here if I want to do lives.
        gameManager.IncreaseSniperScore();
        gameManager.RemoveTargetFromList(gameObject);
        Destroy(gameObject);
    }

    public void GotAway()
    {
        gameManager.IncreaseScore();
        gameManager.RemoveTargetFromList(gameObject);
        Destroy(gameObject);
    }

    public void StopMovement() //Used by stairs mainly
    {
        IssueCommand(CommandType.stop);
        currentTask = CommandType.stop;
    }
}
