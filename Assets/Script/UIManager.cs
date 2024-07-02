using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;



[System.Serializable]
public class MainUI
{
    public GameObject mainUI;
    public TextMeshProUGUI CoinText;
    public TextMeshProUGUI ZemText;

    public TextMeshProUGUI Item1Lv;
    public TextMeshProUGUI Item1Price;
    public TextMeshProUGUI Item2Lv;
    public TextMeshProUGUI Item2Price;
    public TextMeshProUGUI Item3Lv;
    public TextMeshProUGUI Item3Price;
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

    public TextMeshProUGUI ClearScore;
    public TextMeshProUGUI ClearCoin;
    public TextMeshProUGUI OverScore;
    public TextMeshProUGUI OverCoin;
}

public class UIManager : MonoBehaviour
{
    private PlayerController player;


    public MainUI mainUI;
    public GameUI gameUI;
    public EndingUI endUI;

    int ItemPrice = 100;

    public SpawnManager spawnmanager;
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

    // 게임 시작 버튼
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
        GameManager.Instance.SaveCoin();
        Application.Quit();
    }

    public void GoMainBt()
    {
        mainUI.mainUI.SetActive(true);
        endUI.gameOverUI.SetActive(false);
        endUI.gameClearUI.SetActive(false);
    }

    public void ItemBt1()
    {
        int itemLv = GameManager.Instance.AttackItem1Lv;
        int coin = GameManager.Instance.Coin;
        int price = ItemPrice * itemLv;
        if (price <= coin)
        {
            GameManager.Instance.AttackItem1Lv++;
            GameManager.Instance.UseCoin(price);
        }
    }
    public void ItemBt2()
    {
        int itemLv = GameManager.Instance.TimeItem2Lv;
        int coin = GameManager.Instance.Coin;
        int price = ItemPrice * itemLv;
        if (price <= coin)
        {
            GameManager.Instance.TimeItem2Lv++;
            GameManager.Instance.UseCoin(price);
        }
    }
    public void ItemBt3()
    {
        int itemLv = GameManager.Instance.SizeItem3Lv;
        int coin = GameManager.Instance.Coin;
        int price = ItemPrice * itemLv;
        if (price <= coin)
        {
            GameManager.Instance.SizeItem3Lv++;
            GameManager.Instance.UseCoin(price);
        }
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
            gameUI.CountDownText.GetComponent<CountDown>().CountTextOn();
            countdown--;
        }

        gameUI.CountDownText.text = "";
        GameManager.Instance.player.PlayerStart = true;
        spawnmanager.SpawnStart();
    }


    void mainUIUpdate()
    {
        int Coin = GameManager.Instance.Coin;
        mainUI.CoinText.text = GameManager.Instance.FormatNumber(Coin);

        mainUI.Item1Lv.text = GameManager.Instance.AttackItem1Lv.ToString();
        mainUI.Item2Lv.text = GameManager.Instance.TimeItem2Lv.ToString();
        mainUI.Item3Lv.text = GameManager.Instance.SizeItem3Lv.ToString();
        mainUI.Item1Price.text = (100 * GameManager.Instance.AttackItem1Lv).ToString();
        mainUI.Item2Price.text = (100 * GameManager.Instance.TimeItem2Lv).ToString();
        mainUI.Item3Price.text = (100 * GameManager.Instance.SizeItem3Lv).ToString();
    }
    // 다음 레벨의 동물 정보 UI
    void InfoUpdate()
    {
        int playerLv = GameManager.Instance.PlayerLv;       // 현재 플레이어 레벨
        if (playerLv <= 15)
        {
            gameUI.InfoAniName.text = gameUI.AnimalName[playerLv];  // 다음 레벨의 동물 이름
            //gameUI.InfoAniImg.sprite = gameUI.AnimalSpr[playerLv];  // 다음 레벨의 동물 이미지
        }

        // 남은 시간 텍스트
        int CurTime = Mathf.RoundToInt(GameManager.Instance.CurTime);
        gameUI.TimerText.text = TimerText(CurTime);

        if (playerLv > 0) gameUI.ScoreText.text = GameManager.Instance.FormatNumber(GameManager.Instance.Score) + " / " + GameManager.Instance.FormatNumber(GameManager.Instance.goalScore[playerLv - 1]);

        gameUI.LvText.text = GameManager.Instance.PlayerLv.ToString();
        gameUI.AnimalText.text = GameManager.Instance.player.playerstat.Name;
    }

    public void ClearUIUpdate()
    {
        endUI.ClearScore.text = "점수 : " + GameManager.Instance.Score.ToString();
        endUI.ClearCoin.text = "+" + GameManager.Instance.Score.ToString();
    }

    public void OverUIUpdate()
    {
        endUI.OverScore.text = "점수 : " + GameManager.Instance.Score.ToString();
        endUI.OverCoin.text = "+" + GameManager.Instance.Score.ToString();
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
}