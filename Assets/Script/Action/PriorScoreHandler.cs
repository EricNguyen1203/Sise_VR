using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PriorScoreHandler : MonoBehaviour
{
    public TMP_Text scoreText;
    // Start is called before the first frame update
    void Start()
    {
        scoreText = GetComponent<TMP_Text>();
        if (scoreText == null)
        {
            Debug.LogError("Text component not found on the GameObject.");
            return;
        }
    }

    public void IncreaseScore()
    {
        if (scoreText == null)
        {
            Debug.LogError("Text component is not assigned.");
            return;
        }

        float currentScore = float.Parse(scoreText.text);
        if (currentScore == 1.0f)
        {
            Debug.Log("Score is already at maximum.");
            return;
        }

        currentScore += 0.1f; // Increase score by 0.1
        scoreText.text = currentScore.ToString();
    }

    public void DecreaseScore()
    {
        if (scoreText == null)
        {
            Debug.LogError("Text component is not assigned.");
            return;
        }

        float currentScore = float.Parse(scoreText.text);
        if (currentScore == 0.1f)
        {
            Debug.Log("Score is already at minimum.");
            return;
        }

        currentScore -= 0.1f; // Decrease score by 0.1
        scoreText.text = currentScore.ToString();
    }

    public float GetCurrentScore()
    {
        if (scoreText == null)
        {
            Debug.LogError("Text component is not assigned.");
            return 0f;
        }

        float currentScore = float.Parse(scoreText.text);
        return currentScore;
    }
}
