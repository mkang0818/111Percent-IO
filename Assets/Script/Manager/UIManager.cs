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
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI zemText;

    //ItemUI
    public TextMeshProUGUI item1Lv;
    public TextMeshProUGUI item1Price;
    public TextMeshProUGUI item2Lv;
    public TextMeshProUGUI item2Price;
    public TextMeshProUGUI item3Lv;
    public TextMeshProUGUI item3Price;

    //Sound
    public AudioClip btClick;
    public AudioClip buySound;
}

// ���� UI
[System.Serializable]
public class GameUI
{
    public GameObject gameUI;
    public Slider goalSlider;
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI infoAniName;
    public TextMeshProUGUI lvText;
    public TextMeshProUGUI animalText;

    public string[] animalName;
}

// ���ӿ���,Ŭ���� UI
[System.Serializable]
public class EndingUI
{
    public GameObject gameOverUI;
    public GameObject gameClearUI;

    public TextMeshProUGUI clearScore;
    public TextMeshProUGUI clearCoin;
    public TextMeshProUGUI overScore;
    public TextMeshProUGUI overCoin;
}


public class UIManager : MonoBehaviour
{
    private PlayerController player;
    public SpawnManager SpawnManager;
    public MainUI MainUI;
    public GameUI GameUI;
    public EndingUI EndUI;

    int ItemPrice = 100;

    private void Start()
    {
        GameManager.Instance.UIManager = GetComponent<UIManager>();
    }

    private void Update()
    {
        GameUIUpdate();
    }    

    // �� ���� UI ������Ʈ
    void GameUIUpdate()
    {
        if (GameManager.Instance.isStart && GameManager.Instance.getSheet)
        {
            InfoUpdate();
        }
        MainUIUpdate();
    }

    // ���� ȭ�� UI ������Ʈ
    void MainUIUpdate()
    {
        int Coin = GameManager.Instance.coin;
        MainUI.coinText.text = GameManager.Instance.FormatNumber(Coin);

        MainUI.item1Lv.text = "���ݷ� LV. " + GameManager.Instance.attackItemLv.ToString();
        MainUI.item2Lv.text = "�ð� ���� LV. " + GameManager.Instance.timeItemLv.ToString();
        MainUI.item3Lv.text = "ũ�� LV. " + GameManager.Instance.sizeItemLv.ToString();
        MainUI.item1Price.text = (100 * GameManager.Instance.attackItemLv).ToString();
        MainUI.item2Price.text = (100 * GameManager.Instance.timeItemLv).ToString();
        MainUI.item3Price.text = (100 * GameManager.Instance.sizeItemLv).ToString();
    }

    // ���� ������ ���� ���� UI
    void InfoUpdate()
    {
        int playerLv = GameManager.Instance.playerLv; // ���� �÷��̾� ����
        if (playerLv <= 15) GameUI.infoAniName.text = GameUI.animalName[playerLv]; // ���� ������ ���� �̸�

        // ���� �ð� �ؽ�Ʈ
        int CurTime = Mathf.RoundToInt(GameManager.Instance.curTime);
        GameUI.timerText.text = TimerText(CurTime);

        if (playerLv > 0) GameUI.scoreText.text = GameManager.Instance.FormatNumber(GameManager.Instance.curScore) + " / " + GameManager.Instance.FormatNumber(GameManager.Instance.goalScore[playerLv - 1]);

        GameUI.lvText.text = GameManager.Instance.playerLv.ToString();
        GameUI.animalText.text = GameManager.Instance.Player.PlayerStat.name;
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
            GameUI.countdownText.text = countdown.ToString();
            yield return new WaitForSeconds(1);
            GameUI.countdownText.GetComponent<CountDown>().CountDownOn();
            countdown--;
        }

        GameUI.countdownText.text = "";
        GameManager.Instance.Player.isStart = true;
        SpawnManager.SpawnStart();
    }


    ////////////////////////////       ��ư �޼���       /////////////////////////////////////

    // ���� ���� ��ư
    public void GameStartBt()
    {
        SoundManager.Instance.SoundPlay("BtClick", MainUI.btClick);
        GameManager.Instance.isStart = true;
        GameManager.Instance.PlayerIns();

        MainUI.mainUI.SetActive(false);
        GameUI.gameUI.SetActive(true);

        GameManager.Instance.Player.goalSlider = GameUI.goalSlider;
        StartCoroutine(StartCountdown());
    }

    // �� ����
    public void GameQuitBt()
    {
        SoundManager.Instance.SoundPlay("BtClick", MainUI.btClick);
        GameManager.Instance.SaveCoin();
        Application.Quit();
    }

    // ���θ޴� �̵� ��ư
    public void GoMainBt()
    {
        SoundManager.Instance.SoundPlay("BtClick", MainUI.btClick);
        MainUI.mainUI.SetActive(true);
        EndUI.gameOverUI.SetActive(false);
        EndUI.gameClearUI.SetActive(false);
    }

    // ������ 1����ư - �÷��̾� ���ݷ� ����
    public void ItemBt1()
    {
        SoundManager.Instance.SoundPlay("ItemBuy", MainUI.buySound);
        int itemLv = GameManager.Instance.attackItemLv;
        int coin = GameManager.Instance.coin;
        int price = ItemPrice * itemLv;
        if (price <= coin)
        {
            GameManager.Instance.attackItemLv++;
            GameManager.Instance.UseCoin(price);
        }
    }

    // ������ 2����ư - ���� �ð� ����
    public void ItemBt2()
    {
        SoundManager.Instance.SoundPlay("ItemBuy", MainUI.buySound);
        int itemLv = GameManager.Instance.timeItemLv;
        int coin = GameManager.Instance.coin;
        int price = ItemPrice * itemLv;
        if (price <= coin)
        {
            GameManager.Instance.timeItemLv++;
            GameManager.Instance.UseCoin(price);
        }
    }

    // ������ 3����ư - �÷��̾� ũ�� ����
    public void ItemBt3()
    {
        SoundManager.Instance.SoundPlay("ItemBuy", MainUI.buySound);
        int itemLv = GameManager.Instance.sizeItemLv;
        int coin = GameManager.Instance.coin;
        int price = ItemPrice * itemLv;
        if (price <= coin)
        {
            GameManager.Instance.sizeItemLv++;
            GameManager.Instance.UseCoin(price);
        }
    }

    // ����Ŭ���� UI TEXT
    public void ClearUIUpdate(int Score, int Coin)
    {
        EndUI.clearScore.text = "��� : " + GameManager.Instance.FormatNumber(Score).ToString();
        EndUI.clearCoin.text = "+" + GameManager.Instance.FormatNumber(Coin).ToString();
    }

    // ���ӿ��� UI TEXT
    public void OverUIUpdate(int Score, int Coin)
    {
        EndUI.overScore.text = "��� : " + GameManager.Instance.FormatNumber(Score).ToString();
        EndUI.overCoin.text = "+" + GameManager.Instance.FormatNumber(Coin).ToString();
    }
}