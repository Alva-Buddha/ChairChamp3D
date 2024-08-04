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
    
    [Header("'Get Ready' Variables")]
    [Tooltip("The object showing the GET READY... text.")]
    public TextMeshProUGUI getReady; // Variable used to track the 'GET READY...' text
    [Tooltip("How long should the text take to fade.")] 
    public float readyDuration = 3f; // Variable used to track how long it should take for the UI to fade
    private float elapsedTime = 0f; // Variable used to track how much time has passed 
    private Vector3 initialScale; // Variable used to track intial size of the text
    private Color initialColor; // Variable used to track the initial color of the text

    // Start function to set up Get Ready text
    void Start()
    {
        if (getReady == null)
        {
            getReady = GetComponent<TextMeshProUGUI>();
        }

        initialScale = getReady.transform.localScale;
        initialColor = getReady.color;
    }

    void Update()
    {
        // Score
        scoreText.text = "Score: " + ScoreManager.Instance.score.ToString(); // Display the score
        unoccupiedChairsText.text = "Unoccupied Chairs: " + GameManager.Instance.unoccupiedChairs.ToString(); // Display the number of unoccupied chairs

        // Get Ready
        if (elapsedTime < readyDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / readyDuration;

            // Increase scale
            getReady.transform.localScale = Vector3.Lerp(initialScale, initialScale * 2, t);

            // Increase opacity
            Color newColor = initialColor;
            newColor.a = Mathf.Lerp(initialColor.a, 0, t);
            getReady.color = newColor;
        }
    }

    // Function to handle what happens to the UI when the round ends/all chairs are occupied
    public void EndRound()
    {
        
        // Set round end text and colour depending on who won
        if (GameManager.Instance.playerChairs == 0)
        {
            roundEndText.text = "Player loses!";
        }
        else if (GameManager.Instance.npcChairs == 0)
        {
            roundEndText.text = "NPC loses!";
        }
        else
        {
            roundEndText.text = "DRAW!!";
        }
    }
}