using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class LoginSceneController : UIController
{
    private const string URL = "https://parseapi.back4app.com/classes/Userscore";
    private const string APP_ID = "8nkUt6v8S9HyE4RkXICQWF81UUCqgdXS2gUS7waX";
    private const string KEY_ID = "8UUVRZnpPVENrDNGLiyYgBjShEaJ40woLfYJqhUq";


    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(RequestGetScore());
        //StartCoroutine(RequestPostAccount("Tes2"));
        StartCoroutine(RequestPutScore("YuSSDj8D5T", 100));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        LoadScene("MainGameScene");
    }

    private void SetHeaderB4App(UnityWebRequest request)
    {
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("X-Parse-Application-Id", APP_ID);
        request.SetRequestHeader("X-Parse-REST-API-Key", KEY_ID);
    }

    public IEnumerator RequestGetScore()
    {
        UnityWebRequest request = UnityWebRequest.Get(URL);
        SetHeaderB4App(request);
        //byte[] bodyRaw = Encoding.UTF8.GetBytes(query);
        //request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        //request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();


        yield return request.SendWebRequest();

        if (request.error != null)
        {
            print("Error:" + request.error);
        }
        else
        {
            print(request.downloadHandler.text);

            //UserDetail userDetail = JsonUtility.FromJson<UserDetail>(request.downloadHandler.text);

            //if (string.IsNullOrEmpty(userDetail.username))
            //{
            //    WriteWarning("Username not found!");
            //}
            //else if (inputPassword.text != userDetail.password)
            //{
            //    WriteWarning("Password is incorrect!");
            //}
            //else
            //{
            //    //Save Credentials to DDOL
            //    WriteWarning("Login Success! Hello " + _username);
            //    StartCoroutine(LoginSuccessful(_username));
            //}
        }

        yield return new WaitForSeconds(0.1f);
    }

    private IEnumerator RequestPostAccount(string nickname)
    {
        UserScore userscore = new UserScore();
        userscore.nickname = nickname;
        userscore.createdAt = DateTime.Now;
        userscore.updatedAt = DateTime.Now;
        string data = JsonUtility.ToJson(userscore);
        var request = new UnityWebRequest(URL, UnityWebRequest.kHttpVerbPOST);
        SetHeaderB4App(request);
        var jsonBytes = Encoding.UTF8.GetBytes(data);
        request.uploadHandler = new UploadHandlerRaw(jsonBytes);
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.error != null)
        {
            Debug.Log(request.error);
            Debug.Log(request.downloadHandler.text);
        }
        else
        {
            print(request.result);
            print(request.downloadHandler.text);
        }

        yield return new WaitForSeconds(0.1f);
    }

    private IEnumerator RequestPutScore(string objectId, int score)
    {
        UnityWebRequest request = UnityWebRequest.Get(URL + "/" + objectId);
        SetHeaderB4App(request);

        yield return request.SendWebRequest();

        if (request.error != null)
        {
            print("Error:" + request.error);
        }
        else
        {
            UserScore userScore = JsonUtility.FromJson<UserScore>(request.downloadHandler.text);
            userScore.score = score;
            userScore.updatedAt = DateTime.Now;
            string data = JsonUtility.ToJson(userScore);
            request = new UnityWebRequest(URL + "/" + objectId, UnityWebRequest.kHttpVerbPUT);
            SetHeaderB4App(request);
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(data));
            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.SendWebRequest();

            if (request.error != null)
            {
                Debug.Log(request.error);
                Debug.Log(request.downloadHandler.text);
            }
            else
            {
                print(request.result);
                print(request.downloadHandler.text);
            }
        }
    }

    [System.Serializable]
    public class UserScore
    {
        public string objectId;
        public int id_user;
        public string nickname;
        public int score;
        public DateTime createdAt;
        public DateTime updatedAt;
    }
}
