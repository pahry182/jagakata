using UnityEngine;
using System;
using System.Collections.Generic;

/*
 * This code was modified from Simon Lucas' implementation of MCTS
 * http://mcts.ai/code/java.html
 */

public class SimulationValue
{
    public double value;
    public List<XYPoint> storedIndexLetters = new List<XYPoint>();
}

public class TreeNode
{
    static System.Random r = new System.Random();
    static double epsilon = 1e-6;

    public List<TreeNode> children;
    double nVisits, totValue;
    public double uctValue;
    public List<XYPoint> storedBestIndexLetters = new List<XYPoint>();

    public State state;

    public TreeNode(State state)
    {
        children = new List<TreeNode>();
        nVisits = 0;
        totValue = 0;

        this.state = state;
    }

    public void IterateMCTS()
    {
        LinkedList<TreeNode> visited = new LinkedList<TreeNode>();
        TreeNode currentNode = this;
        visited.AddLast(this);
        while (!currentNode.IsLeaf()) //1. SELECTION
        {
            currentNode = currentNode.Select();
            visited.AddLast(currentNode);
        }
        if (currentNode.state.stateResult == Board.RESULT_NONE)
        {
            currentNode.Expand(); //2. EXPANSION
            TreeNode newNode = currentNode.Select();
            visited.AddLast(newNode);
            SimulationValue simulationValue = newNode.Simulate(); //3. SIMULATION
            foreach (TreeNode node in visited)
            {
                node.Backpropagate(simulationValue); //4. BACKPROPAGATION
            }
        }
    }

    public void Expand()
    {
        List<XYPoint> childrenMoves = ListPossibleMoves(state.boardSelectionState);
        //Apply one move for each expansion child
        foreach (XYPoint move in childrenMoves)
        {
            TreeNode childNode = new TreeNode(new State(state.boardState, state.boardSelectionState, state.lastSelectedPos, state.pieceNumber));
            childNode.state.SelectPoint(move);
            childNode.state.stateResult = CheckWordScore(childNode.state, false);
            children.Add(childNode);
        }
    }

    public TreeNode Select()
    {
        TreeNode selected = null;
        double bestValue = double.MinValue;
        foreach (TreeNode childTreeNode in children)
        {
            //UCT value calculation
            double uctValue = 
                childTreeNode.totValue / (childTreeNode.nVisits + epsilon) + 
                Math.Sqrt(Math.Log(nVisits + 1) / (childTreeNode.nVisits + epsilon)) + 
                r.NextDouble() * epsilon; // small random number to break ties randomly in unexpanded nodes
            childTreeNode.uctValue = uctValue;
            if (uctValue > bestValue)
            {
                selected = childTreeNode;
                bestValue = uctValue;
            }
        }
        return selected;
    }

    public bool IsLeaf()
    {
        return children.Count == 0;
    }

    public SimulationValue Simulate()
    {
        State simulationState = new State(Util.Instance.DeepcloneArray(state.boardState), Util.Instance.DeepcloneArray(state.boardSelectionState), state.lastSelectedPos, state.pieceNumber)
        {
            stateResult = state.stateResult
        };

        //simulate semi-randomly(for both players) until a terminal result is achieved
        while (simulationState.stateResult == Board.RESULT_NONE)
        {
            XYPoint chosenMove = DoRandomMove();

            if (simulationState.boardSelectionState[chosenMove.X][chosenMove.Y] == Square.NOT_SELECTED)
            {
                simulationState.SelectPoint(chosenMove);
                simulationState.stateResult = CheckWordScore(simulationState); //check terminal condition
            }
        }

        SimulationValue simulationValue = new SimulationValue()
        {
            value = simulationState.stateResult,
            storedIndexLetters = simulationState.storedIndexLetters
        };
        return simulationValue;
    }

    public void Backpropagate(SimulationValue simulationValue)
    {
        nVisits++;
        totValue += simulationValue.value;
    }

    public List<XYPoint> ListPossibleMoves(char[][] boardSelectionState)
    {
        List<XYPoint> possibleMoves = new List<XYPoint>();
        //list all 9 possible moves
        for (int i = 0; i < Board.BOARD_SIZE; i++)
        {
            for (int j = 0; j < Board.BOARD_SIZE; j++)
            {
                //check for legal board coordinate
                if (boardSelectionState[i][j] == Square.NOT_SELECTED)
                {
                    possibleMoves.Add(new XYPoint(i, j));
                }
            }
        }
        return possibleMoves;
    }

    public XYPoint DoRandomMove()
    {
        return new XYPoint(r.Next(Board.BOARD_SIZE), r.Next(Board.BOARD_SIZE));
    }


    public int CheckWordScore(State state, bool isFromSimulate = true)
    {
        int result = Board.RESULT_NONE;
        if (state.pieceNumber == Board.BOARD_SIZE * Board.BOARD_SIZE)
        {
            result = Board.RESULT_NO_MORE_LETTER;
        }
        else if (state.storedString.Length >= Board.MINIMAL_LETTER_COUNT)
        {
            //Debug.Log("Not Match " + state.storedString);
            try
            {
                string key = state.storedString.ToLower();
                string value = Board.Instance.wordsDictionaryNew[key];
                result = CalculateScore(state);
                //if (isFromSimulate) Debug.Log($"Match {state.storedString} : {result}");
                if (Board.Instance.currentBestScore < result)
                {
                    Board.Instance.currentBestScore = result;
                    Board.Instance.currentBestConfig = Util.DeepCopyList(state.storedIndexLetters);
                }
            }
            catch (KeyNotFoundException e)
            {
                //Debug.LogWarning($"Key {state.storedString} not found: " + e.Message);
            }
        }
        return result;
    }

    public int DetermineWeight(string letter)
    {
        int weight;

        if (Board.Instance.scoreOf5.Contains(letter)) weight = 5;
        else if (Board.Instance.scoreOf4.Contains(letter)) weight = 4;
        else if (Board.Instance.scoreOf3.Contains(letter)) weight = 3;
        else if (Board.Instance.scoreOf2.Contains(letter)) weight = 2;
        else if (Board.Instance.scoreOf1.Contains(letter)) weight = 1;
        else weight = 0;

        return weight;
    }

    public int CalculateScore(State state)
    {
        float _score = 0;

        foreach (var item in state.storedString)
        {
            _score += DetermineWeight(item.ToString());
        }
        return (int)_score;
    }

    //debugging purpose
    public void PrintState(char[][] boardState)
    {
        string s = null;
        for (int y = boardState.Length - 1; y > -1; y--)
        {
            for (int x = 0; x < boardState.Length; x++)
            {
                s += boardState[x][y];
            }
            s += "\n";
        }
        Debug.Log(s);
    }


}