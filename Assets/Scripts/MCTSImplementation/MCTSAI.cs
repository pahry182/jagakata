using UnityEngine;
using System.Diagnostics;
using System.Collections;
using Debug = UnityEngine.Debug;
using System.Collections.Generic;

public class MCTSAI : MonoBehaviour
{
    //public static char myTurn = Board.TURN_X;
    public Board board;
    public int iterationNumber;
    [HideInInspector] public TreeNode treeNode;
    [HideInInspector] public double[][] uctValues;
    public string timeElapsed;
    public string bestWord;
    

    // Use this for initialization
    void Start()
    {
        board.InitDict();
        board.InitBoard();
        InitAI();
        //StartAI();
    }

    public void StartTheGame()
    {
        board.isStarted = true;
        GameManagerMCTS.Instance.PlayBgm("Gameplay");
        GameManagerMCTS.Instance.PlaySfx("ButtonHit");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartAI()
    {
        if (board.isStarted)
        {
            board.lastSelectedPos = null;
            board.pieceNumber = 0;
            board.currentBestScore = 0;
            board.currentBestConfig = new List<XYPoint>();
            board.result = Board.RESULT_NONE;
            int randomIteration = Random.Range(1500, 2000);
            iterationNumber = randomIteration;
            StartCoroutine(CalculateAIMove());
            print("Score: " + board.currentBestScore + " " + bestWord + " " + timeElapsed + " ms.");
            //LogMatrix(board.boardState);
            //LogMatrix(board.boardSelectionState);
        }
    }

    private void LogMatrix(char[][] thisArray)
    {
        string matrixString = "";

        for (int i = 0; i < thisArray.Length; i++)
        {
            for (int j = 0; j < thisArray[i].Length; j++)
            {
                // Concatenate each element to the string
                matrixString += thisArray[i][j] + " ";
            }

            // Add a new line after each row
            matrixString += "\n";
        }

        // Log the entire matrix string
        Debug.Log(matrixString);
    }

    private IEnumerator CalculateAIMove()
    {
        treeNode = new TreeNode(new State(board.boardState, board.boardSelectionState, board.lastSelectedPos, board.pieceNumber)); //create a new TreeNode
        
        var watch = Stopwatch.StartNew();
        for (int i = 0; i < iterationNumber; i++)
        {
            treeNode.IterateMCTS();
        }

        watch.Stop();
        timeElapsed = watch.ElapsedMilliseconds.ToString();
        //Debug.Log("time elapsed for iterateMCTS() = " + elapsedMs + " ms");

        treeNode = treeNode.Select();
        //print(treeNode.state.stateResult);
        //print(treeNode.state.lastSelectedPos.X + " " + treeNode.state.lastSelectedPos.Y + " " +
        //    board.boardState[treeNode.state.lastSelectedPos.X][treeNode.state.lastSelectedPos.Y]
        //    );

        string word = "";
        foreach (XYPoint item in board.currentBestConfig) word += board.boardState[item.X][item.Y];
        //print($"Best: {board.currentBestScore} {word}");
        bestWord = word;

        yield return new WaitForSeconds(0f);

        //shows uctValue for each possible next move taken by the opponent
        //UpdateUCTValues();

        //board.SelectSquare(treeNode.state.lastSelectedPos.X, treeNode.state.lastSelectedPos.Y);

        //if (myTurn == Board.TURN_X)
        //{
        //    board.selectSquare(treeNode.state.lastPos.x, treeNode.state.lastPos.y);
        //}
        //else //myTurn == Board.TURN_O
        //{
        //    board.selectSquare(treeNode.state.lastOPos.x, treeNode.state.lastOPos.y);
        //}
    }

    public void InitAI()
    {
        treeNode = new TreeNode(new State(board.boardState, board.boardSelectionState, board.lastSelectedPos, board.pieceNumber)); //create a new TreeNode

        uctValues = new double[Board.BOARD_SIZE][];
        for (int i = 0; i < uctValues.Length; i++)
        {
            uctValues[i] = new double[Board.BOARD_SIZE];
            for (int j = 0; j < uctValues[i].Length; j++)
            {
                uctValues[i][j] = double.MinValue;
            }
        }
    }

    //Shows UCTValue for each child. A child is one of the possible moves for the opponent.
    void UpdateUCTValues()
    {
        foreach (TreeNode child in treeNode.children)
        {
            //int lastPosX = myTurn == Board.TURN_X ? child.state.lastOPos.x : child.state.lastPos.x;
            //int lastPosY = myTurn == Board.TURN_X ? child.state.lastOPos.y : child.state.lastPos.y;
            //uctValues[lastPosX][lastPosY] = child.uctValue;
            Debug.Log(child.uctValue);
        }
    }
}
