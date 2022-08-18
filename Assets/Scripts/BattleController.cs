   using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class BattleController : MonoBehaviour
{
    public static BattleController Instance { internal set; get; }

    private GameSceneController gameSceneController;

    public Image playerBar, enemyBar;
    public Transform[] letterButtonTransforms;
    public Vector2[] letterButtonPositions;
    public Transform letterActivePlace;
    public Transform letterContainer;
    public Transform canvas;

    public string storedString;
    public string[] storedLetters;
    public List<int> storedIndexLetters;
    public List<string> kbbi;
    public string[] texts;
    public int minimalLetterCount;

    private LetterButton[] letterButtons = new LetterButton[16];

    private bool isPlayerTimeBased;
    private bool isEnemyLetterBased;

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
        gameSceneController = GameSceneController.Instance;
        DetermineLevelType();
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
            if (item.isPlaced) item.UseLetterButton();
        }

        foreach (var item in letterButtons)
        {
            item.GenerateLetter();
        }

        gameSceneController.currentPlayerProgression -= gameSceneController.enemyProgression;
        CheckBar();
    }

    public void PasangButton()
    {
        if (CheckWord())
        {
            if (storedString.ToLower() == gameSceneController.enemyRealName.ToLower())
            {
                gameSceneController.currentEnemyProgression = gameSceneController.enemyBarProgression;
            }
            else
            {
                AdvanceProgressionEnemy();
            }
            
            CheckBar();
            List<LetterButton> temporalLetterPlace = new List<LetterButton>();

            foreach (Transform item in letterActivePlace)
            {
                temporalLetterPlace.Add(item.GetComponent<LetterButton>());
            }

            foreach (var item in temporalLetterPlace)
            {
                item.SystemReturnLetter();
            }

            foreach (var item in temporalLetterPlace)
            {
                item.GenerateLetter();
            }
        }
        else
        {

        }
    }

    public bool CheckWord()
    {
        if (storedIndexLetters.Count >= minimalLetterCount) 
            return Array.Exists(GameManager.Instance.wordsDictionary, jawaban => jawaban == storedString.ToLower());
        return false;
    }

    private void AdvanceProgressionEnemy()
    {
        if (isEnemyLetterBased)
        {
            gameSceneController.currentEnemyProgression += storedString.Length;
        }
        else
        {
            gameSceneController.currentEnemyProgression += gameSceneController.playerProgression;
        }
    }

    private void CheckBar()
    {
        if (gameSceneController.currentPlayerProgression == 0)
        {
            playerBar.fillAmount = 0;
            LoseGame();
        }
        else
        {
            playerBar.fillAmount = (float)gameSceneController.currentPlayerProgression / (float)gameSceneController.playerBarProgression;
        }

        if (gameSceneController.currentEnemyProgression == 0)
        {
            enemyBar.fillAmount = 0;
        }
        else
        {
            enemyBar.fillAmount = gameSceneController.currentEnemyProgression / gameSceneController.enemyBarProgression;
            WinGame();
        }
    }

    private void LoseGame()
    {
        gameSceneController.OpenLoseWindow();
    }

    private void WinGame()
    {
        if (gameSceneController.currentEnemyProgression != gameSceneController.enemyBarProgression) return;
    }

    private void DetermineLevelType()
    {
        switch (gameSceneController.levelType)
        {
            case LevelType.TIME_LETTER:
                isPlayerTimeBased = true;
                isEnemyLetterBased = true;
                break;
            default:
                break;
        }
    }

    private void UpdateProgression()
    {
        if (isPlayerTimeBased)
        {
            gameSceneController.currentPlayerProgression -= Time.deltaTime * gameSceneController.enemyTimeProgression;
            CheckBar();
        }
    }
}
