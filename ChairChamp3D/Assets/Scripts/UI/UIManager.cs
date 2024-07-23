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

    // Start is called before the first frame update
    void Start()
    {
        scoreText.text = "Score: " + GameManager.Instance.score.ToString(); // Display the score
        unoccupiedChairsText.text = "Unoccupied Chairs: " + GameManager.Instance.unoccupiedChairs.ToString(); // Display the number of unoccupied chairs
    }

    // Function to handle what happens to the UI when the round ends/all chairs are occupied
    public void EndRound()
    {
        
        // Set round end text and colour depending on who won
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
    }
}