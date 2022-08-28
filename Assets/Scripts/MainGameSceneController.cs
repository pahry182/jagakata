using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public enum LevelType { TIME_LETTER }

public class MainGameSceneController : UIController
{
    public static MainGameSceneController Instance { get; internal set; }

    private Coroutine indicatorAnimation;

    public DisplayPageTextHandler[] displayPages;
    public GameObject displayTextPrefab;
    public CanvasGroup winWindow, loseWindow;
    public GameObject shade;
    public TextMeshProUGUI indicatorText, scoreText;
    public string[] indicatorTextContent;
    public int maximumPageDisplayText;
    public float cumulativeScore;
    

    [Header("Level Design")]
    public string enemyName;
    public string enemyRealName;
    public LevelType levelType = LevelType.TIME_LETTER;
    public int maxBarProgression;
    public float incrementBarProgression;
    public float decrementAcakBarProgression, decrementTimeBarProgression;
    
    [HideInInspector] public float currentBarProgression;

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

        currentBarProgression = maxBarProgression;
    }

    private void Start()
    {
        indicatorText.gameObject.SetActive(false);
        scoreText.text = "0";
        //Destroy all child upon start on LetterActivePlace
        foreach (Transform item in BattleController.Instance.letterActivePlace.transform)
        {
            Destroy(item.gameObject);
        }
    }

    public void OpenShade()
    {
        shade.SetActive(true);
    }

    public void OpenLoseWindow()
    {
        OpenShade();
        StartCoroutine(FadeIn(loseWindow, 0.4f));
    }

    public void CloseShade()
    {
        shade.SetActive(false);
    }

    public void CloseLoseWindow()
    {
        CloseShade();
        StartCoroutine(FadeOut(loseWindow, 0.4f));
    }

    public void OpenIndicatorText()
    {
        indicatorText.text = indicatorTextContent[BattleController.Instance.storedString.Length - BattleController.Instance.minimalLetterCount];
        if (indicatorAnimation != null) StopCoroutine(indicatorAnimation);
        indicatorAnimation = StartCoroutine(StartIndicatorText());
    }

    private IEnumerator StartIndicatorText()
    {
        indicatorText.gameObject.SetActive(true);
        indicatorText.transform.DOJump(indicatorText.transform.position, 0.2f, 1, 0.8f);

        yield return new WaitForSeconds(0.8f);

        indicatorText.gameObject.SetActive(false);
    }

    public void AddToDisplayPage(string _text)
    {
        Transform parent;

        if (displayPages[0].transform.childCount < maximumPageDisplayText)
        {
            parent = displayPages[0].transform;
        }
        else if (displayPages[1].transform.childCount < maximumPageDisplayText)
        {
            parent = displayPages[1].transform;
        }
        else
        {
            foreach (DisplayPageTextHandler item in displayPages)
            {
                item.RemoveAllChild();
            }
            //Flip the Page Codes here
            parent = displayPages[0].transform;
        }

        GameObject displayText = Instantiate(displayTextPrefab, parent);
        displayText.GetComponent<TextMeshProUGUI>().text = _text;
    }

    public void UpdateScore()
    {
        cumulativeScore += GameManager.Instance.CalculateScore();
        scoreText.text = cumulativeScore.ToString();
    }

    public void EndGame()
    {
        LoadScene("ResultScene");
    }
}
