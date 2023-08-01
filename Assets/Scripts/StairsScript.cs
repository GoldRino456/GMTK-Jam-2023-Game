using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairsScript : MonoBehaviour
{
    //References
    [SerializeField] public StairsScript linkedStairs; //The other end of the stairs, either top or bottom repsectively
    [SerializeField] private bool stillTouchingStair; //Changed by the linkedStairs
    LayerMask targetLayer; //Stairs should only trigger for Targets

    private void Start()
    {
        targetLayer = LayerMask.NameToLayer("Targets");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == targetLayer && !stillTouchingStair)
        {
            stillTouchingStair = false;
            linkedStairs.SetTouchingStair(true);
            collision.gameObject.transform.position = new Vector3(linkedStairs.transform.position.x, linkedStairs.transform.position.y, 0f); //Send colliding object to the other side of the stairs
            collision.gameObject.GetComponent<TargetScript>().StopMovement();

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == targetLayer)
        {
            stillTouchingStair = false; //Other objects can now use the stairs again!
        }
    }

    public void SetTouchingStair(bool tf)
    {
        stillTouchingStair = tf; //True if an object just used the stairs and has not yet moved off, false otherwise
    }
}
