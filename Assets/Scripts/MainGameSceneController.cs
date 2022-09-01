using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public enum LevelType { TIME_LETTER }

public class MainGameSceneController : UIController
{
    public static MainGameSceneController Instance { get; internal set; }

    private Coroutine indicatorAnimation;

    public DisplayPageTextHandler[] displayPages;
    public GameObject displayTextPrefab;
    public CanvasGroup windowStart, windowPause;
    public GameObject shade;
    public TextMeshProUGUI indicatorText, scoreText;
    public GridLayout gridLayoutActiveLetter;
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
        GameManager.Instance.PlayBgm("Gameplay");
        indicatorText.gameObject.SetActive(false);
        scoreText.text = "0";
        //Destroy all child upon start on LetterActivePlace
        foreach (Transform item in BattleController.Instance.letterActivePlace.transform)
        {
            Destroy(item.gameObject);
        }

        Time.timeScale = 0f;
    }

    public void OpenShade()
    {
        shade.SetActive(true);
    }

    public void OpenLoseWindow()
    {
        OpenShade();
        StartCoroutine(ShadeToDark());
    }

    public IEnumerator ShadeToDark()
    {
        Image shadeAlpha = shade.GetComponent<Image>();
        while (shadeAlpha.color.a < 1)
        {
            shadeAlpha.color = new Color(shadeAlpha.color.r, shadeAlpha.color.g, shadeAlpha.color.b, shadeAlpha.color.a + 0.005f);
            yield return new WaitForSeconds(1f);
        }

        yield return new WaitForSeconds(0.8f);
        LoadScene("ResultScene");
    }

    public void CloseShade(float time = 0f)
    {
        StartCoroutine(CloseShadeAnimation(time));
    }

    private IEnumerator CloseShadeAnimation(float time)
    {
        yield return new WaitForSeconds(time);
        shade.SetActive(false);
    }

    public void ClosePauseWindow()
    {
        CloseShade();
        StartCoroutine(FadeOut(windowPause, 0.15f));
        Time.timeScale = 1f;
    }

    public void OpenIndicatorText()
    {
        indicatorText.text = indicatorTextContent[BattleController.Instance.storedString.Length - BattleController.Instance.minimalLetterCount];
        if (indicatorAnimation != null) StopCoroutine(indicatorAnimation);
        indicatorAnimation = StartCoroutine(StartIndicatorText());
        GameManager.Instance.PlaySfx(BattleController.Instance.storedString.Length + " Letters");
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
            BattleController.Instance.SpawnPowerUp(PowerUpTypes.POWER1);
        }

        GameObject displayText = Instantiate(displayTextPrefab, parent);
        displayText.GetComponent<TextMeshProUGUI>().text = _text;
    }

    public void UpdateScore()
    {
        cumulativeScore += BattleController.Instance.CalculateScore();
        scoreText.text = cumulativeScore.ToString();
    }

    public void PauseGame()
    {
        OpenShade();
        StartCoroutine(FadeIn(windowPause, 0.15f));
        Time.timeScale = 0f;
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        StartCoroutine(FadeOut(windowStart, 0.15f));
        CloseShade(0.15f);
    }
}
