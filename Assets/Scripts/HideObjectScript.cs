using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideObjectScript : MonoBehaviour
{
    //References
    private SpriteRenderer sr;

    //Variables
    public Sprite openHidingSpot;
    public Sprite occupiedHidingSpot;
    public bool isOccupied = false;

    private void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        sr.sprite = openHidingSpot;
    }

    public bool GetIsOccupied() { return isOccupied; }

    public void SetIsOccupied() 
    { 
        isOccupied = true;
        sr.sprite = occupiedHidingSpot;
    }

    public void SetIsNotOccupied()
    {
        isOccupied = false;
        sr.sprite = openHidingSpot;
    }


}
