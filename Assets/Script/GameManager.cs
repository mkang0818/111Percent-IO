using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


public class GameManager : MonoBehaviour
{
    const string URL = "https://docs.google.com/spreadsheets/d/1qSbuMSkixGuWZJ--hDFaKnOlVopao0pXk9RJN53N90Q/export?format=tsv&range=A2:H17";
    [HideInInspector] public string StatData;
    public VariableJoystick Joystick;
    public GameObject StartAni;

    [HideInInspector] public PlayerController player;

    private static GameManager _instance;


    public int PlayerLv;
    Vector3 spawnPos = new Vector3(0, 0.1f, 0);

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // 씬에 GameManager가 없으면 새로 생성
                GameObject singleton = new GameObject(typeof(GameManager).ToString());
                _instance = singleton.AddComponent<GameManager>();
                DontDestroyOnLoad(singleton);
            }
            return _instance;
        }
    }
    void Awake()
    {
        player = Instantiate(StartAni, spawnPos, Quaternion.identity).GetComponent<PlayerController>();
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject); // 씬 로드 시 삭제되지 않도록 설정
        }
        else if (_instance != this)
        {
            Destroy(gameObject); // 싱글톤 인스턴스가 이미 존재하면 중복된 오브젝트 삭제
        }
    }
    private void Start()
    {
        GameStart();
    }
    private void Update()
    {
        PlayerLv = player.playerstat.Lv;
    }
    public void GameStart()
    {
        StartCoroutine(www());
    }
    IEnumerator www()
    {
        UnityWebRequest www = UnityWebRequest.Get(URL);
        yield return www.SendWebRequest();
        StatData = www.downloadHandler.text;
        print(StatData);
        PlayerInitStat(StatData, "1");
    }
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
                    player.playerstat.MaxHP = int.Parse(column[2]);
                    player.playerstat.CurHP = int.Parse(column[3]);
                    player.playerstat.At = int.Parse(column[4]);
                    player.playerstat.Speed = int.Parse(column[5]);
                    player.playerstat.MaxExp = int.Parse(column[6]);
                    player.playerstat.CurExp = int.Parse(column[7]);
                }
            }
        }
    }

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
                    prey.stat.MaxHP = int.Parse(column[2]);
                    prey.stat.CurHP = int.Parse(column[3]);
                    prey.stat.At = int.Parse(column[4]);
                    prey.stat.Speed = int.Parse(column[5]);
                    prey.stat.MaxExp = int.Parse(column[6]);
                    prey.stat.CurExp = int.Parse(column[7]);
                }
            }
        }
    }
}
