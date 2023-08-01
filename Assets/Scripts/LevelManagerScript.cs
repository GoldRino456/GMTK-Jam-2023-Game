using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManagerScript : MonoBehaviour
{
    public void LoadLevel()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadEndScreen()
    {
        SceneManager.LoadScene(2);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) 
        {
            Application.Quit();
        }
    }
}
