using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class Square : MonoBehaviour
{
    public TextMeshPro text;
    public char containedLetter;

    [Header("TicTacToe Variables")]
    public int posX;
    public int posY;
    public char status;

    public Board board;

    public MCTSAI mctsai;
    public Text uctValue;

    public const char SELECTED = '1';
    public const char NOT_SELECTED = '0';

    public bool isPlaced;
    public Vector3 lastPos;

    // Use this for initialization
    void Start()
    {
        status = NOT_SELECTED;
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

    //private void SetParentToLetterContainer()
    //{
    //    transform.SetParent(BattleController.Instance.letterContainer);
    //}

    //public void UseLetterButton()
    //{
    //    Board board = Board.Instance;
    //    float duration = 0.3f;
    //    if (isPlaced)
    //    {
    //        isPlaced = false;
    //        transform.SetParent(board.canvas);
    //        transform.DOScale(1.28f, duration);

    //        transform.DOMove(lastPos, duration).onComplete = SetParentToLetterContainer;
    //        board.RemoveLetterPlace(this);
    //        board.temporalLetterPlace.Remove(this);

    //        CheckWordIndicator();
    //    }
    //    else
    //    {
    //        isPlaced = true;
    //        lastPos = board.GetPositionLetterButton(transform);
    //        board.AddLetterPlace(this);
    //        board.temporalLetterPlace.Add(this);

    //        transform.SetParent(board.canvas);
    //        transform.DOMove(board.letterActivePlace.position, duration).OnComplete(() => transform.SetParent(Board.Instance.letterActivePlace));
    //        transform.DOScale(0.78f, duration).OnComplete(() => transform.DOScale(1f, 0.1f));
    //        CheckWordIndicator();
    //    }
    //}

    //private void CheckWordIndicator(bool IsOpeningIndicatorText = true)
    //{
    //    if (Board.Instance.CheckWord())
    //    {
    //        if (IsOpeningIndicatorText) MainGameSceneController.Instance.OpenIndicatorText();

    //        MainGameSceneController.Instance.ToggleSubmitButtonSprite(true);
    //    }
    //    else MainGameSceneController.Instance.ToggleSubmitButtonSprite(false);
    //}
}
