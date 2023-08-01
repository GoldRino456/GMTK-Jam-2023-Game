using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class CanvasScript : MonoBehaviour
{
    //References
    public GameObject bulletShelf;
    private UnityEngine.UI.Image[] bullets;
    private int bulletIndex;

    private void Awake()
    {
        //bullets = bulletShelf.GetComponentsInChildren<UnityEngine.UI.Image>();
        //bulletIndex = 0;
    }

    public void HideNextBullet()
    {
        //bullets[bulletIndex].gameObject.SetActive(false);
        //bulletIndex++;
    }

    public void ShowPreviousBullet()
    {
        //bulletIndex--;
        //bullets[bulletIndex].gameObject.SetActive(true);
    }
}
