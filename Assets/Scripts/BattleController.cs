using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class BattleController : MonoBehaviour
{
    public static BattleController Instance { internal set; get; }

    private GameSceneController battleSceneController;

    public Image playerBar, enemyBar;
    public Transform[] letterButtonTransforms;
    public Vector2[] letterButtonPositions;
    public Transform letterPlace;
    public Transform letterBox;

    public string storedString;
    public string[] storedLetters;
    public List<int> storedIndexLetters;
    public List<string> kbbi;
    public string[] texts;

    private LetterButton[] letterButtons = new LetterButton[25];

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
        battleSceneController = GameSceneController.Instance;
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
        return BattleController.Instance.letterButtonPositions[GetIndexLetterButton(letterButton)];
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

        foreach (Transform item in letterPlace)
        {
            temporalLetterPlace.Add(item.GetComponent<LetterButton>());
        }

        foreach (var item in temporalLetterPlace)
        {
            item.UseLetterButton();
        }

        foreach (var item in letterButtons)
        {
            item.GenerateLetter();
        }

        battleSceneController.currentPlayerProgression -= battleSceneController.enemyProgression;
        CheckBar();
    }

    public void PasangButton()
    {
        if (CheckWord())
        {
            if (storedString.ToLower() == battleSceneController.enemyRealName.ToLower())
            {
                battleSceneController.currentEnemyProgression = battleSceneController.enemyBarProgression;
            }
            else
            {
                AdvanceProgressionEnemy();
            }
            
            CheckBar();
            List<LetterButton> temporalLetterPlace = new List<LetterButton>();

            foreach (Transform item in letterPlace)
            {
                temporalLetterPlace.Add(item.GetComponent<LetterButton>());
            }

            foreach (var item in temporalLetterPlace)
            {
                item.UseLetterButton();
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
        return Array.Exists(GameManager.Instance.kbbi, jawaban => jawaban == storedString.ToLower());
    }

    private void AdvanceProgressionEnemy()
    {
        if (isEnemyLetterBased)
        {
            battleSceneController.currentEnemyProgression += storedString.Length;
        }
        else
        {
            battleSceneController.currentEnemyProgression += battleSceneController.playerProgression;
        }
    }

    private void CheckBar()
    {
        if (battleSceneController.currentPlayerProgression == 0)
        {
            playerBar.fillAmount = 0;
            LoseGame();
        }
        else
        {
            playerBar.fillAmount = (float)battleSceneController.currentPlayerProgression / (float)battleSceneController.playerBarProgression;
        }

        if (battleSceneController.currentEnemyProgression == 0)
        {
            enemyBar.fillAmount = 0;
        }
        else
        {
            enemyBar.fillAmount = battleSceneController.currentEnemyProgression / battleSceneController.enemyBarProgression;
            WinGame();
        }
    }

    private void LoseGame()
    {
        battleSceneController.OpenLoseWindow();
    }

    private void WinGame()
    {
        if (battleSceneController.currentEnemyProgression != battleSceneController.enemyBarProgression) return;
    }

    private void DetermineLevelType()
    {
        switch (battleSceneController.levelType)
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
            battleSceneController.currentPlayerProgression -= Time.deltaTime * battleSceneController.enemyTimeProgression;
            CheckBar();
        }
    }
}
