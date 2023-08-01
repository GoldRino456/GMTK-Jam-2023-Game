using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
    //References
    public GameObject target; //The target prefab

    //Variables
    private GameObject[] spawnPoints; //Array of all possible spawn points
    private GameObject[] targets; //Array of all targets on a given level
    private int numStartingTargets; //Number of all Targets to start
    [SerializeField] private int targetsRescued = 0; //Score!
    [SerializeField] private int targetsKilled = 0; //Sniper kills
    [SerializeField] Color[] targetColors; //Colors given to targets spawned from the Game Manager
    [SerializeField] string[] targetNames; //Names match given colors



    private void Awake()
    {
        //Don't Destroy Yet!
        DontDestroyOnLoad(this);

        //Init Arrays
        targetColors = new Color[] { new Color(1, 0, 0, 1), new Color(0, 0, 1, 1), new Color(0, 1, 0, 1), new Color(1, 0.92f, 0.016f, 1), new Color(1, 0, 1, 1) };
        targetNames = new string[] { "Red", "Blue", "Green", "Yellow", "Purple" };

        Debug.Log(targetColors.Length);
        Debug.Log(targetNames.Length);
        spawnPoints = GameObject.FindGameObjectsWithTag("Spawn"); //Get Spawns for a given Level
        targets = new GameObject[spawnPoints.Length]; //Init Target array of length of Spawn Points

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            Transform spawnPoint = spawnPoints[i].GetComponent<Transform>();
            GameObject newTarget = Instantiate(target, spawnPoint.position, spawnPoint.rotation);
            newTarget.GetComponent<TargetScript>().SetColor(targetColors[i]);
            newTarget.GetComponent<TargetScript>().SetName(targetNames[i]);
            Debug.Log("Target object spawned.");
            targets[i] = newTarget;
        }

        numStartingTargets = targets.Length;
    }

    private void Update()
    {
        if((targetsRescued + targetsKilled) >= numStartingTargets)
        {
            StartCoroutine(OpenEndScene());
            Debug.Log("Game Over!");
        }
    }

    IEnumerator OpenEndScene()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(2); //End Scene
    }

    public GameObject[] GetTargets() { return targets; }

    public void RemoveTargetFromList(GameObject target)
    {
        List<GameObject> tempList = targets.ToList<GameObject>();
        tempList.Remove(target);
        targets = tempList.ToArray();
        Debug.Log(GetTargets().Length);
    }

    public void IncreaseScore()
    {
        targetsRescued++;
    }

    public void IncreaseSniperScore()
    {
        targetsKilled++;
    }

    public int GetTargetsSaved()
    {
        return targetsRescued;
    }

    public int GetTargetsKilled()
    {
        return targetsKilled;
    }
}
