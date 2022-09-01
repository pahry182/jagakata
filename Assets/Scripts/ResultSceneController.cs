using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultSceneController : UIController
{
    public static ResultSceneController Instance { get; internal set; }

    public GameObject entryPrefab;
    public Transform entryParent;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.PlayBgm("Menu");
        StartCoroutine(GameManager.Instance.RequestGetScore());        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RestartButton()
    {
        LoadScene("LoginScene");
    }

    private List<LeaderboardScoreEntry> SetupEntry()
    {
        GameManager bc = GameManager.Instance;
        List<LeaderboardScoreEntry> entryList = new List<LeaderboardScoreEntry>();

        for (int i = 0; i < bc.userScoreDB.userScoreJSONList.Count; i++)
        {
            entryList.Add(Instantiate(entryPrefab, entryParent).GetComponent<LeaderboardScoreEntry>());
        }

        return entryList;
    }

    private void DataEntry(List<LeaderboardScoreEntry> entryList)
    {
        GameManager bc = GameManager.Instance;
        for (int i = 0; i < bc.userScoreDB.userScoreJSONList.Count; i++)
        {
            entryList[i].nickname.text = i + 1 + ". " + bc.userScoreDB.userScoreJSONList[i].nickname;
            entryList[i].score.text = bc.userScoreDB.userScoreJSONList[i].score.ToString();
        }
    }

    private int SortByScore(GameManager.UserScore p1, GameManager.UserScore p2)
    {
        return p2.score.CompareTo(p1.score);
    }

    public void LeaderboardGetScore()
    {
        print(GameManager.Instance.userScoreDB.userScoreJSONList.Count);
        List<LeaderboardScoreEntry> entryList = SetupEntry();
        GameManager.Instance.userScoreDB.userScoreJSONList.Sort(SortByScore);
        DataEntry(entryList);
    }
}
