using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; internal set; }

    public Data data = new Data();
    public new Audio audio = new Audio();
    public Setting setting = new Setting();

    [HideInInspector] public UserScoreList userScoreDB;
    [HideInInspector] public const string URL = "https://parseapi.back4app.com/classes/Userscore";
    [HideInInspector] public const string APP_ID = "8nkUt6v8S9HyE4RkXICQWF81UUCqgdXS2gUS7waX";
    [HideInInspector] public const string KEY_ID = "8UUVRZnpPVENrDNGLiyYgBjShEaJ40woLfYJqhUq";

    public string currentUserSessionID;
    public string loadedTextFileName;
    public string[] wordsDictionary;

    [Serializable]
    public class UserScoreList
    {
        public List<UserScore> results;
    }

    [Serializable]
    public class UserScore
    {
        public string objectId;
        public string nickname;
        public int score;
        public DateTime createdAt;
        public DateTime updatedAt;
    }

    //[Header("Letter Weight")]
    //public string scoreOf5;
    //public string scoreOf4, scoreOf3, scoreOf2, scoreOf1;

    //[Header("PowerUp Sprite")]
    //public Sprite power1;
    //public Sprite power2, power3, nonPower;
    //public float power1mod, power2mod, power3mod;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        SetupAudio();
        TextAsset file = Resources.Load(loadedTextFileName) as TextAsset;
        string txt = file.ToString();
        char[] separators = new char[] { ' ', ',' };
        wordsDictionary = txt.Split(separators, StringSplitOptions.RemoveEmptyEntries);
    }

    private void Start()
    {
        
    }

    public IEnumerator RequestGetScore()
    {
        UnityWebRequest request = UnityWebRequest.Get(URL);
        SetHeaderB4App(request);

        yield return request.SendWebRequest();

        if (request.error != null)
        {
            print("Error:" + request.error);
        }
        else
        {
            print(request.result);
            //print(request.downloadHandler.text);

            string json = request.downloadHandler.text;
            print(json);
            userScoreDB = JsonUtility.FromJson<UserScoreList>(json);
        }

        yield return new WaitForSeconds(0.1f);

        ResultSceneController.Instance.LeaderboardGetScore();
    }

    public void RequestPostAccountMethod(string nickname)
    {
        StartCoroutine(RequestPostAccount(nickname));
    }

    public IEnumerator RequestPostAccount(string nickname)
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
            //Debug.Log(request.downloadHandler.text);
        }
        else
        {
            print(request.result);
            //print(request.downloadHandler.text);

            UserScore userScore = JsonUtility.FromJson<UserScore>(request.downloadHandler.text);
            currentUserSessionID = userScore.objectId;
        }

        yield return new WaitForSeconds(0.1f);
    }

    public IEnumerator RequestPutScore(string objectId, int score)
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
                //Debug.Log(request.downloadHandler.text);
            }
            else
            {
                print(request.result);
                //print(request.downloadHandler.text);
            }
        }
    }

    private void SetHeaderB4App(UnityWebRequest request)
    {
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("X-Parse-Application-Id", APP_ID);
        request.SetRequestHeader("X-Parse-REST-API-Key", KEY_ID);
    }

    public void PlaySfx(string name)
    {
        Sound sfx = Array.Find(audio.soundEffects, sound => sound.name == name);
        if (sfx == null)
        {
            print("Audio " + name + " not found!!");
            return;
        }

        sfx.audioSource.Play();
    }

    public void PlayBgm(string name)
    {
        Sound bgm = Array.Find(audio.backgroundMusics, sound => sound.name == name);
        if (bgm.name == name && bgm.audioSource.isPlaying) return;
        if (bgm == null)
        {
            print("Audio " + name + " not found!!");
            return;
        }

        for (int i = 0; i < audio.backgroundMusics.Length; i++)
        {
            audio.backgroundMusics[i].audioSource.Stop();
        }

        bgm.audioSource.Play();
    }

    public void StopBgm()
    {
        for (int i = 0; i < audio.backgroundMusics.Length; i++)
        {
            audio.backgroundMusics[i].audioSource.Stop();
        }
    }

    public void UpdateVolume()
    {

        for (int i = 0; i < audio.activeSfx.Count; i++)
        {
            audio.activeSfx[i].volume = setting.BgmVolume;
        }

        for (int i = 0; i < audio.activeBgm.Count; i++)
        {
            audio.activeBgm[i].volume = setting.SfxVolume;
        }
    }

    private void SetupAudio()
    {
        for (int i = 0; i < audio.soundEffects.Length; i++)
        {
            audio.soundEffects[i].audioSource = gameObject.AddComponent<AudioSource>();
            audio.soundEffects[i].audioSource.clip = audio.soundEffects[i].clip;
            audio.soundEffects[i].audioSource.volume = audio.soundEffects[i].volume;
            audio.soundEffects[i].audioSource.pitch = audio.soundEffects[i].pitch;
            audio.soundEffects[i].audioSource.loop = audio.soundEffects[i].loop;
            audio.activeSfx.Add(audio.soundEffects[i].audioSource);
        }

        for (int i = 0; i < audio.backgroundMusics.Length; i++)
        {
            audio.backgroundMusics[i].audioSource = gameObject.AddComponent<AudioSource>();
            audio.backgroundMusics[i].audioSource.clip = audio.backgroundMusics[i].clip;
            audio.backgroundMusics[i].audioSource.volume = audio.backgroundMusics[i].volume;
            audio.backgroundMusics[i].audioSource.pitch = audio.backgroundMusics[i].pitch;
            audio.backgroundMusics[i].audioSource.loop = audio.backgroundMusics[i].loop;
            audio.activeBgm.Add(audio.backgroundMusics[i].audioSource);
        }
    }

    public void ToggleMusic(bool value)
    {
        for (int i = 0; i < audio.backgroundMusics.Length; i++)
        {
            audio.backgroundMusics[i].audioSource.mute = value;
        }
    }

    public void ToggleEffects(bool value)
    {
        for (int i = 0; i < audio.soundEffects.Length; i++)
        {
            audio.soundEffects[i].audioSource.mute = value;
        }
    }

    [Serializable]
    public class Audio
    {
        public Sound[] soundEffects;
        public Sound[] backgroundMusics;

        [HideInInspector] public List<AudioSource> activeSfx = new List<AudioSource>();
        [HideInInspector] public List<AudioSource> activeBgm = new List<AudioSource>();
    }

    [Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
        [Range(0.1f, 3f)] public float pitch = 1f;
        public bool loop = true;
        [HideInInspector] public AudioSource audioSource;
    }

    public class Setting
    {
        public float SfxVolume
        {
            get { return PlayerPrefs.GetFloat("sfxVolume"); }
            set { PlayerPrefs.SetFloat("sfxVolume", value); }
        }

        public float BgmVolume
        {
            get { return PlayerPrefs.GetFloat("bgmVolume"); }
            set { PlayerPrefs.SetFloat("bgmVolume", value); }
        }

        public float SfxStatus
        {
            get { return PlayerPrefs.GetInt("sfxStatus"); }
            set { PlayerPrefs.SetInt("sfxStatus", (int)value); }
        }

        public float BgmStatus
        {
            get { return PlayerPrefs.GetInt("bgmStatus"); }
            set { PlayerPrefs.SetInt("bgmStatus", (int)value); }
        }
    }

    public class Data
    {
        public Vector2 GetStageProgress()
        {
            return new Vector2(PlayerPrefs.GetFloat("EXPLORE_X"), PlayerPrefs.GetFloat("EXPLORE_Y"));
        }

        public void SetStageProgress(Vector2 pos)
        {
            PlayerPrefs.SetFloat("EXPLORE_X", pos.x);
            PlayerPrefs.SetFloat("EXPLORE_Y", pos.y);
        }

       
    }
}
