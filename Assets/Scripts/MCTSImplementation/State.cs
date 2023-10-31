using UnityEngine;
using System.Collections.Generic;

public class State
{
    public char[][] boardState;
    public char[][] boardSelectionState;
    public int stateResult;
    public int pieceNumber;

    //MCTS JAGAKATA
    public List<XYPoint> storedIndexLetters = new List<XYPoint>();
    public string storedString;
    public XYPoint lastSelectedPos;

    public State(char[][] prevBoardState, char[][] prevBoardSelectionState, XYPoint lastSelectedPos, int pieceNumber)
    {
        //this.currentTurn = currentTurn;
        this.lastSelectedPos = lastSelectedPos;
        this.pieceNumber = pieceNumber;
        stateResult = Board.RESULT_NONE;

        if (prevBoardState != null) //no previous moves
        {
            boardState = Util.Instance.DeepcloneArray(prevBoardState);
            boardSelectionState = Util.Instance.DeepcloneArray(prevBoardSelectionState);
        }
        else
        {
            Debug.Log("create new empty boardState");
            boardState = new char[Board.BOARD_SIZE][];
            for (int i = 0; i < boardState.Length; i++)
            {
                boardState[i] = new char[Board.BOARD_SIZE];
                for (int j = 0; j < boardState[i].Length; j++)
                {
                    boardState[i][j] = Util.Instance.GetRandomAlphabet();
                    boardSelectionState[i][j] = Square.NOT_SELECTED;
                }
            }
        }
    }

    public void SelectPoint(XYPoint point)
    {
        storedIndexLetters.Add(point);
        lastSelectedPos = point;
        pieceNumber++;
        boardSelectionState[point.X][point.Y] = Square.SELECTED;
        string currentStoredString = "";
        foreach (var item in storedIndexLetters)
        {
            currentStoredString += boardState[item.X][item.Y];
        }
        storedString = currentStoredString;
    }
}
