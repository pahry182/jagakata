using UnityEngine;
using System.Diagnostics;
using System.Collections;

public class MCTSAI : MonoBehaviour
{
    //public static char myTurn = Board.TURN_X;
    public Board board;
    public int iterationNumber;
    [HideInInspector] public TreeNode treeNode;
    [HideInInspector] public double[][] uctValues;

    // Use this for initialization
    void Start()
    {
        InitAI();
    }

    // Update is called once per frame
    void Update()
    {
        if (board.isStarted)
        {
            //if ((board.currentTurn == myTurn) && (board.result == Board.RESULT_NONE))
            {
                bool flag = false;
                if (treeNode.children.Count > 0)
                {
                    foreach (TreeNode child in treeNode.children)
                    {
                        if ((child.state.lastPos.IsEqual(board.lastPos))
                            && (child.state.lastOPos.IsEqual(board.lastOPos)))
                        {
                            treeNode = child; //use the child as current tree
                            flag = true;
                            break;
                        }
                    }
                    if (!flag) UnityEngine.Debug.Log("unreachable code");

                }
                else
                {
                    treeNode = new TreeNode(new State(board.boardState, board.lastPos, board.lastOPos, board.pieceNumber)); //create a new TreeNode
                }

                var watch = Stopwatch.StartNew();
                for (int i = 0; i < iterationNumber; i++)
                {
                    treeNode.IterateMCTS();
                }
                watch.Stop();
                //var elapsedMs = watch.ElapsedMilliseconds;
                //UnityEngine.Debug.Log("time elapsed for iterateMCTS() = " + elapsedMs + " ms");

                treeNode = treeNode.select();

                //shows uctValue for each possible next move taken by the opponent
                updateUCTValues();

                //if (myTurn == Board.TURN_X)
                //{
                //    board.selectSquare(treeNode.state.lastPos.x, treeNode.state.lastPos.y);
                //}
                //else //myTurn == Board.TURN_O
                //{
                //    board.selectSquare(treeNode.state.lastOPos.x, treeNode.state.lastOPos.y);
                //}
            }
        }
    }

    public void InitAI()
    {
        treeNode = new TreeNode(new State(board.boardState, board.lastPos, board.lastOPos, board.pieceNumber)); //create a new TreeNode

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
    void updateUCTValues()
    {
        foreach (TreeNode child in treeNode.children)
        {
            //int lastPosX = myTurn == Board.TURN_X ? child.state.lastOPos.x : child.state.lastPos.x;
            //int lastPosY = myTurn == Board.TURN_X ? child.state.lastOPos.y : child.state.lastPos.y;
            //uctValues[lastPosX][lastPosY] = child.uctValue;
        }
    }
}
