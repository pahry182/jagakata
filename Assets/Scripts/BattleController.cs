using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleController : MonoBehaviour
{
    public static BattleController Instance { internal set; get; }

    private MainGameSceneController gameSceneController;
    private bool isLose;

    public Image progressBar;
    public Transform[] letterButtonTransforms;
    public Vector2[] letterButtonPositions;
    public Transform letterActivePlace;
    public Transform letterContainer;
    public Transform canvas;

    public string storedString;
    public string[] storedLetters;
    public List<int> storedIndexLetters;
    public List<string> submittedWords;
    public int minimalLetterCount;

    [Header("Letter Weight")]
    public string scoreOf5;
    public string scoreOf4, scoreOf3, scoreOf2, scoreOf1;

    [Header("PowerUp Sprite")]
    public Sprite power1;
    public Sprite power2, power3, nonPower;
    public float power1mod, power2mod, power3mod;

    private LetterButton[] letterButtons = new LetterButton[16];
    private LetterButton[] movingLetters;
    private ScoreAnimationHandler _sah;
    [HideInInspector] public List<LetterButton> temporalLetterPlace = new List<LetterButton>();


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

        int index = 0;
        foreach (var item in letterButtonTransforms)
        {
            letterButtonPositions[index] = item.position;
            letterButtons[index] = item.GetComponent<LetterButton>();
            index++;
        }
    }

    private void Start()
    {
        gameSceneController = MainGameSceneController.Instance;
        _sah = GetComponent<ScoreAnimationHandler>();
    }

    private void Update()
    {
        UpdateProgression();
    }

    public int GetIndexLetterButton(Transform letterButton)
    {
        return Array.IndexOf(letterButtonTransforms, letterButton);
    }

    public Vector2 GetPositionLetterButton(Transform letterButton)
    {
        return letterButtonPositions[GetIndexLetterButton(letterButton)];
    }

    public void AddLetterPlace(LetterButton letterButton)
    {
        storedLetters[GetIndexLetterButton(letterButton.transform)] = letterButton.letterContained.text;
        storedIndexLetters.Add(GetIndexLetterButton(letterButton.transform));
        UpdateStoredString();

    }

    public void RemoveLetterPlace(LetterButton letterButton)
    {
        storedLetters[GetIndexLetterButton(letterButton.transform)] = "";
        storedIndexLetters.Remove(GetIndexLetterButton(letterButton.transform));
        UpdateStoredString();
    }

    private void UpdateStoredString()
    {
        string temp_string = "";
        foreach (var item in storedIndexLetters)
        {
            temp_string += storedLetters[item];
        }
        storedString = temp_string;
    }

    public void AcakButton()
    {
        List<LetterButton> temporalLetterPlace = new List<LetterButton>();

        foreach (Transform item in letterActivePlace)
        {
            temporalLetterPlace.Add(item.GetComponent<LetterButton>());
        }

        foreach (var item in temporalLetterPlace)
        {
            if (item.isPlaced) item.SystemReturnLetterNonInstant();
        }

        foreach (var item in letterButtons) item.GetComponent<Button>().interactable = false;

        StartCoroutine(AnimationGenerateLetter());

        gameSceneController.currentBarProgression -= gameSceneController.decrementAcakBarProgression;
        CheckBar();
    }

    private IEnumerator AnimationGenerateLetter()
    {
        for (int i = 0; i < 7; i++)
        {
            foreach (var item in letterButtons) item.GenerateLetter();
            yield return new WaitForSeconds(0.055f);
        }

        foreach (var item in letterButtons) item.GetComponent<Button>().interactable = true;
    }

    public void PasangButton()
    {
        if (CheckWord())
        {
            _sah.AddScoreAnimation(_sah.startPoint.position, 1, CalculateScore());
            //gameSceneController.UpdateScore();
            AdvanceBarProgression();

            foreach (var item in temporalLetterPlace) item.UpdateLetterButtonTypes(PowerUpTypes.NORMAL);

            submittedWords.Add(storedString);
            gameSceneController.AddToDisplayPage(storedString);
            CheckForPowerUpValidSpawn();
            CheckBar();

            foreach (var item in temporalLetterPlace)
            {
                item.SystemReturnLetter();
            }

            foreach (var item in temporalLetterPlace)
            {
                item.GenerateLetter();
            }

            temporalLetterPlace = new List<LetterButton>();
            MainGameSceneController.Instance.ToggleSubmitButtonSprite(false);
        }
    }

    public void CheckForPowerUpValidSpawn()
    {
        PowerUpTypes assginedPower = PowerUpTypes.NORMAL;

        if (storedIndexLetters.Count >= 7) assginedPower = PowerUpTypes.POWER3;
        else if (storedIndexLetters.Count == 6) assginedPower = PowerUpTypes.POWER2;
        else if (storedIndexLetters.Count == 5) assginedPower = PowerUpTypes.POWER1;

        SpawnPowerUp(assginedPower);
    }

    public void SpawnPowerUp(PowerUpTypes type)
    {
        if (type == PowerUpTypes.NORMAL) return;

        int randomPos = UnityEngine.Random.Range(0, letterButtonTransforms.Length);
        StartCoroutine(PlayPowerUpSpawnSFX());
        while (true)
        {
            if (letterButtons[randomPos].powerUpType == PowerUpTypes.NORMAL)
            {
                letterButtons[randomPos].UpdateLetterButtonTypes(type);
                return;
            }
            else
            {
                randomPos = UnityEngine.Random.Range(0, letterButtonTransforms.Length);
            }
        }
    }

    private IEnumerator PlayPowerUpSpawnSFX()
    {
        GameManager.Instance.PlaySfx("7 Letters");
        yield return new WaitForSeconds(0.07f);
        GameManager.Instance.PlaySfx("5 Letters");
        yield return new WaitForSeconds(0.07f);
        GameManager.Instance.PlaySfx("6 Letters");
    }

    public bool CheckWord()
    {
        if (storedIndexLetters.Count >= minimalLetterCount) 
            return Array.Exists(GameManager.Instance.wordsDictionary, jawaban => jawaban == storedString.ToLower());
        return false;
    }

    private void AdvanceBarProgression()
    {
        gameSceneController.currentBarProgression = Mathf.Clamp(gameSceneController.currentBarProgression + CalculateScore(), 0, gameSceneController.maxBarProgression);
    }

    private void CheckBar()
    {
        if (gameSceneController.currentBarProgression < 0)
        {
            progressBar.fillAmount = 0;
            LoseGame();
        }
        else
        {
            progressBar.fillAmount = gameSceneController.currentBarProgression / gameSceneController.maxBarProgression;
        }
    }

    private void LoseGame()
    {
        if (!isLose) GameManager.Instance.RequestPutScoreMethod();
        isLose = true;
        
        gameSceneController.OpenLoseWindow();
    }

    private void UpdateProgression()
    {
        if (gameSceneController.extraProgression > 0)
        {
            gameSceneController.barProgression.color = Color.cyan;
            gameSceneController.extraProgression -= Time.deltaTime * gameSceneController.decrementTimeBarProgression;
        }
        else
        {
            gameSceneController.barProgression.color = new Color32(119, 107, 171, 255); //776BAB hex
            gameSceneController.currentBarProgression -= Time.deltaTime * gameSceneController.decrementTimeBarProgression;
        }
        
        CheckBar();
    }

    public int DetermineWeight(string letter)
    {
        int weight;

        if (scoreOf5.Contains(letter)) weight = 5;
        else if (scoreOf4.Contains(letter)) weight = 4;
        else if (scoreOf3.Contains(letter)) weight = 3;
        else if (scoreOf2.Contains(letter)) weight = 2;
        else if (scoreOf1.Contains(letter)) weight = 1;
        else weight = 0;

        return weight;
    }

    public int CalculateScore()
    {
        float _score = 0;
        float cumulativeScoreMods = 0f;

        foreach (var item in storedString)
        {
            _score += DetermineWeight(item.ToString());
        }

        foreach (var item in temporalLetterPlace)
        {
            switch (item.powerUpType)
            {
                case PowerUpTypes.NORMAL:
                    break;
                case PowerUpTypes.POWER1:
                    cumulativeScoreMods += power1mod;
                    break;
                case PowerUpTypes.POWER2:
                    cumulativeScoreMods += power2mod;
                    gameSceneController.extraProgression += 2;
                    break;
                case PowerUpTypes.POWER3:
                    cumulativeScoreMods += power3mod;
                    gameSceneController.extraProgression += 5;
                    break;
                default:
                    break;
            }
        }
        //print((int)score + " * " + cumulativeScoreMods + " = " + score * cumulativeScoreMods);
        _score += _score * cumulativeScoreMods;
        //print((int)score);
        return (int)_score;
    }
}
