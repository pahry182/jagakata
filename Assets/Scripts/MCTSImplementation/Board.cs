using System;
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
    [HideInInspector] public char[] storedLetters;
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

    [HideInInspector] public List<Square> temporalLetterPlace = new List<Square>();
    public Transform letterActivePlace;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
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
                letterBox.posX = i;
                letterBox.posY = j;
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

        //switch ((status, x, y))
        //{
        //    case RESULT_X:
        //        {
        //            result = RESULT_X;
        //            //Debug.Log("X wins");
        //            StartCoroutine(showGameResult());
        //            break;
        //        }
        //    case RESULT_O:
        //        {
        //            result = RESULT_O;
        //            //Debug.Log("O wins");
        //            StartCoroutine(showGameResult());
        //            break;
        //        }
        //    case RESULT_DRAW:
        //        {
        //            result = RESULT_DRAW;
        //            //Debug.Log("Draw");
        //            StartCoroutine(showGameResult());
        //            break;
        //        }
        //}
    }

    //public void RemoveLetterPlace(Square letterButton)
    //{
    //    storedLetters[GetIndexLetterButton(letterButton.transform)] = "";
    //    storedIndexLetters.Remove(GetIndexLetterButton(letterButton.transform));
    //    UpdateStoredString();
    //}

    //public bool CheckWord()
    //{
    //    if (storedIndexLetters.Length >= MINIMAL_LETTER_COUNT)
    //    {
    //        return Array.Exists(GameManager.Instance.wordsDictionary, jawaban => jawaban == storedString.ToLower());
    //    }

    //    return false;
    //}

    //public Vector2 GetPositionLetterButton(Transform letterButton)
    //{
    //    return letterButtonPositions[GetIndexLetterButton(letterButton)];
    //}

    //public void AddLetterPlace(Square letterButton)
    //{
    //    storedLetters[GetIndexLetterButton(letterButton.transform)] = letterButton.letterContained.text;
    //    storedIndexLetters.Add(GetIndexLetterButton(letterButton.transform));
    //    UpdateStoredString();
    //}

    //public int GetIndexLetterButton(Transform letterButton)
    //{
    //    return Array.IndexOf(letterButtonTransforms, letterButton);
    //}
}
