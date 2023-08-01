using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class States : MonoBehaviour
{
    /******************************************************************
    * 
    * States
    * 
    *******************************************************************/

    //Enum States
    public enum CommandType
    {
        stop,
        walk,
        run,
        use
    }

    public enum FearStates
    {
        normal, //0% < 25%
        scared, //25% < 80%
        panicked // >80%
    }

    public enum SniperStates
    {
        hunting, //Sniper does not have any target in his sights
        chasing, //Sniper has a target in his sights
        reset, //Used for a slight pause when changing from one state to another excluding reload.
        reloading //Sniper should take time to reload after either expending his shots or a kill
    }

    public enum Directions
    {
        north,
        south,
        east,
        west,
        northwest,
        southwest,
        northeast,
        southeast
    }
}
