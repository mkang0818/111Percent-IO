using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;


public class GameManager : MonoBehaviour
{
    [HideInInspector] public string StatData;
    const string url = "https://docs.google.com/spreadsheets/d/1qSbuMSkixGuWZJ--hDFaKnOlVopao0pXk9RJN53N90Q/export?format=tsv&range=A2:J17";

    [HideInInspector] public UIManager UIManager;
    [HideInInspector] public SpawnManager SpawnManager;
    [HideInInspector] public PlayerController Player;
    [HideInInspector] public int playerLv = 1;

    public FloatingJoystick Joystick;
    [SerializeField] GameObject playerPrefab;

    // 게임 관련 변수
    public float maxTime = 90; // 최대 제한 시간
    public float curTime = 90; // 현재 제한 시간
    public int curScore = 0; // 현재 점수
    public long[] goalScore; // 레벨에 따른 목표 점수
    public float[] animalSize;
    public bool isStart = false; // 게임 시작 여부
    public bool getSheet = false; // 시트 받아오기 여부
    Vector3 spawnPos = new Vector3(0, 0.3f, 0);

    // 아이템 playerprefs
    [HideInInspector] public int coin;
    [HideInInspector] public int attackItemLv;
    [HideInInspector] public int timeItemLv;
    [HideInInspector] public int sizeItemLv;

    // 아이템 playerprefs 키
    private string coinKey = "playersCoin";
    private string attackItemLvKey = "attackItemLv";
    private string timeItemLvKey = "timeItemLv";
    private string sizeItemLvKey = "sizeItemLv";

    // 싱글톤
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



    private void Start()
    {
        LoadCoin(); // 저장된 playerprefs 불러오기
    }

    // PlayerPrefs 불러오기
    void LoadCoin()
    {
        if (PlayerPrefs.HasKey(coinKey))
        {
            coin = PlayerPrefs.GetInt(coinKey);
            attackItemLv = PlayerPrefs.GetInt(attackItemLvKey);
            timeItemLv = PlayerPrefs.GetInt(timeItemLvKey);
            sizeItemLv = PlayerPrefs.GetInt(sizeItemLvKey);
        }
        else // 저장된 값이 없을 경우 기본값 0으로 설정
        {            
            coin = 0;
            attackItemLv = 1;
            timeItemLv = 1;
            sizeItemLv = 1;
        }

        // 아이템 적용
        curTime += timeItemLv * 1;
        maxTime += timeItemLv * 1;
    }

    // Playerprefs 저장
    public void SaveCoin()
    {
        PlayerPrefs.SetInt(coinKey, coin);
        PlayerPrefs.SetInt(attackItemLvKey, attackItemLv);
        PlayerPrefs.SetInt(timeItemLvKey, timeItemLv);
        PlayerPrefs.SetInt(sizeItemLvKey, sizeItemLv);
        PlayerPrefs.Save();
    }

    //앱이 일시 중지될 때 마지막으로 저장
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveCoin();
        }
    }

    // 코인 감소
    public void UseCoin(int amount)
    {
        coin -= amount;
        if (coin < 0) coin = 0;
        SaveCoin(); // 저장
    }


    private void Update()
    {
        if (Player != null) playerLv = Player.PlayerStat.lv;
    }

    // 게임 오버 리셋
    public void GameOver(bool over)
    {
        coin += curScore;
        SaveCoin();
        isStart = false;
        curTime = maxTime;

        UIManager.GameUI.gameUI.SetActive(false);
        if (over)
        {
            UIManager.EndUI.gameOverUI.SetActive(true);
            UIManager.OverUIUpdate(curScore, coin);
        }
        else
        {
            UIManager.EndUI.gameClearUI.SetActive(true);
            UIManager.ClearUIUpdate(curScore, coin);
        }

        curScore = 0;
        Destroy(Player.gameObject);
    }

    // 플레이어 프리팹 생성
    public void PlayerIns()
    {
        // 플레이어 생성
        Player = Instantiate(playerPrefab, spawnPos, Quaternion.identity).GetComponent<PlayerController>();

        // (크기)아이템 적용
        Vector3 playerScale = Player.transform.localScale;
        Player.transform.localScale = new Vector3(playerScale.x + 0.02f * sizeItemLv, playerScale.y + 0.02f * sizeItemLv, playerScale.z + 0.02f * sizeItemLv);

        StartCoroutine(GetSheet()); // 구글 스프레드 시트 받아오기 시작
    }

    // 구글스프레드시트 데이터 받아오기
    IEnumerator GetSheet()
    {
        UnityWebRequest www = UnityWebRequest.Get(url);

        yield return www.SendWebRequest();

        StatData = www.downloadHandler.text;
        PlayerInitStat(StatData, "1");
        getSheet = true;
    }

    // 플레이어 스탯 초기화
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
                    Player.PlayerStat.lv = int.Parse(column[0]);
                    Player.PlayerStat.name = column[1];
                    Player.PlayerStat.speed = int.Parse(column[3]);
                }

                // 플레이어 정보가 아닌 그 외 정보 (동물이름, 목표점수, 동물크기...)
                UIManager.GameUI.animalName[i] = column[1];
                goalScore[i] = long.Parse(column[4]);
                animalSize[i] = float.Parse(column[5]);
                SpawnManager.PoolObjText[i] = column[6];
                SpawnManager.spawnTime[i] = float.Parse(column[7]);
            }
        }
        Player.PlayerStat.attack += 3 * attackItemLv; // 공격력 증가 아이템1번 구현
        curScore = 0; // 플레이어 업그레이드 후 목표점수 초기화
    }

    // 동물 오브젝트 스탯 초기화
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
                    // AniCode에 해당하는 스탯 부여
                    prey.stat.Lv = int.Parse(column[0]);
                    prey.stat.Name = column[1];
                    prey.stat.At = double.Parse(column[2]);
                    prey.stat.Speed = int.Parse(column[3]);
                }
            }
        }
    }

    // 자릿수 단위 메서드
    public string FormatNumber(double number)
    {
        if (number >= 1_000_000_000_000)
        {
            return (number / 1_000_000_000_000D).ToString(number >= 100 ? "0" : "0.00") + "T"; // 1조
        }
        else if (number >= 1_000_000_000)
        {
            return (number / 1_000_000_000D).ToString(number >= 100 ? "0" : "0.00") + "B"; // 10억
        }
        else if (number >= 1_000_000)
        {
            return (number / 1_000_000D).ToString(number >= 100 ? "0" : "0.00") + "M"; // 10만
        }
        else if (number >= 100)
        {
            return number.ToString("N0"); // 소숫점 없음
        }
        else
        {
            return number.ToString("N2"); // 소숫점 2자리까지
        }
    }
}