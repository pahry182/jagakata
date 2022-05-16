using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LetterButton : MonoBehaviour
{
    public TextMeshProUGUI letterContained;
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
        letterContained.text = c.ToString();
    }


    public void UseLetterButton()
    {
        BattleController bc = BattleController.Instance;
        if (isPlaced)
        {
            isPlaced = false;
            transform.SetParent(bc.letterBox, false);
            transform.position = lastPos;
            bc.RemoveLetterPlace(this);
        }
        else
        {
            isPlaced = true;
            transform.SetParent(bc.letterPlace, false);
            lastPos = bc.GetPositionLetterButton(this.transform);
            bc.AddLetterPlace(this);
            if (bc.CheckWord())
            {
                BattleSceneController.Instance.OpenIndicatorText();
            }
        }
    }
}
