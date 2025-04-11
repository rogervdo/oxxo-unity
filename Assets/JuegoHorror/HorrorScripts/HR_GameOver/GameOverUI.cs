using UnityEngine;
using UnityEngine.UI;          


public class GameOverUI : MonoBehaviour
{

    public Text finalScoreText; 

    void Start()
    {
        int score = PlayerPrefs.GetInt("LastScore", 0);
        finalScoreText.text = score.ToString();
    }
}