using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;



[System.Serializable]
public class MainUI
{
    public GameObject mainUI;
}

[System.Serializable]
public class GameUI
{
    public GameObject gameUI;
    public Slider GoalSlider;
    public TextMeshProUGUI CountDownText;
    public TextMeshProUGUI TimerText;
    public TextMeshProUGUI ScoreText;

    public TextMeshProUGUI InfoAniName;
    public Image InfoAniImg;

    public Sprite[] AnimalSpr;
    public string[] AnimalName;

    public TextMeshProUGUI LvText;
    public TextMeshProUGUI AnimalText;
}

[System.Serializable]
public class EndingUI
{
    public GameObject gameOverUI;
    public GameObject gameClearUI;
}

public class UIManager : MonoBehaviour
{
    public MainUI mainUI;
    [SerializeField] public GameUI gameUI;
    [SerializeField] public EndingUI overUI;


    private PlayerController player;

    public SpawnManager spawnmanager;
    private void Start()
    {
        GameManager.Instance.UImanager = GetComponent<UIManager>();
    }
    private void Update()
    {
        GameUIUpdate();
    }

    // �� ���� UI ������Ʈ
    void GameUIUpdate()
    {
        if (GameManager.Instance.IsStart && GameManager.Instance.GETSheet)
        {
            InfoUpdate();
        }
    }

    // ���� ���� ��ư
    public void GameStartBt()
    {
        GameManager.Instance.IsStart = true;
        GameManager.Instance.PlayerIns();

        mainUI.mainUI.SetActive(false);
        gameUI.gameUI.SetActive(true);

        GameManager.Instance.player.goalSlider = gameUI.GoalSlider;
        StartCoroutine(StartCountdown());
    }

    public void GameQuitBt()
    {
        Application.Quit();
    }

    public void GoMainBt()
    {
        mainUI.mainUI.SetActive(true);
        overUI.gameOverUI.SetActive(false);
        overUI.gameClearUI.SetActive(false);
    }

    // ī��Ʈ �ٿ�
    IEnumerator StartCountdown()
    {
        int countdown = 3;

        // ī��Ʈ�ٿ� ����
        while (countdown > 0)
        {
            gameUI.CountDownText.text = countdown.ToString();
            yield return new WaitForSeconds(1);
            gameUI.CountDownText.GetComponent<CountDown>().CountTextOn();
            countdown--;
        }

        gameUI.CountDownText.text = "";
        print("���ӽ��� !");
        GameManager.Instance.player.PlayerStart = true;
        spawnmanager.SpawnStart();
    }

    // ���� ������ ���� ���� UI
    void InfoUpdate()
    {
        int playerLv = GameManager.Instance.PlayerLv;       // ���� �÷��̾� ����
        if (playerLv <= 15)
        {
            gameUI.InfoAniName.text = gameUI.AnimalName[playerLv];  // ���� ������ ���� �̸�
            //gameUI.InfoAniImg.sprite = gameUI.AnimalSpr[playerLv];  // ���� ������ ���� �̹���
        }

        // ���� �ð� �ؽ�Ʈ
        int CurTime = Mathf.RoundToInt(GameManager.Instance.player.playerstat.TimeLimit);
        gameUI.TimerText.text = TimerText(CurTime);
        
        if(playerLv > 0) gameUI.ScoreText.text = GameManager.Instance.FormatNumber(GameManager.Instance.Score) + " / " + GameManager.Instance.FormatNumber(GameManager.Instance.goalScore[playerLv - 1]);

        gameUI.LvText.text = GameManager.Instance.PlayerLv.ToString();
        gameUI.AnimalText.text = GameManager.Instance.player.playerstat.Name;
    }

    // ���� �ð� �ؽ�Ʈ
    string TimerText(int CurTime)
    {
        int minutes = CurTime / 60;
        int seconds = CurTime % 60;
        string formattedTime = string.Format("{0:00} : {1:00}", minutes, seconds);
        
        if (CurTime <= 5)
           formattedTime = "<color=red>" + formattedTime + "</color>";

        return formattedTime;
    }
}