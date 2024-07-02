using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


// 메인 UI
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

// 게임 UI
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

// 게임오버,클리어 UI
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

    // 인 게임 UI 업데이트
    void GameUIUpdate()
    {
        if (GameManager.Instance.IsStart && GameManager.Instance.GETSheet)
        {
            InfoUpdate();
        }
        mainUIUpdate();
    }

    // 메인 화면 UI 업데이트
    void mainUIUpdate()
    {
        int Coin = GameManager.Instance.Coin;
        mainUI.CoinText.text = GameManager.Instance.FormatNumber(Coin);

        mainUI.Item1Lv.text = "공격력 LV. " + GameManager.Instance.AttackItem1Lv.ToString();
        mainUI.Item2Lv.text = "시간 증폭 LV. " + GameManager.Instance.TimeItem2Lv.ToString();
        mainUI.Item3Lv.text = "크기 LV. " + GameManager.Instance.SizeItem3Lv.ToString();
        mainUI.Item1Price.text = (100 * GameManager.Instance.AttackItem1Lv).ToString();
        mainUI.Item2Price.text = (100 * GameManager.Instance.TimeItem2Lv).ToString();
        mainUI.Item3Price.text = (100 * GameManager.Instance.SizeItem3Lv).ToString();
    }

    // 다음 레벨의 동물 정보 UI
    void InfoUpdate()
    {
        int playerLv = GameManager.Instance.PlayerLv; // 현재 플레이어 레벨
        if (playerLv <= 15) gameUI.InfoAniName.text = gameUI.AnimalName[playerLv]; // 다음 레벨의 동물 이름

        // 남은 시간 텍스트
        int CurTime = Mathf.RoundToInt(GameManager.Instance.CurTime);
        gameUI.TimerText.text = TimerText(CurTime);

        if (playerLv > 0) gameUI.ScoreText.text = GameManager.Instance.FormatNumber(GameManager.Instance.Score) + " / " + GameManager.Instance.FormatNumber(GameManager.Instance.goalScore[playerLv - 1]);

        gameUI.LvText.text = GameManager.Instance.PlayerLv.ToString();
        gameUI.AnimalText.text = GameManager.Instance.player.playerstat.Name;
    }



    // 제한 시간 텍스트
    string TimerText(int CurTime)
    {
        int minutes = CurTime / 60;
        int seconds = CurTime % 60;
        string formattedTime = string.Format("{0:00} : {1:00}", minutes, seconds);

        if (CurTime <= 5)
            formattedTime = "<color=red>" + formattedTime + "</color>";

        return formattedTime;
    }

    // 카운트 다운
    IEnumerator StartCountdown()
    {
        int countdown = 3;

        // 카운트다운 시작
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


    ////////////////////////////       버튼 메서드       /////////////////////////////////////

    // 게임 시작 버튼
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

    // 앱 종료
    public void GameQuitBt()
    {
        SoundManager.Instance.SoundPlay("BtClick", mainUI.BtClick);
        GameManager.Instance.SaveCoin();
        Application.Quit();
    }

    // 메인메뉴 이동 버튼
    public void GoMainBt()
    {
        SoundManager.Instance.SoundPlay("BtClick", mainUI.BtClick);
        mainUI.mainUI.SetActive(true);
        endUI.gameOverUI.SetActive(false);
        endUI.gameClearUI.SetActive(false);
    }

    // 아이템 1번버튼 - 플레이어 공격력 증가
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

    // 아이템 2번버튼 - 제한 시간 증가
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

    // 아이템 3번버튼 - 플레이어 크기 증가
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

    // 게임클리어 UI TEXT
    public void ClearUIUpdate(int Score, int Coin)
    {
        endUI.ClearScore.text = "기록 : " + GameManager.Instance.FormatNumber(Score).ToString();
        endUI.ClearCoin.text = "+" + GameManager.Instance.FormatNumber(Coin).ToString();
    }

    // 게임오버 UI TEXT
    public void OverUIUpdate(int Score, int Coin)
    {
        endUI.OverScore.text = "기록 : " + GameManager.Instance.FormatNumber(Score).ToString();
        endUI.OverCoin.text = "+" + GameManager.Instance.FormatNumber(Coin).ToString();
    }
}