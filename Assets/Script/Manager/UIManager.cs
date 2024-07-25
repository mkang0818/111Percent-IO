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

// 게임 UI
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

// 게임오버,클리어 UI
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

    // 인 게임 UI 업데이트
    void GameUIUpdate()
    {
        if (GameManager.Instance.isStart && GameManager.Instance.getSheet)
        {
            InfoUpdate();
        }
        MainUIUpdate();
    }

    // 메인 화면 UI 업데이트
    void MainUIUpdate()
    {
        int Coin = GameManager.Instance.coin;
        MainUI.coinText.text = GameManager.Instance.FormatNumber(Coin);

        MainUI.item1Lv.text = "공격력 LV. " + GameManager.Instance.attackItemLv.ToString();
        MainUI.item2Lv.text = "시간 증폭 LV. " + GameManager.Instance.timeItemLv.ToString();
        MainUI.item3Lv.text = "크기 LV. " + GameManager.Instance.sizeItemLv.ToString();
        MainUI.item1Price.text = (100 * GameManager.Instance.attackItemLv).ToString();
        MainUI.item2Price.text = (100 * GameManager.Instance.timeItemLv).ToString();
        MainUI.item3Price.text = (100 * GameManager.Instance.sizeItemLv).ToString();
    }

    // 다음 레벨의 동물 정보 UI
    void InfoUpdate()
    {
        int playerLv = GameManager.Instance.playerLv; // 현재 플레이어 레벨
        if (playerLv <= 15) GameUI.infoAniName.text = GameUI.animalName[playerLv]; // 다음 레벨의 동물 이름

        // 남은 시간 텍스트
        int CurTime = Mathf.RoundToInt(GameManager.Instance.curTime);
        GameUI.timerText.text = TimerText(CurTime);

        if (playerLv > 0) GameUI.scoreText.text = GameManager.Instance.FormatNumber(GameManager.Instance.curScore) + " / " + GameManager.Instance.FormatNumber(GameManager.Instance.goalScore[playerLv - 1]);

        GameUI.lvText.text = GameManager.Instance.playerLv.ToString();
        GameUI.animalText.text = GameManager.Instance.Player.PlayerStat.name;
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
            GameUI.countdownText.text = countdown.ToString();
            yield return new WaitForSeconds(1);
            GameUI.countdownText.GetComponent<CountDown>().CountDownOn();
            countdown--;
        }

        GameUI.countdownText.text = "";
        GameManager.Instance.Player.isStart = true;
        SpawnManager.SpawnStart();
    }


    ////////////////////////////       버튼 메서드       /////////////////////////////////////

    // 게임 시작 버튼
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

    // 앱 종료
    public void GameQuitBt()
    {
        SoundManager.Instance.SoundPlay("BtClick", MainUI.btClick);
        GameManager.Instance.SaveCoin();
        Application.Quit();
    }

    // 메인메뉴 이동 버튼
    public void GoMainBt()
    {
        SoundManager.Instance.SoundPlay("BtClick", MainUI.btClick);
        MainUI.mainUI.SetActive(true);
        EndUI.gameOverUI.SetActive(false);
        EndUI.gameClearUI.SetActive(false);
    }

    // 아이템 1번버튼 - 플레이어 공격력 증가
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

    // 아이템 2번버튼 - 제한 시간 증가
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

    // 아이템 3번버튼 - 플레이어 크기 증가
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

    // 게임클리어 UI TEXT
    public void ClearUIUpdate(int Score, int Coin)
    {
        EndUI.clearScore.text = "기록 : " + GameManager.Instance.FormatNumber(Score).ToString();
        EndUI.clearCoin.text = "+" + GameManager.Instance.FormatNumber(Coin).ToString();
    }

    // 게임오버 UI TEXT
    public void OverUIUpdate(int Score, int Coin)
    {
        EndUI.overScore.text = "기록 : " + GameManager.Instance.FormatNumber(Score).ToString();
        EndUI.overCoin.text = "+" + GameManager.Instance.FormatNumber(Coin).ToString();
    }
}