using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameSceneController : MonoBehaviour
{
    public static GameSceneController Instance { get; internal set; }

    public Sprite submitAcceptedSprite, submitNotAcceptedSprite;
    public Image submitButtonImage;
    public TextMeshProUGUI scoreText;
    public float cumulativeScore;


    [Header("Level Design")]
    public int maxBarProgression;
    public float incrementBarProgression;
    public float decrementAcakBarProgression, decrementTimeBarProgression;

    [HideInInspector] public float currentBarProgression, extraProgression;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateScore(int score)
    {
        cumulativeScore += score;
        scoreText.text = cumulativeScore.ToString();
    }

    public void ToggleSubmitButtonSprite(bool condition)
    {
        if (condition) submitButtonImage.sprite = submitAcceptedSprite;
        else submitButtonImage.sprite = submitNotAcceptedSprite;
    }
}
