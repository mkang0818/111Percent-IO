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

    // ���� ���� ����
    public float maxTime = 90; // �ִ� ���� �ð�
    public float curTime = 90; // ���� ���� �ð�
    public int curScore = 0; // ���� ����
    public long[] goalScore; // ������ ���� ��ǥ ����
    public float[] animalSize;
    public bool isStart = false; // ���� ���� ����
    public bool getSheet = false; // ��Ʈ �޾ƿ��� ����
    Vector3 spawnPos = new Vector3(0, 0.3f, 0);

    // ������ playerprefs
    [HideInInspector] public int coin;
    [HideInInspector] public int attackItemLv;
    [HideInInspector] public int timeItemLv;
    [HideInInspector] public int sizeItemLv;

    // ������ playerprefs Ű
    private string coinKey = "playersCoin";
    private string attackItemLvKey = "attackItemLv";
    private string timeItemLvKey = "timeItemLv";
    private string sizeItemLvKey = "sizeItemLv";

    // �̱���
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
        LoadCoin(); // ����� playerprefs �ҷ�����
    }

    // PlayerPrefs �ҷ�����
    void LoadCoin()
    {
        if (PlayerPrefs.HasKey(coinKey))
        {
            coin = PlayerPrefs.GetInt(coinKey);
            attackItemLv = PlayerPrefs.GetInt(attackItemLvKey);
            timeItemLv = PlayerPrefs.GetInt(timeItemLvKey);
            sizeItemLv = PlayerPrefs.GetInt(sizeItemLvKey);
        }
        else // ����� ���� ���� ��� �⺻�� 0���� ����
        {            
            coin = 0;
            attackItemLv = 1;
            timeItemLv = 1;
            sizeItemLv = 1;
        }

        // ������ ����
        curTime += timeItemLv * 1;
        maxTime += timeItemLv * 1;
    }

    // Playerprefs ����
    public void SaveCoin()
    {
        PlayerPrefs.SetInt(coinKey, coin);
        PlayerPrefs.SetInt(attackItemLvKey, attackItemLv);
        PlayerPrefs.SetInt(timeItemLvKey, timeItemLv);
        PlayerPrefs.SetInt(sizeItemLvKey, sizeItemLv);
        PlayerPrefs.Save();
    }

    //���� �Ͻ� ������ �� ���������� ����
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveCoin();
        }
    }

    // ���� ����
    public void UseCoin(int amount)
    {
        coin -= amount;
        if (coin < 0) coin = 0;
        SaveCoin(); // ����
    }


    private void Update()
    {
        if (Player != null) playerLv = Player.PlayerStat.lv;
    }

    // ���� ���� ����
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

    // �÷��̾� ������ ����
    public void PlayerIns()
    {
        // �÷��̾� ����
        Player = Instantiate(playerPrefab, spawnPos, Quaternion.identity).GetComponent<PlayerController>();

        // (ũ��)������ ����
        Vector3 playerScale = Player.transform.localScale;
        Player.transform.localScale = new Vector3(playerScale.x + 0.02f * sizeItemLv, playerScale.y + 0.02f * sizeItemLv, playerScale.z + 0.02f * sizeItemLv);

        StartCoroutine(GetSheet()); // ���� �������� ��Ʈ �޾ƿ��� ����
    }

    // ���۽��������Ʈ ������ �޾ƿ���
    IEnumerator GetSheet()
    {
        UnityWebRequest www = UnityWebRequest.Get(url);

        yield return www.SendWebRequest();

        StatData = www.downloadHandler.text;
        PlayerInitStat(StatData, "1");
        getSheet = true;
    }

    // �÷��̾� ���� �ʱ�ȭ
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

                // �÷��̾� ������ �ƴ� �� �� ���� (�����̸�, ��ǥ����, ����ũ��...)
                UIManager.GameUI.animalName[i] = column[1];
                goalScore[i] = long.Parse(column[4]);
                animalSize[i] = float.Parse(column[5]);
                SpawnManager.PoolObjText[i] = column[6];
                SpawnManager.spawnTime[i] = float.Parse(column[7]);
            }
        }
        Player.PlayerStat.attack += 3 * attackItemLv; // ���ݷ� ���� ������1�� ����
        curScore = 0; // �÷��̾� ���׷��̵� �� ��ǥ���� �ʱ�ȭ
    }

    // ���� ������Ʈ ���� �ʱ�ȭ
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
                    // AniCode�� �ش��ϴ� ���� �ο�
                    prey.stat.Lv = int.Parse(column[0]);
                    prey.stat.Name = column[1];
                    prey.stat.At = double.Parse(column[2]);
                    prey.stat.Speed = int.Parse(column[3]);
                }
            }
        }
    }

    // �ڸ��� ���� �޼���
    public string FormatNumber(double number)
    {
        if (number >= 1_000_000_000_000)
        {
            return (number / 1_000_000_000_000D).ToString(number >= 100 ? "0" : "0.00") + "T"; // 1��
        }
        else if (number >= 1_000_000_000)
        {
            return (number / 1_000_000_000D).ToString(number >= 100 ? "0" : "0.00") + "B"; // 10��
        }
        else if (number >= 1_000_000)
        {
            return (number / 1_000_000D).ToString(number >= 100 ? "0" : "0.00") + "M"; // 10��
        }
        else if (number >= 100)
        {
            return number.ToString("N0"); // �Ҽ��� ����
        }
        else
        {
            return number.ToString("N2"); // �Ҽ��� 2�ڸ�����
        }
    }
}