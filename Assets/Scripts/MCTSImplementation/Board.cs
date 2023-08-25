using System;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public const int BOARD_SIZE = 4;
    public const int INROW = 4;
    public const int MINIMAL_LETTER_COUNT = 3;
    //public const char TURN_X = '1';
    //public const char TURN_O = '2';

    public const int RESULT_NONE = -1;
    public const int RESULT_DRAW = 0;
    public const int RESULT_X = 1;
    public const int RESULT_O = 2;

    //public float startX, startY;
    public GameObject letterBoxPrefab;
    public List<Square> letterBoxes;
    public string loadedTextFileName;
    public string[] wordsDictionary;
    [HideInInspector] public char[][] boardState;
    [HideInInspector] public int pieceNumber;
    [HideInInspector] public char currentTurn;
    [HideInInspector] public Point lastPos, lastOPos;
    [HideInInspector] public int result;
    [HideInInspector] public Point[] winningPoints;

    [HideInInspector] public bool isStarted;
    [HideInInspector] public bool isLocked;

    private ChildrenArranger childrenArranger;

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        isStarted = false;
        isLocked = false;
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
    }

    public void InitBoard()
    {
        InitDict();
        //currentTurn = TURN_X;
        pieceNumber = 0;
        lastPos = null;
        lastOPos = null;
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
        Util util = GetComponent<Util>();
        boardState = new char[BOARD_SIZE][];
        for (int i = 0; i < boardState.Length; i++)
        {
            boardState[i] = new char[BOARD_SIZE];
            for (int j = 0; j < boardState[i].Length; j++)
            {
                char randomChar = util.GetRandomAlphabet();
                boardState[i][j] = randomChar;
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

    public void SelectSquare(int posX, int posY)
    {
        //TODO optimize?
        foreach (Square square in GetComponentsInChildren<Square>())
        {
            if (square.posX == posX && square.posY == posY)
            {
                if (square.status == Square.SQUARE_EMPTY)
                {
                    square.status = currentTurn;
                    //updateSquare(posX, posY, square.status);
                    //switchTurn();
                }
                else
                {
                    //Debug.Log("selected square is not empty");
                }
                break;
            }
        }
    }

    //public void updateSquare(int x, int y, char currTurn)
    //{
    //    boardState[x][y] = currTurn;
    //    pieceNumber++;
    //    if (currTurn == Board.TURN_X)
    //    {
    //        lastPos = new Point(x, y);
    //    }
    //    else //currTurn == Board.TURN_O
    //    {
    //        lastOPos = new Point(x, y);
    //    }

    //    switch (checkWin(currTurn, x, y))
    //    {
    //        case RESULT_X:
    //            {
    //                result = RESULT_X;
    //                //Debug.Log("X wins");
    //                StartCoroutine(showGameResult());
    //                break;
    //            }
    //        case RESULT_O:
    //            {
    //                result = RESULT_O;
    //                //Debug.Log("O wins");
    //                StartCoroutine(showGameResult());
    //                break;
    //            }
    //        case RESULT_DRAW:
    //            {
    //                result = RESULT_DRAW;
    //                //Debug.Log("Draw");
    //                StartCoroutine(showGameResult());
    //                break;
    //            }
    //    }
    //}
}
