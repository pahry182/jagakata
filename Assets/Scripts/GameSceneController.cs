using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public enum LevelType { TIME_LETTER }

public class GameSceneController : UIController
{
    public static GameSceneController Instance { get; internal set; }

    private Coroutine indicatorAnimation;

    public CanvasGroup winWindow, loseWindow;
    public GameObject shade;
    public TextMeshProUGUI indicatorText;
    public string[] indicatorTextContent;

    [Header("Level Design")]
    public string enemyName;
    public string enemyRealName;
    public LevelType levelType = LevelType.TIME_LETTER;
    public int enemyBarProgression;
    public int playerBarProgression;
    public float enemyProgression, enemyTimeProgression, playerProgression, playerTimeProgression;

    [HideInInspector] public float currentPlayerProgression, currentEnemyProgression;

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

        currentPlayerProgression = playerBarProgression;
    }

    private void Start()
    {
        indicatorText.gameObject.SetActive(false);
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
}
