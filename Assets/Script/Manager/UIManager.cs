using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


// ���� UI
[System.Serializable]
public class MainUI
{
    public GameObject mainUI;
    public TextMeshProUGUI CoinText;
    public TextMeshProUGUI ZemText;

    //ItemUI
    public TextMeshProUGUI Item1Lv;
    public TextMeshProUGUI Item1Price;
    public TextMeshProUGUI Item2Lv;
    public TextMeshProUGUI Item2Price;
    public TextMeshProUGUI Item3Lv;
    public TextMeshProUGUI Item3Price;

    //Sound
    public AudioClip BtClick;
    public AudioClip BuySound;
}

// ���� UI
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
    public TextMeshProUGUI LvText;
    public TextMeshProUGUI AnimalText;

    public string[] AnimalName;
}

// ���ӿ���,Ŭ���� UI
[System.Serializable]
public class EndingUI
{
    public GameObject gameOverUI;
    public GameObject gameClearUI;

    public TextMeshProUGUI ClearScore;
    public TextMeshProUGUI ClearCoin;
    public TextMeshProUGUI OverScore;
    public TextMeshProUGUI OverCoin;
}


public class UIManager : MonoBehaviour
{
    private PlayerController player;
    public SpawnManager spawnmanager;
    public MainUI mainUI;
    public GameUI gameUI;
    public EndingUI endUI;

    int ItemPrice = 100;

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
        mainUIUpdate();
    }

    // ���� ȭ�� UI ������Ʈ
    void mainUIUpdate()
    {
        int Coin = GameManager.Instance.Coin;
        mainUI.CoinText.text = GameManager.Instance.FormatNumber(Coin);

        mainUI.Item1Lv.text = "���ݷ� LV. " + GameManager.Instance.AttackItem1Lv.ToString();
        mainUI.Item2Lv.text = "�ð� ���� LV. " + GameManager.Instance.TimeItem2Lv.ToString();
        mainUI.Item3Lv.text = "ũ�� LV. " + GameManager.Instance.SizeItem3Lv.ToString();
        mainUI.Item1Price.text = (100 * GameManager.Instance.AttackItem1Lv).ToString();
        mainUI.Item2Price.text = (100 * GameManager.Instance.TimeItem2Lv).ToString();
        mainUI.Item3Price.text = (100 * GameManager.Instance.SizeItem3Lv).ToString();
    }

    // ���� ������ ���� ���� UI
    void InfoUpdate()
    {
        int playerLv = GameManager.Instance.PlayerLv; // ���� �÷��̾� ����
        if (playerLv <= 15) gameUI.InfoAniName.text = gameUI.AnimalName[playerLv]; // ���� ������ ���� �̸�

        // ���� �ð� �ؽ�Ʈ
        int CurTime = Mathf.RoundToInt(GameManager.Instance.CurTime);
        gameUI.TimerText.text = TimerText(CurTime);

        if (playerLv > 0) gameUI.ScoreText.text = GameManager.Instance.FormatNumber(GameManager.Instance.Score) + " / " + GameManager.Instance.FormatNumber(GameManager.Instance.goalScore[playerLv - 1]);

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

    // ī��Ʈ �ٿ�
    IEnumerator StartCountdown()
    {
        int countdown = 3;

        // ī��Ʈ�ٿ� ����
        while (countdown > 0)
        {
            gameUI.CountDownText.text = countdown.ToString();
            yield return new WaitForSeconds(1);
            gameUI.CountDownText.GetComponent<CountDown>().CountDownOn();
            countdown--;
        }

        gameUI.CountDownText.text = "";
        GameManager.Instance.player.PlayerStart = true;
        spawnmanager.SpawnStart();
    }


    ////////////////////////////       ��ư �޼���       /////////////////////////////////////

    // ���� ���� ��ư
    public void GameStartBt()
    {
        SoundManager.Instance.SoundPlay("BtClick", mainUI.BtClick);
        GameManager.Instance.IsStart = true;
        GameManager.Instance.PlayerIns();

        mainUI.mainUI.SetActive(false);
        gameUI.gameUI.SetActive(true);

        GameManager.Instance.player.goalSlider = gameUI.GoalSlider;
        StartCoroutine(StartCountdown());
    }

    // �� ����
    public void GameQuitBt()
    {
        SoundManager.Instance.SoundPlay("BtClick", mainUI.BtClick);
        GameManager.Instance.SaveCoin();
        Application.Quit();
    }

    // ���θ޴� �̵� ��ư
    public void GoMainBt()
    {
        SoundManager.Instance.SoundPlay("BtClick", mainUI.BtClick);
        mainUI.mainUI.SetActive(true);
        endUI.gameOverUI.SetActive(false);
        endUI.gameClearUI.SetActive(false);
    }

    // ������ 1����ư - �÷��̾� ���ݷ� ����
    public void ItemBt1()
    {
        SoundManager.Instance.SoundPlay("ItemBuy", mainUI.BuySound);
        int itemLv = GameManager.Instance.AttackItem1Lv;
        int coin = GameManager.Instance.Coin;
        int price = ItemPrice * itemLv;
        if (price <= coin)
        {
            GameManager.Instance.AttackItem1Lv++;
            GameManager.Instance.UseCoin(price);
        }
    }

    // ������ 2����ư - ���� �ð� ����
    public void ItemBt2()
    {
        SoundManager.Instance.SoundPlay("ItemBuy", mainUI.BuySound);
        int itemLv = GameManager.Instance.TimeItem2Lv;
        int coin = GameManager.Instance.Coin;
        int price = ItemPrice * itemLv;
        if (price <= coin)
        {
            GameManager.Instance.TimeItem2Lv++;
            GameManager.Instance.UseCoin(price);
        }
    }

    // ������ 3����ư - �÷��̾� ũ�� ����
    public void ItemBt3()
    {
        SoundManager.Instance.SoundPlay("ItemBuy", mainUI.BuySound);
        int itemLv = GameManager.Instance.SizeItem3Lv;
        int coin = GameManager.Instance.Coin;
        int price = ItemPrice * itemLv;
        if (price <= coin)
        {
            GameManager.Instance.SizeItem3Lv++;
            GameManager.Instance.UseCoin(price);
        }
    }

    // ����Ŭ���� UI TEXT
    public void ClearUIUpdate(int Score, int Coin)
    {
        endUI.ClearScore.text = "��� : " + GameManager.Instance.FormatNumber(Score).ToString();
        endUI.ClearCoin.text = "+" + GameManager.Instance.FormatNumber(Coin).ToString();
    }

    // ���ӿ��� UI TEXT
    public void OverUIUpdate(int Score, int Coin)
    {
        endUI.OverScore.text = "��� : " + GameManager.Instance.FormatNumber(Score).ToString();
        endUI.OverCoin.text = "+" + GameManager.Instance.FormatNumber(Coin).ToString();
    }
}