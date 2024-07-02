using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


public class GameManager : MonoBehaviour
{
    [HideInInspector] public string StatData;
    const string URL = "https://docs.google.com/spreadsheets/d/1qSbuMSkixGuWZJ--hDFaKnOlVopao0pXk9RJN53N90Q/export?format=tsv&range=A2:J17";
    

    public FloatingJoystick Joystick;
    public GameObject StartAni;

    [HideInInspector] public PlayerController player;
    [HideInInspector] public int PlayerLv = 1;
    Vector3 spawnPos = new Vector3(0, 0.3f, 0);
    public int[] AnimalSize;

    public bool IsStart = false;
    public bool GETSheet = false;
     public long[] goalScore;
    public long Score = 0;

    public GameObject PlusText;
    public GameObject MinusText;
    public GameObject TimerText;

    [HideInInspector] public UIManager UImanager;
    [HideInInspector] public SpawnManager Spawnmanager;
    
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("GameManager");
                    _instance = go.AddComponent<GameManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if(player!= null) PlayerLv = player.playerstat.Lv;
    }

    public void GameOver(bool over)
    {
        IsStart = false;
        Score = 0;
        UImanager.gameUI.gameUI.SetActive(false);
        
        if (over) UImanager.overUI.gameOverUI.SetActive(true);
        else UImanager.overUI.gameClearUI.SetActive(true);
        
        Destroy(player.gameObject);
    }

    // 플레이어프리팹 생성
    public void PlayerIns()
    {
        player = Instantiate(StartAni, spawnPos, Quaternion.identity).GetComponent<PlayerController>();
        StartCoroutine(GetSheet());
    }

    // 구글스프레드시트 데이터 받아오기
    IEnumerator GetSheet()
    {
        UnityWebRequest www = UnityWebRequest.Get(URL);
        yield return www.SendWebRequest();
        StatData = www.downloadHandler.text;
        print(StatData);
        PlayerInitStat(StatData, "1");
        GETSheet = true;
    }

    // 
    public void PlayerInitStat(string tsv, string AniCode)
    {
        string[] row = tsv.Split("\n");
        int rowSize = row.Length;
        int columnSize = row[0].Split("\t").Length;

        for (int i = 0; i < rowSize; i++)
        {
            string[] column = row[i].Split("\t");
            for (int j = 0; j < columnSize; j++)
            {
                if (column[0] == AniCode)
                {
                    player.playerstat.Lv = int.Parse(column[0]);
                    player.playerstat.Name = column[1];
                    player.playerstat.TimeLimit = int.Parse(column[2]);
                    player.playerstat.At = long.Parse(column[3]);
                    player.playerstat.Speed = int.Parse(column[4]);
                }
                UImanager.gameUI.AnimalName[i] = column[1];
                goalScore[i] = long.Parse(column[5]);
                print(int.Parse(column[6]));
                AnimalSize[i] = int.Parse(column[6]);
                Spawnmanager.PoolObjText[i] = column[7];
                Spawnmanager.spawnTime[i] = float.Parse(column[8]);
                Spawnmanager.SpawnPosSize[i] = int.Parse(column[9]);
            }
        }
    }

    // 
    public void PreyInitStat(string tsv, string AniCode, PreyAnimal prey)
    {
        string[] row = tsv.Split("\n");
        int rowSize = row.Length;
        int columnSize = row[0].Split("\t").Length;

        for (int i = 0; i < rowSize; i++)
        {
            string[] column = row[i].Split("\t");
            for (int j = 0; j < columnSize; j++)
            {
                if (column[0] == AniCode)
                {
                    prey.stat.Lv = int.Parse(column[0]);
                    prey.stat.Name = column[1];
                    prey.stat.TimeLimit = int.Parse(column[2]);
                    prey.stat.At = long.Parse(column[3]);
                    prey.stat.Speed = int.Parse(column[4]);
                }
            }
        }
    }

    // 
    public string FormatNumber(long number)
    {
        if (number >= 1_000_000_000)
        {
            return (number / 1_000_000_000D).ToString("0.##") + "B";
        }
        else if (number >= 1_000_000)
        {
            return (number / 1_000_000D).ToString("0.##") + "M";
        }
        else
        {
            return number.ToString("N0");
        }
    }
}