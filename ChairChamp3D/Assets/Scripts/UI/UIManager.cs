using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Tooltip("The TextMeshPro object showing the score.")]
    public TextMeshProUGUI scoreText;  // Reference to the score Text component
    [Tooltip("The TextMeshPro object showing the number of unoccupied chairs.")]
    public TextMeshProUGUI unoccupiedChairsText; // Reference to the number of unoccupied chairs left
    [Tooltip("The TextMeshPro object showing the round end text.")]
    public TextMeshProUGUI roundEndText; // Reference to the round end Text component
    [Tooltip("The TextMeshPro object showing the restart button.")]
    public Button restartButton; // Reference to the restart button

    // Start is called before the first frame update
    void Start()
    {
        roundEndText.gameObject.SetActive(false);  // Hide the round end text at the start
        restartButton.gameObject.SetActive(false);  // Hide the round end text at the start
    }

    void Update()
    {
        // Display the score
        scoreText.text = "Score: " + GameManager.Instance.score.ToString();
        // Display the number of unoccupied chairs
        unoccupiedChairsText.text = "Unoccupied Chairs: " + GameManager.Instance.unoccupiedChairs.ToString();
        // If the number of unoccupied chairs = 0, display round end text
        if (GameManager.Instance.unoccupiedChairs == 0)
        {
            if (GameManager.Instance.playerChairs == 0)
            {
                roundEndText.color = Color.yellow;
                roundEndText.text = "Player loses!";
            }
            else if (GameManager.Instance.npcChairs == 0)
            {
                roundEndText.color = Color.blue;
                roundEndText.text = "NPC loses!";
            }
            else
            {
                roundEndText.color = Color.white;
                roundEndText.text = "DRAW!!";
            }
            roundEndText.gameObject.SetActive(true);  // Show the round end text
            restartButton.gameObject.SetActive(true);  // Show the restart button 
        }
    }
}