using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System;

public class LoginSceneController : UIController
{
    public TMP_InputField nicknameInputField;


    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.PlayBgm("Menu");
        //StartCoroutine(RequestGetScore());
        //StartCoroutine(RequestPostAccount("Tes2"));
        //StartCoroutine(RequestPutScore("YuSSDj8D5T", 100));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        GameManager.Instance.RequestPostAccountMethod(nicknameInputField.text);
        LoadScene("MainGameScene");
    }
}
