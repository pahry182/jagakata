using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Square : MonoBehaviour
{
    public TextMeshPro text;
    public char containedLetter;

    [Header("TicTacToe Variables")]
    public int posX;
    public int posY;
    public char status;

    public Board board;
    public Sprite squareemptySprite, squarexSprite, squareoSprite;

    public MCTSAI mctsai;
    public Text uctValue;

    public const char SQUARE_EMPTY = '0';
    public const char SQUARE_X = '1';
    public const char SQUARE_O = '2';

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //update square sprite
        //switch (status)
        //{
        //    case SQUARE_EMPTY:
        //        {
        //            transform.GetComponent<Image>().sprite = squareemptySprite;
        //            break;
        //        }
        //    case SQUARE_X:
        //        {
        //            transform.GetComponent<Image>().sprite = squarexSprite;
        //            break;
        //        }
        //    case SQUARE_O:
        //        {
        //            transform.GetComponent<Image>().sprite = squareoSprite;
        //            break;
        //        }
        //}

        ////update and set UCTValue visibility
        //if (status == SQUARE_EMPTY)
        //{
        //    if (mctsai.uctValues[posX][posY] == double.MinValue)
        //    {
        //        uctValue.text = "?"; //So the double.MinValue will not be shown
        //    }
        //    else
        //    {
        //        uctValue.text = string.Format("{0:0.00}", mctsai.uctValues[posX][posY]);
        //    }
        //}
        //else
        //{
        //    uctValue.gameObject.SetActive(false);
        //}

    }

    public void SelectSquare()
    {
        if ((board.isStarted) && (!board.isLocked))
        {
            board.SelectSquare(posX, posY);
        }
    }

    public void UpdateSquare()
    {
        text.text = containedLetter.ToString();
    }
}
