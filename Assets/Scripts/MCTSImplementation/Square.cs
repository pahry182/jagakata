using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Square : MonoBehaviour
{
    private string allLetter = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

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
    public bool interactable;
    public Vector3 lastPos;
    public GameObject dummyGO;
    public PowerUpTypes powerUpType = PowerUpTypes.NORMAL;

    // Use this for initialization
    void Start()
    {
        status = NOT_SELECTED;
        interactable = true;
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

    public void OnMouseDown()
    {
        if (!interactable) return;

        Board board = Board.Instance;
        float duration = 0.2f;
        if (isPlaced)
        {
            isPlaced = false;
            board.temporalLetterPlace.Remove(this);
            board.UpdateStoredString();
            
            transform.SetParent(board.transform);
            transform.DOScale(0.72f, duration).OnComplete(() => transform.DOScale(1f, 0.1f));
            transform.DOMove(lastPos, duration).onComplete = SetParentToLetterContainer;
            CheckWordIndicator();
        }
        else
        {
            isPlaced = true;
            lastPos = transform.position;
            board.temporalLetterPlace.Add(this);
            board.UpdateStoredString();
            dummyGO = Instantiate(board.emptyGO, board.letterActivePlace);
            transform.DOMove(dummyGO.transform.position, duration).OnComplete(OnComplete);
            transform.DOScale(0.72f, duration).OnComplete(() => transform.DOScale(1f, 0.1f));
            CheckWordIndicator();
        }
    }

    private void OnComplete()
    {
        Destroy(dummyGO);
        transform.SetParent(Board.Instance.letterActivePlace);
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

    private void SetParentToLetterContainer()
    {
        transform.SetParent(BattleController.Instance.letterContainer);
    }

    private void CheckWordIndicator(bool IsOpeningIndicatorText = true)
    {
        if (Board.Instance.CheckWord())
        {
            if (IsOpeningIndicatorText) GameSceneController.Instance.OpenIndicatorText();

            GameSceneController.Instance.ToggleSubmitButtonSprite(true);
        }
        else GameSceneController.Instance.ToggleSubmitButtonSprite(false);
    }

    public void SystemReturnLetter()
    {
        this.DOKill();
        float duration = 0.2f;
        Board board = Board.Instance;
        isPlaced = false;
        transform.SetParent(board.transform);
        transform.DOScale(0.72f, duration).OnComplete(() => transform.DOScale(1f, 0.1f));
        transform.DOMove(lastPos, duration).onComplete = SetParentToLetterContainer;
        board.temporalLetterPlace.Remove(this);
        board.UpdateStoredString();
    }

    public void GenerateLetter()
    {
        char c = allLetter[Random.Range(0, allLetter.Length)];
        Board.Instance.boardState[posY][posX] = c;
        containedLetter = c;
        UpdateSquare();
    }

    public void SystemReturnLetterNonInstant()
    {
        Board bc = Board.Instance;
        float duration = 0.2f;
        isPlaced = false;
        transform.SetParent(board.transform);
        transform.DOScale(0.72f, duration).OnComplete(() => transform.DOScale(1f, 0.1f));
        transform.DOMove(lastPos, duration).onComplete = SetParentToLetterContainer;
        board.temporalLetterPlace.Remove(this);
        board.UpdateStoredString();
        GameSceneController.Instance.ToggleSubmitButtonSprite(false);
    }
}
