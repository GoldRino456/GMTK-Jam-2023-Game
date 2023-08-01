using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FinalScoreScript : MonoBehaviour
{
    //References
    public Sprite okayScore;
    public Sprite perfectScore;
    public Sprite badScore;
    public GameManagerScript gameManager;
    public TextMeshProUGUI tmp;
    public UnityEngine.UI.Image canvasImage;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<GameManagerScript>();
        canvasImage = GetComponent<UnityEngine.UI.Image>();
        updateFinalScoreInfo();
    }

    private void updateFinalScoreInfo()
    {
        int score = gameManager.GetTargetsSaved();
        int missed = gameManager.GetTargetsKilled();
        Destroy(gameManager.gameObject);

        if(score == 0)
        {
            canvasImage.sprite = badScore;
        }

        if(score != 0 && missed != 0)
        {
            canvasImage.sprite = okayScore;
        }

        if(missed == 0)
        {
            canvasImage.sprite = perfectScore;
        }

        tmp.text = score.ToString();
    }
}
