using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class GameSceneController : UIController
{
    public static GameSceneController Instance { get; internal set; }

    private Coroutine indicatorAnimation;
    public Sprite submitAcceptedSprite, submitNotAcceptedSprite;
    public Image submitButtonImage;
    public Button resetButton;
    public TextMeshProUGUI scoreText, scoreAIText, indicatorText;
    public string[] indicatorTextContent;
    public float cumulativeScore, cumulativeScoreAI;
    public GameObject windowStart;
    public TextMeshProUGUI startText;
    public TextMeshProUGUI jagakataText;
    public GameObject shade, button1, button2;

    [Header("Level Design")]
    public int maxPlayerBarProgression;
    public int maxAIBarProgression;
    public float incrementBarProgression;
    public float decrementAcakBarProgression, decrementTimeBarProgression;

    [HideInInspector] public float currentBarProgression, extraProgression, currentMaxBarProgression;

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
        indicatorText.gameObject.SetActive(false);
        currentBarProgression = maxPlayerBarProgression;
        currentMaxBarProgression = maxPlayerBarProgression;
    }

    // Update is called once per frame
    void Update()
    {
        if (Board.Instance.isStarted)
        {
            if (currentBarProgression > 0) currentBarProgression -= Time.deltaTime;
            else
            {
                if (Board.Instance.isPlayerTurn)
                {
                    currentBarProgression = maxAIBarProgression;
                    currentMaxBarProgression = maxAIBarProgression;
                    SwitchTurn(false);
                    OpenIndicatorText("<color=#FF5733>Waktu habis! Giliran komputer!");
                    Board.Instance.StartAISequenceMove();
                }
                else
                {
                    currentBarProgression = maxPlayerBarProgression;
                    currentMaxBarProgression = maxPlayerBarProgression;
                    SwitchTurn(true);
                    OpenIndicatorText("<color=#33FF7E>Giliran pemain!");
                }
            }

            if (Board.Instance.isPlayerTurn)
            {
                if (currentBarProgression < 2) resetButton.interactable = false;
                else resetButton.interactable = true;
            }

            if (cumulativeScore >= 50)
            {
                Board.Instance.isStarted = false;
                startText.text = $"Poin {cumulativeScore}. {indicatorTextContent[Random.Range(0, indicatorTextContent.Length)]}";
                jagakataText.text = "Pemain menang!";
                button1.SetActive(false);
                button2.SetActive(true);
                windowStart.SetActive(true);
                shade.SetActive(true);
            } 
            else if (cumulativeScoreAI >= 50)
            {
                Board.Instance.isStarted = false;
                startText.text = $"Kurang {50 - cumulativeScore} poin lagi untuk menang!";
                jagakataText.text = "Pemain kalah!";
                button1.SetActive(false);
                windowStart.SetActive(true);
                button2.SetActive(true);
                shade.SetActive(true);
            }
        }
    }

    private void SwitchTurn(bool toggle)
    {
        List<Square> temp = new List<Square>();
        foreach (Square item in Board.Instance.temporalLetterPlace) temp.Add(item);
        foreach (Square item in temp) item.SystemReturnLetter();
        foreach (Square item in Board.Instance.letterBoxes) item.interactable = toggle;

        Board.Instance.isPlayerTurn = toggle;
        submitButtonImage.GetComponent<Button>().interactable = toggle;
        resetButton.interactable = toggle;
    }

    public void UpdateScore(int score)
    {
        cumulativeScore += score;
        scoreText.text = cumulativeScore.ToString();
    }
    public void UpdateScoreAI(int score)
    {
        cumulativeScoreAI += score;
        scoreAIText.text = cumulativeScoreAI.ToString();
    }

    public void ToggleSubmitButtonSprite(bool condition)
    {
        if (condition) submitButtonImage.sprite = submitAcceptedSprite;
        else submitButtonImage.sprite = submitNotAcceptedSprite;
    }

    private IEnumerator StartIndicatorText()
    {
        indicatorText.gameObject.SetActive(true);
        indicatorText.transform.DOJump(indicatorText.transform.position, 0.2f, 1, 0.8f);

        yield return new WaitForSeconds(1.2f);

        indicatorText.gameObject.SetActive(false);
    }

    public void OpenIndicatorText(string text = "")
    {
        if (string.IsNullOrEmpty(text))
        {
            int num = Mathf.Clamp(Board.Instance.storedString.Length - Board.MINIMAL_LETTER_COUNT, 0, indicatorTextContent.Length - 1);
            indicatorText.text = indicatorTextContent[num];
            //GameManager.Instance.PlaySfx((num + BattleController.Instance.minimalLetterCount) + " Letters");
        }
        else
        {
            indicatorText.text = text;
        }

        if (indicatorAnimation != null) StopCoroutine(indicatorAnimation);
        indicatorAnimation = StartCoroutine(StartIndicatorText());
    }

    public void Restart() 
    { 
        LoadScene("MCTSImplementation");
    }
}
