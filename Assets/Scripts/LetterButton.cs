using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public enum PowerUpTypes { NORMAL, POWER1, POWER2, POWER3}

public class LetterButton : MonoBehaviour
{
    private Image _image;
    public TextMeshProUGUI letterContained;
    public GameObject ghostButtonPrefab;
    public bool isPlaced;
    public Vector2 lastPos;
    public PowerUpTypes powerUpType = PowerUpTypes.NORMAL;

    private string allLetter = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    private void Awake()
    {
        letterContained = GetComponentInChildren<TextMeshProUGUI>();
        _image = GetComponent<Image>();
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
        this.DOKill();
        BattleController bc = BattleController.Instance;
        isPlaced = false;
        SetParentToLetterContainer();
        transform.position = lastPos;
        transform.DOScale(1.28f, 0.05f);
        bc.RemoveLetterPlace(this);
    }

    private void SetParentToLetterActivePlace()
    {
        transform.SetParent(BattleController.Instance.letterActivePlace);
    }

    private void SetScaleToNormal()
    {
        transform.DOScale(1f, 0.1f);
    }

    private void SetParentToLetterContainer()
    {
        transform.SetParent(BattleController.Instance.letterContainer);
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
            transform.DOScale(1.28f, duration);

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
            transform.DOScale(0.78f, duration).onComplete = SetScaleToNormal;
            if (bc.CheckWord())
            {
                MainGameSceneController.Instance.OpenIndicatorText();
            }
        }
    }

    public void UpdateLetterButtonTypes(PowerUpTypes type)
    {
        powerUpType = type;
        switch (powerUpType)
        {
            case PowerUpTypes.NORMAL:
                _image.sprite = BattleController.Instance.nonPower;
                break;
            case PowerUpTypes.POWER1:
                _image.sprite = BattleController.Instance.power1;
                break;
            case PowerUpTypes.POWER2:
                _image.sprite = BattleController.Instance.power2;
                break;
            case PowerUpTypes.POWER3:
                _image.sprite = BattleController.Instance.power3;
                break;
            default:
                break;
        }
    }
}
