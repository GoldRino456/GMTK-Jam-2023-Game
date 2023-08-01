using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InputManagerScript : MonoBehaviour
{
    //References
    public GameObject targetText;
    private GameManagerScript gameManager;
    private TargetScript selectedTarget;
    private int selectedTargetIndex;

    //Variables
    private bool walkKeyPressed = false; //Used in checking for double input, telling the target to run
    private float walkPressedTime;
    [SerializeField] private float doubleClickTimer = 1.5f; //How long the player has to double click and issue a run command
    private TextMeshProUGUI textElement;
    private bool canStillDoubleKeyPress = false; //True while user can potentially double press the left or right key. False otherwise.


    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<GameManagerScript>();
        selectedTarget = gameManager.GetTargets()[0].GetComponent<TargetScript>(); //Select first target from list, list should have at least 1 target on map
        selectedTargetIndex = 0;
        selectedTarget.SetInContact();
        textElement = targetText.GetComponent<TextMeshProUGUI>();
        
    }

    // Update is called once per frame
    void Update()
    {
        ChangeTargetKeyPress();
        CommandKeyPress();
    }

    private void CommandKeyPress()
    {
        if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            if(CheckDoubleKeyPress(KeyCode.RightArrow) || CheckDoubleKeyPress(KeyCode.D))
            {
                selectedTarget.IssueCommand(States.CommandType.run, 1);
            }
            else
            {
                selectedTarget.IssueCommand(States.CommandType.walk, 1);
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            if (CheckDoubleKeyPress(KeyCode.LeftArrow) || CheckDoubleKeyPress(KeyCode.A))
            {
                selectedTarget.IssueCommand(States.CommandType.run, -1);
            }
            else
            {
                selectedTarget.IssueCommand(States.CommandType.walk, -1);
            }
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            selectedTarget.IssueCommand(States.CommandType.stop);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            selectedTarget.IssueCommand(States.CommandType.use);
        }

        if(!canStillDoubleKeyPress)
        {
            ResetDoubleClick();
        }
    }

    //Code for this was adapted from an answer by felixpk on the Unity forms: https://discussions.unity.com/t/checking-if-a-key-has-been-pressed-twice-in-quick-succession/39076
    //Your valuable 2016 insight was put to use 7 years in the future! Thank you very much and I hope you're doing well!
    private bool CheckDoubleKeyPress(KeyCode key) 
    {

        if(Input.GetKeyDown(key) && walkKeyPressed)
        {
            if(Time.time - walkPressedTime < doubleClickTimer)
            {
                canStillDoubleKeyPress = false;
                return true;
            }
            else
            {
                canStillDoubleKeyPress = false;
                return false;
            } 
        }

        if(Input.GetKeyDown(key) && !walkKeyPressed)
        {
            walkKeyPressed = true;
            canStillDoubleKeyPress = true;
            walkPressedTime = Time.time;
        }

        return false;
    }

    private void ResetDoubleClick()
    {
        walkKeyPressed = false;
    }

    private void ChangeTargetKeyPress()
    {
        if(Input.GetKeyDown(KeyCode.Period) || Input.GetKeyDown(KeyCode.X))
        {
            GetNextTarget();
        }

        if (Input.GetKeyDown(KeyCode.Comma) || Input.GetKeyDown(KeyCode.Z))
        {
            GetPreviousTarget();  
        }

        UpdateTargetUIText();
    }
    
    private void GetNextTarget()
    {
        int listLen = gameManager.GetTargets().Length;

        //If Next Element exists (would index go out of bounds?)
        if(selectedTargetIndex + 2 > listLen) //If list is 1, MAXidx would be 0 + 2 = 2 and loop back to 0. If list is 5, MAXidx would be 4 + 2 = 6, and loop back to 0.
        { 
            selectedTargetIndex = 0;
            selectedTarget.SetOutOfContact();
            selectedTarget = gameManager.GetTargets()[selectedTargetIndex].GetComponent<TargetScript>();
            selectedTarget.SetInContact();
        }
        else
        {
            selectedTargetIndex++;
            selectedTarget.SetOutOfContact();
            selectedTarget = gameManager.GetTargets()[selectedTargetIndex].GetComponent<TargetScript>();
            selectedTarget.SetInContact();
        }
    }

    private void GetPreviousTarget()
    {
        int listLen = gameManager.GetTargets().Length;

        //If Previous Element exists (would index go out of bounds?)
        if (selectedTargetIndex.Equals(0))
        {
            selectedTargetIndex = listLen - 1; //Len of list, but minus 1 (the index of the last element)
            selectedTarget.SetOutOfContact();
            selectedTarget = gameManager.GetTargets()[selectedTargetIndex].GetComponent<TargetScript>();
            selectedTarget.SetInContact();
        }
        else
        {
            selectedTargetIndex--;
            selectedTarget.SetOutOfContact();
            selectedTarget = gameManager.GetTargets()[selectedTargetIndex].GetComponent<TargetScript>();
            selectedTarget.SetInContact();
        }
    }

    private void UpdateTargetUIText()
    {
        textElement.color = selectedTarget.GetColor();
        textElement.text = selectedTarget.GetName();
    }
}
