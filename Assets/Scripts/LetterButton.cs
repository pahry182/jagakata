using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class LetterButton : MonoBehaviour
{
    public TextMeshProUGUI letterContained;
    public GameObject ghostButtonPrefab;
    public bool isPlaced;
    public Vector2 lastPos;

    private string allLetter = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    private void Awake()
    {
        letterContained = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        GenerateLetter();
    }

    public void GenerateLetter()
    {
        char c = allLetter[Random.Range(0, allLetter.Length)];
        // Used if letter generated of 'Q' to automatically becomes 'QU' (Still not fully implemented)
        if (c.Equals('Q'))
        {
            letterContained.text = "QU";
        }
        else
        {
            letterContained.text = c.ToString();
        }
        //letterContained.text = c.ToString();

    }

    public void SystemReturnLetter()
    {
        DOTween.KillAll();
        BattleController bc = BattleController.Instance;
        isPlaced = false;
        transform.SetParent(bc.letterContainer, false);
        transform.position = lastPos;
        bc.RemoveLetterPlace(this);
    }

    public void UseLetterButton()
    {
        BattleController bc = BattleController.Instance;
        float duration = 0.3f;
        if (isPlaced)
        {
            isPlaced = false;
            //transform.SetParent(bc.letterContainer, false);
            //transform.position = lastPos;
            transform.SetParent(bc.canvas);
            transform.DOMove(lastPos, duration).onComplete = SetParentToLetterContainer;
            bc.RemoveLetterPlace(this);
            bc.temporalLetterPlace.Remove(this);
        }
        else
        {
            isPlaced = true;
            lastPos = bc.GetPositionLetterButton(transform);
            bc.AddLetterPlace(this);
            bc.temporalLetterPlace.Add(this);

            transform.SetParent(bc.canvas);
            //GameObject ghostButton = Instantiate(ghostButtonPrefab, bc.letterActivePlace);
            //Destroy(ghostButton, duration);
            transform.DOMove(bc.letterActivePlace.transform.position, duration).onComplete = SetParentToLetterActivePlace;

            if (bc.CheckWord())
            {
                GameSceneController.Instance.OpenIndicatorText();
            }
        }

        void SetParentToLetterActivePlace()
        {
            transform.SetParent(bc.letterActivePlace);
        }

        void SetParentToLetterContainer()
        {
            transform.SetParent(bc.letterContainer);
        }
    }
}
