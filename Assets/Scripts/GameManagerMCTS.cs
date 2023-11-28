using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerMCTS : GameManager
{
    public static new GameManagerMCTS Instance { set; get; }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        SetupAudio();
    }

    // Start is called before the first frame update
    void Start()
    {
        PlayBgm("Menu");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
