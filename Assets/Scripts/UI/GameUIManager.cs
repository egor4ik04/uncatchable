using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    [SerializeField] private GameObject HP1;
    [SerializeField] private GameObject HP0;
    [SerializeField] private GameObject health;
    [SerializeField] private GameObject score;
    [SerializeField] private List<GameObject> lifes;
    private GameManager gameManager;
    private TextMeshProUGUI scoreText;

    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        lifes = new List<GameObject>();

        gameManager.OnHealthChange -= HPUpdate;
        gameManager.OnHealthChange += HPUpdate;
        gameManager.OnScoreChange -= ScoreUpdate;
        gameManager.OnScoreChange += ScoreUpdate;

        scoreText = score.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void HPUpdate()
    {
        CheckLifesNum();
        for (int l = 0; l < gameManager.MaxLifes; l++)
        {
            lifes[l].GetComponent<Image>().sprite = 
                l < gameManager.Lifes ? 
                HP1.GetComponent<Image>().sprite :
                HP0.GetComponent<Image>().sprite;
        }
    }

    private void CheckLifesNum()
    {
        if (lifes.Count != gameManager.MaxLifes)
        {
            foreach (GameObject l in lifes)
                Destroy(l);
            lifes.Clear();
            for (int l = 1; l <= gameManager.MaxLifes; l++)
            {
                var newHP = Instantiate((l > gameManager.Lifes ? HP0 : HP1), health.transform);
                newHP.SetActive(true);
                lifes.Add(newHP);
                lifes[l - 1].GetComponent<RectTransform>().position = 
                    new Vector3(50 + 100 * (l - 1), 
                    lifes[l - 1].GetComponent<RectTransform>().position.y, 0);
            }
        }
    }

    private void ScoreUpdate() => scoreText.text = gameManager.Score.ToString();
}
