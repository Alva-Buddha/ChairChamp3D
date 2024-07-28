using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class EndGameUI : MonoBehaviour
{
    public TextMeshProUGUI winnerText;  // Reference to the score Text component
    public TextMeshProUGUI scoreText;  // Reference to the score Text component

    void Start()
    {
        if (ScoreManager.Instance.GetScore() >= 2)
        {
            winnerText.color = Color.blue;
            winnerText.text = "Player wins!";

            scoreText.color = Color.blue;
            scoreText.text = "Player Score: " + ScoreManager.Instance.GetScore().ToString();
        }
        else
        {
            winnerText.color = Color.yellow;
            winnerText.text = "NPC wins!";

            scoreText.color = Color.yellow;
            scoreText.text = "Player Score: " + ScoreManager.Instance.GetScore().ToString();
        }
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
