using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static Board Instance { set; get; }
    public Util util;
    public Transform canvas;

    public const int BOARD_SIZE = 4;
    public const int MINIMAL_LETTER_COUNT = 3;

    public const int RESULT_NONE = 0;
    public const int RESULT_NO_MORE_LETTER = -1;
    
    [HideInInspector] public char[][] boardState;
    [HideInInspector] public char[][] boardSelectionState;
    [HideInInspector] public char[][] storedLetters = new char[BOARD_SIZE][];
    [HideInInspector] public int[] storedIndexLetters;
    [HideInInspector] public int result;
    [HideInInspector] public int pieceNumber;
    [HideInInspector] public XYPoint lastSelectedPos;
    
    [HideInInspector] public bool isStarted;
    [HideInInspector] public bool isLocked;
    [HideInInspector] public List<XYPoint> currentBestConfig = new List<XYPoint>();
    [HideInInspector] public int currentBestScore;

    public GameObject letterBoxPrefab;
    public List<Square> letterBoxes;
    public string loadedTextFileName;
    public string[] wordsDictionary;
    public Dictionary<string, string> wordsDictionaryNew = new Dictionary<string, string>();
    private ChildrenArranger childrenArranger;

    [Header("Letter Weight")]
    public string scoreOf5;
    public string scoreOf4;
    public string scoreOf3;
    public string scoreOf2;
    public string scoreOf1;
    public float power1mod, power2mod, power3mod;

    public List<Square> temporalLetterPlace = new List<Square>();
    public Transform letterActivePlace;
    public GameObject emptyGO;
    public string storedString;
    public Transform[] letterButtonTransforms;
    public Vector3[] letterButtonPositions;
    public GameSceneController gameSceneController;
    public bool isPlayerTurn = true;
    private bool isAcakButtonAllowed = true;
    private MCTSAI mCTSAI;
    private bool isLoopAgain;
    private Coroutine currentAICoroutine;
    private ScoreAnimationHandler _sah;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        mCTSAI = GetComponent<MCTSAI>();
        _sah = GetComponent<ScoreAnimationHandler>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //isStarted = false;
        //isLocked = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPlayerTurn && isStarted && gameSceneController.currentBarProgression >= 1)
        {
            if (currentAICoroutine == null)
            {
                currentAICoroutine = StartCoroutine(Try());
            }
        }
    }

    public void InitDict()
    {
        TextAsset file = Resources.Load(loadedTextFileName) as TextAsset;
        string txt = file.ToString();
        char[] separators = new char[] { ' ', ',' };
        wordsDictionary = txt.Split(separators, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < wordsDictionary.Length; i++)
        {
            if (!wordsDictionaryNew.ContainsKey(wordsDictionary[i])) wordsDictionaryNew.Add(wordsDictionary[i], wordsDictionary[i]);
        }
    }

    public void InitBoard()
    {
        lastSelectedPos = null;
        pieceNumber = 0;
        currentBestScore = 0;
        currentBestConfig = new List<XYPoint>();
        result = RESULT_NONE;

        //removes all child
        childrenArranger = GetComponent<ChildrenArranger>();
        letterBoxes.Clear();
        int childCount = transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            DestroyImmediate(child.gameObject);
        }

        //init boardState       
        boardState = new char[BOARD_SIZE][];
        boardSelectionState = new char[BOARD_SIZE][];
        for (int i = 0; i < boardState.Length; i++)
        {
            boardState[i] = new char[BOARD_SIZE];
            boardSelectionState[i] = new char[BOARD_SIZE];
            for (int j = 0; j < boardState[i].Length; j++)
            {
                boardState[i][j] = util.GetRandomAlphabet();
                boardSelectionState[i][j] = Square.NOT_SELECTED;
            }
        }

        //init letterBox Visual
        for (int i = 0; i < boardState.Length; i++)
        {
            for (int j = 0; j < boardState[i].Length; j++)
            {
                Square letterBox = Instantiate(letterBoxPrefab, transform).GetComponent<Square>();
                letterBox.posX = j;
                letterBox.posY = i;
                letterBox.containedLetter = boardState[i][j];
                letterBox.UpdateSquare();
                letterBoxes.Add(letterBox);
            }
        }

        childrenArranger.ArrangeChilds();
        //for (int i = 0; i < boardState.Length; i++)
        //{
        //    for (int j = 0; j < boardState[i].Length; j++)
        //    {
        //        print(boardState[i][j]);
        //        print(boardSelectionState[i][j]);
        //    }
        //}
    }

    public void UpdateBoardState()
    {
        
        
    }

    public void ShowCurrentBoard()
    {
        string printable = "";
        for (int i = 0; i < boardState.Length; i++)
        {
            string row = "";
            for (int j = 0; j < boardState[i].Length; j++)
            {
                row += boardState[i][j].ToString();
            }
            printable += row + "\n";
        }
        print(printable);
    }

    public void ShowDict()
    {
        int count = 0;

        foreach (KeyValuePair<string, string> item in wordsDictionaryNew)
        {
            print(item.Key);
            count++;
            if (count == 100) return;
        }
        
    }

    public void SelectSquare(int posX, int posY)
    {
        //TODO optimize?
        foreach (Square square in GetComponentsInChildren<Square>())
        {
            if (square.posX == posX && square.posY == posY)
            {
                if (square.status == Square.NOT_SELECTED)
                {
                    square.status = Square.SELECTED;

                    UpdateSquare(posX, posY, square.status);
                }
                else
                {
                    //Debug.Log("selected square is not empty");
                }
                break;
            }
        }
    }

    public void UpdateSquare(int x, int y, char status)
    {
        boardState[x][y] = status;
        pieceNumber++;
        lastSelectedPos = new XYPoint(x, y);
    }

    public bool CheckWord()
    {
        if (storedString.Length >= MINIMAL_LETTER_COUNT)
        {
            return Array.Exists(wordsDictionary, jawaban => jawaban == storedString.ToLower());
        }

        return false;
    }

    public Vector2 GetPositionLetterButton(Transform letterButton)
    {
        return letterButtonPositions[GetIndexLetterButton(letterButton)];
    }

    public int GetIndexLetterButton(Transform letterButton)
    {
        return Array.IndexOf(letterButtonTransforms, letterButton);
    }

    public void UpdateStoredString()
    {
        string temp_string = "";
        foreach (Square item in temporalLetterPlace)
        {
            temp_string += item.containedLetter;
        }
        storedString = temp_string;
    }

    public void PasangButton()
    {
        if (CheckWord())
        {
            GameManagerMCTS.Instance.PlaySfx("ButtonHit");
            _sah.AddScoreAnimation(_sah.startPoint.position, 1, CalculateScore());
            //gameSceneController.UpdateScore(CalculateScore());
            //AdvanceBarProgression();

            //foreach (var item in temporalLetterPlace) item.UpdateLetterButtonTypes(PowerUpTypes.NORMAL);

            //submittedWords.Add(storedString);
            //gameSceneController.AddToDisplayPage(storedString);
            //CheckForPowerUpValidSpawn();
            //CheckBar();
            List<Square> temp = new List<Square>();

            foreach (Square item in temporalLetterPlace)
            {
                temp.Add(item);
            }

            foreach (Square item in temp)
            {
                item.SystemReturnLetter();
                item.GenerateLetter();
            }

            //foreach (Square item in temporalLetterPlace)
            //{
            //    item.GenerateLetter();
            //}

            temporalLetterPlace = new List<Square>();
            gameSceneController.ToggleSubmitButtonSprite(false);
        }
    }

    public void AIPasangButton()
    {
        _sah.AddScoreAnimationAI(_sah.startPoint.position, 1, CalculateScore());
        //gameSceneController.UpdateScoreAI(CalculateScore());
        //AdvanceBarProgression();

        //foreach (var item in temporalLetterPlace) item.UpdateLetterButtonTypes(PowerUpTypes.NORMAL);

        //submittedWords.Add(storedString);
        //gameSceneController.AddToDisplayPage(storedString);
        //CheckForPowerUpValidSpawn();
        //CheckBar();
        List<Square> temp = new List<Square>();

        foreach (Square item in temporalLetterPlace)
        {
            temp.Add(item);
        }

        foreach (Square item in temp)
        {
            item.SystemReturnLetter();
            item.GenerateLetter();
        }

        //foreach (Square item in temporalLetterPlace)
        //{
        //    item.GenerateLetter();
        //}

        temporalLetterPlace = new List<Square>();
    }

    public int CalculateScore()
    {
        float _score = storedString.Length;
        float cumulativeScoreMods = 0f;

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
                    break;
                case PowerUpTypes.POWER3:
                    cumulativeScoreMods += power3mod;
                    break;
                default:
                    break;
            }
        }
        _score += _score * cumulativeScoreMods;
        return (int)_score;
    }

    public void AcakButton()
    {
        if (!isAcakButtonAllowed) return;
        isAcakButtonAllowed = false;
        GameManagerMCTS.Instance.PlaySfx("ButtonHit");
        List<Square> temporalLetterPlace = new List<Square>();

        foreach (Transform item in letterActivePlace)
        {
            temporalLetterPlace.Add(item.GetComponent<Square>());
        }

        foreach (var item in temporalLetterPlace)
        {
            if (item.isPlaced) item.SystemReturnLetter();
        }

        foreach (var item in letterBoxes) item.GetComponent<Square>().interactable = false;

        StartCoroutine(AnimationGenerateLetter());

        if (gameSceneController.extraProgression <= 0)
        {
            gameSceneController.currentBarProgression -= gameSceneController.decrementAcakBarProgression;
        }
    }

    private IEnumerator AnimationGenerateLetter()
    {
        for (int i = 0; i < 7; i++)
        {
            foreach (var item in letterBoxes) item.GenerateLetter();
            yield return new WaitForSeconds(0.055f);
        }

        foreach (var item in letterBoxes) item.GetComponent<Square>().interactable = true;
        isAcakButtonAllowed = true;
    }

    public void StartAISequenceMove()
    {
        StartCoroutine(StartAI());
    }

    private IEnumerator StartAI()
    {
        float maxTimeLimit = gameSceneController.currentMaxBarProgression;
        yield return new WaitForSeconds(maxTimeLimit - (maxTimeLimit * UnityEngine.Random.Range(0.8f, 1.0f)));
    }

    IEnumerator Try()
    {
        if (isPlayerTurn)
        {
            currentAICoroutine = null;
            print("P");
            yield break;
        }
        print("P2");
        mCTSAI.StartAI();

        yield return new WaitForSeconds(UnityEngine.Random.Range(1f, 2f));
        if (isPlayerTurn)
        {
            currentAICoroutine = null;
            print("P");
            yield break;
        }
        gameSceneController.OpenIndicatorText("Komputer sedang berpikir...");
        yield return new WaitForSeconds(UnityEngine.Random.Range(1f, 2f));
        foreach (XYPoint item in currentBestConfig)
        {
            foreach (Square square in letterBoxes)
            {
                if (square.posY == item.X && square.posX == item.Y)
                {
                    if (isPlayerTurn) 
                    {
                        currentAICoroutine = null;
                        print("P");
                        yield break;
                    }
                    square.SelectSquare(true);
                    yield return new WaitForSeconds(UnityEngine.Random.Range(0.3f, 0.5f));
                }
            }
        }

        if (isPlayerTurn)
        {
            currentAICoroutine = null;
            print("P");
            yield break;
        }

        if (currentBestConfig.Count == 0) AcakButton();
        else AIPasangButton();
        yield return new WaitForSeconds(UnityEngine.Random.Range(1f, 3f));
        currentAICoroutine = null;
    }
}
